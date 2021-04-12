using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Es una clase intermediaria usada por ModelController para abstraer y encapsular la logica
 * de los scripts: ModelSpawner, Eraser, Focus.
 */

public class ModelManager : MonoBehaviour
{
    public BundleManager _bundleManager;

    public ModelSpawner _modelSpawner;
    public Eraser _eraser;
    public Text debuglog;
    public Focus _focus;

    public event ModelSpawner.SpawnFunctionHandler modelInstantiate;
    public event OnModelClick.onModelClickHandler onModelClick;    

                                                            /* BUNDLE MANAGER SECTION START */

    public string GetLastLoadedId()
    {
        return _bundleManager.GetLastLoadedId();
    }

                                                            /* BUNDLE MANAGER SECTION END */



                                                            /* ERASER SECTION START */

    public bool EraseFocusModel()
    {
        return _eraser.EraseFocusModel( _focus.GetFocusedGameObject() );
    }


    public bool EraseAllModels()
    {
        return _eraser.EraseAllModels();
    }

                                                            /* ERASER SECTION END */



                                                            /* ON MODEL CLICK SECTION START */

    public void InitOnModelClick( GameObject prefabBundleGameObject)
    {
        OnModelClick _onModelClick = prefabBundleGameObject.GetComponentInChildren<OnModelClick>();
        _onModelClick.onModelClick += executeOnModelClick;
    }

                                                            // Esta funcion inicializa la subscripcion al evento
                                                            // onModelClick para el modelo actualmente enfocado. Esta funcion
                                                            // se debe ejecutar desde una corrutina y normalmente se usa para
                                                            // inicializar un nuevo modelo cuyo prefabBundle ya existia. 
                                                            // Ejemplo: cuando un prefab bundle esta mostrando el primer modelo
                                                            // de su assetbundle y se desea mostrar el segundo.
                                                            // La funcion se espera hasta el siguiente frame para realizar la 
                                                            // subscripcion porque debe de esperar a que el nuevo modelo se
                                                            // termine de crear, de lo contrario no se asignara bien la 
                                                            // subscripcion y el nuevo modelo no podra disparar los eventos
                                                            // onModelClick.
    public IEnumerator InitOnModelClickOfFocusedModel()
    {
        yield return null;
        GameObject prefabBundleGameObject = _focus.GetFocusedGameObject();
        OnModelClick _onModelClick = prefabBundleGameObject.GetComponentInChildren<OnModelClick>();
        _onModelClick.onModelClick += executeOnModelClick;
    }


    public void executeOnModelClick(GameObject gameObjectToFocus)
    {
        onModelClick?.Invoke(gameObjectToFocus);
    }

                                                            /* ON MODEL CLICK SECTION END */



                                                            /* FOCUS SECTION START */

    public void FocusObject(GameObject gameObjectToFocus)
    {
        _focus.FocusObject( gameObjectToFocus );
    }

                                                            /* FOCUS SECTION END */



                                                            /* MODEL SPAWNER SECTION START */

    public void InitModelSpawner()
    {
        _modelSpawner.spawnFunction += ExecuteSpawnerFunction;
    }


    public void ExecuteSpawnerFunction(GameObject placementIndicator)
    {
        modelInstantiate?.Invoke( placementIndicator );
    }


    public void SetCanSpawnModel( bool canSpawn )
    {
        _modelSpawner.SetCanSpawnModel( canSpawn );
    }

                                                            /* MODEL SPAWNER SECTION END */



                                                            /* PREFAB BUNDLE SECTION START */
                                                            // Aparece/Instancia el prefabBundle vacio para asignarle el
                                                            // id del assetbundle que se desea mostrar. Se le asigna la
                                                            // tag de 'model' para poder diferencia los modelos que no
                                                            // provienen de un target y de los que si. Por ultimo se
                                                            // ejecuta la funcion DisplayAssetBundleInPrefabBundle para
                                                            // que el prefabBundle ya no sea un objeto vacio.
    public void InstantiateModel(GameObject placementIndicator, GameObject prefabBundleEmptyGameObject, string assetBundleId)
    {
        if (assetBundleId == "")
            return;

        GameObject prefabBundleGameObject = Instantiate(prefabBundleEmptyGameObject, placementIndicator.transform.position, placementIndicator.transform.rotation);

        prefabBundleGameObject.GetComponent<PrefabBundle>().id = assetBundleId;
        prefabBundleGameObject.tag = "model";

        DisplayAssetBundleInPrefabBundle( prefabBundleGameObject, assetBundleId );
    }

