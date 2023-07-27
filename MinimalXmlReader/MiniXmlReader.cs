using System.Diagnostics;

namespace MinimalXmlReader;

public ref struct MiniXmlReader
{
    private readonly ReadOnlySpan<char> xml;
    private int position;

    public MiniXmlReader(ReadOnlySpan<char> xml)
    {
        this.xml = xml;
    }

    public void SkipProcessingInstruction()
    {
        SkipSpaces();

        _ = SkipChar('<');
        _ = SkipChar('?');
        SkipUntilChar(' ');

        do
        {
            while (xml[position] != '?') position++;
            position++;
        }
        while (!SkipChar('>'));
    }

    private bool SkipChar(char c)
    {
        var skipped = true;

        Debug.WriteLine($"SkipChar({c}) actual: {xml[position]}");

        if (xml[position] != c) skipped = false;
        position++;
        return skipped;
    }

    private void SkipUntilChar(char c)
    {
        do
        {
            var ch = xml[position];
            if (ch == c) return;
            if (ch == '\r' || ch == '\n') throw new Exception();
            position++;
        }
        while (true);
    }

    private bool SkipSpaces()
    {
        var skipped = false;

        do
        {
            var c = xml[position];
            if (c != ' ' && c != '\t' && c != '\r' && c != '\n') return skipped;
            position++;
            skipped = true;
        }
        while (true);
    }

    public bool SkipStartElement(ReadOnlySpan<char> name)
    {
        SkipSpaces();

        var safePosition = position;

        _ = SkipChar('<');

        for (var i = 0; i < name.Length; i++)
        {
            var ch = xml[position];

            if (ch != name[i])
            {
                position = safePosition;
                return false;
            }

            position++;
        }

        SkipSpaces();

        if (SkipChar('>'))
        {
            return true;
        }

        throw new Exception();

        // No attributes expected yet
    }

    public ReadOnlySpan<char> ReadStartElement()
    {
        SkipSpaces();

        _ = SkipChar('<');

        var start = position;

        while (xml[position] != ' ' && xml[position] != '>') position++;

        var name = xml[start..position];

        SkipSpaces();

        position++;

        return name;
    }

    public void SkipEndElement(ReadOnlySpan<char> name)
    {
        SkipSpaces();

        _ = SkipChar('<');
        _ = SkipChar('/');

        for (var i = 0; i < name.Length; i++)
        {
            var ch = xml[position];
            if (ch != name[i]) throw new Exception();
            position++;
        }

        SkipSpaces();

        if (!SkipChar('>'))
        {
            throw new Exception();
        }
    }

    public void SkipEndElement()
    {
        SkipSpaces();

        _ = SkipChar('<');
        _ = SkipChar('/');
        SkipUntilChar('>');

        position++;
    }

    public ReadOnlySpan<char> ReadContent()
    {
        SkipSpaces();

        var start = position;

        while (xml[position] != '<') position++;

        return xml[start..position];
    }
}
