using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Es una clase intermediaria usada por BundleController para abstraer y encapsular la logica
 * de los scripts: BundleLoader, DropDownId, IdInfoCollection y SetTargets.
 */

public class BundleManager : MonoBehaviour
{
    public ModelManager _modelManager;

    public IdInfoCollection _idInfoCollection;
    public BundleLoader _bundleLoader;
    public DropdownId _dropdownId;
    public SetTargets _setTargets;

    public event BundleLoader.onAssetBundleFinishLoadHandler onAssetBundleFinishLoad;
    public event DropdownId.onDropdownIdSelectedHandler onDropdownIdSelected;
    public event IdInfoCollection.onIdsLoadingDoneHandler onIdsLoadingDone;


                                                            /* MODEL MANAGER SECTION START */

    public void SetCanSpawnModel(bool canSpawn)
    {
        _modelManager.SetCanSpawnModel(canSpawn);
    }

    public void DeleteAllPrefabBundlesWithId(string id)
    {
        _modelManager.DeleteAllPrefabBundlesWithId( id );
    }

    public void DisplayAssetBundleInPendingPrefabBundles(string assetBundleId)
    {
        _modelManager.DisplayAssetBundleInPendingPrefabBundles(assetBundleId);
    }

                                                            /* MODEL MANAGER SECTION END */



                                                            /* BUNDLE LOADER SECTION START */

    public void InitBundleLoader()
    {
        _bundleLoader.InitBundleLoader();
        _bundleLoader.onAssetBundleFinishLoad += ExecuteOnAssetBundleFinishLoad;
    }

    private void ExecuteOnAssetBundleFinishLoad(bool succesfullLoad, string assetBundleId)
    {
        onAssetBundleFinishLoad?.Invoke(succesfullLoad, assetBundleId);
    }

    public void AsyncAddAssetBundle( string id, string assetBundlePath = "")
    {
        _bundleLoader.AsyncAddAssetBundle( id, assetBundlePath );
    }

    public string MaxAssetBundlesInCacheCheck()
    {
        return _bundleLoader.MaxAssetBundlesInCacheCheck();
    }

    public AssetBundle GetAssetBundle( string id )
    {
        return _bundleLoader.GetAssetBundle( id );
    }

    public string GetCurrentId()
    {
        return _bundleLoader.GetCurrentId();
    }

                                                            /* BUNDLE LOADER SECTION END */



                                                            /* DROPDOWN ID SECTION START */

    public void InitDropdownId()
    {
        _dropdownId.InitDropdownId();
        _dropdownId.onDropdownIdSelected += ExecuteOnDropdownIdSelected;
    }
    private void ExecuteOnDropdownIdSelected(string id)
    {
        onDropdownIdSelected?.Invoke(id);
    }

    public void ClearDropdown()
    {
        _dropdownId.ClearDropdown();
    }

    public void AddIdsToDropdown(Dictionary<string, AssetInfo> assetsInfo)
    {
        _dropdownId.AddIdsToDropdown( assetsInfo );
    }

                                                            /* DROPDOWN ID SECTION END */



                                                            /* ID INFO COLLECTION SECTION START */

    public void InitIdInfoCollection()
    {
        _idInfoCollection.onIdsLoadingDone += ExecuteOnIdsLoadingDone;
    }

    public void ExecuteOnIdsLoadingDone(Dictionary<string, AssetInfo> assetsInfo)
    {
        onIdsLoadingDone.Invoke(assetsInfo);
    }

    public void DownloadAndDeserializeIdInfoCollection()
    {
        _idInfoCollection.DownloadAndDeserializeIdInfoCollection();
    }

    /* ID INFO COLLECTION SECTION END */



    /* SET TARGETS SECTION START */

    public void DownloadAndAddTargets(Dictionary<string, AssetInfo> assetsInfo)
    {
        _setTargets.DownloadAndAddTargets( assetsInfo );
    }

                                                            /* SET TARGETS SECTION END */

}
