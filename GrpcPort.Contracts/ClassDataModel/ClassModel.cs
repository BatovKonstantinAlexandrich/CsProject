
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace WinFormsApp.Console.Console_ClassModule.ClassDataModel
{
    /// <summary>
    /// Модель данных для класса
    /// </summary>
    public class ClassModel : HierTable<ClassModel>
    {

        public override int Id { get; set; }

        public Nullable<int> AssemblyId { get; set; }
        public AssemblyModel Assembly { get; set; }


        public override string Name { get => base.Name; set => base.Name = value; }

        [NotMapped]
        public string Label { get; set; }

        [NotMapped]
        public string Description { get; set; }

        /// <summary>
        /// Базовый тип в генерируемом C# (AbstractionModel / ActionModels), например ActionModels.BaseEntity, DimensionTable, HierTable.
        /// Если пусто и задан ParentId — в коде наследование от родительского ClassModel по имени.
        /// </summary>
        [NotMapped]
        public string ExtendsTypeName { get; set; }

        public List<PropertyModel> Properties { get; set; } = new();

        public List<MethodModel> Methods { get; set; } = new();

        public List<ClassModelAttribute> Attributes { get; set; } = new();
    }
}
