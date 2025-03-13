using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int playerHP = 100;
    private int maxHP = 100;
    private int selectedCharacter = 0;

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

    void Update()
    {
        
    }

    public void SetCharacter(string character)
    {
        switch(character)
        {
            case "Witch_Stone":
                maxHP = 200;
                selectedCharacter = 0;
                break;
            case "Witch_Fire":
                maxHP = 100;
                selectedCharacter = 1;
                break;
            case "Witch_Light":
                maxHP = 150;
                selectedCharacter = 2;
                break;
            default:
               Debug.Log("Unknown charater type");
                break;
        }
        playerHP = maxHP;
        Debug.Log("Character Name: " + selectedCharacter);
    }

    public int GetCharacter()
    {
        return selectedCharacter;
    }

    public bool ReceiveDamage(int damage)
    {
        playerHP -= damage;
        return playerHP > 0;
    }

    public int GetHP()
    {
        return playerHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }


}
