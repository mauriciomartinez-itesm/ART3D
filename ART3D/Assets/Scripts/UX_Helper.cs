using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UX_Helper : MonoBehaviour
{
    GameObject Menu;
    GameObject Select;

    public Button Refresh;
    public Button Delete;
    public Button Close;
    public Button Instrucciones;
    public Button Politica;
    public Button Aviso;

    public GameObject Dock;
    public GameObject Console;
    public GameObject MenuDock;
    public GameObject ActionDock;
    public GameObject Options;
    public GameObject[] Controls;
    public bool state = true;
   


    public void ShowDock()
    {
        if (state)
        {
            state = false;
            Debug.Log("on");
            Vector3 pos = transform.position;
            pos.y = -1080;
            LeanTween.moveLocalY(Dock, pos.y, 1).setEaseInExpo();
        }
        else
        {
            state = true;
            Debug.Log("off");
            Vector3 pos = transform.position;
            pos.y = -1280;
            LeanTween.moveLocalY(Dock, pos.y, 1).setEaseOutExpo();
        }
        //;
    }


    //Activa el el menu en el dock
    public void OptionsView()
    {
        Options.SetActive(true);
        MenuDock.SetActive(false);
        
    }

    public void DissmissView()
    {
        Options.SetActive(false);
        MenuDock.SetActive(true);
    }
   
    public void ConsoleMenu()
    {
        ActionDock.SetActive(true);
        Vector3 pos = transform.position;
        pos.y = -200;
        LeanTween.moveY(Console, pos.y, 1).setEaseInSine();
        Debug.Log(Console.transform.position.y);
    }

    public void CloseConsole()
    {
        ActionDock.SetActive(false);
        MenuDock.SetActive(true);

        Vector3 pos = transform.position;
        pos.y = -1090;
        LeanTween.moveY(Console, pos.y, 1f).setEaseOutSine();


    }

    public void expandView()
    {
        Vector3 pos = transform.position;
        pos.y = 1000;
        LeanTween.moveY(Console, pos.y, 1).setEaseInSine();
    }

    

    private void Start()
    {
        MenuDock.SetActive(true);
        ActionDock.SetActive(false);
        Options.SetActive(false);
        
        
    }
    //Dock Behavior

    private void Update()
    {
    }
}
