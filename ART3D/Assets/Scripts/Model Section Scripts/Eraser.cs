using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * La clase se encarga de borrar modelos y tiene 2 formas de borrarlos
 * 1. Borrar el Objeto enfocado actualmente
 * 2. Borrar todos los modelos puestos en el plano.
 */

public class Eraser : MonoBehaviour
{
    public Text debugLog;

                                                            // Borra el modelo enfocado actualmente
    public bool EraseFocusModel(GameObject focuseModelToDelete)
    {
        try
        {
                                                            // Solo se pueden borrar los modelos con tag "model" para evitar 
                                                            // que el usuario borre los modelos provenientes de los 
                                                            // marcadores/targets.
            if (focuseModelToDelete.transform.tag == "model") 
                Destroy(focuseModelToDelete);
        }
        catch (Exception ex)
        {
            debugLog.text += "Excepction eraseFocusModel:" + ex.Message + "\n";
            return false;
        }
        return true;
    }

                                                            // Borra todos los modelos de la escena con la tag "model"
    public bool EraseAllModels()
    {
        var all_models = GameObject.FindGameObjectsWithTag("model");
        try
        {
            foreach(var model in all_models)
                Destroy(model);
        }
        catch(Exception ex)
        {
            debugLog.text += "Excepction eraseAllModels:" + ex.Message + "\n";
            return false;
        }
        return true;
    }

}