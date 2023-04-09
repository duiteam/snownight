using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AspectFillImage : MonoBehaviour
{
    [SerializeField] private RectTransform targetArea;
    private Image m_Image;
    private int m_LastScreenSizeX;
    private int m_LastScreenSizeY;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    private void Start()
    {
        UpdateImageScale();
    }

    private void Update()
    {
        if (Screen.width == m_LastScreenSizeX && Screen.height == m_LastScreenSizeY) return;
        
        m_LastScreenSizeX = Screen.width;
        m_LastScreenSizeY = Screen.height;
        UpdateImageScale();
    }

    private void UpdateImageScale()
    {
        if (m_Image.sprite == null || targetArea == null) return;

        float targetRatio = targetArea.rect.width / targetArea.rect.height;
        float imageRatio = (float)m_Image.sprite.texture.width / (float)m_Image.sprite.texture.height;

        if (imageRatio <= targetRatio)
        {
            float scaleFactor = targetArea.rect.width / m_Image.rectTransform.rect.width;
            m_Image.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
        else
        {
            float scaleFactor = targetArea.rect.height / m_Image.rectTransform.rect.height;
            m_Image.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }
}