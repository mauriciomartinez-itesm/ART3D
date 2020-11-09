using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownId : MonoBehaviour
{
    public LoadBundle _bundleLoader;

    void Start()
    {
        _bundleLoader = FindObjectOfType<LoadBundle>();
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        foreach(var el in IdInfoCollection._assetManager.assets)
        {
            dropdown.options.Add( new Dropdown.OptionData() { text = el.Key } );
        }

        dropdown.onValueChanged.AddListener(delegate { DropdownIdSelected(dropdown); });
    }

    void DropdownIdSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        _bundleLoader.DownloadAssetBundleFromFirebase(dropdown.options[index].text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
