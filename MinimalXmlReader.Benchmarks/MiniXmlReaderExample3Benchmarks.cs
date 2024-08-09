using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Text;
using System.Xml;

namespace MinimalXmlReader.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.NativeAot80)]
[MemoryDiagnoser]
public class MiniXmlReaderExample3Benchmarks
{
    private readonly string xml;

    public MiniXmlReaderExample3Benchmarks()
    {
        xml = File.ReadAllText("XmlRpcMethodResponseExtreme.xml");
    }

    [Benchmark(Baseline = true)]
    public void MiniXmlReader()
    {
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
            r.SkipStartElement("array");
            r.SkipStartElement("data");
            r.SkipStartElement("value");
            r.SkipStartElement("array");
            r.SkipStartElement("data");
            r.SkipStartElement("value");
            r.SkipStartElement("array");
            r.SkipStartElement("data");

            while (r.SkipStartElement("value"))
            {
                r.SkipStartElement("string");

                _ = r.ReadContent().ToString();

                r.SkipEndElement("string");
                r.SkipEndElement("value");
            }

            r.SkipEndElement("data");
            r.SkipEndElement("array");
            r.SkipEndElement("value");
            r.SkipEndElement("data");
            r.SkipEndElement("array");
            r.SkipEndElement("value");
            r.SkipEndElement("data");
            r.SkipEndElement("array");
            r.SkipEndElement("value");
        }
    }

    [Benchmark]
    public void XmlReader()
    {
        using var strR = new StringReader(xml);
        using var r = System.Xml.XmlReader.Create(strR, new() { IgnoreWhitespace = true });

        r.MoveToContent();
        r.ReadStartElement("methodResponse");
        r.ReadStartElement("params");
        r.ReadStartElement("param");
        r.ReadStartElement("value");
        r.ReadStartElement("array");
        r.ReadStartElement("data");

        while (r.Name == "value")
        {
            for (var i = 0; i < 3; i++)
            {
                r.ReadStartElement("value");
                r.ReadStartElement("array");
                r.ReadStartElement("data");
            }

            while (r.Name == "value")
            {
                if (!r.Read())
                {
                    return;
                }

                r.ReadStartElement("string");

                _ = r.ReadContentAsString();

                r.ReadEndElement();
                r.ReadEndElement();
            }

            for (var i = 0; i < 9; i++)
            {
                r.ReadEndElement();
            }
        }
    }
}
