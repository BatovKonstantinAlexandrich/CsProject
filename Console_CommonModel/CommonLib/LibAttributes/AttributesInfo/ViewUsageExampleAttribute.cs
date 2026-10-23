using System;

/// <summary>
/// Пример использования класса представления (C# и/или XML-разметка).
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ViewUsageExampleAttribute : Attribute
{
    public string CSharp { get; }
    public string Xml { get; }

    public ViewUsageExampleAttribute(string csharp, string xml = null)
    {
        CSharp = csharp ?? "";
        Xml = xml ?? "";
    }
}
