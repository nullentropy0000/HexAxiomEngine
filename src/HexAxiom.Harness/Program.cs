namespace HexAxiom.Harness;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("HexAxiom.Harness");
        Console.WriteLine($"args: {(args.Length == 0 ? "(none)" : string.Join(' ', args))}");
        return 0;
    }
}
