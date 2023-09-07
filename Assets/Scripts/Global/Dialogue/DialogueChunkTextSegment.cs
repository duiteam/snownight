public struct DialogueChunkTextSegment
{
    public string Character;
    public string FormatPrefix;
    public string FormatSuffix;

    public override string ToString()
    {
        return FormatPrefix + Character + FormatSuffix;
    }

    public static DialogueChunkTextSegment CreateEmpty()
    {
        return new DialogueChunkTextSegment
        {
            Character = "",
            FormatPrefix = "",
            FormatSuffix = ""
        };
    }
}