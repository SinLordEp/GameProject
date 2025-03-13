using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform spawnPoint; 
    public GameObject[] characters;
    void Start()
    {
        int selectedIndex = GameManager.Instance.GetCharacter();
        Instantiate(characters[selectedIndex], spawnPoint.position, Quaternion.identity);
        Debug.Log("Character spawned: " + characters[selectedIndex].name);
    }

    void Update()
    {
        
    }
}