                                                            // Si el assetbundle que se desea desplegar si esta cargado en
                                                            // la cache entonces se muestra como hijo del prefabBundleGameObject.
                                                            // Si el assetbundle no existe entonces se destruye el 
                                                            // prefabBundleGameObject para evitar tener objetos vacios. Si todo
                                                            // sale bien, el prefabBundleGameObject que contiene el modelo es
                                                            // enfocado e inicializado para subscribirse al evento onModelClick.
    private void DisplayAssetBundleInPrefabBundle(GameObject prefabBundleGameObject, string id)
    {
        PrefabBundle _prefabBundle = prefabBundleGameObject.GetComponent<PrefabBundle>();
        AssetBundle assetBundle = _bundleManager.GetAssetBundle(id);

        if (assetBundle == null)
        {
            Debug.Log("Null assetBundle");
            Destroy(prefabBundleGameObject);
            return;
        }

        bool assetBundleCorrctlyDisplayed = _prefabBundle.DisplayAssetBundleInPrefabBundle(assetBundle);

                                                            // Si todo el proceso de instanciado del modelo salio bien, 
                                                            // se inicializa el OnModelClick del prefabBUndle y se enfoca 
                                                            // el modelo.
        if (assetBundleCorrctlyDisplayed)
        {
            InitOnModelClick( prefabBundleGameObject );
            FocusObject( prefabBundleGameObject );
        }
    }


    public void NextModel()
    {
        GameObject prefabBundleGameObject = _focus.GetFocusedGameObject();
        prefabBundleGameObject.GetComponent<PrefabBundle>().NextModel();
                                                            // La inicializacion esta dentro de una corrutina para que
                                                            // se pueda realizar en el siguiente frame donde el modelo
                                                            // ya exista y sus componentes esten estables.
        StartCoroutine( InitOnModelClickOfFocusedModel() );
    }

    public void PreviousModel( PrefabBundle _prefabBundle )
    {
        _prefabBundle.PreviousModel();
    }

                                                            // Borra todos los objetos que contienen el script PrefabBundle
                                                            // cuyo id coincida con el parametro. Los modelos que provienen
                                                            // de un target son borrados de manera diferente para evitar 
                                                            // problemas al momento de volver a escanear la imagen.
    public void DeleteAllPrefabBundlesWithId( string id )
    {
        var all_prefab_bundles = GameObject.FindObjectsOfType<PrefabBundle>();
        foreach (PrefabBundle prefab_bundle in all_prefab_bundles)
        {
            if (prefab_bundle.id == id)
            {
                                                            // Si es un modelo individual (no proviene de un target) se elimina
                                                            // todo el objeto completo
                if (prefab_bundle.gameObject.transform.tag == "model")
                {
                    Destroy(prefab_bundle.gameObject);
                }
                                                            // Si es un modelo de un target solo se elimina el modelo del objeto
                                                            // ya que el objeto es necesario para identificar la imagen.
                else
                {
                    prefab_bundle.id = "";
                    debuglog.text += $"Start Borrando model del Target. Child 1: {prefab_bundle.transform.GetChild(0)} Child 2: {prefab_bundle.transform.GetChild(1)}\n";
                    try
                    { prefab_bundle.DeleteAssetGameObject(); }
                    catch (Exception ex)
                    { debuglog.text += $"El problema en el borrado del modelo del target es: {ex.Message}"; }

                    debuglog.text += $"End Borrando model del Target. Child Count: {prefab_bundle.transform.childCount}\n";
                }
            }
        }
    }

                                                            /* PREFAB BUNDLE SECTION END */


}
