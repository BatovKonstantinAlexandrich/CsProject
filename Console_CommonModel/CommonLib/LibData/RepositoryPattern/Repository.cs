using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Console_CommonModel.CommonLib.LibData.RepositoryPattern
{

    /// <summary>
    /// Базовая generic реализация паттерна Repository для работы с любыми сущностями через EF Core
    /// Инкапсулирует общую логику доступа к данным
    /// </summary>
    /// <typeparam name="T">Тип сущности, наследующийся от BaseEntity</typeparam>
    /// <typeparam name="TDbContext">Тип контекста базы данных, наследующийся от DbContext</typeparam>
    public class Repository<T, TDbContext> : EfEntityFasade<T>, IRepository<T>
        where T : BaseEntity
        where TDbContext : DbContext
    {
        protected readonly TDbContext _context;
        protected DbSet<T> _dbset { get => _context.Set<T>(); }
        public int CountRecords() => _dbset.Count();

        /// <summary>
        /// Используется для присоединения связанных сущностей
        /// К примеру _includings = new(){ "ProductCatalog" }
        /// Выполнит dbset.Include(p => p.ProductCatalog)
        /// </summary>
        public List<string> _includings = new();

        public IQueryable<T> _qurable
        {
            get
            {
                IQueryable<T> query = _dbset;
                _includings.Sort();

                foreach (var including in _includings)
                {
                    if (!string.IsNullOrWhiteSpace(including))
                    {
                        query = query.Include(including);
                    }
                }
                return query;
            }
        }

        public Repository() : this(typeof(TDbContext).New<TDbContext>()) { }
        public Repository(TDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        /// <summary>
        /// Получить варианты автоподстановки для одного ключевого слова
        /// Результаты отсортированы по релевантности
        /// </summary>
        /// <param name="keyword">Ключевое слово (частично введенный текст)</param>
        /// <param name="maxSuggestions">Максимальное количество вариантов</param>
        /// <returns>Список вариантов продолжения, отсортированных по релевантности</returns>
        public List<RepositoryOption> GetRepositoryOptions(string keyword, int maxSuggestions = 10)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<RepositoryOption>();

            var propertyNames = typeof(Product).GetOwnInputPropertyNames()
                .Where(p => typeof(Product).GetProperty(p)?.PropertyType == typeof(string))
                .ToList();

            if (propertyNames.Count == 0)
                return new List<RepositoryOption>();

            string keywordLower = keyword.Trim().ToLower();
            return GetSuggestionsForKeyword(keywordLower, propertyNames, maxSuggestions);
        }


        /// <summary>
        /// Вычислить релевантность варианта автоподстановки
        /// </summary>
        private int CalculateSuggestionRelevance(
            string originalValue,
            string lowerValue,
            string keyword,
            int keywordIndex,
            (string Text, string FullText) completion)
        {
            int relevance = 0;

            // Если ключевое слово в начале строки - высокая релевантность
            if (keywordIndex == 0)
            {
                relevance += 50;
            }
            // Если ключевое слово в начале слова - средняя релевантность
            else if (keywordIndex > 0 && (lowerValue[keywordIndex - 1] == ' ' || lowerValue[keywordIndex - 1] == '\t'))
            {
                relevance += 30;
            }
            else
            {
                relevance += 10;
            }

            // Если полное значение начинается с ключевого слова - дополнительная релевантность
            if (lowerValue.StartsWith(keyword))
            {
                relevance += 20;
            }

            // Чем короче продолжение, тем выше релевантность (пользователь, вероятно, хочет короткое слово)
            if (completion.Text.Length <= 3)
            {
                relevance += 15;
            }
            else if (completion.Text.Length <= 6)
            {
                relevance += 10;
            }

            // Если это полное слово - дополнительная релевантность
            if (completion.Text == completion.FullText || completion.FullText.Split(' ').Length == 1)
            {
                relevance += 25;
            }

            return relevance;
        }


        /// <summary>
        /// Получить варианты автоподстановки для одного ключевого слова
        /// </summary>
        private List<RepositoryOption> GetSuggestionsForKeyword(
            string keyword,
            IEnumerable<string> propertyNames,
            int maxSuggestions)
        {
            var suggestions = new Dictionary<string, RepositoryOption>();

            // Получаем все продукты, которые содержат ключевое слово
            var matchingProducts = _qurable
                .AsEnumerable() // Переводим в память для работы со строками
                .Where(p => propertyNames.Any(propName =>
                {
                    var prop = typeof(Product).GetProperty(propName);
                    if (prop == null || prop.PropertyType != typeof(string))
                        return false;
                    var value = prop.GetValue(p)?.ToString()?.ToLower() ?? "";
                    return value.Contains(keyword);
                }))
                .ToList();

            foreach (var product in matchingProducts)
            {
                foreach (var propertyName in propertyNames)
                {
                    var propertyInfo = typeof(Product).GetProperty(propertyName);
                    if (propertyInfo == null || propertyInfo.PropertyType != typeof(string))
                        continue;

                    var propertyValue = propertyInfo.GetValue(product)?.ToString();
                    if (string.IsNullOrWhiteSpace(propertyValue))
                        continue;

                    string propertyValueLower = propertyValue.ToLower();

                    // Ищем вхождения ключевого слова в значении свойства
                    int index = propertyValueLower.IndexOf(keyword);
                    if (index >= 0)
                    {
                        // Извлекаем возможные варианты продолжения
                        var completions = ExtractCompletions(propertyValue, propertyValueLower, keyword, index);

                        foreach (var completion in completions)
                        {
                            if (string.IsNullOrWhiteSpace(completion.Text))
                                continue;

                            // Вычисляем релевантность
                            int relevance = CalculateSuggestionRelevance(
                                propertyValue,
                                propertyValueLower,
                                keyword,
                                index,
                                completion);

                            if (suggestions.ContainsKey(completion.Text))
                            {
                                // Если вариант уже есть, увеличиваем релевантность
                                var existing = suggestions[completion.Text];
                                existing.Relevance = Math.Max(existing.Relevance, relevance);
                                existing.Frequency++;
                                existing.SourceProperties.Add(propertyName);
                            }
                            else
                            {
                                suggestions[completion.Text] = new RepositoryOption
                                {
                                    Text = completion.Text,
                                    FullText = completion.FullText,
                                    Relevance = relevance,
                                    Frequency = 1,
                                    SourceProperty = propertyName,
                                    SourceProperties = new HashSet<string> { propertyName }
                                };
                            }
                        }
                    }
                }
            }

            // Сортируем по релевантности и возвращаем топ варианты
            return suggestions.Values
                .OrderByDescending(s => s.Relevance)
                .ThenByDescending(s => s.Frequency)
                .ThenBy(s => s.Text.Length) // Более короткие варианты предпочтительнее
                .Take(maxSuggestions)
                .ToList();
        }

        /// <summary>
        /// Извлечь варианты продолжения из строки
        /// </summary>
        private List<(string Text, string FullText)> ExtractCompletions(
            string originalValue,
            string lowerValue,
            string keyword,
            int keywordIndex)
        {
            var completions = new List<(string Text, string FullText)>();

            // Вариант 1: Продолжение после ключевого слова (до конца слова или строки)
            int afterKeywordIndex = keywordIndex + keyword.Length;
            if (afterKeywordIndex < originalValue.Length)
            {
                // Извлекаем продолжение до пробела или конца строки
                int endIndex = originalValue.IndexOfAny(new[] { ' ', '\t', '\n', '\r' }, afterKeywordIndex);
                if (endIndex < 0)
                    endIndex = originalValue.Length;

                if (endIndex > afterKeywordIndex)
                {
                    string completion = originalValue.Substring(afterKeywordIndex, endIndex - afterKeywordIndex).Trim();
                    if (!string.IsNullOrWhiteSpace(completion))
                    {
                        completions.Add((completion, originalValue));
                    }
                }
            }

            // Вариант 2: Полное слово, содержащее ключевое слово
            // Разбиваем на слова и ищем слова, содержащие ключевое слово
            var words = originalValue.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (word.ToLower().Contains(keyword) && word.Length > keyword.Length)
                {
                    // Извлекаем продолжение ключевого слова в этом слове
                    int wordKeywordIndex = word.ToLower().IndexOf(keyword);
                    if (wordKeywordIndex >= 0)
                    {
                        string wordCompletion = word.Substring(wordKeywordIndex + keyword.Length);
                        if (!string.IsNullOrWhiteSpace(wordCompletion))
                        {
                            completions.Add((wordCompletion, word));
                        }
                    }
                }
            }

            // Вариант 3: Полное значение, если оно начинается с ключевого слова
            if (lowerValue.StartsWith(keyword) && originalValue.Length > keyword.Length)
            {
                string fullCompletion = originalValue.Substring(keyword.Length).Trim();
                if (!string.IsNullOrWhiteSpace(fullCompletion))
                {
                    completions.Add((fullCompletion, originalValue));
                }
            }

            return completions.Distinct().ToList();
        }


        public List<T> Search(string q = "")
        {
            List<T> _dataset = new();
            var words = q.SplitWords().Select(w => w.Trim().ToLower());
            var products = SearchByKeywords(words);
            var page = products.Take(10);
            foreach (var p in page)
            {
                var id = p.Id;
                _dataset.Add(p);
            }
            return _dataset;
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            var q = _qurable
                //.Include(p => p.TCatalog)
                .Where(predicate);
            this.Info(q.ToQueryString());
            return
                q.ToList();
        }

        /// <summary>
        /// Добавить новый продукт
        /// </summary>
        public void Add(T product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _dbset.Add(product);
        }

        /// <summary>
        /// Обновить существующий продукт
        /// </summary>
        public void Update(T product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _dbset.Update(product);
        }

        /// <summary>
        /// Удалить продукт
        /// </summary>
        public void Remove(T product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _dbset.Remove(product);
        }


        /// <summary>
        /// Подсчитать количество продуктов по условию
        /// </summary>
        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _qurable.Count(predicate);
        }

        /// <summary>
        /// Проверить существование продуктов по условию
        /// </summary>
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _qurable.Any(predicate);
        }

        /// <summary>
        /// Сохранить изменения в базе данных
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        public IQueryable<T> PropertyEquals(IQueryable<T> q, string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            var predicate = BuildPropertyEqualsExpression(propertyName, value);
            return q
                //.Include(p => p.TCatalog)
                .Where(predicate);
        }

        public IEnumerable<T> PropertyEquals(string propertyName, string value)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            var predicate = BuildPropertyEqualsExpression(propertyName, value);
            return _qurable
                //.Include(p => p.TCatalog)
                .Where(predicate)
                .ToList();
        }


        public IQueryable<T> Where(IEnumerable<FilterBase> filters)
        {
            IQueryable<T> q = _qurable;
            foreach (var filter in filters)
            {
                if (filter is FilterRange fr)
                {
                    q = PropertyBetween(q, fr.Property, fr.Min, fr.Max);
                }
                else if (filter is FilterSet fs)
                {
                    q = PropertyIn(q, fs.Property, fs.Values);
                }
                else if (filter is FilterNot fn)
                {
                    //q = PropertyIn(q, fs.Property, fs.Values);
                }
                else if (filter is FilterEquals f)
                {
                    q = PropertyEquals(q, f.Property, f.Value);
                }
            }
            return q;

        }

        public IEnumerable<T> PropertyBetween(string propertyName, string minValue, string maxValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            var predicate = BuildPropertyBetweenExpression(propertyName, minValue, maxValue);
            return _qurable
                //.Include(p => p.TCatalog)
                .Where(predicate)
                .ToList();
        }

        public IEnumerable<T> PropertyIn(string propertyName, IEnumerable<string> values)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            if (values == null || values.Count() == 0)
                return new List<T>();


            var predicate = BuildPropertyInExpression(propertyName, values);
            return _qurable
                ////.Include(p => p.TCatalog)
                .Where(predicate)
                .ToList();
        }

        public IEnumerable<T> SearchByKeywords(IEnumerable<string> keywords)
        {
            var props = typeof(T).GetSearchProperties();
            if (props.Count() == 0)
            {
                this.Error($"Не удалось определить поля для поиска по {typeof(T).GetTypeName()}");
            }
            return SearchByKeywords(keywords, props);
        }

        public IEnumerable<T> SearchByKeywords(IEnumerable<string> keywords, IEnumerable<string> propertyNames)
        {
            if (keywords == null || keywords.Count() == 0)
                return _qurable.Take(10).ToList();

            if (propertyNames == null || propertyNames.Count() == 0)
                throw new ArgumentException("Массив названий свойств не может быть пустым", nameof(propertyNames));

            // Строим предикат для поиска
            var predicate = BuildKeywordsSearchExpression(keywords, propertyNames);
            var query = _qurable
                ////.Include(p => p.TCatalog)
                .Where(predicate);

            // Вычисляем релевантность и сортируем
            var results = query.ToList();
            return results
                .Select(product => new { T = product, Relevance = CalculateRelevance(product, keywords, propertyNames) })
                .OrderByDescending(x => x.Relevance)
                .Select(x => x.T)
                .ToList();
        }



        /// <summary>
        /// Вычислить релевантность продукта по количеству совпадений ключевых слов
        /// </summary>
        private int CalculateRelevance(T product, IEnumerable<string> keywords, IEnumerable<string> propertyNames)
        {
            int relevance = 0;

            foreach (var keyword in keywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    continue;

                string keywordLower = keyword.ToLower();

                foreach (var propertyName in propertyNames)
                {
                    var propertyInfo = typeof(T).GetProperty(propertyName);
                    if (propertyInfo == null || propertyInfo.PropertyType != typeof(string))
                        continue;

                    var propertyValue = propertyInfo.GetValue(product)?.ToString();
                    if (string.IsNullOrWhiteSpace(propertyValue))
                        continue;

                    string propertyValueLower = propertyValue.ToLower();

                    if (propertyValueLower.Equals(keywordLower))
                    {
                        relevance += 10;
                    }
                    else if (propertyValueLower.StartsWith(keywordLower))
                    {
                        relevance += 5;
                    }
                    else if (propertyValueLower.Contains(keywordLower))
                    {
                        relevance += 1;
                    }
                }
            }

            return relevance;
        }



        /// <summary>
        /// Получить все сущности
        /// </summary>
        public virtual IEnumerable<T> GetAll()
        {
            return _qurable.ToList();
        }

        /// <summary>
        /// Получить сущность по ID
        /// </summary>
        public virtual T GetById(int id)
        {
            return _qurable.FirstOrDefault(e => e.Id == id);
        }



        public virtual IQueryable<T> PropertyBetween(IQueryable<T> q, string propertyName, string minValue, string maxValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            var predicate = BuildPropertyBetweenExpression(propertyName, minValue, maxValue);
            return q.Where(predicate);
        }

       
        public virtual IQueryable<T> PropertyIn(IQueryable<T> q, string propertyName, IEnumerable<string> values)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Название свойства не может быть пустым", nameof(propertyName));

            if (values == null || values.Count() == 0)
                return q.Where(p => false); // Возвращаем пустой результат

            var predicate = BuildPropertyInExpression(propertyName, values);
            return q.Where(predicate);
        }


        /// <summary>
        /// Поиск сущностей по ключевым словам (использует все свойства сущности для поиска)
        /// </summary>
        public virtual IEnumerable<T> Search(IEnumerable<string> keywords)
        {
            // Получаем все строковые свойства сущности
            var propertyNames = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string))
                .Select(p => p.Name)
                .ToList();

            if (propertyNames.Count == 0)
                return new List<T>();

            return SearchByKeywords(keywords, propertyNames);
        }

        /// <summary>
        /// Построить Expression для фильтрации по свойству и значению
        /// </summary>
        public Expression<Func<T, bool>> BuildPropertyEqualsExpression(string propertyName, string value)
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, propertyName);

            if (property == null)
                throw new ArgumentException($"Свойство '{propertyName}' не найдено в типе {typeof(T).Name}", nameof(propertyName));

            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Свойство '{propertyName}' не найдено в типе {typeof(T).Name}", nameof(propertyName));

            // Преобразуем строковое значение в нужный тип свойства
            object convertedValue = ConvertValue(value, propertyInfo.PropertyType);

            // Создаем константу с преобразованным значением
            var constant = Expression.Constant(convertedValue, propertyInfo.PropertyType);

            // Создаем выражение сравнения (entity.Property == value)
            var equalsExpression = Expression.Equal(property, constant);

            // Создаем лямбда-выражение
            return Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);
        }


        protected Expression<Func<T, bool>> BuildPropertyBetweenExpression(string propertyName, string minValue, string maxValue)
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, propertyName);

            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Свойство '{propertyName}' не найдено в типе {typeof(T).Name}", nameof(propertyName));

            Expression condition = null;

            // Минимальное значение
            if (!string.IsNullOrWhiteSpace(minValue))
            {
                object convertedMinValue = ConvertValue(minValue, propertyInfo.PropertyType);
                var minConstant = Expression.Constant(convertedMinValue, propertyInfo.PropertyType);

                // Для строк используем CompareTo или >=, для остальных >=
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var compareToMethod = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
                    var compareResult = Expression.Call(property, compareToMethod, minConstant);
                    var greaterOrEqual = Expression.GreaterThanOrEqual(compareResult, Expression.Constant(0));
                    condition = greaterOrEqual;
                }
                else
                {
                    condition = Expression.GreaterThanOrEqual(property, minConstant);
                }
            }

            // Максимальное значение
            if (!string.IsNullOrWhiteSpace(maxValue))
            {
                object convertedMaxValue = ConvertValue(maxValue, propertyInfo.PropertyType);
                var maxConstant = Expression.Constant(convertedMaxValue, propertyInfo.PropertyType);

                Expression maxCondition;
                if (propertyInfo.PropertyType == typeof(string))
                {
                    var compareToMethod = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
                    var compareResult = Expression.Call(property, compareToMethod, maxConstant);
                    maxCondition = Expression.LessThanOrEqual(compareResult, Expression.Constant(0));
                }
                else
                {
                    maxCondition = Expression.LessThanOrEqual(property, maxConstant);
                }

                // Объединяем условия
                if (condition != null)
                {
                    condition = Expression.AndAlso(condition, maxCondition);
                }
                else
                {
                    condition = maxCondition;
                }
            }

            if (condition == null)
                throw new ArgumentException("Необходимо указать хотя бы одно значение (minValue или maxValue)", nameof(minValue));

            return Expression.Lambda<Func<T, bool>>(condition, parameter);
        }

        /// <summary>
        /// Построить Expression для фильтрации по множеству значений свойства
        /// </summary>
        protected Expression<Func<T, bool>> BuildPropertyInExpression(string propertyName, IEnumerable<string> values)
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, propertyName);

            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentException($"Свойство '{propertyName}' не найдено в типе {typeof(T).Name}", nameof(propertyName));

            // Преобразуем все значения в нужный тип
            var convertedValues = values
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => ConvertValue(v, propertyInfo.PropertyType))
                .ToList();

            if (convertedValues.Count == 0)
                return Expression.Lambda<Func<T, bool>>(
                    Expression.Constant(false), parameter);

            // Для строк используем Contains, для остальных типов строим OR условия
            if (propertyInfo.PropertyType == typeof(string))
            {
                // Для строк используем Contains
                //var valuesList = convertedValues.Cast<string>().ToList();
                var containsMethod = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) });
                //var valuesConstant = Expression.Constant(valuesList, typeof(List<string>));
                var valuesConstant = Expression.Constant(values, typeof(List<string>));
                
                var containsCall = Expression.Call(valuesConstant, containsMethod, property);
                return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            }
            else
            {
                // Для других типов строим цепочку OR условий
                Expression combinedCondition = null;

                foreach (var convertedValue in convertedValues)
                {
                    var constant = Expression.Constant(convertedValue, propertyInfo.PropertyType);
                    var equalsExpression = Expression.Equal(property, constant);

                    if (combinedCondition == null)
                    {
                        combinedCondition = equalsExpression;
                    }
                    else
                    {
                        combinedCondition = Expression.OrElse(combinedCondition, equalsExpression);
                    }
                }

                return Expression.Lambda<Func<T, bool>>(combinedCondition, parameter);
            }
        }

        /// <summary>
        /// Построить Expression для поиска по ключевым словам
        /// </summary>
        protected Expression<Func<T, bool>> BuildKeywordsSearchExpression(IEnumerable<string> keywords, IEnumerable<string> propertyNames)
        {
            var parameter = Expression.Parameter(typeof(T), "entity");
            Expression combinedCondition = null;

            foreach (var keyword in keywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    continue;

                Expression keywordCondition = null;

                foreach (var propertyName in propertyNames)
                {
                    var propertyInfo = typeof(T).GetProperty(propertyName);
                    if (propertyInfo == null)
                        continue;

                    // Работаем только со строковыми свойствами
                    if (propertyInfo.PropertyType != typeof(string))
                        continue;

                    var property = Expression.Property(parameter, propertyName);
                    var keywordConstant = Expression.Constant(keyword, typeof(string));

                    // Используем Contains для поиска подстроки
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsCall = Expression.Call(property, containsMethod, keywordConstant);

                    // Объединяем условия для одного ключевого слова (OR между свойствами)
                    if (keywordCondition == null)
                    {
                        keywordCondition = containsCall;
                    }
                    else
                    {
                        keywordCondition = Expression.OrElse(keywordCondition, containsCall);
                    }
                }

                // Объединяем условия для всех ключевых слов (AND между ключевыми словами)
                if (keywordCondition != null)
                {
                    if (combinedCondition == null)
                    {
                        combinedCondition = keywordCondition;
                    }
                    else
                    {
                        combinedCondition = Expression.AndAlso(combinedCondition, keywordCondition);
                    }
                }
            }

            if (combinedCondition == null)
            {
                // Если не найдено подходящих свойств, возвращаем условие, которое всегда false
                combinedCondition = Expression.Constant(false);
            }

            return Expression.Lambda<Func<T, bool>>(combinedCondition, parameter);
        }


        /// <summary>
        /// Преобразовать строковое значение в нужный тип
        /// </summary>
        protected object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                // Для nullable типов возвращаем null
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return null;

                // Для value типов возвращаем default
                if (targetType.IsValueType)
                    return Activator.CreateInstance(targetType);

                return null;
            }

            // Обработка nullable типов
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlyingType == typeof(string))
                    return value;

                if (underlyingType == typeof(int) || underlyingType == typeof(int?))
                    return int.Parse(value);

                if (underlyingType == typeof(long) || underlyingType == typeof(long?))
                    return long.Parse(value);

                if (underlyingType == typeof(float) || underlyingType == typeof(float?))
                    return float.Parse(value.Replace(".", ","));

                if (underlyingType == typeof(double) || underlyingType == typeof(double?))
                    return double.Parse(value.Replace(".", ","));

                if (underlyingType == typeof(decimal) || underlyingType == typeof(decimal?))
                    return decimal.Parse(value.Replace(".", ","));

                if (underlyingType == typeof(bool) || underlyingType == typeof(bool?))
                    return bool.Parse(value);

                if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTime?))
                    return DateTime.Parse(value);

                if (underlyingType.IsEnum)
                    return Enum.Parse(underlyingType, value, ignoreCase: true);

                // Используем Convert.ChangeType для других типов
                return Convert.ChangeType(value, underlyingType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Не удалось преобразовать значение '{value}' в тип {targetType.Name}. {ex.Message}",
                    nameof(value),
                    ex);
            }
        }


        /// <summary>
        /// Подсчитать общее количество сущностей
        /// </summary>
        public virtual int Count()
        {
            return _qurable.Count();
        }


        /// <summary>
        /// Применить фильтры к запросу
        /// </summary>
        /*public virtual IQueryable<T> Where<TRelativeEntity1, TRelativeEntity2>(params FilterBase[] filters)
        {
            //var relation = typeof(T).New<BaseEntity>().GetEntityRelations().FirstOrDefault(r => r.RelativeEntityType.ToType().IsExtends(typeof(TRelativeEntity1)));
            //relation.PropertyName
            //_dbSet.Include<TRelativeEntity1>(_context.Set<TRelativeEntity1>().Where());
        }*/

        public virtual IQueryable<T> Where(params FilterBase[] filters)
        {
            IQueryable<T> q = _qurable;
            foreach (var filter in filters)
            {
                if (filter is FilterRange fr)
                {
                    q = PropertyBetween(q, fr.Property, fr.Min, fr.Max);
                }
                else if (filter is FilterSet fs)
                {
                    q = PropertyIn(q, fs.Property, fs.Values);
                }
                else if (filter is FilterNot fn)
                {
                    // Фильтр "не равно" - используем PropertyEquals и инвертируем
                    var predicate = BuildPropertyEqualsExpression(fn.Property, fn.Value);
                    var parameter = predicate.Parameters[0];
                    var notExpression = Expression.Not(predicate.Body);
                    var notPredicate = Expression.Lambda<Func<T, bool>>(notExpression, parameter);
                    q = q.Where(notPredicate);
                }
                else if (filter is FilterEquals f)
                {
                    q = PropertyEquals(q, f.Property, f.Value);
                }
            }
            return q;
        }


        /// <summary>
        /// Получить список фильтров для всех свойств сущности
        /// </summary>
        public virtual IEnumerable<FilterBase> GetFilters()
        {
            List<FilterBase> filters = new();
            var propertyNames = typeof(T).GetProperties()
                .Where(p => !p.PropertyType.IsCollectionType() && p.CanRead)
                .Select(p => p.Name)
                .ToList();
            filters.AddRange(propertyNames.SelectMany(name => new List<FilterBase>() {
                new FilterEquals { Indicator = typeof(T).GetPropertyLabel(name), Property = name, Value = "" },
                new FilterSet { Indicator = typeof(T).GetPropertyLabel(name), Property = name },
                new FilterRange { Indicator = typeof(T).GetPropertyLabel(name), Property = name },
                new FilterNot { Indicator = typeof(T).GetPropertyLabel(name), Property = name },
            }));
            filters.AddRange(propertyNames.Select(name => new FilterEquals { Property = name, Value = "" }));
            return filters;
        }


        public List<Type> GetIncludingsTypes()
        {
            List<Type> types = new();
            return types;
        }



        /// <summary>
        /// Получить IQueryable с выполненной операцией Include по свойству навигации, заданному строковым аргументом
        /// </summary>
        /// <param name="navigationPropertyPath">Путь к свойству навигации (например, "TCatalog" или "TCatalog.Category")</param>
        /// <returns>IQueryable с примененным Include</returns>
        public virtual IQueryable<T> Include(string navigationPropertyPath)
        {
            if (string.IsNullOrWhiteSpace(navigationPropertyPath))
                throw new ArgumentException("Путь к свойству навигации не может быть пустым", nameof(navigationPropertyPath));

            // Проверяем, что свойство существует
            var propertyNames = navigationPropertyPath.Split('.');
            var currentType = typeof(T);

            foreach (var propertyName in propertyNames)
            {
                var property = currentType.GetProperty(propertyName);
                if (property == null)
                    throw new ArgumentException($"Свойство навигации '{propertyName}' не найдено в типе {currentType.Name}", nameof(navigationPropertyPath));

                // Для следующего уровня навигации
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    currentType = property.PropertyType.GetGenericArguments()[0];
                }
                else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    currentType = property.PropertyType;
                }
            }

            return _qurable.Include(navigationPropertyPath);
        }

        /// <summary>
        /// Получить IQueryable с выполненными операциями Include по нескольким свойствам навигации
        /// </summary>
        /// <param name="navigationPropertyPaths">Пути к свойствам навигации</param>
        /// <returns>IQueryable с примененными Include</returns>
        public virtual IQueryable<T> Include(params string[] navigationPropertyPaths)
        {
            if (navigationPropertyPaths == null || navigationPropertyPaths.Length == 0)
                return _qurable;

            IQueryable<T> query = _qurable;
            foreach (var path in navigationPropertyPaths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    query = query.Include(path);
                }
            }
            return query;
        }

        /// <summary>
        /// Получить IQueryable с выполненной операцией Include по свойству навигации через Expression (типобезопасный вариант)
        /// </summary>
        /// <typeparam name="TProperty">Тип свойства навигации</typeparam>
        /// <param name="navigationPropertyPath">Expression, указывающий на свойство навигации</param>
        /// <returns>IQueryable с примененным Include</returns>
        public virtual IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            if (navigationPropertyPath == null)
                throw new ArgumentNullException(nameof(navigationPropertyPath));

            return _qurable.Include(navigationPropertyPath);
        }

        /// <summary>
        /// Получить IQueryable с выполненной операцией ThenInclude для вложенных свойств навигации (строковый вариант)
        /// </summary>
        /// <param name="source">IQueryable с уже примененным Include</param>
        /// <param name="navigationPropertyPath">Путь к вложенному свойству навигации</param>
        /// <returns>IQueryable с примененным ThenInclude</returns>
        public virtual IQueryable<T> ThenInclude(IQueryable<T> source, string navigationPropertyPath)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(navigationPropertyPath))
                throw new ArgumentException("Путь к свойству навигации не может быть пустым", nameof(navigationPropertyPath));

            return source.Include(navigationPropertyPath);
        }


        #region GroupBy and Aggregation Methods

        /// <summary>
        /// Результат группировки с агрегатными значениями
        /// </summary>
        public class GroupByResult
        {
            public object GroupKey { get; set; }
            public Dictionary<string, object> Aggregations { get; set; } = new Dictionary<string, object>();
            public int Count { get; set; }
        }

        /// <summary>
        /// Группировка данных по указанному свойству
        /// </summary>
        /// <param name="groupByProperty">Имя свойства для группировки</param>
        /// <returns>Словарь, где ключ - значение свойства группировки, значение - список сущностей</returns>
        public virtual Dictionary<object, List<T>> GroupBy(string groupByProperty)
        {
            if (string.IsNullOrWhiteSpace(groupByProperty))
                throw new ArgumentException("Имя свойства для группировки не может быть пустым", nameof(groupByProperty));

            var propertyInfo = typeof(T).GetProperty(groupByProperty);
            if (propertyInfo == null)
                throw new ArgumentException($"Свойство '{groupByProperty}' не найдено в типе {typeof(T).Name}", nameof(groupByProperty));

            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, groupByProperty);
            var keySelector = Expression.Lambda(property, parameter);

            var groupByMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "GroupBy" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), propertyInfo.PropertyType);

            var groupedQuery = groupByMethod.Invoke(null, new object[] { _qurable, keySelector }) as IQueryable;

            var result = new Dictionary<object, List<T>>();
            if (groupedQuery != null)
            {
                foreach (var group in groupedQuery)
                {
                    var keyProperty = group.GetType().GetProperty("Key");
                    var elementsProperty = group.GetType().GetProperty("Elements") ?? group.GetType().GetProperty("Items");

                    if (keyProperty != null && elementsProperty != null)
                    {
                        var key = keyProperty.GetValue(group);
                        var elements = elementsProperty.GetValue(group) as IEnumerable<T>;
                        if (elements != null)
                        {
                            result[key] = elements.ToList();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Группировка с агрегатными функциями
        /// </summary>
        /// <param name="groupByProperty">Имя свойства для группировки</param>
        /// <param name="aggregationProperty">Имя свойства для агрегации</param>
        /// <param name="aggregationFunction">Название агрегатной функции: Count, Sum, Average, Min, Max</param>
        /// <returns>Список результатов группировки с агрегатными значениями</returns>
        public virtual List<GroupByResult> GroupByWithAggregation(string groupByProperty, string aggregationProperty, string aggregationFunction)
        {
            if (string.IsNullOrWhiteSpace(groupByProperty))
                throw new ArgumentException("Имя свойства для группировки не может быть пустым", nameof(groupByProperty));

            if (string.IsNullOrWhiteSpace(aggregationFunction))
                throw new ArgumentException("Название агрегатной функции не может быть пустым", nameof(aggregationFunction));

            var groupPropertyInfo = typeof(T).GetProperty(groupByProperty);
            if (groupPropertyInfo == null)
                throw new ArgumentException($"Свойство '{groupByProperty}' не найдено в типе {typeof(T).Name}", nameof(groupByProperty));

            var results = new List<GroupByResult>();

            // Если агрегатная функция - Count и свойство не указано, используем простой Count
            if (aggregationFunction.Equals("Count", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(aggregationProperty))
            {
                var groupedData = GroupBy(groupByProperty);
                foreach (var group in groupedData)
                {
                    results.Add(new GroupByResult
                    {
                        GroupKey = group.Key,
                        Count = group.Value.Count,
                        Aggregations = new Dictionary<string, object> { { "Count", group.Value.Count } }
                    });
                }
                return results;
            }

            // Для других агрегатных функций требуется свойство
            if (string.IsNullOrWhiteSpace(aggregationProperty))
                throw new ArgumentException("Имя свойства для агрегации не может быть пустым для функции " + aggregationFunction, nameof(aggregationProperty));

            var aggPropertyInfo = typeof(T).GetProperty(aggregationProperty);
            if (aggPropertyInfo == null)
                throw new ArgumentException($"Свойство '{aggregationProperty}' не найдено в типе {typeof(T).Name}", nameof(aggregationProperty));

            // Выполняем группировку в памяти для сложных агрегаций
            var groupedData2 = GroupBy(groupByProperty);

            foreach (var group in groupedData2)
            {
                var result = new GroupByResult
                {
                    GroupKey = group.Key,
                    Count = group.Value.Count
                };

                object aggregationValue = null;

                switch (aggregationFunction.ToLower())
                {
                    case "sum":
                        aggregationValue = CalculateSum(group.Value, aggregationProperty);
                        break;
                    case "average":
                    case "avg":
                        aggregationValue = CalculateAverage(group.Value, aggregationProperty);
                        break;
                    case "min":
                        aggregationValue = CalculateMin(group.Value, aggregationProperty);
                        break;
                    case "max":
                        aggregationValue = CalculateMax(group.Value, aggregationProperty);
                        break;
                    case "count":
                        aggregationValue = group.Value.Count;
                        break;
                    default:
                        throw new ArgumentException($"Неподдерживаемая агрегатная функция: {aggregationFunction}. Поддерживаются: Count, Sum, Average, Min, Max", nameof(aggregationFunction));
                }

                result.Aggregations[aggregationFunction] = aggregationValue;
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Группировка с несколькими агрегатными функциями
        /// </summary>
        /// <param name="groupByProperty">Имя свойства для группировки</param>
        /// <param name="aggregations">Словарь: ключ - имя свойства для агрегации, значение - название агрегатной функции (Count, Sum, Average, Min, Max)</param>
        /// <returns>Список результатов группировки с агрегатными значениями</returns>
        public virtual List<GroupByResult> GroupByWithMultipleAggregations(string groupByProperty, Dictionary<string, string> aggregations)
        {
            if (string.IsNullOrWhiteSpace(groupByProperty))
                throw new ArgumentException("Имя свойства для группировки не может быть пустым", nameof(groupByProperty));

            if (aggregations == null || aggregations.Count == 0)
                throw new ArgumentException("Список агрегаций не может быть пустым", nameof(aggregations));

            var groupedData3 = GroupBy(groupByProperty);
            var results = new List<GroupByResult>();

            foreach (var group in groupedData3)
            {
                var result = new GroupByResult
                {
                    GroupKey = group.Key,
                    Count = group.Value.Count
                };

                foreach (var aggregation in aggregations)
                {
                    var aggregationProperty = aggregation.Key;
                    var aggregationFunction = aggregation.Value;

                    if (string.IsNullOrWhiteSpace(aggregationFunction))
                        continue;

                    object aggregationValue = null;

                    // Если свойство пустое, это означает Count без свойства
                    if (string.IsNullOrWhiteSpace(aggregationProperty) && aggregationFunction.Equals("Count", StringComparison.OrdinalIgnoreCase))
                    {
                        aggregationValue = group.Value.Count;
                    }
                    else if (!string.IsNullOrWhiteSpace(aggregationProperty))
                    {
                        switch (aggregationFunction.ToLower())
                        {
                            case "sum":
                                aggregationValue = CalculateSum(group.Value, aggregationProperty);
                                break;
                            case "average":
                            case "avg":
                                aggregationValue = CalculateAverage(group.Value, aggregationProperty);
                                break;
                            case "min":
                                aggregationValue = CalculateMin(group.Value, aggregationProperty);
                                break;
                            case "max":
                                aggregationValue = CalculateMax(group.Value, aggregationProperty);
                                break;
                            case "count":
                                aggregationValue = group.Value.Count;
                                break;
                        }
                    }

                    if (aggregationValue != null)
                    {
                        var key = string.IsNullOrWhiteSpace(aggregationProperty)
                            ? aggregationFunction
                            : $"{aggregationFunction}_{aggregationProperty}";
                        result.Aggregations[key] = aggregationValue;
                    }
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Группировка с агрегатными функциями с поддержкой Include и группировки по связанным данным
        /// </summary>
        /// <param name="includes">Список путей навигации для Include (например, "Category", "Category.Parent")</param>
        /// <param name="groupByProperties">Список свойств для группировки (могут быть вложенными через точку, например "Category.Name")</param>
        /// <param name="aggregations">Словарь агрегаций: ключ - имя свойства для агрегации (может быть вложенным), значение - название агрегатной функции (Count, Sum, Average, Min, Max)</param>
        /// <returns>Список результатов группировки с агрегатными значениями</returns>
        public virtual List<GroupByResult> GroupByWithAggregation(
            List<string> includes,
            List<string> groupByProperties,
            Dictionary<string, string> aggregations)
        {
            if (groupByProperties == null || groupByProperties.Count == 0)
                throw new ArgumentException("Список свойств для группировки не может быть пустым", nameof(groupByProperties));

            if (aggregations == null || aggregations.Count == 0)
                throw new ArgumentException("Список агрегаций не может быть пустым", nameof(aggregations));

            // Применяем Include
            IQueryable<T> query = _dbset;
            if (includes != null && includes.Count > 0)
            {
                foreach (var include in includes)
                {
                    if (!string.IsNullOrWhiteSpace(include))
                    {
                        query = query.Include(include);
                    }
                }
            }

            // Загружаем данные в память для работы с вложенными свойствами
            var allItems = query.ToList();

            // Выполняем группировку по нескольким свойствам
            var groupedData = GroupByMultipleProperties(allItems, groupByProperties);
            var results = new List<GroupByResult>();

            foreach (var group in groupedData)
            {
                var result = new GroupByResult
                {
                    GroupKey = group.Key,
                    Count = group.Value.Count
                };

                foreach (var aggregation in aggregations)
                {
                    var aggregationProperty = aggregation.Key;
                    var aggregationFunction = aggregation.Value;

                    if (string.IsNullOrWhiteSpace(aggregationFunction))
                        continue;

                    object aggregationValue = null;

                    // Если свойство пустое, это означает Count без свойства
                    if (string.IsNullOrWhiteSpace(aggregationProperty) && aggregationFunction.Equals("Count", StringComparison.OrdinalIgnoreCase))
                    {
                        aggregationValue = group.Value.Count;
                    }
                    else if (!string.IsNullOrWhiteSpace(aggregationProperty))
                    {
                        switch (aggregationFunction.ToLower())
                        {
                            case "sum":
                                aggregationValue = CalculateSumByPath(group.Value, aggregationProperty);
                                break;
                            case "average":
                            case "avg":
                                aggregationValue = CalculateAverageByPath(group.Value, aggregationProperty);
                                break;
                            case "min":
                                aggregationValue = CalculateMinByPath(group.Value, aggregationProperty);
                                break;
                            case "max":
                                aggregationValue = CalculateMaxByPath(group.Value, aggregationProperty);
                                break;
                            case "count":
                                aggregationValue = group.Value.Count;
                                break;
                        }
                    }

                    if (aggregationValue != null)
                    {
                        var key = string.IsNullOrWhiteSpace(aggregationProperty)
                            ? aggregationFunction
                            : $"{aggregationFunction}_{aggregationProperty}";
                        result.Aggregations[key] = aggregationValue;
                    }
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Группировка по нескольким свойствам (включая вложенные через точку)
        /// </summary>
        private Dictionary<string, List<T>> GroupByMultipleProperties(List<T> items, List<string> groupByProperties)
        {
            var result = new Dictionary<string, List<T>>();

            foreach (var item in items)
            {
                // Формируем составной ключ из всех свойств группировки
                var keyParts = new List<string>();
                foreach (var propertyPath in groupByProperties)
                {
                    if (string.IsNullOrWhiteSpace(propertyPath))
                        continue;

                    var value = GetValueByPath(item, propertyPath);
                    keyParts.Add(value?.ToString() ?? "null");
                }

                var compositeKey = string.Join("|", keyParts);

                if (!result.ContainsKey(compositeKey))
                {
                    result[compositeKey] = new List<T>();
                }

                result[compositeKey].Add(item);
            }

            return result;
        }

        /// <summary>
        /// Получить значение свойства по пути (поддерживает вложенные свойства через точку)
        /// </summary>
        private object GetValueByPath(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyPath))
                return null;

            var parts = propertyPath.Split('.');
            object current = obj;

            foreach (var part in parts)
            {
                if (current == null)
                    return null;

                var propertyInfo = current.GetType().GetProperty(part);
                if (propertyInfo == null)
                    return null;

                current = propertyInfo.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// Вычислить сумму значений свойства по пути (поддерживает вложенные свойства)
        /// </summary>
        private object CalculateSumByPath(List<T> items, string propertyPath)
        {
            var values = items.Select(item => GetValueByPath(item, propertyPath))
                .Where(v => v != null)
                .ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Sum();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Sum();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Sum();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Sum();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Sum();

            return null;
        }

        /// <summary>
        /// Вычислить среднее значение свойства по пути (поддерживает вложенные свойства)
        /// </summary>
        private object CalculateAverageByPath(List<T> items, string propertyPath)
        {
            var values = items.Select(item => GetValueByPath(item, propertyPath))
                .Where(v => v != null)
                .ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Average();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Average();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Average();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Average();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Average();

            return null;
        }

        /// <summary>
        /// Вычислить минимальное значение свойства по пути (поддерживает вложенные свойства)
        /// </summary>
        private object CalculateMinByPath(List<T> items, string propertyPath)
        {
            var values = items.Select(item => GetValueByPath(item, propertyPath))
                .Where(v => v != null)
                .ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Min();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Min();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Min();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Min();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Min();
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return values.Select(v => Convert.ToDateTime(v)).Min();
            if (type == typeof(string))
                return values.Select(v => v?.ToString()).Where(v => v != null).Min();

            return null;
        }

        /// <summary>
        /// Вычислить максимальное значение свойства по пути (поддерживает вложенные свойства)
        /// </summary>
        private object CalculateMaxByPath(List<T> items, string propertyPath)
        {
            var values = items.Select(item => GetValueByPath(item, propertyPath))
                .Where(v => v != null)
                .ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Max();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Max();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Max();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Max();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Max();
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return values.Select(v => Convert.ToDateTime(v)).Max();
            if (type == typeof(string))
                return values.Select(v => v?.ToString()).Where(v => v != null).Max();

            return null;
        }

        /// <summary>
        /// Вычислить сумму значений свойства
        /// </summary>
        private object CalculateSum(List<T> items, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                return null;

            var values = items.Select(item => propertyInfo.GetValue(item)).Where(v => v != null).ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Sum();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Sum();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Sum();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Sum();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Sum();

            return null;
        }

        /// <summary>
        /// Вычислить среднее значение свойства
        /// </summary>
        private object CalculateAverage(List<T> items, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                return null;

            var values = items.Select(item => propertyInfo.GetValue(item)).Where(v => v != null).ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Average();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Average();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Average();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Average();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Average();

            return null;
        }

        /// <summary>
        /// Вычислить минимальное значение свойства
        /// </summary>
        private object CalculateMin(List<T> items, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                return null;

            var values = items.Select(item => propertyInfo.GetValue(item)).Where(v => v != null).ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Min();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Min();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Min();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Min();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Min();
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return values.Select(v => Convert.ToDateTime(v)).Min();
            if (type == typeof(string))
                return values.Select(v => v?.ToString()).Where(v => v != null).Min();

            return null;
        }

        /// <summary>
        /// Вычислить максимальное значение свойства
        /// </summary>
        private object CalculateMax(List<T> items, string propertyName)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName);
            if (propertyInfo == null)
                return null;

            var values = items.Select(item => propertyInfo.GetValue(item)).Where(v => v != null).ToList();

            if (values.Count == 0)
                return null;

            var firstValue = values.First();
            var type = firstValue.GetType();

            if (type == typeof(int) || type == typeof(int?))
                return values.Select(v => Convert.ToInt32(v)).Max();
            if (type == typeof(long) || type == typeof(long?))
                return values.Select(v => Convert.ToInt64(v)).Max();
            if (type == typeof(float) || type == typeof(float?))
                return values.Select(v => Convert.ToSingle(v)).Max();
            if (type == typeof(double) || type == typeof(double?))
                return values.Select(v => Convert.ToDouble(v)).Max();
            if (type == typeof(decimal) || type == typeof(decimal?))
                return values.Select(v => Convert.ToDecimal(v)).Max();
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return values.Select(v => Convert.ToDateTime(v)).Max();
            if (type == typeof(string))
                return values.Select(v => v?.ToString()).Where(v => v != null).Max();

            return null;
        }

        #endregion

        /// <summary>
        /// Предложить возможные 
        /// </summary>     
        public List<string> GetAvailableIncludes()
        {
            List<List<string>> options = new();
            using (var db = typeof(TDbContext).New<DbContextBase>())
            {
                foreach (var entityType in db.GetEntitiesTypes())
                {
                    List<string> ext = new() { entityType.GetTypeName() };
                    options.Add(new() { entityType.GetTypeName() });
                    var relations = db.GetEntityRelationsForTypeWithExpected(entityType, ext);
                    foreach (var rel in relations.ToList())
                    {
                        options.AddRange(rel.GetRelationsList(new List<string>() { entityType.GetTypeName() }));
                    }
                }
            }
            return options.Select(list => string.Join(".", list)).ToHashSet().ToList();
        }

        private HashSet<Type> GetIncludedTypes()
        {
            List<Type> types = new();
            var result = new List<string>();
            foreach (var include in _includings)
            {
                Type p = typeof(T);
                types.Add(p);
                var ids = include.Split(".");
                foreach (var id in ids)
                {
                    var relations = p.New<BaseEntity>().GetEntityRelations();
                    var navigation = relations.FirstOrDefault(r => r.PropertyName == id);
                    p = navigation.RelativeEntityType.ToType();
                    types.Add(p);
                }
            }

            return types.ToHashSet();
        }
    }
}

