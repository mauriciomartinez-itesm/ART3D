using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/* 
 * Se encarga de cargar el AssetBundle, lo puede cargar de manera local o de internet.
 * Puede cargar el AssetBundle desde el inicio si la variable loadOnStart esta activada
 * o puede ser cargado llamando a la funcion DownloadAssetBundle cuyo parametro debe ser
 * la liga directa al AssetBundle.
 */

public class LoadBundle : MonoBehaviour
{
    [HideInInspector]
    public AssetBundle myAssetBundle;

    [Tooltip("La direccion en la cual se encuentra el archivo AssetBundle, si se obtendra de internet debe de ser un url directo al archivo.")]
    public string pathForLoadOnStart;

    [Tooltip("Desactivar para AssetBundles locales y activar para AssetBundles de internet.")]
    public bool webService;

    [Tooltip("Activar para cargar el AssetBundle desde el inicio del programa.")]
    public bool loadOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (loadOnStart)
        {
            if(pathForLoadOnStart=="")
            {
                Debug.LogError("El pathForLoadOnStart esta vacio");
                return;
            }

            if (webService)
                StartCoroutine(DownloadAssetBundle(pathForLoadOnStart));
            else
                LoadAssetBundle(pathForLoadOnStart);
        }

        Application.targetFrameRate = 60;
    }


    public void LoadAssetBundle(string bundleUrl)
    {
        myAssetBundle = AssetBundle.LoadFromFile(bundleUrl);
        Debug.Log(myAssetBundle == null ? " Failed to load asset bundle " : " Asset bundle succesfully Loaded");
        tryDeleteLeftovers();
    }


    public IEnumerator DownloadAssetBundle(string bundleUrl)
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
                // Get downloaded asset bundle
                myAssetBundle = DownloadHandlerAssetBundle.GetContent(uwr);
            }
        }

        tryDeleteLeftovers();
        Debug.Log(myAssetBundle == null ? " Failed to load asset bundle " : " Asset bundle succesfully Loaded");
    }

    // Si se cargo un nuevo AssetBundle se llama a esta funcion para asegurarse que no quedo un prefabricado
    // del AssetBundle pasado en la escena. Si si hay uno lo borra.
    private void tryDeleteLeftovers()
    {
        try
        {
            PrefabBundle leftover = FindObjectOfType<PrefabBundle>().GetComponent<PrefabBundle>();
            leftover.deleteAssetGameObject();
        }
        catch
        {

        }
    }
}
