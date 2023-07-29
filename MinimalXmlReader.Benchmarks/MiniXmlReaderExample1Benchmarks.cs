using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Xml;

namespace MinimalXmlReader.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.NativeAot70)]
[MemoryDiagnoser]
public class MiniXmlReaderExample1Benchmarks
{
    private readonly string xml;

    public MiniXmlReaderExample1Benchmarks()
    {
        xml = File.ReadAllText("XmlRpcMethodCall.xml");
    }

    [Benchmark]
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
                default:
                    r.ReadContent();
                    break;
            }

            r.SkipEndElement();
            r.SkipEndElement();
            r.SkipEndElement();
        }

        return methodName.ToString();
    }


    [Benchmark]
    public string MicrosoftXmlReader()
    {
        using var strReader = new StringReader(xml);
        using var r = XmlReader.Create(strReader);

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
            r.ReadStartElement();
            var stuff = r.ReadContentAsString();
            r.ReadEndElement();
            r.ReadEndElement();
            r.ReadEndElement();
        }

        return methodName;
    }
}
