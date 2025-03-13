using UnityEngine;

public class SkillDespawn : MonoBehaviour
{
    public float lifetime = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
    }

}
