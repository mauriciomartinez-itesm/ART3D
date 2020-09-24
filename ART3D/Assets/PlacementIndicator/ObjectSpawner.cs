using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Instancia el prefabricado 'objectSpawn' en las coordenadas del 'placementIndicator' (el circulo azul)
 * al tocar la pantalla. Esto solo ocurre si no existe en la escena un objeto de tipo 'prefabBundle', porque
 * solo deseamos visualizar 1 prefabricado a la vez.
 * 
 */
public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectSpawn;
    private PlacementIndicator placementIndicator;

    // Start is called before the first frame update
    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();
    }

    // Update is called once per frame
    void Update()
    {
        PrefabBundle prefabBundle = FindObjectOfType<PrefabBundle>();
        if (prefabBundle==null)
        {
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                GameObject obj = Instantiate(objectSpawn, placementIndicator.transform.position,
                    placementIndicator.transform.rotation);
            }
        }
    }
}
