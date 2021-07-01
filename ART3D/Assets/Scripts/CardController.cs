using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public BundleManager _bundleManager;
    public UX_Helper _ux_Helper;
    public ImagePreviewCollection _imagePreviewCollection;
    public GameObject cardPanel;
    public GameObject cardPrefab;
    public Button favorite;
    public InputField inputAssetBundleNameQuery;

    public Button searchbtn;

    private bool filterByFavorite = false;
    private bool filterByAssetBundle = false;
    private HashSet<string> favoriteIdList = new HashSet<string>();

    public void Start()
    {
        cards = new List<Card>();
        _bundleManager.onIdsLoadingDone += OnIdsLoadingDoneHandler;
        favoriteIdList = SaveLoadData.data.favoriteIdList;
    }

    private void OnIdsLoadingDoneHandler(Dictionary<string, AssetInfo> assetsInfo)
    {
        //Debug.Log("Starting to add cards");
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

            _imagePreviewCollection.DownloadAndSetPreviewImage(element.Key);

            markAsFavoriteBasedOnSavedData(cards[cards.Count - 1]);

        }
    }
    public void markAsFavoriteBasedOnSavedData(Card card)
    {
        if (favoriteIdList.Contains(card.cardGameObject.name))
        {
            SetAsFavorite(cards.IndexOf(card));
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
       // Debug.Log(favBtn);
       
       
        
    }


                                                            // Esta funcion se ejecuta cuando se presiona el boton de favorito
                                                            // de cualquier carta. Para distinguir la carta se utiliza el 
                                                            // parametro cardIndex.
    public void SetAsFavorite(int cardIndex)
    {
        var favAct = Resources.Load<Sprite>("Sprites/favbtnOs");
        var Unfav = Resources.Load<Sprite>("Sprites/heartfav"); 

        Debug.Log("Set this card as fav: " + cards[cardIndex].assetBundleName);
        cards[cardIndex].isFavorite = !cards[cardIndex].isFavorite;

        if (cards[cardIndex].isFavorite)
        {
            cards[cardIndex].cardGameObject.transform.GetChild(4).GetComponent<Button>().image.sprite = favAct;
        }
        else
        {
            cards[cardIndex].cardGameObject.transform.GetChild(4).GetComponent<Button>().image.sprite = Unfav;
        }
        SaveLoadData.stashFavoriteIdList(favoriteIdList);
        SaveLoadData.SaveToFile();

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
        _ux_Helper.CollectionView.SetActive(false);
        _ux_Helper.MainDock.SetActive(false);
        _ux_Helper.ActionDock.SetActive(true);
    }

                                                            // Es llamado al dar click en el boton de filtrar por 
                                                            // favorito.
    public void FilterByFavorite()
    {
        var favAct = Resources.Load<Sprite>("Sprites/favbtnOs");
        var Unfav = Resources.Load<Sprite>("Sprites/favBtnO");

        filterByFavorite = !filterByFavorite;
        FilterCards();

        if (filterByFavorite) {
            filterByFavorite = true;
            favorite.image.sprite = favAct;
        }
        else
        {
            favorite.image.sprite = Unfav;
        }
        Debug.Log("Filtrando por favs");
                                                            // Vuelve al boton un toggle button que puede prender y
                                                            // apagar el filtrado por favoritos,.
    }

    public void FilterByAssetBundleName()
    {
        filterByAssetBundle = !filterByAssetBundle;
        var search = Resources.Load<Sprite>("Sprites/searchbtn");
        var clear = Resources.Load<Sprite>("Sprites/cleansearch");

        if(filterByAssetBundle!= false)
        {
            searchbtn.image.sprite = clear;
            FilterCards();
        }
        else
        {
            filterByAssetBundle = false;
            searchbtn.image.sprite = search;
            inputAssetBundleNameQuery.Select();
            inputAssetBundleNameQuery.text = "";
            FilterCards();

        }
        //FilterByFavorite();
        
    }

    // Realiza el filtrado tomando en cuenta todos los filtros
    // al mismo timepo. Cuando ningun filtro esta activado, activa
    // todas las tarjetas.
    public void FilterCards()
    {
        foreach (var card in cards)
            if (card.cardGameObject != null)
            {
                bool shouldShowCard = doesCardPassedAllFilters(card);
                card.cardGameObject.SetActive(shouldShowCard);
            }
    }

    public bool doesCardPassedAllFilters(Card card)
    {
        return doesCardPassedFavoriteFilter(card) &&
                doesCardPassedAssetBundleNameFilter(card);
    }

    public bool doesCardPassedFavoriteFilter(Card card)
    {
        return (!filterByFavorite | card.isFavorite);
    }

    public bool doesCardPassedAssetBundleNameFilter(Card card)
    {
        string assetBundleName = RemoveAccents(card.assetBundleName.Trim().ToLower());
        string assetBundleNameQuery = RemoveAccents(inputAssetBundleNameQuery.text.Trim().ToLower());

        if (assetBundleNameQuery == "")
            return true;

        return assetBundleName.IndexOf(assetBundleNameQuery) > -1;
    }


    public string RemoveAccents(string text)
    {
        StringBuilder sbReturn = new StringBuilder();
        var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
        foreach (char letter in arrayText)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }
        return sbReturn.ToString();
    }
}