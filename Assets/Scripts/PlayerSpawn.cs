using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform spawnPoint; 
    void Start()
    {
        GameObject character = Resources.Load<GameObject>("Prefabs/" + GameManager.Instance.GetCharacter());

        if (character != null)
        {
            Instantiate(character, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Character does not exist");
        }
    }

    void Update()
    {
        
    }
}
