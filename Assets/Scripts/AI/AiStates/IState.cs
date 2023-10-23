

public interface IState
{   
    /// <summary>
    /// Executed on update, do the work for this state here
    /// </summary>
    void tick();

    /// <summary>
    /// Executes once after transitioning to this state
    /// </summary>
    void onBegin();

    /// <summary>
    /// Executes once before transitioning to the next state
    /// </summary>
    void onEnd();

    /// <summary>
    /// The name of each state should be unique
    /// </summary>
    /// <returns>The name of this state</returns>
    string stateName();
}
