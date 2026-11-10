namespace HexAxiom.Harness;

internal static class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("HexAxiom.Harness");
        Console.WriteLine($"args: {(args.Length == 0 ? "(none)" : string.Join(' ', args))}");

        ///Ingest command line arguments
        /// 0. Both hex grid and character configs are required
        /// 1. Load a hex grid from a file
        /// 2. Load character configs
        /// 3. Optional: load a command sequence from a file
        /// 
        /// 
        /// Start
        /// 1. Verify that the hex grid is valid
        /// 2. Verify that the character configs are valid
        /// 3. Render the hex grid as ASCII text
        /// 4. Cycle timer starts
        /// 5. Each cycle, each character config is evaluated and the hex grid is updated
        /// 6. If a sequence file was provided, the next command is executed, else the user is prompted for a command
        /// 7. [Future] Stop or win conditions are achieved, and the game ends
        /// 







        return 0;
    }
}
