using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Console_CommonModel.CommonLib.Others
{
    /// <summary>
    /// Класс для представления информации о вызове в стеке вызовов
    /// </summary>
    public class StackFrameInfo
    {
        /// <summary>
        /// Название класса
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Имя метода
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Номер строки вызова (может быть null, если информация недоступна)
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// Полное имя типа (Namespace.ClassName)
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Параметры метода с именами и типами
        /// </summary>
        public List<MethodParameterInfo> MethodParameters { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public StackFrameInfo()
        {
            ClassName = string.Empty;
            MethodName = string.Empty;
            FullTypeName = string.Empty;
            LineNumber = null;
            MethodParameters = new List<MethodParameterInfo>();
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        public StackFrameInfo(string className, string methodName, int? lineNumber = null, string fullTypeName = null, List<MethodParameterInfo> methodParameters = null)
        {
            ClassName = className ?? string.Empty;
            MethodName = methodName ?? string.Empty;
            LineNumber = lineNumber;
            FullTypeName = fullTypeName ?? string.Empty;
            MethodParameters = methodParameters ?? new List<MethodParameterInfo>();
        }

        /// <summary>
        /// Возвращает строковое представление информации о вызове
        /// </summary>
        public override string ToString()
        {
            var lineInfo = LineNumber.HasValue ? $" (строка {LineNumber.Value})" : "";

            string argsInfo;
            if (MethodParameters != null && MethodParameters.Count > 0)
            {
                argsInfo = $"({string.Join(", ", MethodParameters.Select(p => p.ToString()))})";
            }
            else
            {
                argsInfo = "()";
            }

            return $"{ClassName}.{MethodName}{argsInfo}{lineInfo}";
        }
    }

    /// <summary>
    /// Статический класс для анализа стека вызовов
    /// </summary>
    public static class StackTraceAnalyzer
    {

        public static List<StackFrameInfo> GetCallStack()
        {
            var results = AnalyzeStackTrace();
            results.Reverse();
            if (results.Count() > 1)
                results.Remove(results.Last());
            if (results.Count() > 1)
                results.Remove(results.Last());
            if (results.Count() > 1)
                results.Remove(results.Last());
            return results;
        }

        /// <summary>
        /// Анализирует Environment.StackTrace и возвращает список объектов StackFrameInfo с информацией о вызовах
        /// </summary>
        /// <param name="stackTrace">Стек вызовов. Если null, используется Environment.StackTrace</param>
        /// <returns>Список объектов StackFrameInfo в порядке вызовов</returns>
        public static List<StackFrameInfo> AnalyzeStackTrace(string stackTrace = null)
        {
            if (stackTrace == null)
            {
                stackTrace = Environment.StackTrace;
            }

            var result = new List<StackFrameInfo>();

            if (string.IsNullOrWhiteSpace(stackTrace))
            {
                return result;
            }

            // Регулярное выражение для парсинга строки стека вызовов
            // Формат: "   at Namespace.ClassName`1[TParam].MethodName`1[TParam](Int32 param1, String param2) in filepath:line X"
            // или: "   at Namespace.ClassName`1.MethodName(parameters)"
            // или: "   at Namespace.ClassName.MethodName(parameters)"
            // Обрабатываем generic типы с обратными кавычками и параметрами в квадратных скобках (включая вложенные)
            // Захватываем параметры метода в скобках
            var pattern = @"at\s+(?<fullName>(?:[a-zA-Z_][a-zA-Z0-9_]*\.?)+(?:`\d+(?:\[[^\]]+\])?)?)\.(?<methodName>[a-zA-Z_][a-zA-Z0-9_]*(?:`\d+(?:\[[^\]]+\])?)?)\((?<methodParams>[^)]*)\)(?:\s+in\s+[^:]+:line\s+(?<lineNumber>\d+))?";
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var lines = stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                
                
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine) || !trimmedLine.StartsWith("at ", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (line.Contains("CommonProgram.cs:line 33"))
                {
                    int x = 0;
                    x.Info(1);
                    regex.Info($"'{trimmedLine}'");
                }
                
                var match = regex.Match(trimmedLine);
                if (match.Success)
                {
                    var fullName = match.Groups["fullName"].Value;
                    var methodName = match.Groups["methodName"].Value;
                    var methodParams = match.Groups["methodParams"].Value;
                    var lineNumberGroup = match.Groups["lineNumber"];

                    // Извлекаем имя класса из полного имени (последняя часть после точки)
                    var className = ExtractClassName(fullName);

                    // Очищаем имя метода от generic параметров для более читаемого отображения
                    methodName = CleanGenericTypeName(methodName);

                    // Преобразуем FullTypeName в читаемый формат (namespace + преобразованное имя класса)
                    var cleanedFullTypeName = CleanFullTypeName(fullName);

                    var methodParameters = ParseMethodParametersWithNames(methodParams);

                    int? lineNumber = null;
                    if (lineNumberGroup.Success && int.TryParse(lineNumberGroup.Value, out int parsedLine))
                        lineNumber = parsedLine;

                    result.Add(new StackFrameInfo(className, methodName, lineNumber, cleanedFullTypeName, methodParameters));
                }
            }

            return result;
        }

        /// <summary>
        /// Извлекает имя класса из полного имени (Namespace.ClassName)
        /// Обрабатывает generic типы с обратными кавычками (например, ClassName`1 или ClassName`2[TParam])
        /// </summary>
        private static string ExtractClassName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            // Разделяем по точкам и берем последнюю часть
            var parts = fullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                var className = parts[parts.Length - 1];
                // Очищаем от generic параметров для более читаемого отображения
                return CleanGenericTypeName(className);
            }

            return CleanGenericTypeName(fullName);
        }

        /// <summary>
        /// Преобразует полное имя типа (Namespace.ClassName`1[TParam]) в читаемый формат (Namespace.ClassName&lt;TParam&gt;)
        /// Сохраняет namespace, преобразует только имя класса
        /// </summary>
        private static string CleanFullTypeName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return string.Empty;
            }

            // Разделяем по точкам
            var parts = fullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return CleanGenericTypeName(fullName);
            }

            // Преобразуем последнюю часть (имя класса) в читаемый формат
            var className = parts[parts.Length - 1];
            var cleanedClassName = CleanGenericTypeName(className);

            // Объединяем namespace с преобразованным именем класса
            if (parts.Length > 1)
            {
                var namespaceParts = parts.Take(parts.Length - 1);
                return string.Join(".", namespaceParts) + "." + cleanedClassName;
            }

            return cleanedClassName;
        }

        /// <summary>
        /// Преобразует имя типа из .NET формата в читаемый C# формат
        /// Преобразует: ClassName`1[UserAccount] -> ClassName&lt;UserAccount&gt;
        /// Преобразует: ClassName`1 -> ClassName&lt;T&gt; (только если параметры не указаны в стеке)
        /// Обрабатывает вложенные generic типы: ClassName`1[Dictionary`2[String, Int32]] -> ClassName&lt;Dictionary&lt;String, Int32&gt;&gt;
        /// </summary>
        private static string CleanGenericTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return string.Empty;
            }

            // Проверяем, есть ли обратная кавычка (признак generic типа)
            int backtickIndex = typeName.IndexOf('`');
            if (backtickIndex < 0)
            {
                // Не generic тип, возвращаем как есть
                return typeName;
            }

            // Извлекаем базовое имя типа (до обратной кавычки)
            string baseName = typeName.Substring(0, backtickIndex);

            // Ищем параметры generic типа в квадратных скобках
            int bracketStart = typeName.IndexOf('[', backtickIndex);
            if (bracketStart < 0)
            {
                // Нет параметров в стеке вызовов, возвращаем базовое имя с placeholder
                // Извлекаем количество параметров из `N
                string genericCountStr = "";
                for (int i = backtickIndex + 1; i < typeName.Length && char.IsDigit(typeName[i]); i++)
                {
                    genericCountStr += typeName[i];
                }
                if (int.TryParse(genericCountStr, out int genericCount))
                {
                    // Создаем placeholder для параметров: T, T1, T2, ...
                    var placeholders = string.Join(", ", Enumerable.Range(0, genericCount).Select(i => i == 0 ? "T" : $"T{i + 1}"));
                    return $"{baseName}<{placeholders}>";
                }
                return baseName;
            }

            // Извлекаем параметры из квадратных скобок, обрабатывая вложенные скобки
            string genericParams = ExtractGenericParameters(typeName, bracketStart);

            if (string.IsNullOrWhiteSpace(genericParams))
            {
                // Параметры пустые, используем placeholder
                string genericCountStr = "";
                for (int i = backtickIndex + 1; i < bracketStart && char.IsDigit(typeName[i]); i++)
                {
                    genericCountStr += typeName[i];
                }
                if (int.TryParse(genericCountStr, out int genericCount))
                {
                    var placeholders = string.Join(", ", Enumerable.Range(0, genericCount).Select(i => i == 0 ? "T" : $"T{i + 1}"));
                    return $"{baseName}<{placeholders}>";
                }
                return baseName;
            }

            // Преобразуем параметры: Dictionary`2[String, Int32] -> Dictionary<String, Int32>
            // UserAccount -> UserAccount (без изменений)
            genericParams = ConvertGenericParametersToCSharpFormat(genericParams);

            return $"{baseName}<{genericParams}>";
        }

        /// <summary>
        /// Извлекает параметры generic типа из квадратных скобок, обрабатывая вложенные скобки
        /// </summary>
        private static string ExtractGenericParameters(string typeName, int startIndex)
        {
            int bracketCount = 0;
            int endIndex = startIndex;

            for (int i = startIndex; i < typeName.Length; i++)
            {
                if (typeName[i] == '[')
                {
                    bracketCount++;
                }
                else if (typeName[i] == ']')
                {
                    bracketCount--;
                    if (bracketCount == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            if (endIndex > startIndex)
            {
                // Извлекаем содержимое скобок (без самих скобок)
                return typeName.Substring(startIndex + 1, endIndex - startIndex - 1);
            }

            return string.Empty;
        }

        /// <summary>
        /// Преобразует параметры generic типа в формат C# (заменяет `N на < >)
        /// Использует конкретные типы из стека вызовов, если они указаны
        /// </summary>
        private static string ConvertGenericParametersToCSharpFormat(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return string.Empty;
            }

            // Разделяем параметры по запятым, учитывая вложенные generic типы
            var paramList = SplitGenericParameters(parameters);
            var convertedParams = new List<string>();

            foreach (var param in paramList)
            {
                var trimmed = param.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                // Проверяем, является ли параметр generic типом
                int backtickIndex = trimmed.IndexOf('`');
                if (backtickIndex >= 0)
                {
                    // Это generic тип, рекурсивно обрабатываем
                    string baseName = trimmed.Substring(0, backtickIndex);
                    int bracketStart = trimmed.IndexOf('[', backtickIndex);

                    if (bracketStart >= 0)
                    {
                        // Есть параметры в квадратных скобках - извлекаем и преобразуем
                        string nestedParams = ExtractGenericParameters(trimmed, bracketStart);
                        if (!string.IsNullOrWhiteSpace(nestedParams))
                        {
                            nestedParams = ConvertGenericParametersToCSharpFormat(nestedParams);
                            convertedParams.Add($"{baseName}<{nestedParams}>");
                        }
                        else
                        {
                            // Параметры пустые, используем placeholder
                            string genericCountStr = "";
                            for (int i = backtickIndex + 1; i < bracketStart && char.IsDigit(trimmed[i]); i++)
                            {
                                genericCountStr += trimmed[i];
                            }
                            if (int.TryParse(genericCountStr, out int genericCount))
                            {
                                var placeholders = string.Join(", ", Enumerable.Range(0, genericCount).Select(i => i == 0 ? "T" : $"T{i + 1}"));
                                convertedParams.Add($"{baseName}<{placeholders}>");
                            }
                            else
                            {
                                convertedParams.Add(baseName);
                            }
                        }
                    }
                    else
                    {
                        // Нет параметров в квадратных скобках, используем placeholder
                        string genericCountStr = "";
                        for (int i = backtickIndex + 1; i < trimmed.Length && char.IsDigit(trimmed[i]); i++)
                        {
                            genericCountStr += trimmed[i];
                        }
                        if (int.TryParse(genericCountStr, out int genericCount))
                        {
                            var placeholders = string.Join(", ", Enumerable.Range(0, genericCount).Select(i => i == 0 ? "T" : $"T{i + 1}"));
                            convertedParams.Add($"{baseName}<{placeholders}>");
                        }
                        else
                        {
                            convertedParams.Add(baseName);
                        }
                    }
                }
                else
                {
                    // Обычный тип (не generic), возвращаем как есть
                    // Это может быть UserAccount, String, Int32 и т.д.
                    convertedParams.Add(trimmed);
                }
            }

            return string.Join(", ", convertedParams);
        }

        /// <summary>
        /// Разделяет параметры generic типа по запятым, учитывая вложенные generic типы
        /// </summary>
        private static List<string> SplitGenericParameters(string parameters)
        {
            var result = new List<string>();
            int bracketCount = 0;
            int startIndex = 0;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == '[')
                {
                    bracketCount++;
                }
                else if (parameters[i] == ']')
                {
                    bracketCount--;
                }
                else if (parameters[i] == ',' && bracketCount == 0)
                {
                    // Запятая на верхнем уровне - разделитель параметров
                    result.Add(parameters.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }

            // Добавляем последний параметр
            if (startIndex < parameters.Length)
            {
                result.Add(parameters.Substring(startIndex));
            }

            return result;
        }

        /// <summary>
        /// Парсит параметры метода из строки стека вызовов с именами и типами
        /// Формат: "Int32 param1, String param2" -> MethodParameterInfo[]
        /// </summary>
        private static List<MethodParameterInfo> ParseMethodParametersWithNames(string parametersString)
        {
            var result = new List<MethodParameterInfo>();

            if (string.IsNullOrWhiteSpace(parametersString))
            {
                return result;
            }

            // Разделяем параметры по запятым, учитывая вложенные generic типы
            var paramList = SplitGenericParameters(parametersString);

            foreach (var param in paramList)
            {
                var trimmed = param.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                // Параметр может быть в формате: "Int32 param1" или просто "Int32"
                var spaceIndex = trimmed.IndexOf(' ');
                string typePart;
                string namePart = string.Empty;

                if (spaceIndex > 0)
                {
                    // Есть имя параметра
                    typePart = trimmed.Substring(0, spaceIndex);
                    namePart = trimmed.Substring(spaceIndex + 1).Trim();
                }
                else
                {
                    // Нет имени параметра, только тип
                    typePart = trimmed;
                    namePart = $"param{result.Count + 1}"; // Генерируем имя по умолчанию
                }

                // Преобразуем тип в читаемый формат (обрабатываем generic типы)
                var cleanedType = CleanGenericTypeName(typePart);

                // Значения параметров в стеке вызовов обычно недоступны
                // Но можно попытаться извлечь, если они есть в формате "param = value"
                string value = string.Empty;
                var equalsIndex = namePart.IndexOf('=');
                if (equalsIndex > 0)
                {
                    value = namePart.Substring(equalsIndex + 1).Trim();
                    namePart = namePart.Substring(0, equalsIndex).Trim();
                }

                result.Add(new MethodParameterInfo(namePart, cleanedType, value));
            }

            return result;
        }

        /// <summary>
        /// Получает информацию о текущем методе вызова
        /// </summary>
        /// <returns>Объект StackFrameInfo для текущего метода</returns>
        public static StackFrameInfo GetCurrentMethod()
        {
            var stackTrace = AnalyzeStackTrace();
            // Пропускаем первый элемент (это сам метод GetCurrentMethod) и берем второй (вызывающий метод)
            if (stackTrace.Count > 1)
            {
                return stackTrace[1];
            }
            return stackTrace.FirstOrDefault() ?? new StackFrameInfo();
        }

        /// <summary>
        /// Получает информацию о вызывающем методе (на уровень выше текущего)
        /// </summary>
        /// <returns>Объект StackFrameInfo для вызывающего метода</returns>
        public static StackFrameInfo GetCallerMethod()
        {
            var stackTrace = AnalyzeStackTrace();
            // Пропускаем первые два элемента (GetCallerMethod и вызывающий метод) и берем третий
            if (stackTrace.Count > 2)
            {
                return stackTrace[2];
            }
            return stackTrace.Count > 1 ? stackTrace[1] : stackTrace.FirstOrDefault() ?? new StackFrameInfo();
        }

        /// <summary>
        /// Получает полный стек вызовов в виде форматированной строки
        /// </summary>
        /// <param name="stackTrace">Стек вызовов. Если null, используется Environment.StackTrace</param>
        /// <returns>Форматированная строка со стеком вызовов</returns>
        public static string GetFormattedStackTrace(string stackTrace = null)
        {
            var calls = AnalyzeStackTrace(stackTrace);
            if (calls.Count == 0)
            {
                return "Стек вызовов пуст";
            }

            var lines = new List<string>();
            for (int i = 0; i < calls.Count; i++)
            {
                var frameInfo = calls[i];
                var lineInfo = frameInfo.LineNumber.HasValue ? $" (строка {frameInfo.LineNumber.Value})" : "";
                lines.Add($"{i + 1}. {frameInfo.ClassName}.{frameInfo.MethodName}{lineInfo}");
            }

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Фильтрует стек вызовов, оставляя только вызовы из указанных классов
        /// </summary>
        /// <param name="classNames">Имена классов для фильтрации</param>
        /// <param name="stackTrace">Стек вызовов. Если null, используется Environment.StackTrace</param>
        /// <returns>Отфильтрованный список объектов StackFrameInfo</returns>
        public static List<StackFrameInfo> FilterByClassNames(
            IEnumerable<string> classNames,
            string stackTrace = null)
        {
            var allCalls = AnalyzeStackTrace(stackTrace);
            var classNamesSet = new HashSet<string>(classNames, StringComparer.OrdinalIgnoreCase);

            return allCalls
                .Where(call => classNamesSet.Contains(call.ClassName))
                .ToList();
        }

        /// <summary>
        /// Фильтрует стек вызовов, оставляя только вызовы из указанных методов
        /// </summary>
        /// <param name="methodNames">Имена методов для фильтрации</param>
        /// <param name="stackTrace">Стек вызовов. Если null, используется Environment.StackTrace</param>
        /// <returns>Отфильтрованный список объектов StackFrameInfo</returns>
        public static List<StackFrameInfo> FilterByMethodNames(
            IEnumerable<string> methodNames,
            string stackTrace = null)
        {
            var allCalls = AnalyzeStackTrace(stackTrace);
            var methodNamesSet = new HashSet<string>(methodNames, StringComparer.OrdinalIgnoreCase);

            return allCalls
                .Where(call => methodNamesSet.Contains(call.MethodName))
                .ToList();
        }
    }
}

