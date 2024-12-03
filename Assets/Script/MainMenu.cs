using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image characterDisplay;         
    public Sprite[] characterSprites;      
    public Button leftButton, rightButton;
    public GameObject selectButton;   
    public GameObject selectedButton;
    public Button playButton;

    private int currentCharacterIndex = 0; 
    private int selectedCharacterIndex = -1; 

    void Start()
    {
        UpdateCharacterDisplay();
        playButton.interactable = false;
    }

    public void NextCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % characterSprites.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + characterSprites.Length) % characterSprites.Length;
        UpdateCharacterDisplay();
    }

    public void SelectCharacter()
    {
        if (selectedCharacterIndex == currentCharacterIndex)
        {
            selectedCharacterIndex = -1;
            UpdateButtonState(false);
            playButton.interactable = false;
        }
        else
        {
            selectedCharacterIndex = currentCharacterIndex;
            UpdateButtonState(true);
            playButton.interactable = true;
        }
    }

    private void UpdateCharacterDisplay()
    {
        characterDisplay.sprite = characterSprites[currentCharacterIndex];

        if (selectedCharacterIndex == currentCharacterIndex)
        {
            UpdateButtonState(true);
        }
        else
        {
            UpdateButtonState(false);
        }
    }

    private void UpdateButtonState(bool isSelected)
    {
        selectButton.SetActive(!isSelected);
        selectedButton.SetActive(isSelected);
    }

    public void PlayGame()
    {
        if (selectedCharacterIndex >= 0)
        {
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
            SceneManager.LoadScene("Game");
        }
    }
}
