using Lean.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Este script se carga en un objeto vacio e invoca los modelos del assetbundle
 * como sus hijos. Solo puede invocar estos modelos utilizando la funcion 
 * DisplayAssetBundleInPrefabBundle.
 * Cuando carga un modelo le añade ciertos scripts para darle interaccion al objeto,
 * por ejemplo: rotacion y escalamiento. Los modelos son escalados automaticamente
 * a una altura estandar, para que todos los modelos aparsecan de tamano similar.
 * Tambien la altura es modificada para que los modelos esten tocando el piso sin importar
 * la altura original.
 * Ademas se guardan todas las partes visibles del modelo en la lista de partes llamada
 * parts y se añade un box collider para cada parte.
 */
public class PrefabBundle : MonoBehaviour
{

    [HideInInspector]
    public List<Tuple<string, GameObject>> parts;
    public string id = "";
            
                                                            // high_y y low_y son utilizadas para escalar el modelo
                                                            // para que todos los modelos tengas la misma altura.
                                                            // high_y contiene la posicion en y del punto mas alto del 
                                                            // modelo, mientras que y_low el mas bajo.
    private float high_y=int.MinValue, low_y=int.MaxValue;
    private int numberOfModelsInAssetBundle = 0;
    private GameObject modelContainer = null;
    private AssetBundle assetBundle;
    private GameObject model = null;
    private int model_index = 0;


                                                            // Se encarga de mostrar en tamaño y posicion correcta el
                                                            // priemr modelo del assetbundle pasado como parametro.
                                                            // Regresa true si el instanciamiento del modelo salio bien.
    public bool DisplayAssetBundleInPrefabBundle(AssetBundle assetBundleForPreab)
    {
        assetBundle = assetBundleForPreab;
        numberOfModelsInAssetBundle = assetBundle.GetAllAssetNames().Length;
        parts = new List<Tuple<string, GameObject>>();

                                                            // Se utiliza un model container para asegurarnos que el pivote
                                                            // del modelo estara posicionado pegado al suela permitiendo un
                                                            // escalamiento correcto a la hora de usar el LeanPinchScale.
        if( modelContainer == null )
        {
            CreateModelContainer();
        }

        model_index = (model_index + 1) >= (numberOfModelsInAssetBundle) ? model_index : model_index + 1;

        return InstantiateObjectFromBundle(0);     
    }

                                                            // Como los assetbundles pueden tener mas de 1 modelo, esta 
                                                            // funcion muestra el siguiente modelo del assetbundle (si ya 
                                                            // llego al ultimo vuelve a mostrar el primero)
    public void NextModel()
    {
        try
        {
            InstantiateObjectFromBundle(model_index);
            model_index = (model_index + 1) % (numberOfModelsInAssetBundle);
        }
        catch { }
    }

                                                            // Muestra el modelo previo del assetbundle (si esta en el primer
                                                            // modelo muestra el ultimo modelo)
    public void PreviousModel()
    {
        try
        {
            InstantiateObjectFromBundle(model_index);
            model_index = model_index == 0 ? (numberOfModelsInAssetBundle - 1) : model_index - 1;
        }
        catch { }
    }

    public void DeleteAssetGameObject()
    {
        if (model != null)
        {
            Destroy(model);
            model = null;
        }
    }

                                                            // Utilizando el indice obtiene el nombre del modelo del
                                                            // assetbundle para llamar a la funcion InstantiateObjectFromBundle
                                                            // que instancia el modelo utilizando su nombre como parametro.
    private bool InstantiateObjectFromBundle(int assetIndex)
    {
        try
        {
            string assetName = assetBundle.GetAllAssetNames()[assetIndex];
            return InstantiateObjectFromBundle(assetName);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error: " + ex.Message);
            return false;
        }
    }

