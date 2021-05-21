using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Firebase.Storage;

public class ImagePreviewCollection : MonoBehaviour
{
    public Sprite[] images = new Sprite[100];
    public int imageIndex = 0;
    public GameObject cardGrid;

    private string firebaseBasePath = "gs://art3d-e7c95.appspot.com/img/";
    private FirebaseStorage storage;

                                                            // Utilizando metodos auxiliares descarga la 
                                                            // preview image de firebase storgae que correponde
                                                            // al id especificado. Despues la asigna a su carta
                                                            // correspondiente.
    async public void DownloadAndSetPreviewImage(string id)
    {
        storage = FirebaseStorage.DefaultInstance;
        string link = await GetFileUrlFromFirebase(id, "jpg");
        if(link=="")
            link = await GetFileUrlFromFirebase(id, "png");

        Debug.Log("link del firebase: " + link);
        if (link != "")
        {
            int localImageIndex = imageIndex;
            imageIndex++;
            StartCoroutine(DownloadAndSetPreviewImageFromUrl(link, id, localImageIndex));
        }
    }

                                                            // Retorna un link temporal que apunta directamente al archivo
                                                            // con la preview image del id especificado.
    private async Task<string> GetFileUrlFromFirebase(string id, string imageExtension)
    {
        string reference_link = $"{firebaseBasePath}{id}.{imageExtension}";
        StorageReference storage_ref = storage.GetReferenceFromUrl(reference_link);

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

                                                            // Utilizando el link descarga la preview image y
                                                            // la asigna a la carta cuyo nombre corresponde al 
                                                            // id especificado.
    private IEnumerator DownloadAndSetPreviewImageFromUrl(string link, string id, int localImageIndex)
    {
        Texture2D image;
                                                             // Descarga de imagen

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(link);
        yield return request.SendWebRequest();
        yield return null;

        if (request.isNetworkError || request.isHttpError)
            Debug.Log("Error en el unity web request: " + request.error);
        else
        {
            Debug.Log("Descarga de Imagen exitosa.");
            image = ((DownloadHandlerTexture)request.downloadHandler).texture;
            
                                                            // Hace una conversion de Texture2D a Sprite
            Rect rec = new Rect(0, 0, image.width, image.height);
            images[localImageIndex] = Sprite.Create(image, rec, new Vector2(0, 0), 1);

                                                            // Le asigna la sprite a la carta cuyo nombre
                                                            // corresponde al id pasado como parametro.
            if (images[localImageIndex] != null)
                cardGrid.transform.Find(id).transform.GetChild(0).GetComponent<Image>().sprite = images[localImageIndex];
            else
                Debug.LogError("Error en la descarga de la preview image");
        }
    }
    
}
