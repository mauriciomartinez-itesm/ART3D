using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePartsKeyboard : MonoBehaviour
{
    public PrefabBundle asset;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
