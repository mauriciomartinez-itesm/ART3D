﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]

//Estructura de la carta
public class Card
{

    public GameObject cardGameObject; //El objeto
    public string assetBundleName; // Nombre del assetBundle
    public string NumberofModels; //Numero de Modelos
    public string CRN; // CRN de la Materia
    public Sprite thumbnail; //Imagen del objeto
    public bool isFavorite;
}




