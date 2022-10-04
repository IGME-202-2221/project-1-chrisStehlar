using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // FIELDS

    public GameObject target;
    public Vector3 offset;

    // MONO

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // track the target
        this.transform.position = target.transform.position;
        this.transform.Translate(offset);
    }

    // METHODS


}
