using UnityEngine;

public class OneShotTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
}
