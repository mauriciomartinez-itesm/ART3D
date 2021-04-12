using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Es una clase prueba para demostrar que las partes de los modelos pueden
 * ser desplegadas a voluntad.
 */

public class TogglePartsKeyboard : MonoBehaviour
{
    public PrefabBundle asset;

    void Update()
    {
        for (int index = 0; index < asset.parts.Count; index++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + index + 1))
            {
                Debug.Log("Toggle Part: " + asset.parts[index].Item1);
                asset.parts[index].Item2.SetActive(!asset.parts[index].Item2.activeInHierarchy);
            }
        }
    }
}
