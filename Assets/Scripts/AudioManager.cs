using UnityEngine;

public class AudioManger : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip runSound;
    public AudioClip chargeSound;
    public AudioClip attackSound;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtSound);
    }

    public void PlayRunSound()
    {
        if(audioSource.clip == chargeSound)
        {
            audioSource.Stop();
        }
        audioSource.clip = runSound;
        if(!audioSource.isPlaying){
            audioSource.Play();
        }   
    }

    public void PlayChargeSound()
    {
        if(audioSource.clip == runSound)
        {
            audioSource.Stop();
        }
        audioSource.clip = chargeSound;
        if(!audioSource.isPlaying){
            audioSource.Play();
        }
    }

    public void PlayAttackSound()
    {
        audioSource.Stop();
    }

}
