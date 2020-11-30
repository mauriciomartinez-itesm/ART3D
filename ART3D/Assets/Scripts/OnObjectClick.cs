using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Este script lo llevan todos los modelos de la escena, se encarga
 * de esperar/escuchar los touchs que se dan sobre los modelos para llamar a la
 * funcion para enfocarlos.
 * Al detectar un click/touch sobre el objeto, el script revisa que el touch no
 * este siendo dado sobre un boton de la UI, si no hay un boton en medio activa la 
 * bandera objectClicked. Esto significa que el dedo esta sobre el modelo; el modelo
 * no se enfoca hasta comprobar que el click/touch duro menos de 0.22 segundos. Si duro
 * mas el modelo no se enfoca.
 */

public class OnObjectClick : MonoBehaviour
{
	private bool objectClicked = false;
	private float timer = 0.0f;

	void Update()
    {
		if (objectClicked)
		{
			timer += Time.deltaTime;
			if (timer < 0.22f)
			{
				if (Input.touchCount == 0 && !Input.GetMouseButton(0))
				{
					Debug.Log("Se detecto la intencion de enfocar");
					Focus.focusObject(this.transform.parent.gameObject);
					objectClicked = false;
				}
			}
			else
			{
				objectClicked = false;
			}
		}
		else
		{
			timer = 0;
		}
	}

	private void OnMouseDown()
	{
		if(EventSystem.current.currentSelectedGameObject == null)
			objectClicked = true;
	}
}
