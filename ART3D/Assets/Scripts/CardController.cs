using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    public BundleManager _bundleManager;
    public UX_Helper _ux_Helper;
    public GameObject cardPanel;
    public GameObject cardPrefab;

    private bool filterByFavorite = false;

    public void Start()
    {
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

    public void AddFavoriteButtonListener(Button favBtn, int cardIndex)
    {
        favBtn.onClick.AddListener(delegate { SetAsFavorite(cardIndex); });
    }

    public void SetAsFavorite(int cardIndex)
    {
        Debug.Log("Set this card as fav: " + cards[cardIndex].assetBundleName);
        cards[cardIndex].isFavorite = !cards[cardIndex].isFavorite;
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
