using UnityEngine;
using UnityEngine.SceneManagement;

public class Boundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")) CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}