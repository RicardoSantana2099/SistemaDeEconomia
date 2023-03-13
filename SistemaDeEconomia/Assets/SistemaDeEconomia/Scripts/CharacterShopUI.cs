using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterShopUI : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] float itemSpacing = 5f;
    float itemHeight;

    [Header("UI elements")]
    [SerializeField] Image selectedCharacterIcon;
    [SerializeField] Transform ShopMenu;
    [SerializeField] Transform ShopItemsContainer;
    [SerializeField] GameObject itemPrefab;
    [Space(20)]
    [SerializeField] CharacterShopDatabase characterDB;

    [Space(20)]
    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button openShopButton;
    [SerializeField] Button closeShopButton;

    [Space(20)]
    [Header("Main Menu")]
    [SerializeField] Image mainMenuCharacterImage;
    [SerializeField] TMP_Text mainMenuCharacterName;

    int newSelectedItemIndex = 0;
    int previousSelectedItemIndex = 0;

    void Start()
    {
        AddShopEvents();
        GenerateShopItemsUI();
        SetSelectedCharacter();
        SelectItemUI(GameDataManager.GetSelectedCharacterIndex ());
        ChangePlayerSkin();
    }

    void SetSelectedCharacter()
    {
        int index = GameDataManager.GetSelectedCharacterIndex();

        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
    }

    void GenerateShopItemsUI()
    {
        for(int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
        {
            int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);
        }

        //Borrar items
        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        ShopItemsContainer.DetachChildren();

        //Genera items
        for(int i = 0; i < characterDB.CharactersCount; i++)
        {
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();

            //Mueve el item de posición
            uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));

            //Nombre de items en la jerarquia
            uiItem.gameObject.name = "Item" + i + "-" + character.name;

            //Añade información de un item
            uiItem.SetCharacterName(character.name);
            uiItem.SetCharacterImage(character.image);
            uiItem.SetCharacterSpeed(character.speed);
            uiItem.SetCharacterPower(character.power);
            uiItem.SetCharacterPrice(character.price);

            if(character.isPurchased)
            {
                uiItem.SetCharacterAsPurchased();
                uiItem.OnItemSelect(i, OnItemSelected);
            }
            else
            {
                uiItem.SetCharacterPrice(character.price);
                uiItem.OnItemPurchase(i, OnItemPurchased);
            }
            ShopItemsContainer.GetComponent<RectTransform>().sizeDelta =
                Vector2.up * ((itemHeight + itemSpacing) * characterDB.CharactersCount + itemSpacing);
        }
    }

    void ChangePlayerSkin()
    {
        Character character = GameDataManager.GetSelectedCharacter();
        if(character.image != null)
        {
            mainMenuCharacterImage.sprite = character.image;
            mainMenuCharacterName.text = character.name;

            selectedCharacterIcon.sprite = GameDataManager.GetSelectedCharacter().image;
        }
    }

    void OnItemSelected(int index)
    {
        SelectItemUI(index);

        selectedCharacterIcon.sprite = characterDB.GetCharacter(index).image;

        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);

        ChangePlayerSkin();
    }
    void SelectItemUI(int itemIndex)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        CharacterItemUI prevUiItem = GetItemUI(previousSelectedItemIndex);
        CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

        prevUiItem.DeselectItem();
        newUiItem.SelectItem();
    }

    CharacterItemUI GetItemUI(int index)
    {
        return ShopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
    }
    void OnItemPurchased(int index)
    {
        Character character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItemUI(index);

        if(GameDataManager.CanSpendCoins(character.price))
        {
            GameDataManager.SpendCoins(character.price);
            GameSharedUI.Instance.UpdateCoinsUIText();
            characterDB.PurchaseCharacter(index);
            uiItem.SetCharacterAsPurchased();
            uiItem.OnItemSelect(index, OnItemSelected);
            GameDataManager.AddPurchasedCharacter(index);
        }
        else
        {
            Debug.Log("No enough coins!!");
        }
    }

    void AddShopEvents()
    {
        openShopButton.onClick.RemoveAllListeners();
        openShopButton.onClick.AddListener(OpenShop);

        closeShopButton.onClick.RemoveAllListeners();
        closeShopButton.onClick.AddListener(CloseShop);
    }

    void OpenShop()
    {
        shopUI.SetActive(true);
    }

    void CloseShop()
    {
        shopUI.SetActive(false);
    }
}
