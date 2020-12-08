using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Recorre la colleccion de ids obtenidos de firebase (IdInfoCollection._assetManager.assets)
 * y añade cada id al drop down. Define la funcion DropdownIdSelected que se encarga de descargar
 * el id seleccionado en el dropdown a traves del _bundleLoader y su funcion DownloadAssetBundleFromFirebase.
 */
 
public class DropdownId : MonoBehaviour
{
    public LoadBundle _bundleLoader;
    private string defaultMessage = "Select id";

    void Start()
    {
        _bundleLoader = FindObjectOfType<LoadBundle>();
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = defaultMessage });

        foreach (var el in IdInfoCollection._assetManager.assets)
        {
            dropdown.options.Add( new Dropdown.OptionData() { text = el.Key } );
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownIdSelected(dropdown); });
    }

    void DropdownIdSelected(Dropdown dropdown)
    {
        
        int index = dropdown.value;
        if (dropdown.options[index].text != defaultMessage)
            _bundleLoader.DownloadAssetBundleFromFirebase(dropdown.options[index].text);
    }
}
