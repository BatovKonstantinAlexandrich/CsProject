using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Console_CommonModel.CommonLib.LibData
{
    [Label("Проверка уровня данных")]
    public class DbContextTests : TestingElement
    {
        public DbContextTests(IServiceProvider provider) : base(provider)
        {

        }

        public override void OnTest()
        {
            Messages.AddRange(ValidateDatabases());
        }

        public List<string> ValidateDatabases()
        {
            this.Info("Валидация контекстов данных");
            List<string> errors = new();
            //ValidateEntity(errors, typeof(DbContextUser), typeof(UserContext));
            foreach (var pdbtype in ServiceFactory.Get().GetTypesExtended<DbContext>().Where(p => !p.IsAbstract && p.GetGenericArguments().Count() == 0))
            {
                errors.AddRange(ValidateDatabase(pdbtype));
            }
            return errors;
        }

        public List<string> ValidateDatabase(Type pdbtype)
        {
            List<string> errors = new();
            this.Info($"Проверяю {pdbtype.GetTypeName()}");
            Assert((p) =>
            {
                try
                {
                    using (var pdb = (DbContext)pdbtype.New())
                    {
                        pdb.Database.EnsureDeleted();
                        pdb.Database.EnsureCreated();
                        var initiallizer = pdb.GetInitializerBase();
                        if (initiallizer is not null)
                        {
                            initiallizer.Call("Init", new List<object>() { pdb });
                        }
                        else
                        {
                            Messages.Add($"Не реализован класс наполнения базы данных {pdb.GetTypeName()}");
                        }
                        foreach (var entity in pdb.GetEntitiesTypes())
                        {
                            ValidateEntity(errors, pdbtype, entity);
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Messages.Add(ex.Message);
                    return false;
                }
            },
            $"База данных {pdbtype.GetTypeName()} прошла проверку и готова к использованию",
            $"База данных {pdbtype.GetTypeName()} непрошла проверку и непригодна для использования");
            return errors;
        }

        public void ValidateEntity(List<string> errors, Type pdbtype, Type entity)
        {
            Assert(p =>
            {
                try
                {
                    this.Info($"Проверяю {pdbtype.GetTypeName()} {entity.GetTypeName()}");

                    var name = entity.GetTypeName();
                    var canCrud = CanCrud(name);
                    var hasData = HasData(name);
                    //this.Info($"{pdb.GetType().GetTypeName()}.{entity}: canCrud: {canCrud} hasData: {hasData}");

                    if (!canCrud) errors.Add($"Не удалось выполнить операции. вставки удаления, редактирования таблиц сущности {name}");
                    if (!hasData) errors.Add($"Не заполнены данными таблицы сущности {name}");
                    this.Info($"Проверка {entity.GetTypeName()} завершена => : canCrud: {canCrud} hasData: {hasData}");
                    if (errors.Count() > 0)
                        throw new Exception($"Ошибка при проверки сущности {entity.GetTypeName()}: \n{errors.ToJson()}");
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }, "", "");
        }
        public void ValidateEntity(Type entity)
        {
            var errors = new List<string>();
            var pdbtype = entity.GetDbContextWithEntityType();
            this.Info($"Проверяю {pdbtype.GetTypeName()} {entity.GetTypeName()}");

            var name = entity.GetTypeName();
            var canCrud = CanCrud(name);
            var hasData = HasData(name);
            //this.Info($"{pdb.GetType().GetTypeName()}.{entity}: canCrud: {canCrud} hasData: {hasData}");

            if (!canCrud) errors.Add($"Не удалось выполнить операции. вставки удаления, редактирования таблиц сущности {name}");
            if (!hasData) errors.Add($"Не заполнены данными таблицы сущности {name}");
            this.Info($"Проверка {entity.GetTypeName()} завершена => : canCrud: {canCrud} hasData: {hasData}");
            if (errors.Count() > 0)
                throw new Exception($"Ошибка при проверки сущности {entity.GetTypeName()}: \n{errors.ToJson()}");
        }
        private bool HasData(string name)
        {
            object fasade = typeof(EfEntityFasade<>).MakeGenericType(name.ToType()).New(new object[] { name.ToType().GetDbContext() });
            int result = (int)fasade.Call("Count", new List<object>());

            return result > 0;
        }

        private bool CanCrud(string name)
        {
            if (name == nameof(UserMessageFile))
            {
                int x = 0;

                x += 1;
                Console.WriteLine(x);
                x += 1;
                Console.WriteLine(x);

            }
            try
            {
                object fasade = typeof(EfEntityFasade<>).MakeGenericType(name.ToType()).GetConstructors().First().Invoke(new object[] { name.ToType().GetDbContext() });
                var data = name.New();
                fasade.GetType().GetMethod("Create").Invoke(fasade, new object[] { data });
                fasade.GetType().GetMethod("Remove").Invoke(fasade, new object[] { data.GetValue("Id") });
                //fasade.Update( );
                fasade.GetType().GetMethod("GetAll").Invoke(fasade, new object[0]);
                //var crud = (dynamic)typeof(CrudController<>).MakeGenericType(name.ToType()).New();
                //crud.SearchDialog();
                //crud.CreateDialog();
                //crud.EditDialog();
                //crud.DeleteDialog();
                return true;
            }
            catch (Exception ex)
            {
                this.Error(ex.ToDocument());
                Messages.Add($"Не удалось выполнить CRUD для {name}: {ex.ToDocument()}");
                return false;
            }
        }


    }
}
