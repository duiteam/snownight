public abstract class DialogueChunk
{
    // Element is either a XmlNode or a XmlText, depending on the type of chunk
    protected readonly object Element;

    protected DialogueChunk(object element)
    {
        Element = element;
    }
}