                                                            // Invoca el modelo del assetbundle que le corresponde al id y 
                                                            // le agrega scripts para agregarle interacciones. Cuando invoca 
                                                            // un nuevo modelo se encarga de disponer del modelo previo, solo 
                                                            // puede haber 1 modelo por instancia de PrefabBundle. Aqui es donde
                                                            // el modelo es escalado y reposicionado para asegurarnos que todos
                                                            // los modelos seran del mismo tamaño y estaran tocando el piso. A
                                                            // los modelos se le agregan scripts para interactuar con el modelo:
                                                            // - RotateAxis: Se encarga de rotar el modelo con 1 dedo.
                                                            // - LeanPinchScale: Permite que el usuario pueda escalar el objeto
                                                            // - OnModelClick: Lee el click somre un Modelo (Se usa para enfocar)
                                                            // Tambien se guardan sus partes para cualquier utilidad futura.
    private bool InstantiateObjectFromBundle(string assetName)
    {
        try
        {
            var prefab = assetBundle.LoadAsset(assetName);

            DeleteAssetGameObject();
            
            model = (GameObject)Instantiate(prefab, modelContainer.transform.position, modelContainer.transform.rotation, modelContainer.transform);

            model.AddComponent<OnModelClick>();

                                                            // Reset de las variables dependientes del modelo que esta
                                                            // en proceso de desplegarse.
            parts = new List<Tuple<string, GameObject>>();
            high_y = int.MinValue;
            low_y = int.MaxValue;
            GetPartsRecursiveAndAddColliders(model);

            Debug.Log("Low y: " + low_y);

                                                            // Modifica la escala del modelo para que todos los modelos empiezen 
                                                            // con la misma altura.
            float newScale = 0.3f / (high_y - low_y);
            model.transform.localScale = new Vector3(newScale, newScale, newScale);

                                                            // Posiciona el objeto como si estuviera apoyado en la mesa sin importar
                                                            // la posicion original.
            Vector3 positionHelper = model.transform.localPosition;
            positionHelper.y += low_y * -1 * newScale;
            model.transform.localPosition = positionHelper;

            return true;
        }
        catch(Exception ex)
        {
            Debug.LogError("Error:" + ex.Message);
            return false;
        }
    }


    private void CreateModelContainer()
    {
        modelContainer = new GameObject("modelContainer");
        modelContainer.transform.parent = this.transform;
        modelContainer.transform.localPosition = new Vector3(0,0,0);
        modelContainer.transform.localScale = new Vector3(1, 1, 1);


        modelContainer.AddComponent<RotateAxis>();
        modelContainer.AddComponent<LeanPinchScale>();
    }

                                                            // Recorre todo el modelo en busca de Meshes que cataloga como
                                                            // partes que va guardando en el diccionario 'parts'. Aprovechando
                                                            // el recorrido a cada parte se le crea un collider para poder
                                                            // captar el click en el modelo.
    private void GetPartsRecursiveAndAddColliders(GameObject gameObject)
    {

        if (containsMesh(gameObject))
        {
            parts.Add(new Tuple<string, GameObject>(gameObject.transform.name, gameObject));

                                                            // Obtiene las medidas de cada render no importa si es MeshRender 
                                                            // o SkinnedMeshRender
            Vector3 partCenter = new Vector3();
            Vector3 partSize = new Vector3();
            MeshRenderer tempMR = gameObject.GetComponent<MeshRenderer>();
            SkinnedMeshRenderer tempSMR = gameObject.GetComponent<SkinnedMeshRenderer>();

            if (tempMR != null)
            {
                partCenter = tempMR.bounds.center;
                partSize = tempMR.bounds.size;
            }
            if (tempSMR != null)
            {
                partCenter = tempSMR.bounds.center;
                partSize = tempSMR.bounds.size;
            }

            Vector3 denominator = new Vector3(1,1,1);

            Transform tempTransform = model.transform;

                                                            // Recolecta las escalas del modelo y de su padre para
                                                            // poder crear los box collider del tamano correcto
            while(tempTransform != null)
            {
                denominator.x *= tempTransform.localScale.x;
                denominator.y *= tempTransform.localScale.y;
                denominator.z *= tempTransform.localScale.z;
                tempTransform = tempTransform.parent;
            }

            partSize.x /= denominator.x;
            partSize.y /= denominator.y;
            partSize.z /= denominator.z;

            partCenter -= model.transform.position;
            partCenter.x /= denominator.x;
            partCenter.y /= denominator.y;
            partCenter.z /= denominator.z;

                                                            // Crea un box collider por cada pieza (cada redner)
            BoxCollider tempBox = model.AddComponent<BoxCollider>();
            tempBox.size = partSize;
            tempBox.center = partCenter;

                                                            // high_y contiene la posicion en y de la parte mas alta del
                                                            // modelo hasta ahora mientras que la low_y la mas baja.
                                                            // Esta informacion sera utilizada para re-escalar el modelo
                                                            // para que todos sean de la misma altura.
            high_y = Math.Max(high_y, partCenter.y + (partSize.y) / 2);
            low_y = Math.Min(low_y, partCenter.y - (partSize.y) / 2);
        }

        for (int hijo = 0; hijo < gameObject.transform.childCount; hijo++)
            GetPartsRecursiveAndAddColliders(gameObject.transform.GetChild(hijo).gameObject);

    }

    private bool containsMesh(GameObject gameObject)
    {
        return gameObject.GetComponent<MeshRenderer>() != null || gameObject.GetComponent<SkinnedMeshRenderer>() != null;
    }

}
