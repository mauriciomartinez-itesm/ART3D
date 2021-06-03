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

    public string GetCurrentId()
    {
        return _bundleManager.GetCurrentId();
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

        InitOnModelClick(prefabBundleGameObject);
        FocusObject(prefabBundleGameObject);

        StartCoroutine( DisplayAssetBundleInPrefabBundle( prefabBundleGameObject, assetBundleId ) );
    }

                                                            // Si el assetbundle que se desea desplegar si esta cargado en
                                                            // la cache entonces se muestra dentro del prefabBundleGameObject.
                                                            // Si el assetbundle no existe entonces se termina el metodo dejando al
                                                            // prefabBundleGameObject con la animacion de loading. En la clase 
                                                            // BundleController, cuando se termina de cargar el assetbundle se 
                                                            // manda a llamar DisplayAssetBundleInPendingPrefabBundles. De esta
                                                            // manera los modelos que quedaron en la etapa de loading se despliegan.
    public IEnumerator DisplayAssetBundleInPrefabBundle(GameObject prefabBundleGameObject, string id)
    {
        PrefabBundle _prefabBundle = prefabBundleGameObject.GetComponent<PrefabBundle>();
        AssetBundle assetBundle = _bundleManager.GetAssetBundle(id);

        if (assetBundle == null)
        {
            Debug.Log("Null assetBundle");
            yield break;
        }

        bool assetBundleCorrctlyDisplayed = _prefabBundle.DisplayAssetBundleInPrefabBundle(assetBundle);

        yield return null;
                                                            // Si todo el proceso de instanciado del modelo salio bien, 
                                                            // se inicializa el OnModelClick del prefabBUndle
        if (assetBundleCorrctlyDisplayed)
        {
            InitOnModelClick( prefabBundleGameObject );
        }
    }

    public void DisplayAssetBundleInPendingPrefabBundles(string assetBundleId)
    {
        var all_prefab_bundles = GameObject.FindObjectsOfType<PrefabBundle>();
        foreach (PrefabBundle prefab_bundle in all_prefab_bundles)
        {
            if(prefab_bundle.numberOfModelsInAssetBundle == 0)
                StartCoroutine( DisplayAssetBundleInPrefabBundle(prefab_bundle.gameObject, assetBundleId));
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
