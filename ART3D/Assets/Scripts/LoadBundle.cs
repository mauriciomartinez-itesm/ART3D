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
 * La cantidad maxima de Asset Bundles que pueden estar cargados en memoria esta definida en la variable
 * maxCacheAssetBundles. Cuando hay mas asset bundles se elimina de memoria el assetbundle mas viejo. Esto
 * se realiza en el Update.
 * ADVERTENCIA: Tratar de cargar 2 veces el mismo assetbundle crea un error si no se ha eiminado de
 * memoria (solo puede haber un assetbundle unico a la vez). Si se puede tener N assetbundles pero
 * tienen que ser diferentes.
 */

public class LoadBundle : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, AssetBundle> myAssetBundles;

    private Queue<string> registry;

    private int maxCacheAssetBundles = 2;

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

    // Revisa en cada Frame si la cantidad de asset bundles en el cache,
    // si es mayor de maxCacheAssetBundles entonces se elimina el assetbundle
    // mas viejo. Esto se realiza en el Update porque la eliminacion de los modelos
    // necesita hacerse en el thread prinicpal para evitar problemas.
    void Update()
    {
        if (registry.Count > maxCacheAssetBundles)
        {
            removeAssetBundle(registry.Dequeue());
        }
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
                AssetBundle assetBundletemp = DownloadHandlerAssetBundle.GetContent(uwr);
                debuglog.text += $"The name of the asset bundle is: {assetBundletemp}\n";
                if(assetBundletemp!=null)
                {
                    myAssetBundles.Add(id, assetBundletemp);
                    debuglog.text += $"Asset Bundle Added to the Dictionary\n";
                    registry.Enqueue(id);
                    lastId = id;

                    debuglog.text += $"Finish Asset Bundle Process\n";
                }
                else
                {
                    debuglog.text += $"The downloaded asset bundle is empty, probably you are trying to load an already existing asset bundle but with a different id. Or the asset bundle file is not valid.\n";
                }
                
            }
        }
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
    // a esta funcion para borrar el asset bundle y los modelos puestos en escena.
    private void removeAssetBundle(string id)
    {
        try
        {
            debuglog.text += $"Destroying AssetBundle with id: {id}\n";
            myAssetBundles[id].Unload(true);
            myAssetBundles.Remove(id);
            // Aqui se borran los prefabBundles que usaban el assetbundle
            // que se esta removiendo.
            var all_prefab_bundles = GameObject.FindObjectsOfType<PrefabBundle>();
            foreach (var prefab_bundle in all_prefab_bundles)
                if(prefab_bundle.id == id)
                {
                    // Si es un modelo individual (no proviene de un target) se elimina
                    // todo el objeto completo
                    if (prefab_bundle.gameObject.transform.tag == "model")
                    {
                        Destroy(prefab_bundle.gameObject);
                    }
                    // Si es un modelo de un target solo se elimina el modelo del objeto
                    // ya que el objeto es necesario para identificar la imagen.
                    else
                    {
                        prefab_bundle.id = "";
                        debuglog.text += $"Start Borrando model del Target. Child 1: {prefab_bundle.transform.GetChild(0)} Child 2: {prefab_bundle.transform.GetChild(1)}\n";
                        try
                        { prefab_bundle.deleteAssetGameObject(); }
                        catch (Exception ex)
                        { debuglog.text += $"El problema en el borrado del modelo del target es: {ex.Message}"; }
                        debuglog.text += $"End Borrando model del Target. Child Count: {prefab_bundle.transform.childCount}\n";
                    }
                }
                    
        }
        catch
        {

        }
    }
}
