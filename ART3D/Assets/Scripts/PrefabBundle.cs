using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBundle : MonoBehaviour
{
    private LoadBundle loadBundle;
    private GameObject obj;
    private int index = 0;
    private bool createObj = false;
    private float timer = 0.0f;

    [HideInInspector]
    public List<Tuple<string, GameObject>> partes;

    // Start is called before the first frame update
    void Start()
    {
        loadBundle = FindObjectOfType<LoadBundle>();
        createObj = true;
        partes = new List<Tuple<string, GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // No hace nada si todavia no se termina de cargar el assetBundle.
        if (loadBundle.myAssetBundle == null)
            return;

        // Si no esta mostrando ningun modelo automaticamente muestra el primer
        // prefabricado del assetBundle
        if (this.transform.childCount ==0)
        {
            InstantiateObjectFromBundle(0);
        }

        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButton(0))
        {
            createObj = true;
        }
        if ((Input.touchCount == 0) && Input.GetMouseButton(0) == false)
        {
            if (timer < 0.25f && createObj)
            {
                InstantiateObjectFromBundle(index);
                index = (index + 1) % (loadBundle.myAssetBundle.GetAllAssetNames().Length);
            }
            timer = 0.0f;
            createObj = false;
        }
    }


    public bool InstantiateObjectFromBundle(string ass)
    {
        try
        {
            var prefab = loadBundle.myAssetBundle.LoadAsset(ass);

            deleteAssetGameObject();
            obj = (GameObject)Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform);
            obj.transform.localScale=new Vector3(0.4f,0.4f,0.4f);
            obj.AddComponent<RotateAxis>();
            obj.AddComponent<LeanPinchScale>();

            partes = new List<Tuple<string, GameObject>>();
            getPartsRecursive(obj);

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
            string assetName = loadBundle.myAssetBundle.GetAllAssetNames()[assetIndex];
            var prefab = loadBundle.myAssetBundle.LoadAsset(assetName);

            deleteAssetGameObject();
            obj = (GameObject)Instantiate(prefab, this.transform.position, Quaternion.identity, this.transform);
            obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            obj.AddComponent<RotateAxis>();
            obj.AddComponent<LeanPinchScale>();

            partes = new List<Tuple<string, GameObject>>();
            getPartsRecursive(obj);

            return true;
        }
        catch(Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return false;
        }
    }

    public void deleteAssetGameObject()
    {
        if (obj != null)
            Destroy(obj);
    }

    void getPartsRecursive(GameObject gameObject)
    {

        if (containsMesh(gameObject))
        {
            partes.Add(new Tuple<string, GameObject>(gameObject.transform.name, gameObject));
        }

        for (int hijo = 0; hijo < gameObject.transform.childCount; hijo++)
            getPartsRecursive(gameObject.transform.GetChild(hijo).gameObject);

    }

    bool containsMesh(GameObject gameObject)
    {
        return gameObject.GetComponent<MeshRenderer>() != null || gameObject.GetComponent<SkinnedMeshRenderer>() != null;
    }
}
