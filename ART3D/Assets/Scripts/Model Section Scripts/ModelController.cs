using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Este script posee un alto nivel de abstraccion y se centra en el manejo de los modelos 3D que se
 * desean ver en la realidad aumentada. Aqui se maneja su instanciamiento, enfoque y borrado.
 * Cuando se da click en pantalla (No cuenta si fue en un boton) se ejecuta lo que este dentro de
 * la funcion ModelInstantiate, solo si esta permitido aparecer mas modelos. Esto se especifica en 
 * la variable canSpawnModel de la clase ModelSpawner la cual es modificada solo en los siguientes
 * 4 casos:
 * - Al empezar a cargar un nuevo assetbundle se vuelve falsa.
 * - Cuando se termina de cargar existosamente el assetbundle se vuelve true.
 * - Al presionar el boton "One more Model" se vuelve true.
 * - Al aparecer un nuevo modelo automaticamente se vuelve falsa.
 * 
 * Cuando se le da click a un modelo se invoca la funcion onModelClick.
 * 
 */

public class ModelController : MonoBehaviour
{
    public ModelManager _modelManager;
    public GameObject prefabBundleEmptyGameObject;
    public Text debugLog;

    void Start()
    {
                                                            // Inicializa el ModelSpawner para poderse subscribir a la
                                                            // funcion/evento ModelInstantiate
        _modelManager.InitModelSpawner();

        _modelManager.onModelClick += onModelClick;
        _modelManager.modelInstantiate += ModelInstantiate;
    }

                                                            // Recibe como parametro el GameObject que recibio el click
                                                            // y lo enfoca.
    public void onModelClick( GameObject gameObjectToFocus )
    {
        debugLog.text += $"Model Controller click was detcted \n";
        _modelManager.FocusObject( gameObjectToFocus );
    }

                                                            // Al tocar la pantalla se dispara este evento/funcion que
                                                            // instancia el prefabricado 'prefabBundleEmptyGameObject' en 
                                                            // las coordenadas del 'placementIndicator' (el circulo azul)
                                                            // al tocar la pantalla. El prefabricado sera llenado con el
                                                            // primer modelo del ultimo assetbundle correctamente descargado.
    public void ModelInstantiate(GameObject placementIndicator)
    {
        string assetBundleId = _modelManager.GetLastLoadedId();
        _modelManager.InstantiateModel( placementIndicator, prefabBundleEmptyGameObject, assetBundleId );
    }


                                                            // Metodo para ejecutar la funcion eraseFocuseModel con un boton
    public void ButtonEraseFocusModel()
    {
        bool eraseSuccessfull = _modelManager.EraseFocusModel();

        if (eraseSuccessfull)
            debugLog.text += "Se borro el modelo exitosamente\n";
        else
            debugLog.text += "Error en el borrado del modelo\n";
    }


                                                            // Metodo para ejecutar la funcion eraseAllModels con un boton
    public void ButtonEraseAllModels()
    {
        bool eraseSuccessfull = _modelManager.EraseAllModels();

        if (eraseSuccessfull)
            debugLog.text += "Se borraron todos los modelos exitosamente\n";
        else
            debugLog.text += "Error en el borrado de todos los modelos\n";
    }


                                                            // Metodo para habilitar CanSpawnModel con un boton.
    public void ButtonEnableCanSpawnModel()
    {
        _modelManager.SetCanSpawnModel( true );
    }
}
