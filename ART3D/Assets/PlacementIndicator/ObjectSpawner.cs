using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * Instancia el prefabricado 'objectSpawn' en las coordenadas del 'placementIndicator' (el circulo azul)
 * al tocar la pantalla. Esto solo ocurre si la variable canSpawnObject es verdadera y se dio un touch en
 * la pantalla (el touch no vale si fue dado sobre un boton).
 */
public class ObjectSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool canSapwnObject { get; set; }
    public GameObject objectSpawn;
    private PlacementIndicator placementIndicator;
    private LoadBundle _bundleLoader;
    public Text debuglog;


    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        _bundleLoader = FindObjectOfType<LoadBundle>();
        canSapwnObject = true;
    }

    void Update()
    {
        if (canSapwnObject  && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && EventSystem.current.currentSelectedGameObject == null)
        {
            if(_bundleLoader.lastId != "")
            {
                debuglog.text += "Start. Se detecto el click.\n";
                GameObject obj = Instantiate(objectSpawn, placementIndicator.transform.position, placementIndicator.transform.rotation);
                
                // El modelo se enfoca por primera vez dentro del Script PrefabBundle cuando se termina de instanciar
                obj.GetComponent<PrefabBundle>().id = _bundleLoader.lastId;
                obj.tag = "model";
                canSapwnObject = false;
                debuglog.text += "END. Se detecto el click.\n";
            }
            else
            {
                debuglog.text += "El modelo no se puede colocar porque el assetBundle esta vacio.\n";
            }
        }

    }

}
