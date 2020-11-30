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
    public bool eraseFocuseModel()
    {
        try
        {
            GameObject model = Focus.onFocus;
            // Solo se pueden borrar los modelos con tag "model" para evitar que el usuario borre los modelos provinientes de los
            // marcadores/targets.
            if (model.transform.tag == "model") 
                Destroy(model);
        }
        catch (Exception ex)
        {
            debugLog.text += "Excepction eraseFocusModel:" + ex.Message + "\n";
            return false;
        }
        return true;
    }

    // Metodo para ejecutar la funcion eraseFocuseModel con un boton
    public void buttonEraseFocuseModel()
    {
        if(eraseFocuseModel())
            debugLog.text += "Se borro el modelo exitosamente\n";
        else
            debugLog.text += "Error en el borrado del modelo\n";
    }

    // Borra todos los modelosde la escena con la tag "model"
    public bool eraseAllModels()
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

    // Metodo para ejecutar la funcion eraseAllModels con un boton
    public void buttonEraseAllModels()
    {
        if (eraseAllModels())
            debugLog.text += "Se borraron todos los modelos exitosamente\n";
        else
            debugLog.text += "Error en el borrado de todos los modelos\n";
    }
}