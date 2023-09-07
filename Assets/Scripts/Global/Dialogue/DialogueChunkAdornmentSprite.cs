using System;
using System.Xml;
using UnityEngine;

public class DialogueChunkAdornmentSprite : DialogueChunkAdornment
{
    public DialogueChunkAdornmentSprite(XmlNode element) : base(element)
    {
    }

    public Sprite GetSprite()
    {
        // assert that Node is an XmlNode
        if (!(Element is XmlNode xmlNode)) throw new Exception("Node must be an XmlNode");

        // get the sprite path
        var spritePath = xmlNode.Attributes?["path"].Value;

        return Resources.Load<Sprite>(spritePath);
    }
}