using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerHP = 100;
    public string selectedCharacter = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCharacter(string character)
    {
        selectedCharacter = character.Replace("Button_","");
    }

    public bool ReceiveDamage(int damage)
    {
        playerHP -= damage;
        return playerHP > 0;
    }

}
