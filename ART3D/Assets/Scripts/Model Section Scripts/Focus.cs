using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Se encarga del proceso de "enfocado" y "desenfocado". Tiene 1 sola funcion
 * que enfoca un nuevo modelo y desenfoca el modelo que estaba siendo enfocado.
 * Desenfocar un modelo significa que se bloquean sus interacciones y se esconden sus iconos
 * que señalan que esta siendo enfocado. Para enfocar un modelo se desbloquean sus 
 * interacciones y se muestran sus iconos que señalan que esta siendo enfocado.
 */

public class Focus : MonoBehaviour
{
    public bool shouldHideBottomMarker = false;
    private GameObject focusedGameObject=null;
    private int childIndexMarkers = 0;
    private int childIndexModel = 1;

    public GameObject GetFocusedGameObject()
    {
        return focusedGameObject;
    }

    public void FocusObject( GameObject gameObjectToFocus)
    {
        Debug.Log("Is focused game object == null ? " + (focusedGameObject == null).ToString() );


                                                            // Si ya habia un modelo enfocado lo desenfoca bloqueando
                                                            // sus habilidades de rotar y escalar y desactiva sus iconos.
        if ( focusedGameObject!=null )
        {
            try
            {
                focusedGameObject.transform.GetChild(childIndexMarkers).gameObject.SetActive(false); // Desactivacion de iconos
                focusedGameObject.GetComponentInChildren<RotateAxis>().enabled = false;
                focusedGameObject.GetComponentInChildren<LeanPinchScale>().enabled = false;                
            }
            catch
            {
                                                            // Si llega a tener algun problema desenfocando el modelo, prueba si
                                                            // es porque el modelo/hijo esta desactivo. Empieza verificando si 
                                                            // tiene hijo y si este esta desactivo, despues la activa para tratar 
                                                            // de desenfocarlo y si no funciona lo borra.
                if (focusedGameObject.transform.childCount>1 && !focusedGameObject.transform.GetChild(childIndexModel).gameObject.activeSelf)
                {
                    try
                    {
                        focusedGameObject.transform.GetChild(childIndexModel).gameObject.SetActive(true);
                        focusedGameObject.transform.GetChild(childIndexMarkers).gameObject.SetActive(false);
                        focusedGameObject.GetComponentInChildren<RotateAxis>().enabled = false;
                        focusedGameObject.GetComponentInChildren<LeanPinchScale>().enabled = false;
                        focusedGameObject.transform.GetChild(childIndexModel).gameObject.SetActive(false);
                    }
                    catch { Destroy(focusedGameObject); }
                }
                else { Destroy(focusedGameObject); }
            }
        }

        focusedGameObject = gameObjectToFocus;

        try
        {
                                                            // Trata de enfocar el modelo habilitando la rotacion y la escala,
                                                            // esto solo funciona si el modelo ya existia.
            focusedGameObject.GetComponentInChildren<RotateAxis>().enabled = true;
            focusedGameObject.GetComponentInChildren<LeanPinchScale>().enabled = true;
            Debug.Log("Scripts activated");
        }
        catch
        {
                                                            // Quiere decir que hubo un problema y probablemente el modelo 
                                                            // todavia no era instanciado por eso no encuentra el script RotateAxis 
                                                            // y LeanPinchScale.
        }

                                                            // Hace aparecer los iconos de enfoque
        focusedGameObject.transform.GetChild(childIndexMarkers).gameObject.SetActive(true);

                                                            // Si estamos en la vista del modelo sin AR entpnces
                                                            // no se desea usar el marcador inferior entonces se
                                                            // deshabilita.
        if (shouldHideBottomMarker)
            focusedGameObject.transform.GetChild(childIndexMarkers).GetChild(0).gameObject.SetActive(false);

    }
}
