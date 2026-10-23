using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.Linq.Expressions;


namespace Console_CommonModel.CommonLib.LibData.RepositoryPattern
{
    /// <summary>
    /// Базовый generic интерфейс репозитория для работы с любыми сущностями
    /// Определяет общий контракт для работы с данными через EF Core
    /// </summary>
    /// <typeparam name="T">Тип сущности, наследующийся от BaseEntity</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Получить все сущности
        /// </summary>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Получить сущность по ID
        /// </summary>
        T GetById(int id);

        /// <summary>
        /// Найти сущности по условию
        /// </summary>
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Найти сущности по названию свойства и значению (динамическая фильтрация)
        /// </summary>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="value">Значение в виде строки (будет преобразовано в нужный тип)</param>
        /// <returns>Список сущностей, у которых указанное свойство равно указанному значению</returns>
        IEnumerable<T> PropertyEquals(string propertyName, string value);

        /// <summary>
        /// Найти сущности по диапазону значений свойства (динамическая фильтрация)
        /// </summary>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="minValue">Минимальное значение в виде строки (может быть null)</param>
        /// <param name="maxValue">Максимальное значение в виде строки (может быть null)</param>
        /// <returns>Список сущностей, у которых указанное свойство находится в указанном диапазоне</returns>
        IEnumerable<T> PropertyBetween(string propertyName, string minValue, string maxValue);

        /// <summary>
        /// Найти сущности по множеству значений свойства (динамическая фильтрация)
        /// </summary>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="values">Массив значений в виде строк (будут преобразованы в нужный тип)</param>
        /// <returns>Список сущностей, у которых указанное свойство входит в указанный список значений</returns>
        IEnumerable<T> PropertyIn(string propertyName, IEnumerable<string> values);

        /// <summary>
        /// Поиск сущностей по ключевым словам в указанных свойствах с сортировкой по релевантности
        /// </summary>
        /// <param name="keywords">Массив ключевых слов для поиска</param>
        /// <param name="propertyNames">Массив названий свойств, по которым производится поиск</param>
        /// <returns>Список сущностей, отсортированный по релевантности (количество совпадений)</returns>
        IEnumerable<T> SearchByKeywords(IEnumerable<string> keywords, IEnumerable<string> propertyNames);

        /// <summary>
        /// Поиск сущностей по ключевым словам (использует все строковые свойства сущности для поиска)
        /// </summary>
        /// <param name="keywords">Массив ключевых слов для поиска</param>
        /// <returns>Список сущностей, отсортированный по релевантности (количество совпадений)</returns>
        IEnumerable<T> SearchByKeywords(IEnumerable<string> keywords);

        /// <summary>
        /// Добавить новую сущность
        /// </summary>
        void Add(T entity);

        /// <summary>
        /// Обновить существующую сущность
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Подсчитать общее количество сущностей
        /// </summary>
        int Count();

        /// <summary>
        /// Подсчитать количество сущностей по условию
        /// </summary>
        int Count(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Проверить существование сущностей по условию
        /// </summary>
        bool Exists(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Сохранить изменения в базе данных
        /// </summary>
        void Save();
    }
}

