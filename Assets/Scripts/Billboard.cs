using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Transform cameraTrans;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.LookAt(cameraTrans);

        gameObject.transform.rotation = Quaternion.Euler(90f,0f,0f);
    }
}
