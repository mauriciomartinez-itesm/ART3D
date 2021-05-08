using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/* 
 * Obtiene un json de Firebase con la informacion de los modelos existentes.
 * Deserializa el json utilizando la funcion Deserialize de la clase AssetBundleInfoFields que
 * recorre el json creando 1 sub-json por cada modelo, los cuales son deserealizados a las clases
 * AssetInfo y Solicitor para guardarlos en el diccionario 'assetsInfo' cuya llave es el id
 * y su valor es la informacion del modelo.
 * Solo si obtuvo exito en obtener la informacion de los assets invoca la funcion onIdsLoadingDoneHandler.
 */

public class IdInfoCollection : MonoBehaviour
{
    [HideInInspector]
    public AssetBundleInfoFields _assetBundleInfoFields;
    public delegate void onIdsLoadingDoneHandler(Dictionary<string, AssetInfo> assetsInfo);
    public event onIdsLoadingDoneHandler onIdsLoadingDone;


    public void DownloadAndDeserializeIdInfoCollection()
    {
        StartCoroutine( DownloadAndDeserialize( "https://art3d-e7c95.firebaseio.com/assets.json") );
    }

                                                            // Obtiene el json del url de firebase y lo deserealiza con 
                                                            // la funcion Deserialize para obtener los ids e informacion
                                                            // adicional como nombre y descripcion. Si tuvo exito en la
                                                            // descarga de la info de assetbundles entonces invoca el metodo
                                                            // onIdsLoadingDoneHandler.
    private IEnumerator DownloadAndDeserialize(string url)
    {
                                                            // Parte que obtiene el Json
        var request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
        string jsonResponse = request.downloadHandler.text;

        Debug.Log("Respuesta " + jsonResponse);

                                                            // Parte que deserializa el Json
        _assetBundleInfoFields = new AssetBundleInfoFields();
        _assetBundleInfoFields.Deserialize(jsonResponse);

                                                            // Revisa que exista informacion dentro del diccionario assetsInfo
        if (_assetBundleInfoFields.assetsInfo.Count > 0)
        {
                                                            // Imprime los ids obtenidos
            foreach (var el in _assetBundleInfoFields.assetsInfo)
            {
                Debug.Log($"Key: {el.Key} Name: {el.Value.name} Description: {el.Value.description} Tags: {string.Join(", ", el.Value.tags.ToArray())}");
            }

            onIdsLoadingDone.Invoke( _assetBundleInfoFields.assetsInfo );
        }
        else
        {
            Debug.LogError("El diccionario de Ids esta vacio");
        }
        
    }

}



