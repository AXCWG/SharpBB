namespace SharpBB.Server;

public class ConsoleHelper : IDisposable
{
    private ConsoleColor BeforeForeground { get; } = Console.ForegroundColor;
    private ConsoleColor BeforeBackground { get; } = Console.BackgroundColor;

    public void BlueWhite()
    {
        Console.BackgroundColor = ConsoleColor.Blue; 
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    public void Dispose()
    {
        Console.ForegroundColor = BeforeForeground;
        Console.BackgroundColor = BeforeBackground;
    }
}