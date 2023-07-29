using System.Globalization;
using System.Xml;

namespace MinimalXmlReader.Tests;

public class MiniXmlReaderTests
{
    [Fact]
    public void IntegrationTest_XmlRpcMethodCall()
    {
        var xml = File.ReadAllText("XmlRpcMethodCall.xml");

        var r = new MiniXmlReader(xml);

        Assert.True(r.SkipProcessingInstruction());
        Assert.True(r.SkipStartElement("methodCall"));
        Assert.True(r.SkipStartElement("methodName"));
        Assert.Equal(expected: "TrackMania.PlayerChat", actual: r.ReadContent().ToString());
        Assert.True(r.SkipEndElement("methodName"));
        Assert.True(r.SkipStartElement("params"));

        var expectedParams = new object[]
        {
            0,
            "bigbang1112_dev",
            "TrackMania.PlayerFinish 237 bigbang1112 0",
            false
        };

        var actualParams = new List<object>();

        while (r.SkipStartElement("param"))
        {
            Assert.True(r.SkipStartElement("value"));

            var typeName = r.ReadStartElement();

            switch (typeName)
            {
                case "i4":
                    actualParams.Add(int.Parse(r.ReadContent()));
                    break;
                case "string":
                    actualParams.Add(r.ReadContent().ToString());
                    break;
                case "boolean":
                    actualParams.Add(r.ReadContentAsBoolean());
                    break;
                default:
                    throw new Exception("Invalid type");
            }

            Assert.True(r.SkipEndElement(typeName));
            Assert.True(r.SkipEndElement("value"));
            Assert.True(r.SkipEndElement("param"));
        }

        Assert.Equal(expectedParams, actualParams);

        Assert.True(r.SkipEndElement("params"));
        Assert.True(r.SkipEndElement("methodCall"));
    }

    [Fact]
    public void IntegrationTest_Random()
    {
        var xml = File.ReadAllText("Random.xml");

        var r = new MiniXmlReader(xml);

        Assert.False(r.SkipProcessingInstruction());
        Assert.True(r.SkipStartElement("root"));
        Assert.True(r.ReadStartElement("person", out var personAtts));
        Assert.Equal(expected: "Tierney", actual: personAtts["firstname"]);
        Assert.Equal(expected: "Shirberg", actual: personAtts["lastname"]);
        Assert.Equal(expected: "Port-au-Prince", actual: personAtts["city"]);
        Assert.Equal(expected: "Sao Tome and Principe", actual: personAtts["country"]);
        Assert.Equal(expected: "Anallese", actual: personAtts["firstname2"]);
        Assert.Equal(expected: "Chick", actual: personAtts["lastname2"]);
        Assert.Equal(expected: "Anallese.Chick@yopmail.com", actual: personAtts["email"]);
        Assert.True(r.SkipStartElement("random"));
        Assert.Equal(expected: 58, actual: int.Parse(r.ReadContent()));
        Assert.True(r.SkipEndElement("random"));
        Assert.True(r.SkipStartElement("random_float"));
        Assert.Equal(expected: 9.873, actual: double.Parse(r.ReadContent(), NumberStyles.Number, CultureInfo.InvariantCulture));
        Assert.True(r.SkipEndElement("random_float"));
        Assert.True(r.SkipStartElement("bool"));
        Assert.True(r.ReadContentAsBoolean());
        Assert.True(r.SkipEndElement("bool"));
        Assert.True(r.SkipStartElement("date"));
        Assert.Equal(expected: "1990-09-10", actual: r.ReadContent().ToString());
        Assert.True(r.SkipEndElement("date"));
        Assert.True(r.SkipStartElement("regEx"));
        Assert.Equal(expected: "helloooooooooooooooooooooooooooooo to you", actual: r.ReadContent().ToString());
        Assert.True(r.SkipEndElement("regEx"));
        Assert.True(r.SkipStartElement("enum"));
        Assert.Equal(expected: "xml", actual: r.ReadContent().ToString());
        Assert.True(r.SkipEndElement("enum"));

        var expectedListElt = new[] { "Lilith", "Correy", "Roslyn", "Magdalena", "Ninnetta" };

        var actualListElt = new List<string>();

        while (r.SkipStartElement("elt"))
        {
            actualListElt.Add(r.ReadContent().ToString());
            Assert.True(r.SkipEndElement("elt"));
        }

        Assert.Equal(expectedListElt, actualListElt);

        Assert.True(r.SkipStartElement("Anallese"));
        Assert.True(r.SkipStartElement("age"));
        Assert.Equal(expected: 34, actual: int.Parse(r.ReadContent()));
        Assert.True(r.SkipEndElement("age"));
        Assert.True(r.SkipEndElement("Anallese"));
    }

    [Fact]
    public void IntegrationTest_XmlRpcMethodResponseExtreme()
    {
        var xml = File.ReadAllText("XmlRpcMethodResponseExtreme.xml");

        var r = new MiniXmlReader(xml);

        r.SkipProcessingInstruction();
        r.SkipStartElement("methodResponse");
        r.SkipStartElement("params");
        r.SkipStartElement("param");
        r.SkipStartElement("value");
        r.SkipStartElement("array");
        r.SkipStartElement("data");

        while (r.SkipStartElement("value"))
        {
            Assert.True(r.SkipStartElement("array"));
            Assert.True(r.SkipStartElement("data"));
            Assert.True(r.SkipStartElement("value"));
            Assert.True(r.SkipStartElement("array"));
            Assert.True(r.SkipStartElement("data"));
            Assert.True(r.SkipStartElement("value"));
            Assert.True(r.SkipStartElement("array"));
            Assert.True(r.SkipStartElement("data"));

            while (r.SkipStartElement("value"))
            {
                r.SkipStartElement("string");

                var str = r.ReadContent().ToString();

                r.SkipEndElement("string");
                r.SkipEndElement("value");
            }

            Assert.True(r.SkipEndElement("data"));
            Assert.True(r.SkipEndElement("array"));
            Assert.True(r.SkipEndElement("value"));
            Assert.True(r.SkipEndElement("data"));
            Assert.True(r.SkipEndElement("array"));
            Assert.True(r.SkipEndElement("value"));
            Assert.True(r.SkipEndElement("data"));
            Assert.True(r.SkipEndElement("array"));
            Assert.True(r.SkipEndElement("value"));
        }
    }
}
