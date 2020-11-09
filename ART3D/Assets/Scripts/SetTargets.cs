

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
    public ARTrackedImageManager trackImageManager;
    public Text debugLog;
    public XRReferenceImageLibrary runtimeImageLibrary;
    public GameObject prefabOnTrack;
    private FirebaseStorage storage;


    async void Start()
    {
        // ************ Esto es tempral para probar la carga de targets utilizando
        // ************ una lista estatica de ids.
        try
        {
            debugLog.text += "Starting to set targets\n";
            storage = FirebaseStorage.DefaultInstance;
            List<string> ids = new List<string>(){ "na201WeCoKxZxqwnVjEa" };
            await DownloadAndAddTargetsFromFirebase(ids);
        }
        catch (Exception ex)
        {
            debugLog.text += "Problema en la descarga y asignacion de targets: " + ex.Message;
        }
    }

    // Usando una lista de ids busca sus targets en FirebaseStorage y obtiene su url de descarga 
    // (esto lo hace con la funcion getFileUrlFromFirebase), despues descarga cada target y lo 
    // añade a la coleccion/libreria de targets (esto lo hace con la funcion 
    // DownloadAndAddTargetsFromUrl) para que los rastree el ARTrackedImageManager.
    public async Task DownloadAndAddTargetsFromFirebase(List<string> ids)
    {
        List<string> links = new List<string>();
        List<string> validIds = new List<string>();

        foreach (var id in ids)
        {
            string link = await getFileUrlFromFirebase(id);
            
            if (link != "")
            {
                links.Add(link);
                validIds.Add(id);
            }
        }

        StartCoroutine( DownloadAndAddTargetsFromUrl(links, validIds) );
    }


    // Descarga los targets utilizando las urls de entrada y los añade
    // a la colleccion/libreria de targets para que ar foundation pueda
    // detectarlas. El nombre de cada target es el id al que esta vinculado.
    private IEnumerator DownloadAndAddTargetsFromUrl(List<string> links, List<string> ids)
    {
        // Empareja el id con su textura.   image.item1 es el id, image.item2 es la textura2D
        List<Tuple<string, Texture2D>> allImages = new List< Tuple<string, Texture2D> > ();

        // Descarga de imagenes
        for (int h=0; h < links.Count; h++)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture( links[h] );
            yield return request.SendWebRequest();
            Texture2D texture2D;


            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                debugLog.text += "Descarga de Imagen exitosa.\n";
                texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;
                allImages.Add( new Tuple<string, Texture2D>( ids[h], texture2D ) );
            }
        }


        // Creacion de coleccion/libreria de targets para ar foundation.
        try
        {
            trackImageManager.referenceLibrary = trackImageManager.CreateRuntimeLibrary(runtimeImageLibrary);
            trackImageManager.maxNumberOfMovingImages = 3;
            trackImageManager.trackedImagePrefab = prefabOnTrack;
            

            trackImageManager.enabled = true;

            MutableRuntimeReferenceImageLibrary mutableRuntimeReferenceImageLibrary = trackImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;

            debugLog.text += $"TextureFormat.RGBA32 supported: {mutableRuntimeReferenceImageLibrary.IsTextureFormatSupported(TextureFormat.RGBA32)}\n";

            foreach(var image in allImages) // image.item1 es el id, image.item2 es la textura2D
            {
                Texture2D texture2D = image.Item2;
                debugLog.text += $"TextureFormat size: {texture2D.width}px width {texture2D.height}px height\n";

                var jobHandle = mutableRuntimeReferenceImageLibrary.ScheduleAddImageJob(texture2D, image.Item1, 0.1f);

                while (!jobHandle.IsCompleted)
                {
                    debugLog.text += "Job Running...\n";
                    Task.Delay(100).Wait();
                }

                //debugLog.text += $"Job Completed ({mutableRuntimeReferenceImageLibrary.count})\n";
                //debugLog.text += $"Supported Texture Count ({mutableRuntimeReferenceImageLibrary.supportedTextureFormatCount})\n";
                //debugLog.text += $"trackImageManager.trackables.count ({trackImageManager.trackables.count})\n";
                //debugLog.text += $"trackImageManager.trackedImagePrefab.name ({trackImageManager.trackedImagePrefab.name})\n";
                //debugLog.text += $"trackImageManager.maxNumberOfMovingImages ({trackImageManager.maxNumberOfMovingImages})\n";
                //debugLog.text += $"trackImageManager.supportsMutableLibrary ({trackImageManager.subsystem.SubsystemDescriptor.supportsMutableLibrary})\n";
                //debugLog.text += $"trackImageManager.requiresPhysicalImageDimensions ({trackImageManager.subsystem.SubsystemDescriptor.requiresPhysicalImageDimensions})\n";
            }
        }
        catch (Exception e)
        {
            debugLog.text += e.ToString();
        }
    }


    private async Task<string> getFileUrlFromFirebase(string id)
    {     
        StorageReference storage_ref = storage.GetReferenceFromUrl("gs://art3d-e7c95.appspot.com/targets/" + id + ".jpg");

        string link = "";
        await storage_ref.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                link = task.Result.ToString();
            }
            else
            {
                debugLog.text += $"Failed to load link. Task Faulted: {task.IsFaulted}\n";
            }
        });

        return link;       
    }

}
