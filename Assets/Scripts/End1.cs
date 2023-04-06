using UnityEngine;
using UnityEngine.SceneManagement;

public class End1 : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Debug.Log("ｽ眄ﾖ1");
            SceneManager.LoadScene("End1");
        }
    }
}