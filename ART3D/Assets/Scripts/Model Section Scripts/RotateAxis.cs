using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Gira el objeto en el eje inidcado en 'Axis' (default eje Y) al arrastrar
 * el dedo (o mouse dando click) horizontalmente. Solo se realiza la rotacion 
 * si el usuario esta usando 1 solo dedo.
 */

public class RotateAxis : MonoBehaviour
{
	[Tooltip("The axis of rotation.")]
	public Vector3 Axis = Vector3.down;

	[Tooltip("Rotate locally or globally?")]
	public Space Space = Space.Self;

	private float lastPositionX = -1.0f;
	private float lastPositionY = -1.0f;
	private float sensibility = 0.5f;

    public void Update()
    {
															// Hace el mapeo de pixeles que se movio el dedo horizontalmente y 
															// lo convierte a grados que rotara el objeto. lastPositionX es la 
															// coordenada del frame pasado del dedo horizontalmente.
		if (lastPositionX != -1.0f && Input.GetMouseButton(0) && Input.touchCount <= 1)
		{
			var twistDegreesX = (Input.mousePosition.x - lastPositionX) * sensibility;
			var twistDegreesY = (Input.mousePosition.y - lastPositionY) * sensibility;

															// Realiza la rotacion.
			if(Mathf.Abs(twistDegreesX) > Mathf.Abs(twistDegreesY) )
				transform.Rotate(Vector3.down, twistDegreesX, Space);
			else
				transform.Rotate(Vector3.right, twistDegreesY, Space);
			lastPositionX = Input.mousePosition.x;
			lastPositionY = Input.mousePosition.y;
		}

															// Solo se realiza la rotacion si el usuario esta usando 1 solo dedo.
		if (Input.GetMouseButton(0) && Input.touchCount <= 1)
        {
			lastPositionX = Input.mousePosition.x;
			lastPositionY = Input.mousePosition.y;
		}
		else
        {
			lastPositionX = -1;
			lastPositionY = -1;
		}
	}


}
