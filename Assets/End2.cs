using UnityEngine;
using UnityEngine.SceneManagement;

public class End2 : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            Debug.Log("ｽ眄ﾖ2");
            SceneManager.LoadScene("End2");
        }
    }
}