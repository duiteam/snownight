using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

public static class DialogueParser
{
    public static List<DialogueChunk> Parse(string xmlTextFragment)
    {
        var modifiedXmlText = "<root>" + xmlTextFragment + "</root>";
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(modifiedXmlText);

        Debug.Assert(xmlDocument.DocumentElement != null, "xmlDocument.DocumentElement != null");
        var xmlNode = xmlDocument.DocumentElement;

        var dialogueChunks = new List<DialogueChunk>();

        foreach (var node in xmlNode.ChildNodes)
            switch (node)
            {
                // if the node is a text, emit a DialogueChunkText
                case XmlText xmlText:
                    dialogueChunks.Add(new DialogueChunkText(xmlText, xmlText.Value));
                    break;

                // if the node is an element,
                // emit an DialogueChunkAdornmentPuzzle if <adornment-puzzle> is found
                // emit an DialogueChunkAdornmentSprite if <adornment-sprite> is found
                case XmlElement xmlElement:
                    switch (xmlElement.Name)
                    {
                        case "adornment-puzzle":
                            dialogueChunks.Add(new DialogueChunkAdornmentPuzzle(xmlElement));
                            break;
                        case "adornment-sprite":
                            dialogueChunks.Add(new DialogueChunkAdornmentSprite(xmlElement));
                            break;
                        default:
                            throw new XmlException("Unknown element name: " + xmlElement.Name);
                    }

                    break;

                // if the node is a comment, ignore it
                case XmlComment _:
                    break;

                // if the node is something else, throw an exception
                default:
                    throw new XmlException("Unknown node type: " + node.GetType());
            }

        return dialogueChunks;
    }
}