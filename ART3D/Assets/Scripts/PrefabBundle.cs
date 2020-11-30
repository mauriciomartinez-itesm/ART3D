using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Este script se carga en un objeto vacio e invoca los modelos del assetbundle
 * como sus hijos. Solo puede invocar estos modelos cuando su id no esta vacio
 * y ademas existe el assetbundle con ese id en el _bundleLoader.myAssetBundles.
 * Cuando carga un modelo le añade ciertos scripts para darle interaccion al objeto,
 * por ejemplo: rotacion y escalamiento. Ademas se guardan todas las partes visibles
 * del modelo en la lista de partes llamada parts y se añade un box collider para cada
 * parte.
 */
public class PrefabBundle : MonoBehaviour
{
    private LoadBundle _bundleLoader;
    private GameObject model = null;
    private float high_y=int.MinValue, low_y=int.MaxValue;
    private int model_index = 0;
    public string id = "";

    [HideInInspector]
    public List<Tuple<string, GameObject>> parts;


    void Start()
    {
        _bundleLoader = FindObjectOfType<LoadBundle>();
        parts = new List<Tuple<string, GameObject>>();
    }


    void Update()
    {

        // No hace nada si todavia no se termina de cargar el assetBundle.
        if (id == "" || !_bundleLoader.myAssetBundles.ContainsKey(id))
        {
            deleteAssetGameObject();
            return;
        }

        // Si no esta mostrando ningun modelo automaticamente muestra el primer
        // prefabricado del assetBundle
        if (this.transform.childCount == 1)
        {
            InstantiateObjectFromBundle(0);
            model_index = (model_index + 1) % (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length);
        }

    }

    // Como los assetbundles pueden tener mas de 1 modelo, esta funcion muestra
    // el siguiente modelo del assetbundle (si ya llego al ultimo vuelve a mostrar el primero)
    public void nextModel()
    {
        if (id != "" && _bundleLoader.myAssetBundles.ContainsKey(id))
        {
            InstantiateObjectFromBundle(model_index);
            model_index = (model_index + 1) % (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length);
        }
    }

    // Muestra el modelo previo del assetbundle (si esta en el primer modelo muestra el ultimo modelo)
    public void previousModel()
    {
        if (id != "" && _bundleLoader.myAssetBundles.ContainsKey(id))
        {
            InstantiateObjectFromBundle(model_index);
            model_index = model_index == 0 ? (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length - 1) : model_index - 1;
        }
    }


    // Invoca el modelo del assetbundle que le corresponde al id y le agrega scripts
    // para agregarle interacciones. Cuando invoca un nuevo modelo se encarga de disponer
    // del modelo previo, solo puede haber 1 modelo por instancia de PrefabBundle.
    public bool InstantiateObjectFromBundle(string ass)
    {
        try
        {
            var prefab = _bundleLoader.myAssetBundles[id].LoadAsset(ass);

            deleteAssetGameObject();
            model = (GameObject)Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform);
            model.AddComponent<RotateAxis>();
            model.AddComponent<LeanPinchScale>();
            model.AddComponent<OnObjectClick>();

            parts = new List<Tuple<string, GameObject>>();
            getPartsRecursiveAndAddColliders(model);

            // Modifica la escala del modelo para que todos los modelos empiezen con la
            // misma altura.
            float newScale = (high_y - low_y) * 0.5f / (high_y - low_y);
            model.transform.localScale = new Vector3(newScale, newScale, newScale);

            // Cuando todo el proceso de instanciado del modelo salio bien, se enfoca el modelo.
            Focus.focusObject(this.gameObject);

            return true;
        }
        catch(Exception ex)
        {
            Debug.LogError("Error:" + ex.Message);
            return false;
        }
    }



    public bool InstantiateObjectFromBundle(int assetIndex)
    {
        try
        {
            string assetName = _bundleLoader.myAssetBundles[id].GetAllAssetNames()[assetIndex];
            return InstantiateObjectFromBundle(assetName);        
        }
        catch(Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return false;
        }
    }

    public void deleteAssetGameObject()
    {
        if (model != null)
        {
            Destroy(model);
            model = null;
        }
    }

    void getPartsRecursiveAndAddColliders(GameObject gameObject)
    {

        if (containsMesh(gameObject))
        {
            parts.Add(new Tuple<string, GameObject>(gameObject.transform.name, gameObject));

            // Obtiene las medidas de cada render
            Vector3 partCenter  = gameObject.GetComponent<MeshRenderer>().bounds.center;
            Vector3 partSize    = gameObject.GetComponent<MeshRenderer>().bounds.size;
            Vector3 denominator = new Vector3(1,1,1);

            Transform tempTransform = model.transform;

            while(tempTransform != null)
            {
                denominator.x *= tempTransform.localScale.x;
                denominator.y *= tempTransform.localScale.y;
                denominator.z *= tempTransform.localScale.z;
                tempTransform = tempTransform.parent;
            }

            partSize.x /= denominator.x;
            partSize.y /= denominator.y;
            partSize.z /= denominator.z;

            partCenter -= model.transform.position;
            partCenter.x /= denominator.x;
            partCenter.y /= denominator.y;
            partCenter.z /= denominator.z;

            // Crea un box collider por cada pieza (cada redner)
            BoxCollider tempBox = model.AddComponent<BoxCollider>();
            tempBox.size = partSize;
            tempBox.center = partCenter;

            // Informacion que sera utilizada para re-escalar el modelo
            // para que todos sean de la misma altura.
            high_y = Math.Max(high_y, partCenter.y + (partSize.y) / 2);
            low_y = Math.Min(low_y, partCenter.y - (partSize.y) / 2);
        }

        for (int hijo = 0; hijo < gameObject.transform.childCount; hijo++)
            getPartsRecursiveAndAddColliders(gameObject.transform.GetChild(hijo).gameObject);

    }

    bool containsMesh(GameObject gameObject)
    {
        return gameObject.GetComponent<MeshRenderer>() != null || gameObject.GetComponent<SkinnedMeshRenderer>() != null;
    }
}
