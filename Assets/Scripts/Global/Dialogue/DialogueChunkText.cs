using System.Collections.Generic;
using System.Xml;

public class DialogueChunkText : DialogueChunk
{
    private readonly List<DialogueChunkTextSegment> m_Segments = new List<DialogueChunkTextSegment>();
    private readonly string m_Text;

    public DialogueChunkText(XmlText element, string text) : base(element)
    {
        m_Text = text.Replace("[", "<").Replace("]", ">");

        // split the text into segments. The <> tags are guaranteed to not be nested.
        // e.g.
        // - "Hello <b>world</b>!" -> [DialogueChunkTextSegment(["H", "e", "l", "l", "o", " "], "", ""), DialogueChunkTextSegment(["w", "o", "r", "l", "d"], "<b>", "</b>"), DialogueChunkTextSegment(["!"], "", "")]
        // - "<b>Hello</b> <i>world</i>!" -> [DialogueChunkTextSegment(["H", "e", "l", "l", "o"], "<b>", "</b>"), DialogueChunkTextSegment([" "], "", ""), DialogueChunkTextSegment(["w", "o", "r", "l", "d"], "<i>", "</i>"), DialogueChunkTextSegment(["!"], "", "")]

        var index = 0;
        while (index < m_Text.Length)
        {
            string prefix = "", content = "", suffix = "";

            if (m_Text[index] == '<' && m_Text.IndexOf('>', index) != -1)
            {
                var closeIndex = m_Text.IndexOf('>', index);
                if (m_Text[index + 1] == '/')
                {
                    suffix = m_Text.Substring(index, closeIndex - index + 1);
                    index = closeIndex + 1;
                }
                else
                {
                    prefix = m_Text.Substring(index, closeIndex - index + 1);
                    index = closeIndex + 1;

                    closeIndex = m_Text.IndexOf('<', index);
                    if (closeIndex != -1 && m_Text.IndexOf('>', closeIndex) != -1)
                    {
                        var suffixStart = closeIndex;
                        closeIndex = m_Text.IndexOf('>', closeIndex);
                        suffix = m_Text.Substring(suffixStart, closeIndex - suffixStart + 1);
                    }
                }
            }

            var nextOpenTagIndex = m_Text.IndexOf('<', index);
            if (nextOpenTagIndex == -1) // No more tags found
                content = m_Text.Substring(index);
            else
                content = m_Text.Substring(index, nextOpenTagIndex - index);

            index += content.Length;

            m_Segments.Add(new DialogueChunkTextSegment
            {
                Character = content,
                FormatPrefix = prefix,
                FormatSuffix = suffix
            });
        }
    }

    // EnumerateTextSegments enumerates the text by "character chunks". That is,
    // it should yield the text character by character, with appropriate prefix and suffix of that segment.
    // The prefix and suffix should not be duplicated when emitting multiple characters of that same segment,
    // but rather the prefix and suffix should always be emitted, and the characters inside the segment should be
    // emitted one by one.
    // E.g.:
    // - [DialogueChunkTextSegment(["H", "e", "l", "l", "o", " "], "", ""), DialogueChunkTextSegment(["w", "o", "r", "l", "d"], "<b>", "</b>"), DialogueChunkTextSegment(["!"], "", "")] shall emit:
    // "H", "He", "Hel", "Hell", "Hello", "Hello ", "Hello <b>w</b>", "Hello <b>wo</b>", "Hello <b>wor</b>", "Hello <b>worl</b>", "Hello <b>world</b>", "Hello <b>world</b>!"
    public IEnumerable<string> EnumerateTextSegments()
    {
        var cumulativeText = "";
        foreach (var segment in m_Segments)
        {
            cumulativeText += segment.FormatPrefix;
            foreach (var t in segment.Character)
            {
                cumulativeText += t;
                yield return cumulativeText + segment.FormatSuffix;
            }

            cumulativeText += segment.FormatSuffix;
        }
    }
}