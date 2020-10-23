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
 * del modelo en la lista de pares llamada parts.
 */
public class PrefabBundle : MonoBehaviour
{
    private LoadBundle _bundleLoader;
    private GameObject model = null;
    private int index = 0;
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
        if (this.transform.childCount == 0)
        {
            InstantiateObjectFromBundle(0);
            index = (index + 1) % (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length);
        }
    }

    // Como los assetbundles pueden tener mas de 1 modelo, esta funcion muestra
    // el siguiente modelo del assetbundle (si ya llego al ultimo vuelve a mostrar el primero)
    public void nextModel()
    {
        if (id != "" && _bundleLoader.myAssetBundles.ContainsKey(id))
        {
            InstantiateObjectFromBundle(index);
            index = (index + 1) % (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length);
        }
    }

    // Muestra el modelo previo del assetbundle (si esta en el primer modelo muestra el ultimo modelo)
    public void previousModel()
    {
        if (id != "" && _bundleLoader.myAssetBundles.ContainsKey(id))
        {
            InstantiateObjectFromBundle(index);
            index = index == 0 ? (_bundleLoader.myAssetBundles[id].GetAllAssetNames().Length - 1) : index - 1;
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
            model.transform.localScale=new Vector3(0.4f,0.4f,0.4f);
            model.AddComponent<RotateAxis>();
            model.AddComponent<LeanPinchScale>();

            parts = new List<Tuple<string, GameObject>>();
            getPartsRecursive(model);

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

    void getPartsRecursive(GameObject gameObject)
    {

        if (containsMesh(gameObject))
        {
            parts.Add(new Tuple<string, GameObject>(gameObject.transform.name, gameObject));
        }

        for (int hijo = 0; hijo < gameObject.transform.childCount; hijo++)
            getPartsRecursive(gameObject.transform.GetChild(hijo).gameObject);

    }

    bool containsMesh(GameObject gameObject)
    {
        return gameObject.GetComponent<MeshRenderer>() != null || gameObject.GetComponent<SkinnedMeshRenderer>() != null;
    }
}
