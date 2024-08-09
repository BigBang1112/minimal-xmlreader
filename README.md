# Minimal XmlReader

[![Nuget](https://img.shields.io/nuget/v/MinimalXmlReader?style=for-the-badge)](https://www.nuget.org/packages/MinimalXmlReader/)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/BigBang1112/minimal-xmlreader?include_prereleases&style=for-the-badge)](https://github.com/BigBang1112/minimal-xmlreader/releases)
[![Code Coverage](https://img.shields.io/badge/Code%20Coverage-72%25-yellow?style=for-the-badge)](https://github.com/BigBang1112/minimal-xmlreader)

Minimal XmlReader is a simple, lightweight, and fast XML reader for .NET, mainly viable for NativeAOT.

**Official XmlReader uses at least up to 6MB of additional self-contained executable binary size!** This library tries to reduce this size, while also reducing memory usage and improving performance, in case you don't want to do any extra fancy XML reading or validating.

**Beware this is still considered unstable** and on larger XML files this may be still less efficient than the official XmlReader.

## Usage

Currently, only string input is supported. For reading streams, official XmlReader is more viable.

## Benchmarks

These benchmarks compare how fast each reader is able to read XML will all the data inside.

### [XmlRpcMethodCall.xml](Examples/XmlRpcMethodCall.xml)

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

#### Results

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.303
  [Host]        : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 8.0      : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.7, X64 NativeAOT AVX2


| Method        | Job           | Runtime       | Mean       | Error    | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |-------------- |-------------- |-----------:|---------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| MiniXmlReader | .NET 8.0      | .NET 8.0      |   759.6 ns |  8.87 ns |   7.86 ns |  1.00 |    0.01 | 0.0267 |      - |     224 B |        1.00 |
| XmlReader     | .NET 8.0      | .NET 8.0      | 2,479.4 ns | 49.21 ns | 127.02 ns |  3.26 |    0.17 | 1.4420 | 0.0572 |   12071 B |       53.89 |
|               |               |               |            |          |           |       |         |        |        |           |             |
| MiniXmlReader | NativeAOT 8.0 | NativeAOT 8.0 | 1,174.3 ns | 10.59 ns |   9.39 ns |  1.00 |    0.01 | 0.0267 |      - |     224 B |        1.00 |
| XmlReader     | NativeAOT 8.0 | NativeAOT 8.0 | 3,461.0 ns | 66.06 ns |  81.12 ns |  2.95 |    0.07 | 1.4420 | 0.0572 |   12071 B |       53.89 |
```

### [Random.xml](Examples/Random.xml)

```xml
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

#### Results

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.303
  [Host]        : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 8.0      : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.7, X64 NativeAOT AVX2


| Method        | Job           | Runtime       | Mean     | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |-------------- |-------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| MiniXmlReader | .NET 8.0      | .NET 8.0      | 1.685 us | 0.0153 us | 0.0136 us |  1.00 |    0.01 | 0.1755 |      - |   1.44 KB |        1.00 |
| XmlReader     | .NET 8.0      | .NET 8.0      | 3.612 us | 0.0718 us | 0.1515 us |  2.14 |    0.09 | 1.5869 | 0.0725 |  12.97 KB |        9.02 |
|               |               |               |          |           |           |       |         |        |        |           |             |
| MiniXmlReader | NativeAOT 8.0 | NativeAOT 8.0 | 2.369 us | 0.0195 us | 0.0173 us |  1.00 |    0.01 | 0.1755 |      - |   1.44 KB |        1.00 |
| XmlReader     | NativeAOT 8.0 | NativeAOT 8.0 | 4.663 us | 0.0900 us | 0.0842 us |  1.97 |    0.04 | 1.5793 | 0.0687 |   12.9 KB |        8.97 |
```

### [XmlRpcMethodResponseExtreme.xml](Examples/XmlRpcMethodResponseExtreme.xml)

```xml
<?xml version="1.0" encoding="utf-8"?>
<methodResponse>
<params>
<param><value><array><data>
<value><array><data>
<value><array><data>
<value><array><data>
<value><string>array</string></value>
</data></array></value>
</data></array></value>
</data></array></value>
<value><array><data>
<value><array><data>
<value><array><data>
<value><string>array</string></value>
... (1800+ lines more)
```

#### Results

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.303
  [Host]        : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 8.0      : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.7, X64 NativeAOT AVX2


| Method        | Job           | Runtime       | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------- |-------------- |-------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
| MiniXmlReader | .NET 8.0      | .NET 8.0      | 113.2 us | 1.21 us | 1.13 us |  1.00 |    0.01 | 1.9531 |  16.57 KB |        1.00 |
| XmlReader     | .NET 8.0      | .NET 8.0      | 225.9 us | 2.76 us | 2.58 us |  2.00 |    0.03 | 3.6621 |     30 KB |        1.81 |
|               |               |               |          |         |         |       |         |        |           |             |
| MiniXmlReader | NativeAOT 8.0 | NativeAOT 8.0 | 167.3 us | 1.74 us | 1.62 us |  1.00 |    0.01 | 1.9531 |  16.57 KB |        1.00 |
| XmlReader     | NativeAOT 8.0 | NativeAOT 8.0 | 316.8 us | 2.75 us | 2.44 us |  1.89 |    0.02 | 3.4180 |  29.93 KB |        1.81 |
```
