using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Firebase.Storage;

/* 
 * Esta clase se encarga de construir el conjunto/libreria de targets para
 * asignarselo al ARTrackedImageManager. Las imagenes/targets son descargadas
 * de Firestorage usando un id, este id es del assetbundle que se desea mostrar
 * cuando el target sea detectado por la camara. En esta clase ni en ninguna otra
 * se cambian los ids, los ids ya vienen prestablecidos en firebase y son usados
 * para mapear los targets a los assetbundles. 
 */

public class SetTargets : MonoBehaviour
{  
    public XRReferenceImageLibrary runtimeImageLibrary;
    public ARTrackedImageManager trackImageManager;
    public GameObject prefabOnTrack;
    public Text debugLog;

    private string firebaseBasePath = "gs://art3d-e7c95.appspot.com/img/";
    private FirebaseStorage storage;

                                                            // Utilizando el diccionario assetsInfo crea una lista de los
                                                            // ids y se la pasa al metodo DownloadAndAddTargetsFromFirebase
                                                            // para descargar y agregar los targets.
    public async void DownloadAndAddTargets(Dictionary<string, AssetInfo> assetsInfo)
    {
        storage = FirebaseStorage.DefaultInstance;

        try
        {
            debugLog.text += "Starting to set targets\n";

                                                            // Crea la lista de ids extrayendo las llaves del diccionario
                                                            // assetInfo.
            List<string> ids = new List<string>();
            int counter = 16;
            foreach (var id in assetsInfo)
            {
                ids.Add(id.Key);
                counter--;
                if (counter == 0)
                    break;
            }

            await DownloadAndAddTargetsFromFirebase(ids);
        }
        catch (Exception ex)
        {
            debugLog.text += $"Problema en la descarga y asignacion de targets: {ex.Message} \n";
        }
    }

                                                            // Usando una lista de ids busca sus targets en FirebaseStorage y  
                                                            // obtiene su url de descarga (esto lo hace con la funcion 
                                                            // getFileUrlFromFirebase), despues descarga cada target y lo añade 
                                                            // a la coleccion/libreria de targets (esto lo hace con la funcion 
                                                            // DownloadAndAddTargetsFromUrl) para que los rastree el 
                                                            // ARTrackedImageManager.
    private async Task DownloadAndAddTargetsFromFirebase(List<string> ids)
    {
        List<string> links = new List<string>();
        List<string> validIds = new List<string>();

        foreach (var id in ids)
        {
            string link = await GetFileUrlFromFirebase(id);

            if (link != "")
            {
                links.Add(link);
                validIds.Add(id);
            }
        }

        StartCoroutine(DownloadAndAddTargetsFromUrl(links, validIds));
    }

                                                            // Retorna un link temporal que apunta directamente al archivo
                                                            // con el assetbundle del id especificado.
    private async Task<string> GetFileUrlFromFirebase(string id)
    {
        string reference_link = $"{firebaseBasePath}{id}.jpg";
        StorageReference storage_ref = storage.GetReferenceFromUrl( reference_link );
        //debugLog.text += $"Reference link: {reference_link}\n";

        string link = "";
        await storage_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                link = task.Result.ToString();
            }
            else
            {
                //debugLog.text += $"Failed to load link. Task Faulted: {task.IsFaulted} \n";
            }
        });

        return link;
    }


                                                            // Descarga los targets utilizando las urls de entrada y los 
                                                            // añade a la colleccion/libreria de targets para que AR Foundation 
                                                            // pueda detectarlas. El nombre de cada target es el id al que esta 
                                                            // vinculado.
    private IEnumerator DownloadAndAddTargetsFromUrl(List<string> links, List<string> ids)
    {
                                                            // Empareja el id con su textura.   
                                                            // image.item1 es el id
                                                            // image.item2 es la textura2D
        List<Tuple<string, Texture2D>> allImages = new List<Tuple<string, Texture2D>>();

                                                            // Descarga de imagenes
        for (int h = 0; h < links.Count; h++)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(links[h]);
            yield return request.SendWebRequest();
            Texture2D texture2D;


            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                debugLog.text += "Descarga de Imagen exitosa.\n";
                texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;
                allImages.Add(new Tuple<string, Texture2D>(ids[h], texture2D));
            }
        }


                                                            // Creacion de coleccion/libreria de targets para AR foundation.
        try
        {
            trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
            trackImageManager.maxNumberOfMovingImages = 3;
            trackImageManager.trackedImagePrefab = prefabOnTrack;


            trackImageManager.enabled = true;

            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            debugLog.text += $"TextureFormat.RGBA32 supported: {mutableRuntimeReferenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)} \n";

                                                            // Recorre la lista de pares para añadir la textura2D/Target
                                                            // a la mutableRuntimeReferenceImageLibrary con el id como nombre.
                                                            // image.item1 es el id
                                                            // image.item2 es la textura2D
            foreach (var image in allImages) 
            {
                Texture2D texture2D = image.Item2;
                debugLog.text += $"TextureFormat size: {texture2D.width}px width {texture2D.height}px height \n";

                var jobHandle = mutableRuntimeReferenceImageLibrary.ScheduleAddImageJob(texture2D, image.Item1, 0.1f);

                while (!jobHandle.IsCompleted)
                {
                    debugLog.text += "Job Running... \n";
                    Task.Delay(100).Wait();
                }

            }
        }
        catch (Exception e)
        {
            debugLog.text += e.ToString() + "\n";
        }
    }

}
