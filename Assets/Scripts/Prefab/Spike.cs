using UnityEngine;
using UnityEngine.SceneManagement;

public class Spike : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) SceneManager.LoadScene("ScratchpadScene");
    }
}