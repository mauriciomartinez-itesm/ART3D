using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetVector = Camera.main.transform.position - transform.position;
        float newYAngle = Mathf.Atan2(targetVector.z, targetVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -1 * newYAngle + 90, 0);
    }
}
