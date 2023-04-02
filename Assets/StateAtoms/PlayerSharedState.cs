using System;
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

public struct PlayerColliderModifier
{
    public Vector2 Offset;
    public Vector2 Size;
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
            PlayerSnowInventory.Four => new Vector3(1.0f, 1.0f, 1.0f),
            _ => throw new ArgumentOutOfRangeException(nameof(inventory), inventory, null)
        };
    }

    public static bool Increment(this ref PlayerSnowInventory inventory)
    {
        if (inventory >= PlayerSnowInventory.Four) return false;

        inventory++;

        return true;
    }

    public static bool Decrement(this ref PlayerSnowInventory inventory)
    {
        if (inventory <= PlayerSnowInventory.None) return false;

        inventory--;

        return true;
    }

    public static Sprite ToSprite(this PlayerSnowInventory inventory)
    {
        return inventory switch
        {
            PlayerSnowInventory.None => Resources.Load<Sprite>("SeparateSprite/Snowman1"),
            PlayerSnowInventory.One => Resources.Load<Sprite>("SeparateSprite/Snowman2"),
            PlayerSnowInventory.Two => Resources.Load<Sprite>("SeparateSprite/Snowman3"),
            PlayerSnowInventory.Three => Resources.Load<Sprite>("SeparateSprite/Snowman4"),
            PlayerSnowInventory.Four => Resources.Load<Sprite>("SeparateSprite/Snowman5"),
            _ => throw new ArgumentOutOfRangeException(nameof(inventory), inventory, null)
        };
    }
    
    public static Sprite ToPlatformSprite(this PlayerSnowInventory inventory)
    {
        return inventory switch
        {
            PlayerSnowInventory.None => Resources.Load<Sprite>("SeparateSprite/PressurePlatform1"),
            PlayerSnowInventory.One => Resources.Load<Sprite>("SeparateSprite/PressurePlatform2"),
            PlayerSnowInventory.Two => Resources.Load<Sprite>("SeparateSprite/PressurePlatform3"),
            PlayerSnowInventory.Three => Resources.Load<Sprite>("SeparateSprite/PressurePlatform4"),
            PlayerSnowInventory.Four => Resources.Load<Sprite>("SeparateSprite/PressurePlatform4"),
            _ => throw new ArgumentOutOfRangeException(nameof(inventory), inventory, null)
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
    
    public static PlayerColliderModifier ToColliderModifier(this PlayerSnowInventory inventory)
    {
        return inventory switch
        {
            PlayerSnowInventory.None => new PlayerColliderModifier
            {
                Offset = new Vector2(0.1266441f, -1.664455f),
                Size = new Vector2(2.865166f, 3.671089f)
            },
            PlayerSnowInventory.One => new PlayerColliderModifier
            {
                Offset = new Vector2(0.08141708f, -0.9407792f),
                Size = new Vector2(3.245094f, 5.118442f)
            },
            PlayerSnowInventory.Two => new PlayerColliderModifier
            {
                Offset = new Vector2(0.08141708f, -0.5518031f),
                Size = new Vector2(3.679298f, 5.896394f)
            },
            PlayerSnowInventory.Three => new PlayerColliderModifier
            {
                Offset = new Vector2(0.1356926f, -0.2351947f),
                Size = new Vector2(4.04113f, 6.529611f)
            },
            PlayerSnowInventory.Four => new PlayerColliderModifier
            {
                Offset = new Vector2(0, 0),
                Size = new Vector2(5f, 7f)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(inventory), inventory, null)
        };
    }
}

[CreateAssetMenu]
public class PlayerSharedState : ScriptableObject
{
    [Tooltip("PlayerSnowInventory is an enum that represents the amount of snow the player has.")]
    // do not persist this value
    [NonSerialized]
    public PlayerSnowInventory snowInventory = PlayerSnowInventory.Four;
}