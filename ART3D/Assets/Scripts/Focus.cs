﻿using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Se encarga del proceso de "enfocado" y "desenfocado". Tiene 1 sola funcion
 * que enfoca un nuevo modelo y desenfoca el modelo que estaba siendo enfocado.
 * Para desenfocar un modelo se bloquean sus interacciones y se esconden sus iconos
 * que señalan que esta siendo enfocado. Para enfocar un modelo se desbloquean sus 
 * interacciones y se muestran sus iconos que señalan que esta siendo enfocado.
 */

public class Focus : MonoBehaviour
{
    [HideInInspector]
    public static GameObject onFocus=null;
    private static int childIndexModel = 1;
    private static int childIndexMarkers = 0;

    public static void focusObject(GameObject obj)
    {
        // Si ya habia un modelo enfocado lo desenfoca bloqueando
        // sus habilidades de rotar y escalar y desactiva sus iconos.
        if(onFocus!=null)
        {
            try
            {
                onFocus.transform.GetChild(childIndexMarkers).gameObject.SetActive(false); // Desactivacion de iconos
                onFocus.GetComponentInChildren<RotateAxis>().enabled = false;
                onFocus.GetComponentInChildren<LeanPinchScale>().enabled = false;                
            }
            catch
            {
                // Si llega a tener algun problema desenfocando el modelo, prueba si
                // es porque el modelo/hijo esta desactivo. Empieza verificando si tiene hijo
                // y si este esta desactivo, despues la activa para tratar de desenfocarlo
                // y si no funciona lo borra.
                if (onFocus.transform.childCount>1 && !onFocus.transform.GetChild(childIndexModel).gameObject.activeSelf)
                {
                    try
                    {
                        onFocus.transform.GetChild(childIndexModel).gameObject.SetActive(true);
                        onFocus.transform.GetChild(childIndexMarkers).gameObject.SetActive(false);
                        onFocus.GetComponentInChildren<RotateAxis>().enabled = false;
                        onFocus.GetComponentInChildren<LeanPinchScale>().enabled = false;
                        onFocus.transform.GetChild(childIndexModel).gameObject.SetActive(false);
                    }
                    catch { Destroy(onFocus); }
                }
                else { Destroy(onFocus); }
            }
        }

        onFocus = obj;

        try
        {
            // Trata de enfocar el modelo habilitando la rotacion y la escala,
            // esto solo funciona si el modelo ya existia.
            onFocus.GetComponentInChildren<RotateAxis>().enabled = true;
            onFocus.GetComponentInChildren<LeanPinchScale>().enabled = true;
        }
        catch
        {
            // Quiere decir que hubo un problema y probablemente el modelo todavia no era instanciado
            // por eso no encuentra el script RotateAxis y LeanPinchScale
        }

        // Hace aparecer los iconos de enfoque
        onFocus.transform.GetChild(childIndexMarkers).gameObject.SetActive(true);

    }
}