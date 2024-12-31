using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MinimalXmlReader;

/// <summary>
/// A minimal XML reader.
/// </summary>
public ref struct MiniXmlReader
{
    private readonly ReadOnlySpan<char> xml;
    private int position;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniXmlReader"/> struct.
    /// </summary>
    /// <param name="xml">The XML to read.</param>
    public MiniXmlReader(ReadOnlySpan<char> xml)
    {
        this.xml = xml;
    }

    /// <summary>
    /// Skips a processing instruction.
    /// </summary>
    /// <returns>True if a processing instruction was skipped; otherwise, false.</returns>
    public bool SkipProcessingInstruction()
    {
        _ = SkipSpaces();

        var safePosition = position;

        if (!SkipChar('<'))
        {
            return false;
        }

        if (!SkipChar('?'))
        {
            position = safePosition;
            return false;
        }

        SkipAttributes(expectsProcessingInstruction: true);

        return true;
    }

    /// <summary>
    /// Skips a start element.
    /// </summary>
    /// <param name="name">The name of the element to skip.</param>
    public bool SkipStartElement(ReadOnlySpan<char> name)
    {
        _ = SkipSpaces();

        var safePosition = position;

        if (!SkipChar('<'))
        {
            throw new Exception("Not a start element");
        }

        if (!ValidateElementName(name))
        {
            position = safePosition;
            return false;
        }

        SkipAttributes();

        return true;
    }

    /// <summary>
    /// Skips a start element.
    /// </summary>
    /// <returns>True if a start element was skipped; otherwise, false.</returns>
    /// <exception cref="Exception">Something unexpected has failed.</exception>
    public bool SkipStartElement()
    {
        SkipSpaces();

        if (!SkipChar('<'))
        {
            throw new Exception("Not a start element");
        }

        while (true)
        {
            var c = xml[position];

            if (CharIsSpace(c) || c == '/' || c == '>')
            {
                break;
            }

            Advance();
        }

        SkipAttributes();

        return true;
    }

    /// <summary>
    /// Reads a start element.
    /// </summary>
    /// <param name="attributes">The attributes of the element.</param>
    /// <returns>The name of the element.</returns>
    public ReadOnlySpan<char> ReadStartElement(out Dictionary<string, string> attributes)
    {
        var name = BeginReadStartElement();

        attributes = ReadAttributes();

        return name;
    }

    /// <summary>
    /// Reads a start element.
    /// </summary>
    /// <returns>The name of the element.</returns>
    public ReadOnlySpan<char> ReadStartElement()
    {
        var name = BeginReadStartElement();

        SkipAttributes();

        return name;
    }

    /// <summary>
    /// Reads a start element.
    /// </summary>
    /// <param name="name">The name of the element to read.</param>
    /// <param name="attributes">The attributes of the element.</param>
    /// <returns>True if a start element was read; otherwise, false.</returns>
    /// <exception cref="Exception">Something unexpected has failed.</exception>
    public bool ReadStartElement(ReadOnlySpan<char> name, [NotNullWhen(true)] out Dictionary<string, string>? attributes)
    {
        SkipSpaces();

        var safePosition = position;

        if (!SkipChar('<'))
        {
            throw new Exception("Not a start element");
        }

        if (!ValidateElementName(name))
        {
            position = safePosition;
            attributes = null;
            return false;
        }

        attributes = ReadAttributes();

        return true;
    }

    /// <summary>
    /// Skips an end element.
    /// </summary>
    /// <param name="name">The name of the element to skip.</param>
    /// <returns>True if an end element was skipped; otherwise, false.</returns>
    /// <exception cref="Exception">Something unexpected has failed.</exception>
    public bool SkipEndElement(ReadOnlySpan<char> name)
    {
        _ = SkipSpaces();

        var safePosition = position;

        if (!SkipChar('<') || !SkipChar('/') || !ValidateElementName(name))
        {
            position = safePosition;
            return false;
        }

        _ = SkipSpaces();

        if (!SkipChar('>'))
        {
            throw new Exception("No closing char");
        }

        return true;
    }

    /// <summary>
    /// Skips an end element.
    /// </summary>
    /// <returns>True if an end element was skipped; otherwise, false.</returns>
    /// <exception cref="Exception">Something unexpected has failed.</exception>
    public bool SkipEndElement()
    {
        _ = SkipSpaces();

        if (!SkipChar('<') || !SkipChar('/'))
        {
            return false;
        }

        while (true)
        {
            var c = xml[position];

            if (c == '>')
            {
                AdvanceSafe();
                return true;
            }

            if (CharIsSpace(c))
            {
                break;
            }

            Advance();
        }

        _ = SkipSpaces();

        if (!SkipChar('>'))
        {
            throw new Exception("No closing char");
        }

        AdvanceSafe();

        return true;
    }

    /// <summary>
    /// Reads content of an element.
    /// </summary>
    /// <returns>The content of the element.</returns>
    public ReadOnlySpan<char> ReadContent()
    {
        SkipSpaces();

        var start = position;

        while (xml[position] != '<') Advance();

        return xml[start..position];
    }

    /// <summary>
    /// Reads content of an element as a string.
    /// </summary>
    /// <returns>The content of the element as a string.</returns>
    public string ReadContentAsString()
    {
        return ReadContent().ToString();
    }

    /// <summary>
    /// Reads content of an element as a boolean.
    /// </summary>
    /// <returns>The content of the element as a boolean.</returns>
    public bool ReadContentAsBoolean()
    {
        var content = ReadContent();

        return content switch
        {
            "0" => false,
            "1" => true,
            _ => bool.Parse(content),
        };
    }

    private bool ValidateElementName(ReadOnlySpan<char> name)
    {
        for (var i = 0; i < name.Length; i++)
        {
            var ch = xml[position];

            if (ch != name[i])
            {
                return false;
            }

            Advance();
        }

        var c = xml[position];

        if (!CharIsSpace(c) && c != '/' && c != '>')
        {
            return false;
        }

        return true;
    }

    private Dictionary<string, string> ReadAttributes(bool expectsProcessingInstruction = false)
    {
        SkipSpaces();

        var attributes = new Dictionary<string, string>();

        while (!SkipEnd(expectsProcessingInstruction, out var _))
        {
            var attName = ReadUntilChar('=');

            Advance();

            var isApostrophe = false;

            if (!SkipChar('"'))
            {
                if (SkipChar('\''))
                {
                    isApostrophe = true;
                }
                else
                {
                    throw new Exception("Expected \" or '");
                }
            }

            var attValue = ReadUntilChar(isApostrophe ? '\'' : '"', includeSpaces: true);

            Advance();

            attributes.Add(attName.ToString(), attValue.ToString());

            SkipSpaces();
        }

        return attributes;
    }

    private ReadOnlySpan<char> BeginReadStartElement()
    {
        SkipSpaces();

        if (!SkipChar('<'))
        {
            throw new Exception("Not a start element");
        }

        var start = position;

        while (true)
        {
            var c = xml[position];

            if (CharIsSpace(c) || c == '/' || c == '>')
            {
                break;
            }

            Advance();
        }

        var name = xml[start..position];

        Debug.WriteLine($"BeginReadStartElement({name})");
        return name;
    }

    private bool SkipChar(char c)
    {
        Debug.WriteLine($"SkipChar({c}) actual: {xml[position]}");

        if (xml[position] != c)
        {
            return false;
        }

        AdvanceSafe();
        return true;
    }

    private void SkipUntilChar(char c, bool includeSpaces = false)
    {
        do
        {
            var ch = xml[position];
            if (ch == c) return;

            if (!includeSpaces && CharIsSpace(c))
            {
                throw new Exception("Unexpected space");
            }

            position++;
        }
        while (true);
    }

    private bool SkipSpaces()
    {
        var skipped = false;

        while (CharIsSpace(xml[position]))
        {
            position++;
            skipped = true;
        }

        return skipped;
    }

    private void SkipAttributes(bool expectsProcessingInstruction = false)
    {
        SkipSpaces();

        while (!SkipEnd(expectsProcessingInstruction, out var _))
        {
            SkipUntilChar('=');

            Advance();

            var isApostrophe = false;

            if (!SkipChar('"'))
            {
                if (SkipChar('\''))
                {
                    isApostrophe = true;
                }
                else
                {
                    throw new Exception("Expected \" or '");
                }
            }

            SkipUntilChar(isApostrophe ? '\'' : '"', includeSpaces: true);

            Advance();

            SkipSpaces();
        }
    }

    private ReadOnlySpan<char> ReadUntilChar(char expectedChar, bool includeSpaces = false)
    {
        var start = position;

        while (true)
        {
            var c = xml[position];

            if (c == expectedChar)
            {
                break;
            }

            if (!includeSpaces && CharIsSpace(c))
            {
                throw new Exception("Unexpected space");
            }

            Advance();
        }

        return xml[start..position];
    }

    private void Advance()
    {
        if (position == xml.Length - 1)
        {
            throw new Exception("Unexpected end of XML");
        }

        position++;
    }

    private bool AdvanceSafe()
    {
        if (position == xml.Length - 1)
        {
            return false;
        }

        position++;
        return true;
    }

    private bool SkipEnd(bool expectsProcessingInstruction, out EndType endType)
    {
        if (!expectsProcessingInstruction && SkipChar('/'))
        {
            endType = EndType.SelfClosed;
        }
        else if (expectsProcessingInstruction && SkipChar('?'))
        {
            endType = EndType.ProcessingInstruction;
        }
        else
        {
            endType = EndType.None;
        }

        return SkipChar('>');
    }

    private static bool CharIsSpace(char c) => c is ' ' or '\t' or '\r' or '\n';
}
