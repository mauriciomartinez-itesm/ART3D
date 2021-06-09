using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


/* 
 * Para cargar assetbundles se debe usar la funcion AsyncAddAssetBundle la 
 * cual puede cargar assetbundles de manera local, desde un web link o desde firebase. Sin importar el metodo
 * de carga, el id debe ser unico, de lo contrario podria no cargarse porque ya existe en cache un assetbundle 
 * con ese id.
 * Cuando se termina de cargar un assetbundle se invoca la funcion/evento onAssetBundleFinishLoad sin importar
 * si fracaso o no. Se pasa como parametro una variable booleana indicando si la operacion fue un exito o no.
 * La cantidad maxima de Asset Bundles que pueden estar cargados en memoria esta definida en la 
 * variable maxCacheAssetBundlesCount. Cuando hay mas asset bundles se elimina de memoria el assetbundle mas  
 * viejo. Esto se realiza en la funcion MaxAssetBundlesInCacheCheck.
 * ADVERTENCIA: Tratar de cargar 2 veces el mismo assetbundle crea un error si no se ha eiminado de
 * memoria (solo puede haber un assetbundle unico a la vez). Si se puede tener N assetbundles pero
 * tienen que ser diferentes.
 */


public class BundleLoader : MonoBehaviour
{  
    public delegate void onAssetBundleFinishLoadHandler(bool succesfullLoad, string assetBundleId);
    public event onAssetBundleFinishLoadHandler onAssetBundleFinishLoad;
    public Text debuglog;

    private Dictionary<string, AssetBundle> myAssetBundles;
    private Queue<string> assetBundleIdRegistry;
    private int maxCacheAssetBundlesCount = 2;
    private FirebaseStorage _firebasestorage;
    private string currentId = ""; 
    private string lastId = "";


    #if UNITY_ANDROID
        private string firebaseBasePath = "gs://art3d-e7c95.appspot.com/assetbundle/android/";
    #elif UNITY_IOS
        private string firebaseBasePath = "gs://art3d-e7c95.appspot.com/assetbundle/ios/";
    #endif

    public void InitBundleLoader()
    {
        _firebasestorage = FirebaseStorage.DefaultInstance;
        myAssetBundles = new Dictionary<string, AssetBundle>();
        assetBundleIdRegistry = new Queue<string>();
    }


    public AssetBundle GetAssetBundle( string id )
    {
        if( myAssetBundles.ContainsKey(id) )
            return myAssetBundles[id];
        return null;
    }

                                                            // Descarga el assetbundle de manera local, con web link o Firebase.
                                                            // El id es un parametro obligatorio mientras que el assetBundlePath
                                                            // es opcional. Si no se define el path se intuye que la descarga
                                                            // sera a traves de Firebase. Si si se especifica el path entonces
                                                            // se checa si es un web link o un path local para realizar su 
                                                            // respectiva carga.
                                                            // Si el id ya existe en myAssetBundles significa que el assetbundle
                                                            // ya esta descargado y no se necesita redescargar.
    public void AsyncAddAssetBundle( string id, string assetBundlePath="" )
    {
        if(id=="")
        {
            debuglog.text += "Error, the ID is empty \n";
            return;
        }

        currentId = id;

        if (myAssetBundles.ContainsKey(id))
        {
            debuglog.text += "Ya esta cargado el AsetBundle \n";
            onAssetBundleFinishLoad?.Invoke( true, id );
            lastId = currentId;
            return;
        }

        if (assetBundlePath == "")
        {
            DownloadAndAddAssetBundleFromFirebase( id );
            return;
        }

        if (assetBundlePath.IndexOf("http") != -1)
            StartCoroutine( DownloadAndAddAssetBundleFromUrl( assetBundlePath, id ) );
        else
            LoadAssetBundle( assetBundlePath, id );

    }


