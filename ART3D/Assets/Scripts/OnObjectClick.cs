using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnObjectClick : MonoBehaviour
{
	private bool objectClicked = false;
	private float timer = 0.0f;

	// Update is called once per frame
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
		objectClicked = true;
	}
}
