using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperPanel : MonoBehaviour
{
    Transform child;
    private void Start()
    {
        child = transform.GetChild(0);
        child.gameObject.SetActive(false);

    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            child.gameObject.SetActive(true);
        if(Input.GetKeyUp(KeyCode.Q))
            child.gameObject.SetActive(false);

    }
}
