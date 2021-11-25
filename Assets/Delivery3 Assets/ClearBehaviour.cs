using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public void clearChildren()
    {

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

    }
}
