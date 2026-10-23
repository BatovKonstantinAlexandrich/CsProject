using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using System.Linq;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{

    /// <summary>
    /// Поставщик сведений по типам
    /// </summary>
    public class TypeInfoProvider
    {

        /// <summary>
        /// Рабочи модели классов
        /// </summary>
        private static ConcurrentDictionary<string, ClassModel> classTypes = new();


        /// <summary>
        /// Создавать модели типов при запуске.
        /// Если установлен флаг, то при запуске пересоздаётся
        /// контекст данных ClientClassDbContext и наполняется 
        /// данных исходя из выполняемой сборки, при обращении.
        /// Если флаг не установлен, то атрибуты и модели типов 
        /// загружаются из базы в память.
        /// </summary>
        public static bool CreateClassModels { get; set; } = true;




        /// <summary>
        /// Получение атрибутов класса
        /// </summary>        
        public static Dictionary<string, string> GetAttributes(Type ptype)
        {
            using (var db = new ClassDbContext())
            {


                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                foreach (var attr in classTypes[typeName].Attributes)
                {
                    result[attr.Name] = attr.Value;
                }
                return result;
            }

        }


        public static Dictionary<string, string> AddOrReplacePropertyAttribute(Type ptype, string property, string attributeName, string attributeValue)
        {
            using (var db = new ClassDbContext())
            {
                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                PropertyModel propertyRef = classTypes[typeName].Properties.FirstOrDefault(p => p.Name == property);
                if (propertyRef is null)
                    throw new ArgumentException(nameof(property), "Имя свойства задано неверно");

                var currentAttribute = propertyRef.Attributes.FirstOrDefault(a => a.Name == attributeName);
                if (currentAttribute is not null)
                {
                    currentAttribute.Value = attributeValue;
                    db.Update(currentAttribute);
                    db.SaveChanges();
                }
                else
                {
                    var current = new PropertyModelAttribute() { ParentId = propertyRef.Id, Name = attributeName, Value = attributeValue };
                    var list = propertyRef.Attributes.ToList();
                    list.Add(current);
                    propertyRef.Attributes = list;
                    db.Add(current);
                    db.SaveChanges();
                }
                return new Dictionary<string, string>(
                    propertyRef.Attributes.Select(a => new KeyValuePair<string, string>(a.Name, a.Value))
                );
            }

        }

        public static Dictionary<string, string> RemovePropertyAttribute(Type ptype, string property, string attributeName)
        {
            using (var db = new ClassDbContext())
            {
                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                PropertyModel propertyRef = classTypes[typeName].Properties.FirstOrDefault(p => p.Name == property);
                if (propertyRef is null)
                    throw new ArgumentException(nameof(property), "Имя свойства задано неверно");

                var currentAttribute = propertyRef.Attributes.FirstOrDefault(a => a.Name == attributeName);
                if (currentAttribute is not null)
                {
                    classTypes[typeName].Properties.FirstOrDefault(p => p.Name == property).Attributes.Remove(currentAttribute);
                    db.Remove(currentAttribute);
                    db.SaveChanges();
                }
                return new Dictionary<string, string>(
                    propertyRef.Attributes.Select(a => new KeyValuePair<string, string>(a.Name, a.Value))
                );
            }

        }



        /// <summary>
        /// Добавление, изменение атрибутов класса
        /// </summary>        
        public static Dictionary<string, string> AddOrReplaceAttribute(Type ptype, string attributeName, string attributeValue)
        {
            using (var db = new ClassDbContext())
            {
                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                var attributeModel = classTypes[typeName].Attributes.FirstOrDefault(attr => attr.Name == attributeName);
                if (attributeModel is null)
                {

                }
                classTypes[typeName] = db.GetClassModel(ptype);
                return result;
            }
        }

        /// <summary>
        /// Добавление, изменение атрибутов класса
        /// </summary>        
        public static Dictionary<string, string> RemoveAttribute(Type ptype, string attributeName)
        {
            using (var db = new ClassDbContext())
            {
                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                var attributeModel = classTypes[typeName].Attributes.FirstOrDefault(attr => attr.Name == attributeName);
                if (attributeModel is not null)
                {
                    db.Remove(attributeModel);
                    classTypes[typeName].Attributes.Remove(attributeModel);
                    db.SaveChanges();
                }
                classTypes[typeName] = db.GetClassModel(ptype);
                return result;
            }
        }



        /// <summary>
        /// Получение атрибутов свойства класса
        /// </summary>        
        public static Dictionary<string, string> GetPropertyAttributes(Type ptype, string propertyName)
        {
            var result = new Dictionary<string, string>();

            try
            {
                var typeName = ptype.GetTypeName();
                Console.WriteLine(GetExtendedTypes(ptype).ToJsonOnScreen());

                // Создаём контекст только когда типа ещё нет в кеше
                if (!classTypes.ContainsKey(typeName))
                {
                    using (var db = new ClassDbContext())
                    {
                        if (CreateClassModels)
                        {
                            classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                            var p = classTypes[typeName];
                            foreach (var name in ptype.GetExtendedTypes().Select(t => t.GetTypeName()))
                            {
                                if (name != "Object" && !classTypes.ContainsKey(name))
                                {
                                    try
                                    {
                                        var pExtType = name.ToType();
                                        if (pExtType is null)
                                            throw new Exception($"Не найден тип {name}");
                                        classTypes[name] = db.CreateAndSaveClassModel(pExtType);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Исключение при формировании модели класса {name}=> {ex.Message}");
                                    }
                                }
                                if (name != "Object")
                                {
                                    p.ParentId = classTypes[name].Id;
                                    db.SaveChanges();
                                    p = classTypes[name];
                                }
                            }
                        }
                        else
                        {
                            classTypes[typeName] = db.GetClassModel(ptype);
                        }
                    }
                }

                var propertyModel = FindProperty(propertyName, typeName);
                if (propertyModel is null)
                {
                    //FindProperty(propertyName, typeName);
                    throw new Exception($"Свойство {propertyName} не определено в типе {ptype.GetTypeName()}");
                }

                foreach (var attr in propertyModel.Attributes)
                    result[attr.Name] = attr.Value;
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
                //                return result;
            }
        }

        /// </summary>        
        public static List<string> GetExtendedTypes(Type ptype)
        {
            var result = new List<string>();

            try
            {
                var typeName = ptype.GetTypeName();

                // Создаём контекст только когда типа ещё нет в кеше
                if (!classTypes.ContainsKey(typeName))
                {
                    using (var db = new ClassDbContext())
                    {
                        if (CreateClassModels)
                        {
                            classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                            var p = classTypes[typeName];
                            foreach (var name in ptype.GetExtendedTypes().Select(t => t.GetTypeName()))
                            {
                                if (name != "Object" && !classTypes.ContainsKey(name))
                                {
                                    try
                                    {
                                        var pExtType = name.ToType();
                                        if (pExtType is null)
                                            throw new Exception($"Не найден тип {name}");
                                        classTypes[name] = db.CreateAndSaveClassModel(pExtType);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Исключение при формировании модели класса {name}=> {ex.Message}");
                                    }
                                }
                                if (name != "Object")
                                {
                                    p.ParentId = classTypes[name].Id;
                                    db.SaveChanges();
                                    p = classTypes[name];
                                }
                            }
                        }
                        else
                        {
                            classTypes[typeName] = db.GetClassModel(ptype);
                        }


                    }
                }
                var pp = classTypes[typeName];
                using (var db = new ClassDbContext())
                {
                    while (pp.ParentId is not null)
                    {
                        pp = db.Classes.Find(pp.ParentId);
                        result.Add(pp.Name);
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
                //                return result;
            }
        }


        /// <summary>
        /// Получение атрибутов метода класса
        /// </summary>        
        public static Dictionary<string, string> GetMethodAttributes(Type ptype, string methodName)
        {
            using (var db = new ClassDbContext())
            {
                var p = ptype;
                var typeName = p.GetTypeName();

                do
                {
                    var n = p.GetTypeName();
                    if (CreateClassModels)
                    {
                        if (!classTypes.ContainsKey(n))
                        {
                            classTypes[n] = db.CreateAndSaveClassModel(p);
                        }
                    }
                    else
                    {
                        if (!classTypes.ContainsKey(n))
                        {
                            classTypes[n] = db.GetClassModel(p);
                        }
                    }
                    p = p.BaseType;
                } while (p is not null && p != typeof(object));

                Dictionary<string, string> result = new();
                MethodModel methodModel = FindMethod(methodName, typeName);

                if (methodModel is null && ptype.BaseType is not null && ptype.BaseType != typeof(object))
                {
                    methodModel = FindMethod(methodName, ptype.BaseType.GetTypeName());


                }
                if (methodModel is not null)
                {
                    foreach (var attr in methodModel.Attributes)
                    {
                        result[attr.Name] = attr.Value;
                    }
                }

                return result;
            }

        }



        /// <summary>
        /// Поиск метода по иерархии классов 
        /// </summary>
        public static MethodModel FindMethod(string methodName, string typeName)
        {
            MethodModel result = null;
            do
            {
                result = classTypes[typeName].Methods.FirstOrDefault(p => p.Name == methodName);
                if (classTypes[typeName].ParentId is null)
                {
                    break;
                }
                using (var db = new ClassDbContext())
                {
                    var parentClass = db.Classes.Find(classTypes[typeName].ParentId);
                    typeName = parentClass.Name;
                }
            }
            while (result is null);
            return result;
        }


        /// <summary>
        /// Поиск метода по иерархии классов 
        /// 
        /// 
        /// </summary>
        public static PropertyModel FindProperty(string methodName, string typeName)
        {

            PropertyModel result = null;

            if (!classTypes.ContainsKey(typeName))
            {
                using (var db = new ClassDbContext())
                {
                    Console.WriteLine(db.Classes.ToList().ToJsonOnScreen());
                    classTypes[typeName] = db.GetClassModel(typeName.ToType());
                }
            }
            var pType = typeName.ToType().BaseType == typeof(object) ? null : typeName.ToType().BaseType.GetTypeName();
            if (pType is not null && pType != typeof(object).GetTypeName() && classTypes[typeName].ParentId is null)
            {
                using (var db = new ClassDbContext())
                {
                    var parentDd = db.CreateAndSaveClassModel(pType.ToType()).Id;
                    classTypes[typeName] = db.GetClassModel(typeName.ToType());
                    classTypes[typeName].ParentId = parentDd;
                    db.SaveChanges();

                }

            }

            result = classTypes[typeName].Properties.FirstOrDefault(p => p.Name == methodName);
            if (result is null)
            {
                if (classTypes[typeName].ParentId is not null)
                {
                    if (typeName.ToType().BaseType != typeof(object))
                    {
                        using (var db = new ClassDbContext())
                        {
                            classTypes[typeName.ToType().BaseType.GetTypeName()] = db.GetClassModel(typeName.ToType().BaseType);
                            classTypes[typeName].ParentId = classTypes[typeName.ToType().BaseType.GetTypeName()].Id;
                            db.SaveChanges();
                        }
                    }
                }

                if (classTypes[typeName].ParentId is not null)
                {
                    using (var db = new ClassDbContext())
                    {
                        var parentClass = db.Classes.Find(classTypes[typeName].ParentId);
                        typeName = parentClass.Name;
                        result = FindProperty(methodName, parentClass.Name);
                    }
                }

            }
            return result;
        }


        /// <summary>
        /// Получение атрибутов параметра метода класса
        /// </summary>        
        public static Dictionary<string, string> GetMethodParameterAttributes(Type ptype, string methodName, string parameterName)
        {
            using (var db = new ClassDbContext())
            {
                var typeName = ptype.GetTypeName();
                if (CreateClassModels)
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.CreateAndSaveClassModel(ptype);
                    }
                }
                else
                {
                    if (!classTypes.ContainsKey(typeName))
                    {
                        classTypes[typeName] = db.GetClassModel(ptype);
                    }
                }

                Dictionary<string, string> result = new();
                var methodModel = classTypes[typeName].Methods.FirstOrDefault(p => p.Name == methodName);
                if (methodModel is null)
                {
                    throw new Exception($"Метод {methodName} не определен в типе {ptype.GetTypeName()}");
                }
                var parameterModel = methodModel.Parameters.FirstOrDefault(p => p.Name == parameterName);
                if (parameterModel is null)
                {
                    throw new Exception($"Метод {methodName} не определяет параметр {parameterName} в типе {ptype.GetTypeName()}");
                }
                foreach (var attr in parameterModel.Attributes)
                {
                    result[attr.Name] = attr.Value;
                }
                return result;

            }
        }

        internal static string GetMethodAttribute(Type ptype, string method, string name)
        {
            var attrs = GetMethodAttributes(ptype, method);
            if (!attrs.ContainsKey(name))
                throw new ArgumentException(nameof(name), $"Метод {method} типа {ptype.GetTypeName()} не объявляет атрибут {name}");
            return attrs[name];
        }

        internal static string GetPropertyAttribute(Type ptype, string property, string name)
        {
            var attrs = GetPropertyAttributes(ptype, property);
            if (!attrs.ContainsKey(name))
                throw new ArgumentException(nameof(name), $"Свойство {property} типа {ptype.GetTypeName()} не объявляет атрибут {name}");
            return attrs[name];
        }

        internal static void AddOrReplaceParameterAttribute(Type ptype, string method, string parameter, string name, string value)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveMethodAttribute(Type ptype, string method, string name)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveParameterAttribute(Type ptype, string method, string parameter, string name)
        {
            throw new NotImplementedException();
        }

        internal static void AddOrReplaceMethodAttribute(Type ptype, string method, string name, string value)
        {
            throw new NotImplementedException();
        }
    }






}
