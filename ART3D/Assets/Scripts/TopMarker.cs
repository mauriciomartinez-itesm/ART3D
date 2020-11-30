using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Modifica la rotacion del icono/marcador sueprior que señala a un objeto enfocado.
 * Este icono siempre debe de estar perpendicular a la camara en el eje y.
 */

public class TopMarker : MonoBehaviour
{
    void Update()
    {
        Vector3 targetVector = Camera.main.transform.position - transform.position;
        float newYAngle = Mathf.Atan2(targetVector.z, targetVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, -1 * newYAngle + 90, 0);
    }
}
