using System;
using System.Xml;

public class DialogueChunkAdornmentPuzzle : DialogueChunkAdornment
{
    public DialogueChunkAdornmentPuzzle(XmlNode element) : base(element)
    {
    }

    public string GetPuzzleName()
    {
        // assert that Node is an XmlNode
        if (!(Element is XmlNode xmlNode)) throw new Exception("Node must be an XmlNode");

        // get the puzzle name
        var puzzleName = xmlNode.Attributes?["name"].Value;

        return puzzleName;
    }

    public Puzzle GetPuzzle()
    {
        return Puzzles.instance.Get(GetPuzzleName());
    }
}