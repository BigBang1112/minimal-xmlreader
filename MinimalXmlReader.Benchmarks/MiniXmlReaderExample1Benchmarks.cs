using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Text;
using System.Xml;

namespace MinimalXmlReader.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.NativeAot70)]
[MemoryDiagnoser]
public class MiniXmlReaderExample1Benchmarks
{
    private readonly string xml;
    private readonly MemoryStream ms;
    private readonly XmlReader xmlReader;

    public MiniXmlReaderExample1Benchmarks()
    {
        xml = File.ReadAllText("XmlRpcMethodCall.xml");

        ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        xmlReader = System.Xml.XmlReader.Create(ms, new() { IgnoreWhitespace = true });
    }

    [Benchmark(Baseline = true)]
    public string MiniXmlReader()
    {
        var r = new MiniXmlReader(xml);

        r.SkipProcessingInstruction();
        r.SkipStartElement("methodCall");
        r.SkipStartElement("methodName");
        var methodName = r.ReadContent().ToString();
        r.SkipEndElement();
        r.SkipStartElement("params");

        while (r.SkipStartElement("param"))
        {
            r.SkipStartElement("value");

            switch (r.ReadStartElement())
            {
                case "i4":
                    int.Parse(r.ReadContent());
                    break;
                case "string":
                    r.ReadContent().ToString();
                    break;
                case "boolean":
                    r.ReadContentAsBoolean();
                    break;
                default:
                    throw new Exception("Invalid type");
            }

            r.SkipEndElement();
            r.SkipEndElement("value");
            r.SkipEndElement("param");
        }

        return methodName.ToString();
    }

    [Benchmark]
    public string XmlReader()
    {
        using var strR = new StringReader(xml);
        using var r = System.Xml.XmlReader.Create(strR, new() { IgnoreWhitespace = true });

        r.MoveToContent();
        r.ReadStartElement("methodCall");
        r.ReadStartElement("methodName");
        var methodName = r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadStartElement("params");

        while (r.Name == "param")
        {
            if (!r.Read())
            {
                return methodName;
            }

            r.ReadStartElement("value");

            switch (r.Name)
            {
                case "i4":
                    r.ReadElementContentAsInt();
                    break;
                case "string":
                    r.ReadElementContentAsString();
                    break;
                case "boolean":
                    r.ReadElementContentAsBoolean();
                    break;
                default:
                    throw new Exception("Invalid type");
            }

            r.ReadEndElement();
            r.ReadEndElement();
        }

        return methodName;
    }
}
