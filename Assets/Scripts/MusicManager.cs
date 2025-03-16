using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
