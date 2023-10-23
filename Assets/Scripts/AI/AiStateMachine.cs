using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStateMachine
{
    #region State Variables
    private IState _currentState; //self explanable

    //each state has its own list of transitions
    //A dictionary of state name strings, containing their transitions
    private Dictionary<string, List<Transition>> _transitions = new Dictionary<string, List<Transition>>();

    //cached list of the transitions for the current state
    private List<Transition> _currentTransitions = new List<Transition>(); 

    private List<Transition> _anyTransitions = new List<Transition>(); //Transitions to always check for
    private static List<Transition> _emptyTransitions = new List<Transition>(capacity: 0); //Empty transition list used for code safety
    #endregion

    /// <summary>
    /// Executed on update, check for transitions, then do the _currentState's work
    /// </summary>
    public void Tick()
    {
        Transition nextTransition = GetTransition(); //try to get a new transition
        if (nextTransition != null) //set the new state if we found a transition
            SetState(nextTransition.To);

        _currentState?.tick(); //Do the currentState's work
        
    }

    /// <summary>
    /// Use this function to change the _currentState of this state machine so the 
    /// IState's onBegin, and onEnd methods are executed
    /// and the _currentTransitions are updated to match the new state
    /// </summary>
    /// <param name="newState">The new state to change to</param>
    public void SetState(IState newState)
    {
        if (_currentState == newState || newState == null) return; //don't do anything unless the newState is a different state

        _currentState?.onEnd(); //End the current state
        _currentState = newState; //Swap to the next state

        //Update _currentTransitions to reflect the current state's transitions
        _transitions.TryGetValue(_currentState.stateName(), out _currentTransitions);
        if (_currentTransitions == null) {
            _currentTransitions = _emptyTransitions;
        }

        _currentState.onBegin(); //Begin this state
    }

    /// <summary>
    /// Add a transition to the list of transitions for the from state in this state machine
    /// </summary>
    /// <param name="from">The state to transition from</param>
    /// <param name="to">The state to transition to</param>
    /// <param name="condition">True when the transition should happen</param>
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        //create an empty list of transitions if this state has no transition list
        if(_transitions.TryGetValue(from.stateName(), out var transition) == false) {
            transition = new List<Transition>();
            _transitions[from.stateName()] = transition;
        }

        transition.Add(new Transition(to, condition));
    }
   
    /// <summary>
    /// Add a transition to all states for this state machine
    /// </summary>
    /// <param name="to">The state to transition to</param>
    /// <param name="condition">True when the transition should happen</param>
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        _anyTransitions.Add(new Transition(to, condition));
    }

    /// <summary>
    /// Data structure for storing a condition to transition, and a state to transition to
    /// </summary>
    private class Transition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    /// <summary>
    /// Check the available transitions and return the first transition whoose condition is true
    /// First checks all _anyTransitions, then check the _currentTransitions in order they were added
    /// </summary>
    /// <returns></returns>
    private Transition GetTransition()
    {
        //check each _anyTransition for a true transition condition
        foreach(var transition in _anyTransitions)
            if (transition.Condition())
                return transition;  
        
        //then check the _currentTransitions conditions
        foreach(var transition in _currentTransitions)
            if (transition.Condition()) 
                return transition;

        
        return null; //No available transition condition is true
    }
}