using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UX_Helper : MonoBehaviour
{
    private bool state = false;
    private bool activate = false;
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
            
            MainDock.SetActive(false);
            ActionDock.SetActive(true);
            CollectionView.SetActive(true);
            //crea las cards

            for (int i = 0; i < 2; i++)
            {

                GameObject NewCard = Instantiate(CardPrefab, transform.position, transform.rotation) as GameObject;
                NewCard.transform.SetParent(GameObject.FindGameObjectWithTag("Cardpanel").transform, false);


            }
            

        }
        else
        {
            activate = true;
            
            CollectionView.SetActive(false);
            ActionDock.SetActive(false);
            MainDock.SetActive(true);
           
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

    public void CloseConsole()
    {
        ActionDock.SetActive(false);
        MainDock.SetActive(true);

        Vector3 pos = transform.position;
        pos.y = -1090;
        LeanTween.moveY(Console, pos.y, 1f).setEaseOutSine();


    }

    public void CollectionClick()
    {
        if (click)
        {
            click = true;
            Debug.Log("click listener on");
        }
        else
        {
            click = false;
            Debug.Log("click listener off");

        }
    }

   

    private void Start()
    {
        ShowDock();
        MainDock.SetActive(true);
        ActionDock.SetActive(false);
        OptionsView.SetActive(false);
        CollectionView.SetActive(false);

        CollectionBtn.onClick.AddListener(CollectionClick);
    }
    //Dock Behavior

    private void Update()
    {
       
    }
}
