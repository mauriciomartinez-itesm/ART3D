using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class IdInfoCollection : MonoBehaviour
{
    public UnityEvent OnIdLoadingDone = new UnityEvent();
    [HideInInspector]
    public static AssetManager _assetManager;

    void Start()
    {
        StartCoroutine(Get("https://art3d-e7c95.firebaseio.com/assets.json"));
    }

    IEnumerator Get(string url)
    {
        var request = new UnityWebRequest(url);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        string jsonResponse = request.downloadHandler.text;

        Debug.Log("Respuesta " + jsonResponse);

        _assetManager = new AssetManager();
        _assetManager.Deserialize(jsonResponse);

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

        while (p2 < completeJson.Length - 3)
        {
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

[Serializable]
public class PackSender
{
    public float tableSize;
    public int tableNumber;
    public float threshold;
    public float distance;
    public string walkingPath;
    public string polygonName;
    public float scaleX;
    public float scaleY;
}