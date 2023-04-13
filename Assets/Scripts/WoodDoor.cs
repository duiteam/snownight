using UnityEngine;

public class WoodDoor : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Projectile")) return;
        
        SFXBehavior.Instance.PlaySFX(SFXTracks.DoorBroken);
        Destroy(gameObject);
    }
}
