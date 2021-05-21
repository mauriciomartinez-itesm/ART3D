using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UX_Helper : MonoBehaviour
{
    private bool state = false;
    private bool activate = true;
    private bool visible = false;
    private bool showing = false;
    private bool click = false;

    public Button CollectionBtn;

    GameObject Menu;
    GameObject Select;

    public GameObject Dock;
    public GameObject Console;

    // Seccion de Docks (Arreglos de botones)
    public GameObject MainDock;
    public GameObject ActionDock;
    // seccion de vistas
    public GameObject OptionsView;
    public GameObject CollectionView;
    //seccion de prefabs
    public GameObject CardPrefab;

    //Muestra el Dock (Barra inferior con acciones)
    public void ShowDock()
    {
        if (state)
        {
            //Oculta el Dock 
            state = false;
            Vector3 pos = transform.position;
            pos.y = -1080;
            LeanTween.moveLocalY(Dock, pos.y, 1).setEaseInExpo();
        }
        else
        {
            //Muestra el Dock
            state = true;
            Vector3 pos = transform.position;
            pos.y = -1280;
            LeanTween.moveLocalY(Dock, pos.y, 1).setEaseOutExpo();
        }
        //;
    }


    //Activa el el menu en el dock
    public void ShowCollection()
    {
        if (activate)
        {
            activate = false;
            
            MainDock.SetActive(true);
            ActionDock.SetActive(false);
            CollectionView.SetActive(activate);
            

        }
        else
        {
            activate = true;
            
            CollectionView.SetActive(activate);
            ActionDock.SetActive(true);
            MainDock.SetActive(false);
           
        }

        
    }

    public void ShowOptions() 
    {
        if (showing)
        {
            showing = false;
            Debug.Log("Option menu off");
            OptionsView.SetActive(showing);    
        }
        else
        {
            showing = true;
            Debug.Log("option menu on");
            MainDock.SetActive(true);
            OptionsView.SetActive(showing);
        }
    }

  
   
    public void ConsoleMenu()
    {
        ActionDock.SetActive(true);
        Vector3 pos = transform.position;
        pos.y = -200;
        LeanTween.moveY(Console, pos.y, 1).setEaseInSine();
        Debug.Log(Console.transform.position.y);
    }

  

    private void Start()
    {

        MainDock.SetActive(true);
        ActionDock.SetActive(false);
        OptionsView.SetActive(false);
        CollectionView.SetActive(false);
    }
    //Dock Behavior

    private void Update()
    {
       
    }
}
