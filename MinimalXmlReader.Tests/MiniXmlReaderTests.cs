namespace MinimalXmlReader.Tests;

public class MiniXmlReaderTests
{
    [Fact]
    public void Test()
    {
        var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<methodCall>
<methodName>TrackMania.PlayerChat</methodName>
<params>
<param><value><i4>0</i4></value></param>
<param><value><string>bigbang1112_dev</string></value></param>
<param><value><string>TrackMania.PlayerFinish 237 bigbang1112 0</string></value></param>
<param><value><boolean>0</boolean></value></param>
</params>
</methodCall>";

        var reader = new MiniXmlReader(xml);

        reader.SkipProcessingInstruction();
        reader.SkipStartElement("methodCall");
        reader.SkipStartElement("methodName");
        var methodName = reader.ReadContent();
        reader.SkipEndElement();
        reader.SkipStartElement("params");

        while (reader.SkipStartElement("param"))
        {
            reader.SkipStartElement("value");

            switch (reader.ReadStartElement())
            {
                case "i4":
                    var i4 = reader.ReadContent();
                    break;
                default:
                    var bla = reader.ReadContent();
                    break;
            }

            reader.SkipEndElement();
            reader.SkipEndElement();
            reader.SkipEndElement();
        }
    }
}
