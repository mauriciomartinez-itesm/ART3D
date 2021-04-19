using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Este script posee un alto nivel de abstraccion y se centra en la descarga de los ids y assetbundles.
 * Al empezar, todos los ids son cargados a la cache y mostrados en un dropdown.
 * La carga/descarga de los assetbundles se dispara por la seleccion del dropdown de los ids.
 * Otra manera de cargar un assetbundle es usando la opcion loadOnStart la cual se recomienda para
 * realizar pruebas en el editor de unity.
 */

public class BundleController : MonoBehaviour
{

    public BundleManager _bundleManager;
    
    public Text debuglog;

    [Tooltip("La direccion en la cual se encuentra el archivo AssetBundle, si se obtendra de internet debe de ser un url directo al archivo.")]
    public string pathForLoadOnStart;

    [Tooltip("Activar para cargar el AssetBundle desde el inicio del programa.")]
    public bool loadOnStart;

    void Start()
    {
                                                            // Inicializa el BundleLoader y el DropDownId para poder subscribirse
                                                            // a los eventos onAssetBundleFinishLoad y onDropdownIdSelected
        _bundleManager.InitBundleLoader();
        _bundleManager.InitDropdownId();

                                                            // Borra las opciones que contenia el dropdown para llenarlo con
                                                            // los ids disponibles en firebase. Cuando termina la descarga
                                                            // de todos los ids ejecuta la funcion onIdLoadingDoneHandler.
        _bundleManager.ClearDropdown();
        _bundleManager.DownloadAndDeserializeIdInfoCollection( OnIdLoadingDoneHandler );

        _bundleManager.onDropdownIdSelected += OnDropdownIdSelected;
        _bundleManager.onAssetBundleFinishLoad += OnAssetBundleFinishLoad;

                                                            // Si la opcion loadOnStart esta activada significa que se desea
                                                            // mostrar un assetbundle desde el inicio. Por esto se agrega
                                                            // un assetbundle con id="Test" que se encuentra en pathForLoadOnStart.
                                                            // El path puede ser local o una link de internet directo al archivo.
        if ( loadOnStart && pathForLoadOnStart != "" )
        {
            _bundleManager.AsyncAddAssetBundle("Test", pathForLoadOnStart);
        }

        Application.targetFrameRate = 10;
    }

                                                            // Este metodo se ejecuta cuando se selecciona un id del dropdown.
                                                            // El metodo deshabilita la capacidad de instanciar un modelo
                                                            // y empieza la descarga asincrona del assetbundle.
    private void OnDropdownIdSelected(string id)
    {
        debuglog.text += $"Se selecciono: {id} \n";
        _bundleManager.SetCanSpawnModel( false );
        _bundleManager.AsyncAddAssetBundle(id);
    }

                                                            // Este metodo es llamado cuando se terminan de recolectar los IDs 
                                                            // de los asset bundles. El parametro que recibe es un diccionario 
                                                            // en donde la llave es el id y su contenido es informacion del asset 
                                                            // bundle como descripcion y nombre. Dentro de la funcion se 
                                                            // agregan/despliegan los ids en el dropdown y se descargan y registran 
                                                            // sus respectivos targets (si es que existen).
    private void OnIdLoadingDoneHandler(Dictionary<string, AssetInfo> assetsInfo)
    {
        _bundleManager.AddIdsToDropdown( assetsInfo );
        //_bundleManager.DownloadAndAddTargets( assetsInfo );
    }


                                                            // Este metodo se ejecuta cuando se termina de cargar el assetbundle
                                                            // que se estaba descargando. Si tuvo exito, se habilita la capacidad
                                                            // de instanciar un modelo y realiza el control de los assetbundles
                                                            // del cache.
    private void OnAssetBundleFinishLoad(bool succesfullLoad)
    {
        if (succesfullLoad)
        {
            debuglog.text += $"Model Controller bundle was correctly loaded \n";
            _bundleManager.SetCanSpawnModel( true );

                                                            // Revisa que no haya mas assetbundles en la cache que lo
                                                            // permitido, ese numero se especifica en el script BundleLoader.
                                                            // Regresa un string vacio si no se supero el maximo de asset bundles.
                                                            // Si se supero el numero permitido, regresa el id del assetbundle que
                                                            // se removio para poder remover los objetos en escena que utilizaban
                                                            // el assetbundle removido.
            string assetBundleIdToRemove = _bundleManager.MaxAssetBundlesInCacheCheck();

            if (assetBundleIdToRemove != "")
            {
                debuglog.text += $"Asset Bundle to remove: {assetBundleIdToRemove} \n";
                _bundleManager.DeleteAllPrefabBundlesWithId( assetBundleIdToRemove );
            }
                
        }
        else
        {
            debuglog.text += $"Model Controller bundle was INCORRECTLY loaded \n";
        }
    }

}
