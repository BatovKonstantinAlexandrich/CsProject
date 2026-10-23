using Microsoft.AspNetCore.Components;

namespace Blazor_CommonModel.App.WebInterface.SharedLib
{
    [Label("Базовый класс для модулей общего назначения.")]
    [Description("Модули формируют классы объекты из которых строится приложение.")]
    public abstract class SharedBaseModule: ComponentBase
    {
        [Label("Модуль тестирование")]
        [NotNullNotEmpty]
        public abstract TestingUnit GetTestUnit();

        [Label("Компоненты представления")]
        [NotNullNotEmpty]
        public abstract List<Type> GetRazorComponents();

        [Label("Источники данных")]
        [NotNullNotEmpty]
        public abstract List<Type> GetDataSources();

        [Label("Создание компонентов связанных с данными( представление+источник )")]
        [NotNullNotEmpty]
        public abstract List<Type> BuildWebComponents(Type componentType, Type datasourceType);

        [Label("Получение полного набора палитры элементов")]
        [NotNullNotEmpty]
        public abstract List<Type> GenerateWebPallete();




    }
}
