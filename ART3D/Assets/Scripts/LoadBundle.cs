using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/* 
 * Se encarga de cargar el AssetBundle, lo puede cargar de manera local o de internet.
 * Puede cargar el AssetBundle desde el inicio si la variable loadOnStart esta activada
 * o puede ser cargado llamando a la funcion DownloadAssetBundleFromUrl cuyo parametro debe ser
 * la liga directa al AssetBundle y un id unico. Tambien se puede descargar de FirebaseStorage
 * usando la funcion DownloadAssetBundleFromFirebase cuyo parametro es el id del assetbundle
 * que se desea descargar.
 * ADVERTENCIA: Tratar de cargar 2 veces el mismo assetbundle crea un error si no se ha eiminado de
 * memoria (solo puede haver un assetbundle unico a la vez). Si se puede tener N assetbundles pero
 * tienen que ser diferentes.
 */

public class LoadBundle : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, AssetBundle> myAssetBundles;

    private Queue<string> registry;

    private int maxCacheAssetBundles = 5;

    [HideInInspector]
    public string lastId = "";

    [Tooltip("La direccion en la cual se encuentra el archivo AssetBundle, si se obtendra de internet debe de ser un url directo al archivo.")]
    public string pathForLoadOnStart;

    [Tooltip("Desactivar para AssetBundles locales y activar para AssetBundles de internet.")]
    public bool webService;

    [Tooltip("Activar para cargar el AssetBundle desde el inicio del programa.")]
    public bool loadOnStart;

    private FirebaseStorage _firebasestorage;
    public Text debuglog;


    void Start()
    {

        _firebasestorage = FirebaseStorage.DefaultInstance;
        myAssetBundles = new Dictionary<string, AssetBundle>();
        registry = new Queue<string>();

        if (loadOnStart)
        {
            if(pathForLoadOnStart=="")
            {
                Debug.LogError("El pathForLoadOnStart esta vacio");
                return;
            }

            if (webService)
                StartCoroutine(DownloadAssetBundleFromUrl(pathForLoadOnStart, "Test"));
            else
                LoadAssetBundle(pathForLoadOnStart, "LocalTest");
        }

        Application.targetFrameRate = 60;
    }


    // Utilizando el id se obtiene el url de firbase para descargar el assetbundle,
    // lo descarga y lo agrega al diccionario/collecion de assetbundles. Si el id ya
    // existe en myAssetBundles significa que el assetbundle ya esta descargado y no
    // se necesita redescargar.
    public async Task DownloadAssetBundleFromFirebase(string id)
    {
        try
        {
            debuglog.text += "Starting downloading asset bundle\n";
            
            if (myAssetBundles.ContainsKey(id))
            { lastId = id; debuglog.text += "Ya esta cargado el AsetBundle\n"; return; }

            string link = await getFileUrlFromFirebase(id);
            if (link != "")
            {
                StartCoroutine(DownloadAssetBundleFromUrl(link, id));
            }
            else
            {
                debuglog.text += "Error getting firebase link\n";
            }
            
        }
        catch (Exception ex)
        {
            debuglog.text += $"Error en la descarga del asset bundle: {ex.Message}\n";
        }
    }

    private void LoadAssetBundle(string bundleUrl, string id)
    {
        myAssetBundles.Add(id , AssetBundle.LoadFromFile(bundleUrl));
        Debug.Log(myAssetBundles[id] == null ? " Failed to load asset bundle " : " Asset bundle succesfully Loaded");
    }


    private IEnumerator DownloadAssetBundleFromUrl(string bundleUrl, string id)
    {

        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl))
        {
            yield return uwr.SendWebRequest();
            
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Añade el Asset bundle al diccionario para accesarlo desde el prefabBundle
                debuglog.text += $"Before dic add\n";
                myAssetBundles.Add(id, DownloadHandlerAssetBundle.GetContent(uwr));
                //myAssetBundles[id].
                debuglog.text += $"After dic add\n";
                registry.Enqueue(id);
                lastId = id;

                if (registry.Count > maxCacheAssetBundles)
                {
                    removeAssetBundle(registry.Dequeue());
                }
            }
        }

        Debug.Log(myAssetBundles[id] == null ? " Failed to load asset bundle " : " Asset bundle succesfully Loaded");
        debuglog.text += myAssetBundles[id] == null ? " Failed to load asset bundle \n" : " Finish downloading asset bundle\n";
    }


    private async Task<string> getFileUrlFromFirebase(string id)
    {
        StorageReference storage_ref = _firebasestorage.GetReferenceFromUrl("gs://art3d-e7c95.appspot.com/assets/" + id );

        string link = "";
        await storage_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                link = task.Result.ToString();
            }
        });

        return link;

    }

    // Si se cargo un nuevo AssetBundle y ya hay mas de maxCacheAssetBundles se llama 
    // a esta funcion para borrar el asset bundle
    private void removeAssetBundle(string id)
    {
        try
        {
            debuglog.text += $"Destroying AssetBundle with id: {id}\n";
            myAssetBundles[id].Unload(true);
            myAssetBundles.Remove(id);
            // Aqui se deben de borrar los prefabBundles que usaban el assetbundle
            // que se esta removiendo.
            //PrefabBundle leftover = FindObjectOfType<PrefabBundle>().GetComponent<PrefabBundle>();
            //leftover.deleteAssetGameObject();
        }
        catch
        {

        }
    }
}
