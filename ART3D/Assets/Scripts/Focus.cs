using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    [HideInInspector]
    public static GameObject onFocus=null;

    public static void focusObject(GameObject obj)
    {
        // Si ya habia un modelo enfocado lo desenfoca bloqueando
        // sus habilidades de rotar y escalar y desactiva sus iconos.
        if(onFocus!=null)
        {
            try
            {
                onFocus.transform.GetChild(0).gameObject.SetActive(false); // Desactivacion de iconos
                onFocus.GetComponentInChildren<RotateAxis>().enabled = false;
                onFocus.GetComponentInChildren<LeanPinchScale>().enabled = false;                
            }
            catch
            {
                // Si llega a tener algun problema desenfocando el modelo, prueba si
                // es porque el modelo/hijo esta desactivo. Empieza verificando si tiene hijo
                // y si este esta desactivo, despues la activa para tratar de desenfocarlo
                // y si no funciona lo borra.
                if (onFocus.transform.childCount>1 && !onFocus.transform.GetChild(1).gameObject.activeSelf)
                {
                    try
                    {
                        onFocus.transform.GetChild(1).gameObject.SetActive(true);
                        onFocus.transform.GetChild(0).gameObject.SetActive(false);
                        onFocus.GetComponentInChildren<RotateAxis>().enabled = false;
                        onFocus.GetComponentInChildren<LeanPinchScale>().enabled = false;
                        onFocus.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    catch { Destroy(onFocus); }
                }
                else { Destroy(onFocus); }
            }
        }

        onFocus = obj;

        try
        {
            // Trata de enfocar el modelo habilitando la rotacion y la escala y activa sus iconos,
            // esto solo funciona si el modelo ya existia.
            onFocus.GetComponentInChildren<RotateAxis>().enabled = true;
            onFocus.GetComponentInChildren<LeanPinchScale>().enabled = true;
            onFocus.transform.GetChild(0).gameObject.SetActive(true);
        }
        catch
        {
            // Quiere decir que hubo un problema y probablemente el modelo todavia no era instanciado
            // por eso no encuentra el script RotateAxis y LeanPinchScale
        }
        
    }
}
