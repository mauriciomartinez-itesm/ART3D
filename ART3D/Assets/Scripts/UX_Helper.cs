using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UX_Helper : MonoBehaviour
{
    public GameObject Dock;
    public GameObject[] Controls;
    public bool state = true;

    public void expandDock()
    {
        if (state)
        {
            state = false;
            Debug.Log("on");
            LeanTween.scaleY(Dock, 6, 2).setEaseInOutCirc();
        }
        else
        {
            state = true;
            Debug.Log("off");
            LeanTween.scaleY(Dock, 1, 2).setEaseOutExpo();


        }
        //;
    }
    private void Start()
    {
        
    }
    //Dock Behavior

    private void Update()
    {
    }
}
