[Label("Методы расширения исп. базовые программы")]
public static class ProgramExtensions
{
    public static T RunConsoleControl<T>(this T target, ref string[] args)
    {
        ConsoleProgram.RunInteractive<T>(target, ref args);
        return target;
    }
}