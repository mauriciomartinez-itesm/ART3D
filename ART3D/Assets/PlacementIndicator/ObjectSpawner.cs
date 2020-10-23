using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Instancia el prefabricado 'objectSpawn' en las coordenadas del 'placementIndicator' (el circulo azul)
 * al tocar la pantalla. Esto solo ocurre si la variable canSpawnObject es verdadera y se dio un touch en
 * la pantalla.
 */
public class ObjectSpawner : MonoBehaviour
{
    [HideInInspector]
    public bool canSapwnObject { get; set; }
    public GameObject objectSpawn;
    private PlacementIndicator placementIndicator;
    private LoadBundle _bundleLoader;
    public Text debuglog;


    // Start is called before the first frame update
    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        _bundleLoader = FindObjectOfType<LoadBundle>();
        canSapwnObject = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSapwnObject && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            debuglog.text += "Start. Se detecto el click.\n";
            GameObject obj = Instantiate(objectSpawn, placementIndicator.transform.position, placementIndicator.transform.rotation);
            obj.GetComponent<PrefabBundle>().id = _bundleLoader.lastId;
            canSapwnObject = false;
            debuglog.text += "END. Se detecto el click.\n";
        }
    }

}
