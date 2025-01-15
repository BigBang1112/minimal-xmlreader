using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Globalization;

namespace MinimalXmlReader.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.NativeAot80)]
[MemoryDiagnoser]
public class MiniXmlReaderExample2Benchmarks
{
    private readonly string xml;

    public MiniXmlReaderExample2Benchmarks()
    {
        xml = File.ReadAllText("Random.xml");
    }

    [Benchmark(Baseline = true)]
    public void MiniXmlReader()
    {
        var r = new MiniXmlReader(xml);

        r.SkipProcessingInstruction();
        r.SkipStartElement("root");
        r.TryReadStartElement("person");
        r.SkipStartElement("random");
        int.Parse(r.ReadContent());
        r.SkipEndElement("random");
        r.SkipStartElement("random_float");
        double.Parse(r.ReadContent(), NumberStyles.Number, CultureInfo.InvariantCulture);
        r.SkipEndElement("random_float");
        r.SkipStartElement("bool");
        r.ReadContentAsBoolean();
        r.SkipEndElement("bool");
        r.SkipStartElement("date");
        r.ReadContent().ToString();
        r.SkipEndElement("date");
        r.SkipStartElement("regEx");
        r.ReadContent().ToString();
        r.SkipEndElement("regEx");
        r.SkipStartElement("enum");
        r.ReadContent().ToString();
        r.SkipEndElement("enum");

        while (r.SkipStartElement("elt"))
        {
            r.ReadContent().ToString();
            r.SkipEndElement("elt");
        }

        r.SkipStartElement("Anallese");
        r.SkipStartElement("age");
        r.ReadContent();
        r.SkipEndElement("age");
        r.SkipEndElement("Anallese");
    }

    [Benchmark]
    public void XmlReader()
    {
        using var strR = new StringReader(xml);
        using var r = System.Xml.XmlReader.Create(strR, new() { IgnoreWhitespace = true });

        r.MoveToContent();
        r.ReadStartElement("root");
        r.ReadStartElement("person");
        r.GetAttribute("firstname");
        r.GetAttribute("lastname");
        r.GetAttribute("city");
        r.GetAttribute("country");
        r.GetAttribute("firstname2");
        r.GetAttribute("lastname2");
        r.GetAttribute("email");
        r.ReadStartElement("random");
        r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadStartElement("random_float");
        r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadStartElement("bool");
        r.ReadContentAsBoolean();
        r.ReadEndElement();
        r.ReadStartElement("date");
        r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadStartElement("regEx");
        r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadStartElement("enum");
        r.ReadContentAsString();
        r.ReadEndElement();

        while (r.Name == "elt")
        {
            if (!r.Read())
            {
                return;
            }

            r.ReadContentAsString();

            r.ReadEndElement();
        }

        r.ReadStartElement("Anallese");
        r.ReadStartElement("age");
        r.ReadContentAsString();
        r.ReadEndElement();
        r.ReadEndElement();
    }
}
