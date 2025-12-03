using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueSequence : MonoBehaviour
{
    [Header("Image Sequence")]
    public List<Sprite> images = new List<Sprite>();
    public Image targetImage;
    [SerializeField] private Image Enter;
    private int currentIndex = 0;
    RectTransform panel;

    void Start()
    {
        if (PlayerPrefs.GetString("welcome") == "true")
        {
            Debug.Log(PlayerPrefs.GetString("welcome"));
            gameObject.SetActive(false);
        }
        else {
            PlayerPrefs.SetString("welcome", "true");
            gameObject.SetActive(true);
        }

        panel = GetComponent<RectTransform>();
        if (images.Count > 0 && targetImage != null)
        {
            targetImage.sprite = images[0];
        }
        FlashHint(Enter);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            
            ShowNextImage();
        }
    }

    private void ShowNextImage()
    {
        //audio to accept
        SFXManager.instance.PlaySFX(4);
        
        currentIndex++;

        // Finished? Deactivate object
        if (currentIndex >= images.Count)
        {
            DoPopOutAnimation(panel);
            return;
        }

        targetImage.sprite = images[currentIndex];
        DoPopAnimation(targetImage.rectTransform);
    }

    // -----------------------------
    // UI FLASH ANIMATION FUNCTION
    // -----------------------------
    public void FlashHint(Image img)
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);

        img.DOFade(1f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    
    private void DoPopAnimation(RectTransform rt)
    {
        rt.localScale = Vector3.one * 0.8f;       // start smaller
        rt.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    private void DoPopOutAnimation(RectTransform rt)
    {
        rt.DOScale(0f, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            rt.gameObject.SetActive(false);
        });;
    }

}
