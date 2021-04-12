using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/*
 * Es el responsable del raycast que proyecta el cursor. El cursor es el hijo
 * del objeto que tiene este script.
 */
public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;

    void Start ()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
    }
    
    void Update ()
    {
                                                            // Dispara el raycast desde el centro de la pantalla
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

                                                            // Si el raycast choca contra el plano/superfice AR, se actualiza  
                                                            // la posicion y rotacion del GameObject que contiene este script.
        if(hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }
}