using UnityEngine;

public enum PlayerSnowInventory
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4
}

public struct PlayerVelocityScaleFactor
{
    public float Jump;
    public float Walk;
}

public enum PlayerOrientation
{
    Left,
    Right
}

// have player snow inventory and scale player accordingly
// map player snow inventory to corresponding scale
public static class PlayerSnowInventoryExtensions
{
    public static Vector3 ToColliderScale(this PlayerSnowInventory inventory)
    {
        return inventory switch
        {
            PlayerSnowInventory.None => new Vector3(1.0f, 0.6f, 1.0f),
            PlayerSnowInventory.One => new Vector3(1.0f, 0.7f, 1.0f),
            PlayerSnowInventory.Two => new Vector3(1.0f, 0.8f, 1.0f),
            PlayerSnowInventory.Three => new Vector3(1.0f, 0.9f, 1.0f),
            PlayerSnowInventory.Four => new Vector3(1.0f, 1.0f, 1.0f)
        };
    }

    public static PlayerVelocityScaleFactor ToVelocityScale(this PlayerSnowInventory inventory)
    {
        const float velocityMultiplier = 0.2f;

        var inventoryMultiplier = 4 - (int)inventory;

        return new PlayerVelocityScaleFactor
        {
            Jump = 1.0f + inventoryMultiplier * (velocityMultiplier * 0.5f),
            Walk = 1.0f + inventoryMultiplier * velocityMultiplier
        };
    }
}

[CreateAssetMenu]
public class PlayerSharedState : ScriptableObject
{
    [Tooltip("PlayerSnowInventory is an enum that represents the amount of snow the player has.")]
    public PlayerSnowInventory snowInventory = PlayerSnowInventory.Four;
}