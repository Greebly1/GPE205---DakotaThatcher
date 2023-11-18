using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestinputComponent : MonoBehaviour
{
    [SerializeField] private string message = "Default Message";
    [SerializeField] private KeyCode keyCode = KeyCode.Space;


    private void OnEnable()
    {
        Debug.Log(this.gameObject.name + " Enabled");
    }

    private void OnDisable()
    {
        Debug.Log(this.gameObject.name + " Disabled");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.gameObject.name + " Start");
    }

    private void Awake()
    {
        Debug.Log(this.gameObject.name + " Awake");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyCode)) { Debug.Log(message); }
    }
}
