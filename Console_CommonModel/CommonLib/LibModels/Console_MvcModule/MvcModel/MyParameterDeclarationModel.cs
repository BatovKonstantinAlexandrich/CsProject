using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// Модель параметра вызова метода или процедуры или функции.
/// ПО этой модели приложение-клиент создаёт поле для ввода информации
/// на форме выполнения операции.
/// </summary>
public class MyParameterDeclarationModel: HierTable<MyActionModel>
{
    public override string Name { get; set; }
    public string Type { get; set; }
    public bool IsOptional { get; set; }
    public int Position { get; set; }
    public string DefValue { get; set; }

    [NotMapped]
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
}
