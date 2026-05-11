using System.Buffers;
using System.Diagnostics;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Оптимизация и рефакторинг C# / .NET 10");
Console.WriteLine("Измеряем до и после, без внешних NuGet-пакетов.");
Console.WriteLine();

Benchmark("String + в цикле", () => StringConcatWithPlus(Enumerable.Range(1, 1_000).Select(i => $"word{i}")));
Benchmark("StringBuilder", () => StringConcatWithBuilder(Enumerable.Range(1, 1_000).Select(i => $"word{i}")));
Benchmark("string.Join", () => string.Join(',', Enumerable.Range(1, 1_000).Select(i => $"word{i}")));

Console.WriteLine();
var lookupList = Enumerable.Range(0, 10_000).ToList();
var lookupSet = lookupList.ToHashSet();
Benchmark("List.Contains", () => lookupList.Contains(9_999));
Benchmark("HashSet.Contains", () => lookupSet.Contains(9_999));

Console.WriteLine();
var ipText = "192.168.1.42";
Console.WriteLine($"IPv4 parsed: {TryParseIpV4(ipText, out var ip)} -> {ip}");
Console.WriteLine($"Hex via stackalloc: {ToHexString([0xDE, 0xAD, 0xBE, 0xEF])}");
var csv = "id,name\n1,Alice\n2,Bob\n";
Console.WriteLine($"CSV rows parsed: {ParseCsvNoSplit(csv).Count}");
Console.WriteLine();

await ProcessStreamWithArrayPoolAsync(new MemoryStream(Encoding.UTF8.GetBytes("demo stream payload")));
Console.WriteLine("ArrayPool demo completed.");
Console.WriteLine();
Console.WriteLine("Нажмите любую клавишу для выхода...");
if (!Console.IsInputRedirected)
{
    Console.ReadKey(intercept: true);
}

static void Benchmark(string name, Func<object> action, int iterations = 2_000)
{
    action();
    var before = GC.GetAllocatedBytesForCurrentThread();
    var sw = Stopwatch.StartNew();
    object? last = null;

    for (var i = 0; i < iterations; i++)
    {
        last = action();
    }

    sw.Stop();
    var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
    Console.WriteLine($"{name,-22} {sw.Elapsed.TotalMilliseconds,8:0.00} ms | alloc {allocated / 1024d,8:0.0} KB | sample {FormatSample(last)}");
}

static string StringConcatWithPlus(IEnumerable<string> words)
{
    var result = string.Empty;
    foreach (var word in words)
    {
        result += word;
        result += ',';
    }

    return result;
}

static string StringConcatWithBuilder(IEnumerable<string> words)
{
    var builder = new StringBuilder();
    foreach (var word in words)
    {
        builder.Append(word).Append(',');
    }

    return builder.ToString();
}

static string FormatSample(object? value)
{
    return value is string text && text.Length > 80
        ? $"string(length={text.Length})"
        : value?.ToString() ?? "<null>";
}

static bool TryParseIpV4(ReadOnlySpan<char> input, out uint result)
{
    result = 0;
    var remaining = input;

    for (var octet = 0; octet < 4; octet++)
    {
        var dotIndex = octet < 3 ? remaining.IndexOf('.') : remaining.Length;
        if (dotIndex < 0)
        {
            return false;
        }

        if (!byte.TryParse(remaining[..dotIndex], out var byteValue))
        {
            return false;
        }

        result = (result << 8) | byteValue;
        remaining = octet < 3 ? remaining[(dotIndex + 1)..] : [];
    }

    return remaining.IsEmpty;
}

static string ToHexString(ReadOnlySpan<byte> bytes)
{
    Span<char> chars = stackalloc char[bytes.Length * 2];
    for (var i = 0; i < bytes.Length; i++)
    {
        var b = bytes[i];
        chars[i * 2] = ToHexChar(b >> 4);
        chars[i * 2 + 1] = ToHexChar(b & 0xF);
    }

    return new string(chars);

    static char ToHexChar(int value)
    {
        return (char)(value < 10 ? '0' + value : 'a' + value - 10);
    }
}

static List<string[]> ParseCsvNoSplit(ReadOnlySpan<char> content)
{
    var result = new List<string[]>();

    while (!content.IsEmpty)
    {
        var lineEnd = content.IndexOf('\n');
        var line = lineEnd >= 0 ? content[..lineEnd] : content;
        if (!line.IsEmpty)
        {
            result.Add(ParseFields(line));
        }

        content = lineEnd >= 0 ? content[(lineEnd + 1)..] : [];
    }

    return result;
}

static string[] ParseFields(ReadOnlySpan<char> line)
{
    var fields = new List<string>();
    while (!line.IsEmpty)
    {
        var comma = line.IndexOf(',');
        var field = comma >= 0 ? line[..comma] : line;
        fields.Add(field.ToString());
        line = comma >= 0 ? line[(comma + 1)..] : [];
    }

    return fields.ToArray();
}

static async Task ProcessStreamWithArrayPoolAsync(Stream stream)
{
    var buffer = ArrayPool<byte>.Shared.Rent(4096);
    try
    {
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, 4096));
        var payload = Encoding.UTF8.GetString(buffer.AsSpan(0, bytesRead));
        Console.WriteLine($"Read {bytesRead} bytes: {payload}");
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
    }
}
