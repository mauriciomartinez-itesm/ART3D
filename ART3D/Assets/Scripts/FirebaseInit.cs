using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using Firebase;
using Firebase.Extensions;

/*
 * Se encarga de establecer la comunicacion con Firebase. Tiene una lista
 * de eventos 'OnFirebaseInitialized' que se disparan cuando se valida
 * la comunicacion con Firebase, esto se usa para habilitar scripts que
 * utilizan algun servicio de firebase para que lo usen despues de que la
 * comunicacion haya sido completada.
 * ADVERTENCIA: No usar servicios de firebase sin antes haber concretado la
 * comunicacion porque generaran errores. Por esto es importante usar los
 * eventos 'OnFirebaseInitialized'.
 */
public class FirebaseInit : MonoBehaviour
{
    public UnityEvent OnFirebaseInitialized = new UnityEvent();
    public Text debugLog;
    

    void Start()
    {
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                debugLog.text += "\nStarting Firebase Init\n";
                if (task.Exception != null)
                {
                    debugLog.text += $"Failed to init Firebase with {task.Exception}\n";
                    return;
                }
                debugLog.text += "Trying to Invoke\n";
                OnFirebaseInitialized.Invoke();
                debugLog.text += "Firebase Init OK\n";
            }
            );
        }
        catch(Exception ex)
        {
            debugLog.text += $"Failed to init Firebase with error: {ex.Message}\n";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
