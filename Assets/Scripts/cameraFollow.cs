using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject focus;
    public Transform offset;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position += offset.position - gameObject.transform.position;
        gameObject.transform.SetParent(focus.transform);
        
        //gameObject.transform.rotation += offset.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.position = cameraPosition();
        //gameObject.transform.rotation = focus.transform.rotation;
    }

    private Vector3 cameraPosition()
    {
        return focus.transform.position;
    }
}
