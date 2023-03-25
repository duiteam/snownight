using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum SnowdriftType
{
    Ephemeral,
    Permanent
}

public class SnowdriftBehavior : MonoBehaviour
{
    [Tooltip("What type of snowdrift is this?")]
    public SnowdriftType snowdriftType = SnowdriftType.Ephemeral;
    
    [Tooltip("How many snowballs can be collected from this snowdrift? (Only applicable to ephemeral snowdrifts)")]
    public int allowableSnowballCount = 1;
    
    private int m_CurrentSnowballLeft;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_CurrentSnowballLeft = allowableSnowballCount;
    }
    
    public void CollectSnow()
    {
        if (snowdriftType == SnowdriftType.Permanent) return;
        
        m_CurrentSnowballLeft--;
        if (m_CurrentSnowballLeft <= 0)
        {
            Destroy(gameObject);
        }
    }
}
