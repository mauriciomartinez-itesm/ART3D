using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Modifica la rotacion del icono/marcador sueprior que señala a un objeto enfocado.
 * Este icono siempre debe de estar perpendicular a la camara en el eje y.
 */

public class FaceCameraInAngleY : MonoBehaviour
{
    public float xAngle = 0;
    public float zAngle = 0;
    void Update()
    {
        Vector3 targetVector = Camera.main.transform.position - transform.position;
        float newYAngle = Mathf.Atan2(targetVector.z, targetVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(xAngle, -1 * newYAngle + 90, zAngle);
    }
}
