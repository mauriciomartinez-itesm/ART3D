﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System;

/*
 * Va de la mano de ARTrackImageManager que sirve para detectar imagenes
 * con la camara. La coleccion/libreria de imagenes/targets es generada en 
 * el script SetTargets.cs y en este script se usa un listener para 
 * detecta una imagen/target. En cada frame se ejecuta la funcion InImageChange() 
 * la cual valida si se esta viendo alguna imagen. Si hay una imagen a la vista, se
 * utiliza el nombre (id) de la imagen detectada para carga el assetbundle y pone
 * visible el modelo. Si no hay una imagen a la vista se hace invisible el modelo.
 * Si el assetbundle ya esta en cache no lo descarga de firebase.
 */

public class OnImageChange : MonoBehaviour
{
    public Text debuglog;
    private LoadBundle _bundleLoader;
    private ARTrackedImageManager _arTrackedImageManager;

    private void Awake()
    {
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        _bundleLoader = FindObjectOfType<LoadBundle>();
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.updated)
        {
            // La imagen no esta visible para la camara
            if (trackedImage.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                if (trackedImage.transform.GetChild(0).gameObject.activeSelf)
                {
                    debuglog.text += $"NO se ve la imagen con id: {trackedImage.referenceImage.name}\n";
                    trackedImage.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            // La imagen si esta visible para la camara
            else
            {
                if (trackedImage.GetComponent<PrefabBundle>().id=="" || !trackedImage.transform.GetChild(0).gameObject.activeSelf)
                {
                    // Usa el nombre (id) de la imagen para cargar el assetbundle de firebase
                    try
                    {
                        debuglog.text += $"SI se detecto la imagen. Child Count: {trackedImage.transform.childCount}\n";
                        trackedImage.GetComponent<PrefabBundle>().id = trackedImage.referenceImage.name;
                        _bundleLoader.DownloadAssetBundleFromFirebase(trackedImage.referenceImage.name);
                        trackedImage.transform.GetChild(0).gameObject.SetActive(true);                        
                        debuglog.text += $"Se completa el asociamiento del prefabbundle al asset bundle\n";
                    }
                    catch(Exception ex)
                    {
                        debuglog.text += $"Error en la asociacion al assetbundle: {ex.Message}\n";
                    }

                    Focus.focusObject(trackedImage.gameObject);
                }               
            }
        }
    }
}
