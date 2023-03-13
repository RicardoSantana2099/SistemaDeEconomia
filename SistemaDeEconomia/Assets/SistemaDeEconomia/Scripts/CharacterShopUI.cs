﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CharacterShopUI : MonoBehaviour
{
	[Header("Layout Settings")]
	[SerializeField] float itemSpacing = .5f;
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
	[SerializeField] Button scrollUpButton;

	[Space(20)]
	[Header("Main Menu")]
	[SerializeField] Image mainMenuCharacterImage;
	[SerializeField] TMP_Text mainMenuCharacterName;

	[Space(20)]
	[Header("Scroll View")]
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] GameObject topScrollFade;
	[SerializeField] GameObject bottomScrollFade;

	[Space(20)]
	[Header("Purchase Fx & Error messages")]
	[SerializeField] ParticleSystem purchaseFx;
	[SerializeField] Transform purchaseFxPos;
	[SerializeField] TMP_Text noEnoughCoinsText;

	int newSelectedItemIndex = 0;
	int previousSelectedItemIndex = 0;

	void Start()
	{
		//Move Fx to the exact same position of the coin image (for different screen sizes)
		purchaseFx.transform.position = purchaseFxPos.position;

		AddShopEvents();

		//Fill the shop's UI list with items
		

		//Set selected character in the playerDataManager .
		SetSelectedCharacter();

		//Select UI item
		SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

		//update player skin (Main menu)
		ChangePlayerSkin();

		//Auto scroll to selected character  in the shop
		AutoScrollShopList(GameDataManager.GetSelectedCharacterIndex());
	}

	void AutoScrollShopList(int itemIndex)
	{
		//scrollRect.verticalNormalizedPosition = 0f; //means scroll to the bottom
		//scrollRect.verticalNormalizedPosition = 1f; //means scroll to the top
		scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - (itemIndex / (float)(characterDB.CharactersCount - 1)));
	}

	void SetSelectedCharacter()
	{
		//Get saved index
		int index = GameDataManager.GetSelectedCharacterIndex();

		//Set selected character
		GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
	}

	

	void ChangePlayerSkin()
	{
		Character character = GameDataManager.GetSelectedCharacter();
		if (character.image != null)
		{
			//Change Main menu's info (image & text)
			mainMenuCharacterImage.sprite = character.image;
			mainMenuCharacterName.text = character.name;

			//Set selected Character Image at the top of shop menu
			selectedCharacterIcon.sprite = GameDataManager.GetSelectedCharacter().image;
		}
	}

	void OnItemSelected(int index)
	{
		// Select item in the UI
		SelectItemUI(index);

		//Save Data
		GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);

		//Change Player Skin
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

		if (GameDataManager.CanSpendCoins(character.price))
		{
			//Proceed with the purchase operation
			GameDataManager.SpendCoins(character.price);

			//Play purchase FX
			purchaseFx.Play();

			//Update Coins UI text
			GameSharedUI.Instance.UpdateCoinsUIText();

			//Update DB's Data
			characterDB.PurchaseCharacter(index);

			uiItem.SetCharacterAsPurchased();
			uiItem.OnItemSelect(index, OnItemSelected);

			//Add purchased item to Shop Data
			GameDataManager.AddPurchasedCharacter(index);

		}
		else
		{
			//No enough coins..
			AnimateNoMoreCoinsText();
			uiItem.AnimateShakeItem();
		}
	}

	void AnimateNoMoreCoinsText()
	{
		// Complete animations (if it's running)
		noEnoughCoinsText.transform.DOComplete();
		noEnoughCoinsText.DOComplete();

		noEnoughCoinsText.transform.DOShakePosition(3f, new Vector3(5f, 0f, 0f), 10, 0);
		noEnoughCoinsText.DOFade(1f, 3f).From(0f).OnComplete(() => {
			noEnoughCoinsText.DOFade(0f, 1f);
		});

	}

	void AddShopEvents()
	{
		openShopButton.onClick.RemoveAllListeners();
		openShopButton.onClick.AddListener(OpenShop);

		closeShopButton.onClick.RemoveAllListeners();
		closeShopButton.onClick.AddListener(CloseShop);

		scrollRect.onValueChanged.RemoveAllListeners();
		scrollRect.onValueChanged.AddListener(OnShopListScroll);

		scrollUpButton.onClick.RemoveAllListeners();
		scrollUpButton.onClick.AddListener(OnScollUpClicked);
	}

	void OnScollUpClicked()
	{
		scrollRect.DOVerticalNormalizedPos(1f, .5f).SetEase(Ease.OutBack);
	}

	void OnShopListScroll(Vector2 value)
	{
		float scrollY = value.y;
		//Top fade 
		if (scrollY < 1f)
			topScrollFade.SetActive(true);
		else
			topScrollFade.SetActive(false);

		//Bottom fade 
		if (scrollY > 0f)
			bottomScrollFade.SetActive(true);
		else
			bottomScrollFade.SetActive(false);

		//Scroll Up button
		if (scrollY < .7f)
			scrollUpButton.gameObject.SetActive(true);
		else
			scrollUpButton.gameObject.SetActive(false);
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