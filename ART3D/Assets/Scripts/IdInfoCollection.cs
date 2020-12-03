using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/* 
 * Obtiene de Firebase un json con la informacion de los modelos existentes.
 * Deserializa el json utilizando la funcion Deserialize de la clase AssetManager que
 * recorre el json creando 1 sub-json por cada modelo, los cuales son deserealizados a las clases
 * AssetInfo y Solicitor para guardarlos en el diccionario 'assets' cuya llave es el id
 * y su valor es la informacion del modelo.
 */

public class IdInfoCollection : MonoBehaviour
{
    public UnityEvent OnIdLoadingDone = new UnityEvent();
    [HideInInspector]
    public static AssetManager _assetManager;

    void Start()
    {
        StartCoroutine(GetAndDeserialize("https://art3d-e7c95.firebaseio.com/assets.json"));
    }

    // Obtiene el json y lo deserealiza con la funcion Deserialize
    IEnumerator GetAndDeserialize(string url)
    {
        // Parte que obtiene el Json
        var request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        string jsonResponse = request.downloadHandler.text;

        Debug.Log("Respuesta " + jsonResponse);

        // Parte que deserializa el Json
        _assetManager = new AssetManager();
        _assetManager.Deserialize(jsonResponse);

        // Imprime los ids obtenidos
        foreach (var el in _assetManager.assets)
        {
            Debug.Log($"Key: {el.Key} Created by: {el.Value.createdby} Description: {el.Value.desc}");
        }
        if (_assetManager.assets.Count > 0)
            OnIdLoadingDone.Invoke();
        else
            Debug.LogError("El diccionario de Ids esta vacio");
    }
}



[Serializable]
public class AssetManager
{
    public Dictionary<string, AssetInfo> assets = new Dictionary<string, AssetInfo>(); // <mapid, score>

    public void Deserialize(string completeJson)
    {
        
        string id = "";
        string miniJson = "";
        int p1 = 0, p2 = 0, index = 0;

        // Recorre el json completa extrayendo y deserealizando 1 mini-json 
        // en cada iteracion
        while (p2 < completeJson.Length - 3)
        {
            // Processo de extraccion de mini json
            p1 = completeJson.IndexOf('\"', p2);
            p2 = completeJson.IndexOf('\"', p1 + 1);
            id = completeJson.Substring(p1 + 1, p2 - p1 - 1);
            p1 = completeJson.IndexOf('{', p2);
            index = p1 + 1;
            int bracketCount = 1;

            while (bracketCount != 0 && index < completeJson.Length)
            {
                if (completeJson[index] == '{')
                    bracketCount++;
                else if (completeJson[index] == '}')
                    bracketCount--;
                index++;
            }
            p2 = index - 1;
            miniJson = completeJson.Substring(p1, p2 - p1 + 1);

            //Debug.Log($"The id {id}");
            //Debug.Log($"Our mini Json {miniJson}");

            // Proceso de deserializacion del mini-json
            try
            {
                AssetInfo info = JsonUtility.FromJson<AssetInfo>(miniJson);
                if (info.restype == "assetbundle")
                    assets.Add(id, info);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error al añadir los asset al diccionario: {ex.Message}");
            }
        }

        Debug.Log($"El tamaño del diccionario es: {assets.Count}");
    }
}

[Serializable]
public class AssetInfo
{
    public string createdby = "";
    public string desc = "";
    public string name = "";
    public string restype = "";
    public Solicitor solicitor = new Solicitor();
    //public string updatedby = "";
}

[Serializable]
public class Solicitor
{
    public string email = "";
    public string name = "";
    public string role = "";
    public string subject = "";
}