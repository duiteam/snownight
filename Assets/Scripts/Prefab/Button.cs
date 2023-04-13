using System.Collections;
using UnityEngine;

public class Button : MonoBehaviour
{
    public bool requirement
    {
        get => m_CollidedObjects > 0;
        set => throw new System.NotImplementedException();
    }

    public int collidedObjects
    {
        get => m_CollidedObjects;
        set
        {
            var previousValue = m_CollidedObjects;
            m_CollidedObjects = value;
            if (value == 1 && previousValue == 0)
            {
                SFXBehavior.Instance.PlaySFX(SFXTracks.DoorOpen);
            } else if (value == 0 && previousValue == 1)
            {
                SFXBehavior.Instance.PlaySFX(SFXTracks.DoorClose);
            }
        }
    }
    
    private int m_CollidedObjects;
    private int m_UnstableCollidedObjects;
    
    private double m_LastStateChangeTime;
    private IEnumerator m_SetCollidedObjectsCoroutine;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // when player or snowball collide with button, requirement = true
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Collidable::CollectableSnow"))
        {
            SetCollidedObjects(1);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        // when player or snowball exit button, requirement = false
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Collidable::CollectableSnow"))
        {
            SetCollidedObjects(-1);
        }
    }
    
    private void SetCollidedObjects(int delta) {
        m_UnstableCollidedObjects += delta;

        if (m_SetCollidedObjectsCoroutine != null)
        {
            StopCoroutine(m_SetCollidedObjectsCoroutine);
        }
        m_SetCollidedObjectsCoroutine = SetCollidedObjectsCoroutine(m_UnstableCollidedObjects);
        StartCoroutine(m_SetCollidedObjectsCoroutine);
    }
    
    private IEnumerator SetCollidedObjectsCoroutine(int value)
    {
        yield return new WaitForSeconds(0.1f);
        collidedObjects = value;
        m_SetCollidedObjectsCoroutine = null;
    }

}
