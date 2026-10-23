using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{

    [Label("Инициализация моделей классов")]
    public class ClassDbContextInitializer : DbContextInitiliazer<ClassDbContext>
    {
        public static ChangingList<Type> ClassTypes = new ChangingList<Type>();
        private static int CreatedClassTypes = CreateClassTypes();

        public ClassDbContextInitializer()
        {
            //ClassTypes.OnChanged += OnClassTypesChanged;
        }

        private void OnClassTypesChanged(object sender, IChangingListEvent evt)
        {
            var type = (Type)evt.Target.Item;
            switch (evt.Target.Action)
            {
                case "added":
                    using (var db = new ClassDbContext())
                    {
                        db.CreateClassModel(type);
                    }
                    break;
                case "removed":
                    using (var db = new ClassDbContext())
                    {
                        db.RemoveClassModel(type);
                    }
                    break;
            }
        }

        private static int CreateClassTypes()
        {
            ClassTypes.AddRange(typeof(ClassDbContextInitializer).Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsPublic && t.GetTypeName().IsEng()));
            return ClassTypes.Count();
        }

        public override Dictionary<string, int> Init(ClassDbContext context)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            results[nameof(ClassModel)] = 0;
            results[nameof(AttributeBase<ClassModel>)] = 0;
            results[nameof(PropertyModel)] = 0;
            results[nameof(AttributeBase<PropertyModel>)] = 0;
            results[nameof(MethodModel)] = 0;
            results[nameof(AttributeBase<MethodModel>)] = 0;
            results[nameof(MethodParameterModel)] = 0;
            results[nameof(AttributeBase<MethodParameterModel>)] = 0;
            var typeHelper = new TypeHelperStrict();
            foreach (var type in ClassTypes)
            {
                var classId = context.Create(new ClassModel()
                {
                    Name = type.GetTypeName()
                });
                results[nameof(ClassModel)]++;

                var attributes = typeHelper.GetAttributes(type);
                foreach (var attr in attributes.Select(attr => new AttributeBase<ClassModel>()
                {
                    ParentId = classId,
                    Name = attr.Key,
                    Value = attr.Value
                }))
                {
                    attr.Validate();
                    context.Create(attr);
                    results[nameof(AttributeBase<ClassModel>)]++;
                }

                foreach (var prop in type.GetOwnPropertyNames().Select(prop => new PropertyModel()
                {
                    ParentId = classId,
                    Name = prop,
                    Type = type.GetProperties().FirstOrDefault(p => p.Name == prop).PropertyType.GetTypeName(),
                    Label = prop,
                    Description = prop
                }))
                {
                    var propertyId = context.Create(prop);
                    results[nameof(PropertyModel)]++;
                    var propertyAttributes = typeHelper.GetPropertyAttributes(type, prop.Name);
                    foreach (var attr in propertyAttributes.Select(attr => new AttributeBase<PropertyModel>()
                    {
                        ParentId = propertyId,
                        Name = attr.Key,
                        Value = attr.Value
                    }))
                    {
                        results[nameof(AttributeBase<PropertyModel>)]++;
                        context.Create(attr);
                    }
                }

                foreach (var method in type.GetMethods().Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_") && !m.IsStatic && m.DeclaringType.GetTypeName() == type.GetTypeName()).Select(m => m.Name).Select(method => new MethodModel()
                {
                    ParentId = classId,
                    Name = method,
                    Label = method,
                    Description = method
                }))
                {
                    var methodId = context.Create(method);
                    results[nameof(MethodModel)]++;
                    var methodAttributes = typeHelper.GetMethodAttributes(type, method.Name);
                    foreach (var attr in methodAttributes.Select(attr => new AttributeBase<MethodModel>()
                    {
                        ParentId = methodId,
                        Name = attr.Key,
                        Value = attr.Value
                    }))
                    {
                        results[nameof(AttributeBase<MethodModel>)]++;
                        context.Create(attr);
                    }

                    foreach (var par in type.GetMethods().FirstOrDefault(m => m.Name == method.Name).GetParameters())
                    {
                        var parameterId = context.Create(new MethodParameterModel()
                        {
                            ParentId = methodId,
                            Name = par.Name,
                            Type = par.ParameterType.GetTypeName()
                        });
                        results[nameof(MethodParameterModel)]++;
                        foreach (var attr in typeHelper.GetParameterAttributes(par).Select(attr => new AttributeBase<MethodParameterModel>()
                        {
                            ParentId = parameterId,
                            Name = attr.Key,
                            Value = attr.Value
                        }))
                        {
                            results[nameof(AttributeBase<MethodParameterModel>)]++;
                            context.Create(attr);
                        }
                    }
                }
            }
            return results;
        }
    }
}
