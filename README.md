# Minimal XmlReader

[![Nuget](https://img.shields.io/nuget/v/MinimalXmlReader?style=for-the-badge)](https://www.nuget.org/packages/MinimalXmlReader/)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/BigBang1112/minimal-xmlreader?include_prereleases&style=for-the-badge)](https://github.com/BigBang1112/minimal-xmlreader/releases)

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
BenchmarkDotNet v0.13.6, Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]        : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 6.0      : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  NativeAOT 7.0 : .NET 7.0.8-servicing.23318.7, X64 NativeAOT AVX2


|        Method |       Runtime |       Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|-------------- |-------------- |-----------:|---------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| MiniXmlReader |      .NET 6.0 | 1,292.3 ns | 10.28 ns |  9.11 ns |  1.00 |    0.00 | 0.0267 |      - |     224 B |        1.00 |
|     XmlReader |      .NET 6.0 | 3,118.2 ns | 37.74 ns | 31.51 ns |  2.41 |    0.03 | 1.4496 | 0.0572 |   12144 B |       54.21 |
|               |               |            |          |          |       |         |        |        |           |             |
| MiniXmlReader |      .NET 7.0 | 1,168.6 ns |  9.61 ns |  8.99 ns |  1.00 |    0.00 | 0.0267 |      - |     224 B |        1.00 |
|     XmlReader |      .NET 7.0 | 2,986.5 ns | 58.65 ns | 76.26 ns |  2.56 |    0.07 | 1.4534 | 0.0610 |   12144 B |       54.21 |
|               |               |            |          |          |       |         |        |        |           |             |
| MiniXmlReader | NativeAOT 7.0 |   991.7 ns |  8.01 ns |  7.10 ns |  1.00 |    0.00 | 0.0267 |      - |     224 B |        1.00 |
|     XmlReader | NativeAOT 7.0 | 3,329.7 ns | 65.77 ns | 80.77 ns |  3.37 |    0.08 | 1.4496 | 0.0572 |   12144 B |       54.21 |
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
BenchmarkDotNet v0.13.6, Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]        : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 6.0      : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  NativeAOT 7.0 : .NET 7.0.8-servicing.23318.7, X64 NativeAOT AVX2


|        Method |       Runtime |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen0 |   Gen1 | Allocated | Alloc Ratio |
|-------------- |-------------- |---------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| MiniXmlReader |      .NET 6.0 | 2.491 us | 0.0198 us | 0.0185 us |  1.00 |    0.00 | 0.1755 |      - |   1.44 KB |        1.00 |
|     XmlReader |      .NET 6.0 | 4.121 us | 0.0658 us | 0.0615 us |  1.65 |    0.03 | 1.5869 |      - |  12.97 KB |        9.02 |
|               |               |          |           |           |       |         |        |        |           |             |
| MiniXmlReader |      .NET 7.0 | 2.229 us | 0.0114 us | 0.0101 us |  1.00 |    0.00 | 0.1755 |      - |   1.44 KB |        1.00 |
|     XmlReader |      .NET 7.0 | 3.644 us | 0.0717 us | 0.0826 us |  1.64 |    0.04 | 1.5869 | 0.0725 |  12.97 KB |        9.02 |
|               |               |          |           |           |       |         |        |        |           |             |
| MiniXmlReader | NativeAOT 7.0 | 2.075 us | 0.0151 us | 0.0126 us |  1.00 |    0.00 | 0.1755 |      - |   1.44 KB |        1.00 |
|     XmlReader | NativeAOT 7.0 | 4.306 us | 0.0288 us | 0.0240 us |  2.08 |    0.01 | 1.5869 | 0.0687 |  12.97 KB |        9.02 |
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
BenchmarkDotNet v0.13.6, Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 3700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100-preview.6.23330.14
  [Host]        : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 6.0      : .NET 6.0.20 (6.0.2023.32017), X64 RyuJIT AVX2
  .NET 7.0      : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  NativeAOT 7.0 : .NET 7.0.8-servicing.23318.7, X64 NativeAOT AVX2


|        Method |       Runtime |     Mean |   Error |  StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|-------------- |-------------- |---------:|--------:|--------:|------:|--------:|-------:|----------:|------------:|
| MiniXmlReader |      .NET 6.0 | 181.6 us | 2.43 us | 2.15 us |  1.00 |    0.00 | 1.9531 |  16.57 KB |        1.00 |
|     XmlReader |      .NET 6.0 | 285.2 us | 1.82 us | 1.61 us |  1.57 |    0.02 | 3.4180 |     30 KB |        1.81 |
|               |               |          |         |         |       |         |        |           |             |
| MiniXmlReader |      .NET 7.0 | 155.6 us | 1.27 us | 1.06 us |  1.00 |    0.00 | 1.9531 |  16.57 KB |        1.00 |
|     XmlReader |      .NET 7.0 | 261.5 us | 5.23 us | 7.66 us |  1.68 |    0.06 | 3.4180 |     30 KB |        1.81 |
|               |               |          |         |         |       |         |        |           |             |
| MiniXmlReader | NativeAOT 7.0 | 135.1 us | 0.68 us | 0.57 us |  1.00 |    0.00 | 1.9531 |  16.57 KB |        1.00 |
|     XmlReader | NativeAOT 7.0 | 325.6 us | 2.56 us | 2.40 us |  2.41 |    0.02 | 3.4180 |     30 KB |        1.81 |
```
