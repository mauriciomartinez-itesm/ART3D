using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UX_Helper : MonoBehaviour
{

    public Button Close;
    public Button Info;
    public Button Menu;

    public Button Instrucciones;
    public Button Politica;
    public Button Aviso;



    public GameObject Dock;
    public GameObject ExpandMenu;
    public GameObject[] Controls;
    public bool state = true;
    public GameObject InfoMenu;

    public void ShowDock()
    {
        if (state)
        {
            state = false;
            Debug.Log("on");
            Vector3 pos = transform.position;
            pos.y = -1080;
            LeanTween.moveLocalY(Dock, pos.y, 2).setEaseInOutCirc();
        }
        else
        {
            state = true;
            Debug.Log("off");
            Vector3 pos = transform.position;
            pos.y = -1280;
            LeanTween.moveLocalY(Dock, pos.y, 2).setEaseOutExpo();


        }
        //;
    }

    public void ShowMenu()
    {
        ExpandMenu.SetActive(true);
        LeanTween.alpha(ExpandMenu, 1f, 2f).setEase(LeanTweenType.linear);
        Debug.Log("Suposely show");
    }

    public void HideMenu()
    {
        ExpandMenu.SetActive(false);
        LeanTween.alpha(ExpandMenu, 2f, 1f).setEase(LeanTweenType.linear);
        Debug.Log("Suposely Hide");
    }

    public void infoMenu()
    {
        ExpandMenu.SetActive(true);
        InfoMenu.SetActive(true);
      
    }

    private void Start()
    {
        ExpandMenu.SetActive(false);
        InfoMenu.SetActive(false);
        
    }
    //Dock Behavior

    private void Update()
    {
    }
}
