using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/*
 * Este archivo contiene un conjunto de clases usadas para deserealizar y guardar el json que contiene la informacion
 * de los assetbundles. AssetBundleInfoFields se encarga la deserealizacion, ya que unity no cuenta con la capacidad de
 * deserealizar diccionarios es necesario hacerlo manualmente extrayendo la llave y utilizar su contenido como si fuera
 * un json individual el cual si se puede deserealziar con unity. A ese contenido se le llama mini-json en el proceso
 * de deserializacion.
 */

[Serializable]
public class AssetBundleInfoFields
{
    public Dictionary<string, AssetInfo> assetsInfo = new Dictionary<string, AssetInfo>();

    public void Deserialize(string completeJson)
    {

        string id = "";
        string miniJson = "";
        int p1 = 0, p2 = 0, index = 0;

                                                            // Recorre el json completo extrayendo y deserealizando 1 mini-json 
                                                            // en cada iteracion
        while (p2 < completeJson.Length - 3)
        {
            try
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
                AssetInfo info = JsonUtility.FromJson<AssetInfo>(miniJson);
                if (info.restype == "assetbundle")
                    assetsInfo.Add(id, info);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al añadir los asset al diccionario: {ex.Message}");
            }
        }

        Debug.Log($"El tamaño del diccionario es: {assetsInfo.Count}");
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
}

[Serializable]
public class Solicitor
{
    public string email = "";
    public string name = "";
    public string role = "";
    public string subject = "";
}
