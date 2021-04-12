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
 * bandera modelClicked. Esto significa que el dedo esta sobre el modelo; la funcion
 * onModelClick no se invoca hasta comprobar que el click/touch duro menos de 0.22 segundos. 
 * Si duro mas, la funcion no se invoca.
 */

public class OnModelClick : MonoBehaviour
{

	public delegate void onModelClickHandler( GameObject gameObjectToFocus);
	public event onModelClickHandler onModelClick;

    private bool modelClicked = false;
    private float timer = 0.0f;


    // MEJORAR utilizar corrutinas y yields con waitforseconds para no tener que usar un loop.
    void Update()
    {
        if (modelClicked)
        {
            timer += Time.deltaTime;
            if (timer < 0.22f)
            {
                if (Input.touchCount == 0 && !Input.GetMouseButton(0))
                {
                    onModelClick.Invoke( this.transform.parent.parent.gameObject );
                    Debug.Log("raw onclick detected");
                    modelClicked = false;
                }
            }
            else
            {
                modelClicked = false;
            }
        }
        else
        {
            timer = 0;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            modelClicked = true;
    }
}
