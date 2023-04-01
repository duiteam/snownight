using UnityEngine;
using System.Collections;

public class LevelNameFadeOut : MonoBehaviour
{
    public float fadeTime = 2f;  // ����ʱ��
    public float timer = 0f;  // ��ʱ��
    private SpriteRenderer spriteRenderer;  // Sprite Renderer���

    void Start()
    {
        timer = 0f;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fadeTime)
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        while (spriteRenderer.color.a > 0)
        {
            float alpha = Mathf.Lerp(1f, 0f, (timer - fadeTime) / fadeTime);
            Color newColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            spriteRenderer.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}
