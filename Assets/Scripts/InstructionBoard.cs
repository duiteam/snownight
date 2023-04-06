using System.Collections;
using UnityEngine;

public class InstructionBoard : MonoBehaviour
{
    public GameObject InstructionUI;
    
    private Animator m_Animator;
    
    private void Awake()
    {
        m_Animator = InstructionUI.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        InstructionUI.SetActive(true);
        m_Animator.Play("TipsContainerFadeInAnimation");
        yield return new WaitForSeconds(15.0f / 60.0f);
    }

    private IEnumerator FadeOut()
    {
        m_Animator.Play("TipsContainerFadeOutAnimation");
        yield return new WaitForSeconds(15.0f / 60.0f);
        InstructionUI.SetActive(false);
    }
}

