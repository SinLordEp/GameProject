using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSelection : MonoBehaviour
{
    public Button[] buttons;
    private Button selectedButton = null;
    public Button startGame;

    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => SelectButton(button));
        }
        startGame.onClick.AddListener(() => StartGame());
    }

    void SelectButton(Button clickedButton)
    {
        if (selectedButton == clickedButton) return;
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
        clickedButton.interactable = false;
        selectedButton = clickedButton;
        startGame.interactable = true;
    }

    void StartGame()
    {   
        string characterName = (selectedButton.gameObject.name).Replace("Button_","");
        GameManager.Instance.SetCharacter(characterName);
        SceneManager.LoadScene("Scene1");
    }

    void Update()
    {
        
    }
}
