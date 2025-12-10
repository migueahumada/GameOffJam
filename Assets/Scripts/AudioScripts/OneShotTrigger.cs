using FMODUnity;
using UnityEngine;

public class OneShotTrigger : MonoBehaviour
{
    [SerializeField] private EventReference[] eventRef;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int isPlayed = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OneShotSFX(int indexSFX)
    {
        SFXManager.instance.PlaySFXAttached(indexSFX, gameObject);
    }

    public void PlaySFX(int indexSFX)
    {
        SFXManager.instance.PlaySFX(indexSFX);
    }

    public void PlayOneAtOnce(int indexSFX)
    {
        if (isPlayed < 3) isPlayed ++;    
        else SFXManager.instance.PlaySFXOneAtTime(indexSFX);
    }

    public void PlayByReference (int indexSFX)
    {
        RuntimeManager.PlayOneShot(eventRef[indexSFX]);
    }
}
