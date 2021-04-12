using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Ejecuta la funcion/evento spawnFunction si la variable canSpawnObject es verdadera 
 * y se dio un touch en la pantalla (el touch no vale si fue dado sobre un boton).
 */

public class ModelSpawner : MonoBehaviour
{
    public delegate void SpawnFunctionHandler(GameObject placementIndicator);
    public event SpawnFunctionHandler spawnFunction;
    public PlacementIndicator placementIndicator;
    public Text debuglog;

    private bool canSpawnModel { get; set; }

    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        canSpawnModel = true;
    }

    void Update()
    {
        if ( canSpawnModel  && isTouchOrMouseDown() )
        {
            debuglog.text += "Start. Se detecto el click.\n";
            spawnFunction?.Invoke( placementIndicator.gameObject );
                
            canSpawnModel = false;
            debuglog.text += "END. Se detecto el click.\n";
        }

    }

    public void SetCanSpawnModel(bool canSpawn)
    {
        canSpawnModel = canSpawn;
    }

                                                            // Retorna true si el touch o el click izquierdo del mouse han sido 
                                                            // presionados, siempre y cuando no sea sobre un elemento de la UI.
    bool isTouchOrMouseDown()
    {
        return ( ( Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began ) ||
            ( Input.GetMouseButtonDown(0) ) ) &&
            EventSystem.current.currentSelectedGameObject == null;
    }

}
