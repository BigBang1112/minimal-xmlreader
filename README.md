# Minimal XmlReader

Minimal XmlReader is a simple, lightweight, and fast XML reader for .NET, maily viable for NativeAOT.

Official XmlReader will use up to 6MB of additional executable binary size. This library tries to reduce this size, while also reduce memory usage and improve performance.

**Beware this is still considered unstable** and on larger XML files this may be still less efficient than the official XmlReader.

## Usage

Currently, only string input is supported. For reading streams, official XmlReader is more viable.

## Benchmarks

### XmlRpcMethodCall.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<methodCall>
<methodName>TrackMania.PlayerChat</methodName>
<params>
<param><value><i4>0</i4></value></param>
<param><value><string>bigbang1112_dev</string></value></param>
<param><value><string>TrackMania.PlayerFinish 237 bigbang1112 0</string></value></param>
<param><value><boolean>0</boolean></value></param>
</params>
</methodCall>
```

```
BenchmarkDotNet v0.13.6, Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]        : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 6.0      : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  NativeAOT 7.0 : .NET 7.0.8-servicing.23318.7, X64 NativeAOT AVX2


|             Method |           Job |       Runtime |     Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------------- |-------------- |-------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|      MiniXmlReader |      .NET 6.0 |      .NET 6.0 | 1.265 us | 0.0091 us | 0.0085 us | 0.0267 |      - |     224 B |
| MicrosoftXmlReader |      .NET 6.0 |      .NET 6.0 | 3.226 us | 0.0556 us | 0.0493 us | 1.4496 | 0.0572 |   12144 B |
|      MiniXmlReader |      .NET 7.0 |      .NET 7.0 | 1.148 us | 0.0058 us | 0.0051 us | 0.0267 |      - |     224 B |
| MicrosoftXmlReader |      .NET 7.0 |      .NET 7.0 | 2.936 us | 0.0587 us | 0.0721 us | 1.4534 | 0.0610 |   12144 B |
|      MiniXmlReader | NativeAOT 7.0 | NativeAOT 7.0 | 1.013 us | 0.0100 us | 0.0094 us | 0.0267 |      - |     224 B |
| MicrosoftXmlReader | NativeAOT 7.0 | NativeAOT 7.0 | 3.285 us | 0.0346 us | 0.0307 us | 1.4496 | 0.0572 |   12144 B |
```

### Random.xml

In this example, MiniXmlReader creates attribute dictionary, while XmlReader ignores the attributes.

```
<root>
	<person
	firstname="Tierney"
	lastname="Shirberg"
	city="Port-au-Prince"
	country="Sao Tome and Principe"
	firstname2="Anallese"
	lastname2="Chick"
	email="Anallese.Chick@yopmail.com"
  />
	<random>58</random>
	<random_float>9.873</random_float>
	<bool>true</bool>
	<date>1990-09-10</date>
	<regEx>helloooooooooooooooooooooooooooooo to you</regEx>
	<enum>xml</enum>
	<elt>Lilith</elt>
	<elt>Correy</elt>
	<elt>Roslyn</elt>
	<elt>Magdalena</elt>
	<elt>Ninnetta</elt>
	<Anallese>
		<age>34</age>
	</Anallese>
</root>
```

```
BenchmarkDotNet v0.13.6, Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]        : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 6.0      : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  NativeAOT 7.0 : .NET 7.0.8-servicing.23318.7, X64 NativeAOT AVX2


|             Method |           Job |       Runtime |     Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------------- |-------------- |-------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|      MiniXmlReader |      .NET 6.0 |      .NET 6.0 | 2.598 us | 0.0147 us | 0.0137 us | 0.1755 |      - |   1.44 KB |
| MicrosoftXmlReader |      .NET 6.0 |      .NET 6.0 | 3.953 us | 0.0767 us | 0.0821 us | 1.5869 |      - |  12.97 KB |
|      MiniXmlReader |      .NET 7.0 |      .NET 7.0 | 2.247 us | 0.0273 us | 0.0213 us | 0.1755 |      - |   1.44 KB |
| MicrosoftXmlReader |      .NET 7.0 |      .NET 7.0 | 3.711 us | 0.0703 us | 0.0722 us | 1.5869 | 0.0725 |  12.97 KB |
|      MiniXmlReader | NativeAOT 7.0 | NativeAOT 7.0 | 2.142 us | 0.0176 us | 0.0164 us | 0.1755 |      - |   1.44 KB |
| MicrosoftXmlReader | NativeAOT 7.0 | NativeAOT 7.0 | 4.019 us | 0.0558 us | 0.0726 us | 1.5869 | 0.0687 |  12.97 KB |
```
