using System.Collections;
using UnityEngine;

public class LevelLoaderBehavior : MonoBehaviour
{
    private Animator m_Animator;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    
    public IEnumerator StartFadeOut()
    {
        m_Animator.SetTrigger("Start");
        
        yield return new WaitForSeconds(0.75f);
    }
    
    public IEnumerator StartFadeIn()
    {
        yield return new WaitForSeconds(0.75f);
    }
}
