using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Esta clase posee un set de funciones para editar el dropdown, pero antes de usarlas
 * debe ser llamada la funcion InitDropDownId. Cuando se selecciona una opcion del dropdown
 * se invoca la funcion/evento onDropdownIdSelected.
 */

public class DropdownId : MonoBehaviour
{
    public delegate void onDropdownIdSelectedHandler( string id );
    public event onDropdownIdSelectedHandler onDropdownIdSelected;
    public Dropdown dropdown;
    
    private string defaultMessage = "Select id";

    public void InitDropdownId()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownIdSelected(dropdown); });
    }

    public void ClearDropdown() 
    {
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = defaultMessage });
    }

    public void AddIdsToDropdown(Dictionary<string, AssetInfo> assetsInfo)
    {
        foreach (var el in assetsInfo)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = el.Key });
        }
    }

    private void DropdownIdSelected(Dropdown dropdown)
    {

        int index = dropdown.value;
        if (dropdown.options[index].text != defaultMessage)
            ExecuteOnDropDownIdSelected(dropdown.options[index].text);
    }

    protected virtual void ExecuteOnDropDownIdSelected(string id)
    {
        onDropdownIdSelected?.Invoke(id);
    }

}
