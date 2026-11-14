using System;
using WinFormsApp.Console.Console_ClassModule.ClassDataModel;


namespace WinFormsApp.Console.Console_ClassModule.ClassServiceModel
{

    public interface IClassDbContextService
    {
        void ClearClassModel();
        ClassModel CreateAndSaveClassModel(Type type);
        ClassModel GetClassModel(int id);
        ClassModel GetClassModel(Type type);
    }

}