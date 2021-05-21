using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    private List<Card> cards;
    public BundleManager _bundleManager;
    public UX_Helper _ux_Helper;
    public ImagePreviewCollection _imagePreviewCollection;
    public GameObject cardPanel;
    public GameObject cardPrefab;

    private bool filterByFavorite = false;

    public void Start()
    {
        cards = new List<Card>();
        _bundleManager.onIdsLoadingDone += OnIdsLoadingDoneHandler;
    }

    private void OnIdsLoadingDoneHandler(Dictionary<string, AssetInfo> assetsInfo)
    {
        Debug.Log("Starting to add cards");
        AddCards(assetsInfo);
    }

    private void AddCards(Dictionary<string, AssetInfo> assetsInfo)
    {
        foreach (var element in assetsInfo)
        {
            GameObject NewCard = Instantiate(cardPrefab, transform.position, transform.rotation) as GameObject;
            NewCard.transform.SetParent(cardPanel.transform, false);

                                                            // Asigna el ID del assetbundle como el nombre de la carta
                                                            // para poder usarlo en el resto de la logica (BundleLoader,
                                                            // SetTargets, etc.)
            NewCard.transform.name = element.Key;

                                                            // Muestra el nombre del assetBundle en el display de la carta.
            NewCard.transform.GetChild(2).GetComponent<Text>().text = element.Value.name;

                                                            // Agrega el GameObject de la nueva carta junto con su informacion
                                                            // a la lista de cartas.
            AddCardToCardList(NewCard, element.Value);

                                                            // Conecta la accion onclick del boton de favorito al metodo
                                                            // SetAsFavorite.
            AddFavoriteButtonListener( NewCard.transform.GetChild(4).GetComponent<Button>(), cards.Count-1 );

                                                            // Conecta la accion onclick del boton de la carta al metodo
                                                            // OnCardClick.
            AddCardButtonListener( NewCard.GetComponent<Button>(), cards.Count - 1);

            //if (cards.Count <= 10)
            //{
            _imagePreviewCollection.DownloadAndSetPreviewImage(element.Key);
            //}
        }
    }

    private void AddCardToCardList(GameObject cardGameObject, AssetInfo assetInfo)
    {
        Card newCard = new Card();

        newCard.cardGameObject = cardGameObject;
        newCard.assetBundleName = assetInfo.name;
        newCard.isFavorite = false;
        cards.Add(newCard);
    }

                                                            // La asignacion del onclick Listener debe realizarse dentro
                                                            // de un metodo y no directamente en el for del metodo AddCards
                                                            // para que la variable cardIndex sea guardada en el stack de 
                                                            // memoria asegurandonos que al presionar el boton se enviara 
                                                            // su verdadero cardIndex a la funcion SetAsFavorite. Si se hicera
                                                            // en el for, se enviaria el ultimo indice por el manejo de memoria.
    public void AddFavoriteButtonListener(Button favBtn, int cardIndex)
    {
        favBtn.onClick.AddListener(delegate { SetAsFavorite(cardIndex); });
    }

                                                            // Esta funcion se ejecuta cuando se presiona el boton de favorito
                                                            // de cualquier carta. Para distinguir la carta se utiliza el 
                                                            // parametro cardIndex.
    public void SetAsFavorite(int cardIndex)
    {
        Debug.Log("Set this card as fav: " + cards[cardIndex].assetBundleName);
        cards[cardIndex].isFavorite = !cards[cardIndex].isFavorite;
    }

                                                            // La asignacion del onclick Listener debe realizarse dentro
                                                            // de un metodo y no directamente en el for del metodo AddCards
                                                            // para que la variable cardIndex sea guardada en el stack de 
                                                            // memoria asegurandonos que al presionar el boton se enviara 
                                                            // su verdadero cardIndex a la funcion SetAsFavorite. Si se hicera
                                                            // en el for, se enviaria el ultimo indice por el manejo de memoria.
    public void AddCardButtonListener(Button cardBtn, int cardIndex)
    {
        cardBtn.onClick.AddListener(delegate { OnCardClick(cardBtn, cardIndex); });
    }

                                                            // Esta funcion se ejecuta cuando se presiona cualquier carta. Para 
                                                            // distinguir la carta se utiliza el parametro cardIndex. Dentro de la
                                                            // funcion se dispara la descarga del assetbundle correspondiente al
                                                            // id de la carta, y se activa la capacidad de poner objetos en la escena AR.
    public void OnCardClick(Button cardBtn, int cardIndex)
    {
        string id = cards[cardIndex].cardGameObject.transform.name;
        Debug.Log("Se presiono la carta con ID: " + id);
        _bundleManager.AsyncAddAssetBundle(id);
        _bundleManager.SetCanSpawnModel(true);
    }

                                                            // Es llamado al dar click en el boton de filtrar por 
                                                            // favorito.
    public void FilterByFavorite()
    {
        Debug.Log("Filtrando por favs");
                                                            // Vuelve al boton un toggle button que puede prender y
                                                            // apagar el filtrado por favoritos,.
        filterByFavorite = !filterByFavorite;

        FilterCards();
    }

                                                            // Realiza el filtrado tomando en cuenta todos los filtros
                                                            // al mismo timepo. Cuando ningun filtro esta activado, activa
                                                            // todas las tarjetas.
    public void FilterCards()
    {
        foreach (var card in cards)
            if (card.cardGameObject != null)
            {
                bool shouldShowCard = (!filterByFavorite | card.isFavorite);
                card.cardGameObject.SetActive( shouldShowCard );
            }
    }

}