                                                            // Checa si el numero de asset bundles cargados no excede el numero 
                                                            // maixmo permitido (maxCacheAssetBundlesCount). Retorna un string 
                                                            // vacio si no supera el limite, de lo contrario elimina de la cache 
                                                            // el asset budnle mas viejo y retorna su id para que con el se pueda
                                                            // localizar y borrar los prefabs bundles que lo usaban. 
    public string MaxAssetBundlesInCacheCheck()
    {
        string assetBundleIdToRemove = "";

        if (assetBundleIdRegistry.Count > maxCacheAssetBundlesCount)
        {
            assetBundleIdToRemove = assetBundleIdRegistry.Dequeue();
            try
            {
                debuglog.text += $"Unloading AssetBundle with id: {assetBundleIdToRemove}\n";
                myAssetBundles[assetBundleIdToRemove].Unload(true);
                myAssetBundles.Remove(assetBundleIdToRemove);
                debuglog.text += $"Succesfully Unloaded AssetBundle\n";
            }
            catch
            {
                debuglog.text += $"Error in Unloading AssetBundle\n";
            }
        }

        return assetBundleIdToRemove;
    }


    public string GetCurrentId()
    {
        return currentId;
    }

                                                            // Carga de manera local el assetbundle que se encuentra en
                                                            // el path assetBundleFilePath. Al finalizar invoca la funcion
                                                            // onAssetBundleFinishLoad.
    private void LoadAssetBundle(string assetBundleFilePath, string id)
    {
        try
        {
            myAssetBundles.Add(id, AssetBundle.LoadFromFile(assetBundleFilePath));
            lastId = id;
        }
        catch { }

        onAssetBundleFinishLoad?.Invoke( myAssetBundles.ContainsKey(id), id );
    }

                                                            // Utilizando el id se obtiene el url de firbase para descargar el 
                                                            // assetbundle, lo descarga utilizando la funcion 
                                                            // DownloadAndAddAssetBundleFromUrl en donde lo agrega al 
                                                            // diccionario/collecion de assetbundles. 
    private async Task DownloadAndAddAssetBundleFromFirebase(string id)
    {
        try
        {
            debuglog.text += "Starting downloading asset bundle\n";

            string link = await GetFileUrlFromFirebase(id);
            if (link != "")
            {
                StartCoroutine(DownloadAndAddAssetBundleFromUrl(link, id));
            }
            else
            {
                debuglog.text += "Error getting firebase link\n";
                throw new Exception("Error getting firebase link");
            }

        }
                                                            // Si existe un problema en la descarga, el currentId
                                                            // regresa al ultimo id que tuvo exito para evitar que
                                                            // el usuario trate de instanciar del assetbundle que no 
                                                            // existe.
        catch (Exception ex)
        {
            debuglog.text += $"Error en la descarga del asset bundle: {ex.Message}\n";
            currentId = lastId;
        }
    }

                                                            // Retorna un link temporal que apunta directamente al archivo
                                                            // con el assetbundle del id especificado.
    private async Task<string> GetFileUrlFromFirebase(string id)
    {
        StorageReference storage_ref = _firebasestorage.GetReferenceFromUrl(firebaseBasePath + id);

        string link = "";
        await storage_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                link = task.Result.ToString();
            }
        });

        return link;
    }

                                                            // Carga de manera remota el assetbundle que se encuentra en
                                                            // el el web link bundleUrl. Al finalizar invoca la funcion
                                                            // onAssetBundleFinishLoad.
    private IEnumerator DownloadAndAddAssetBundleFromUrl(string bundleUrl, string id)
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
                debuglog.text += $"The name of the asset bundle is: {assetBundletemp} \n";
                if (assetBundletemp != null)
                {
                    myAssetBundles.Add(id, assetBundletemp);
                    debuglog.text += $"Asset Bundle Added to the Dictionary \n";
                    assetBundleIdRegistry.Enqueue(id);
                    lastId = id;

                    debuglog.text += $"Finish Asset Bundle Process \n";
                }
                else
                {
                    debuglog.text += $"The downloaded asset bundle is empty, probably you are " +
                        "trying to load an already existing asset bundle but with a different id. " +
                        "Or the asset bundle file is not valid. \n";
                    currentId = lastId;
                }

            }
        }

        onAssetBundleFinishLoad?.Invoke(myAssetBundles.ContainsKey(id), id);
    }

}