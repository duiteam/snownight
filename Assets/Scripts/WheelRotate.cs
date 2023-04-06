using UnityEngine;
using UnityEngine.SceneManagement;

public class WheelRotate : MonoBehaviour
{
    private float rotZ;
    public float RotationSpeed;
    public bool ClockwiseRotation;
   

    // Update is called once per frame
    void Update()
    {
      if(ClockwiseRotation == false)
        {
            rotZ += Time.deltaTime * RotationSpeed;
        }
        else
        {
            rotZ += -Time.deltaTime * RotationSpeed; 
        }
      transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player")) CustomSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
