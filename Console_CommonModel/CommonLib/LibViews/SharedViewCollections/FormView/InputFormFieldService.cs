using System.Collections;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputComponents;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.CheckboxModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.Others;


namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.FormView
{

    [Description("Создаёт представления для управления вводом")]
    public class InputFormFieldService : FormFieldFactory
    {

        [Label("СОздание управляющего элемента")]
        public AppViewBase CreateControl(InputFormField inputFormField)
        {
            this.Info($"{inputFormField.Name}:{inputFormField.DataType}:{inputFormField.Type}");
            if (inputFormField.Type.ToLower().StartsWith("structurecollection"))
            {
                inputFormField.Control = CreateStructuredCollection(inputFormField);
            }
            else if (inputFormField.Type.ToLower().StartsWith("interfacecollection"))
            {
                inputFormField.Control = CreateInterfaceCollection(inputFormField);
            }
            else if (inputFormField.Type.ToLower().StartsWith("constructor"))
            {
                inputFormField.Control = CreateConstructor(inputFormField);
            }
            else if (inputFormField.Type.ToLower().StartsWith("builder"))
            {
                inputFormField.Control = CreateBuilder(inputFormField);
            }
            else if (inputFormField.Type.ToLower().StartsWith("search"))
            {
                inputFormField.Control = CreateSearch(inputFormField);
                ((AppViewBase)inputFormField.Control).ShowDialog();
            }
            else switch (inputFormField.Type.ToLower())
                {
                    case "eventexpressions":
                        inputFormField.Control = CreateEventExpressionsControl(inputFormField);
                        break;
                    case "form":
                    case "object":
                        inputFormField.Control = CreateForm(inputFormField);
                        break;
                    case "entitytype":
                    case "inputtype":
                        inputFormField.Control = CreateSelect(inputFormField);
                        break;
                    case "select":
                        inputFormField.Control = CreateSelect(inputFormField);
                        break;
                    case "enum":
                    case "selectenum":
                        inputFormField.Control = CreateSelectEnum(inputFormField);
                        break;
                    case "selectdictionary":
                        inputFormField.Control = CreateSelectDictionary(inputFormField);
                        break;
                    case "singleselectapi":
                        inputFormField.Control = CreateSingleSelectApi(inputFormField);
                        break;
                    case "selectexpression":
                        inputFormField.Control = CreateSelectExpression(inputFormField);
                        break;
                    case "collectiondictionary":
                        inputFormField.Control = CreateCollectionDictionary(inputFormField);
                        break;
                    case "dictionary":
                        inputFormField.Control = CreateDictionary(inputFormField);
                        break;
                    case "structurecollection":
                        inputFormField.Control = CreateStructuredCollection(inputFormField);
                        break;
                    case "structuredcollection":
                        inputFormField.Control = CreateStructuredCollection(inputFormField);
                        break;
                    case "primitivecollection":
                        inputFormField.Control = CreatePrimitiveCollection(inputFormField);
                        break;
                    case "image":
                        inputFormField.Control = CreateImage(inputFormField);
                        break;
                    case "engword":
                        inputFormField.Control = CreateEngword(inputFormField);
                        break;
                    case "engtext":
                        inputFormField.Control = CreateEngtext(inputFormField);
                        break;
                    case "rustext":
                        inputFormField.Control = CreateRustext(inputFormField);
                        break;
                    case "rusword":
                        inputFormField.Control = CreateRusword(inputFormField);
                        break;
                    case "year":
                        inputFormField.Control = CreateYear(inputFormField);
                        break;
                    case "week":
                        inputFormField.Control = CreateMonth(inputFormField);
                        break;
                    case "month":
                        inputFormField.Control = CreateMonth(inputFormField);
                        break;
                    case "phone":
                        inputFormField.Control = CreatePhone(inputFormField);
                        break;
                    case "email":
                        inputFormField.Control = CreateEmail(inputFormField);
                        break;
                    case "combobox":
                        inputFormField.Control = CreateCombobox(inputFormField);
                        break;
                    case "time":
                    case "date":
                        inputFormField.Control = CreateDate(inputFormField);
                        break;
                    case "datetime":
                        inputFormField.Control = CreateDateTime(inputFormField);
                        break;
                    case "multilinetext":
                        inputFormField.Control = CreateMultilineText(inputFormField);
                        break;
                    case "postalcode":
                    case "text":
                        inputFormField.Control = CreateText(inputFormField);
                        break;
                    case "url":
                        inputFormField.Control = CreateUrl(inputFormField);
                        break;
                    case "password":
                        inputFormField.Control = CreatePassword(inputFormField);
                        break;
                    case "icon":
                        inputFormField.Control = CreateIcon(inputFormField);
                        break;
                    case "int":
                        inputFormField.Control = CreateNumber(inputFormField);
                        break;
                    case "percent":
                        inputFormField.Control = CreatePercent(inputFormField);
                        break;
                    case "currency":
                        inputFormField.Control = CreateMoney(inputFormField);
                        break;
                    case "creditcard":
                        inputFormField.Control = CreateInputCreditCard(inputFormField);
                        break;
                    case "money":

                    case nameof(Int32):
                    case "number":
                        inputFormField.Control = CreateNumber(inputFormField);
                        break;
                    case "positive":
                    case "natural":
                    case "positiveint":
                        inputFormField.Control = CreateNatural(inputFormField);
                        break;

                        inputFormField.Control = CreatePositiveInt(inputFormField);
                        break;
                    case "bool":
                    case "boolean":
                    case "logic":
                    case "checkbox":
                        inputFormField.Control = CreateBool(inputFormField);
                        break;
                    case "custom":
                        inputFormField.Control = CreateCustomForm(inputFormField);
                        break;
                    case "color":
                        inputFormField.Control = CreateColor(inputFormField);
                        break;
                    case "audio":
                        inputFormField.Control = CreateAudio(inputFormField);
                        break;
                    case "video":
                        inputFormField.Control = CreateVideo(inputFormField);
                        break;
                    case "camera":
                        inputFormField.Control = CreateCamera(inputFormField);
                        break;
                    case "signalrvideo":
                    case "signalr_video":
                        inputFormField.Control = CreateSignalRVideo(inputFormField);
                        break;
                    case "xml":
                        inputFormField.Control = CreateXml(inputFormField);
                        break;
                    case "upload":
                    case "file":
                    case "filedata":
                        inputFormField.Control = CreateUpload(inputFormField);
                        break;
                    case "single":
                    case "double":
                    case "float":
                    case "decimal":
                        inputFormField.Control = CreateDecimal(inputFormField);
                        break;
                    case "duration":
                        inputFormField.Control = CreateText(inputFormField);
                        break;
                    case "filepath":
                    case "file_path":
                        inputFormField.Control = CreateInputFilePath(inputFormField);
                        break;
                    case "tcpipaddress":
                        inputFormField.Control = CreateInputTcpIpAddress(inputFormField);
                        break;
                    case "directory":
                    case "fileitem":
                    case "punctuation":
                    case "confirmation":
                        inputFormField.Control = CreateText(inputFormField);
                        break;

                    case "type":
                        inputFormField.Control = CreateInputType(inputFormField);
                        break;
                    case "controltype":
                        inputFormField.Control = CreateSelect(inputFormField);
                        break;

                    default:
                        throw new NotImplementedException(
                        $"Не реализована функция создания элемента управдления для {inputFormField.Type}");

                }
            if (inputFormField.Control is not null)
            {
                inputFormField.Control.Parent = inputFormField;
                ApplyDefaultControlMinHeight(inputFormField);
            }
            return inputFormField.Control;
        }

        private AppViewBase CreateInputCreditCard(InputFormField inputFormField)
        {
            return CreateCreditCard(inputFormField);
        }

        private AppViewBase CreateInputTcpIpAddress(InputFormField inputFormField)
        {
            return CreateTcpIpAddress(inputFormField);
        }

        private AppViewBase CreateInputFilePath(InputFormField inputFormField)
        {
            return CreateFilePath(inputFormField);
        }



        /// <summary>
        /// Для простых полей — компактная высота; для коллекций и таблиц сохраняется рассчитанный размер.
        /// </summary>
        private static void ApplyDefaultControlMinHeight(InputFormField inputFormField)
        {
            if (IsTallFieldType(inputFormField.Type))
                return;

            var min = Math.Max(inputFormField.Control.MinHeight, 55);
            inputFormField.Control.MinHeight = min;
            if (inputFormField.Height < min + 52)
            {
                inputFormField.Height = min + 52;
                inputFormField.MinHeight = inputFormField.Height;
            }
        }

        private static bool IsTallFieldType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return false;

            type = type.ToLowerInvariant();
            return type is "structuredcollection" or "structurecollection" or "primitivecollection"
                or "collectiondictionary" or "dictionary" or "multilinetext" or "xml" or "form" or "object"
                or "audio" or "video" or "camera" or "signalrvideo" or "signalr_video"
                || type.StartsWith("structurecollection")
                || type.StartsWith("interfacecollection")
                || type.StartsWith("search")
                || type.StartsWith("constructor");
        }

        private static AppViewBase CreateCustomForm(InputFormField inputFormField)
        {
            AppViewBase ViewModel = null;



            var pane = new AppViewBase();
            var checkbox = new CheckboxView();

            pane.LayoutTop(checkbox);

            inputFormField.Control = pane;
            checkbox.Text = inputFormField.Label;
            inputFormField.Label = "";
            AppViewBase card = null;
            int height = 0;

            pane.Controls.OnAdded((i, pnode) =>
            {
                var b = (AppViewBase)pnode;
                if (b.control is not null)
                {
                    pane.MinHeight += height = b.control.Height;
                }
                else
                {
                    pane.MinHeight += height = b.ToControl().Height;
                }
            });
            pane.Controls.OnRemoved((i, pnode) =>
            {
                pane.Height -= height;
                pane.MinHeight -= height;
            });
            EditView parent = null;
            bool completed = false;
            EditView pe = null;
            Task.Run(() =>
            {
                object p = null;
                if (inputFormField.Value is not null)
                {
                    p = inputFormField.Value;
                }
                else
                {
                    var ptype = inputFormField.DataType.ToType();
                    if (ptype.GetConstructors().Any(c => c.GetParameters().Count() == 0))
                    {
                        p = ptype.New();
                    }
                    else
                    {
                        throw new Exception($"Не удалось создать объект типа {ptype.GetTypeName()}");
                    }
                }
                pe = EditView.CreateEditViewBase(
                        inputFormField.Value is not null ? inputFormField.Value : inputFormField.DataType.ToType().New(),
                        (success, data) =>
                        {
                            parent.GoBack();
                            pane.MinHeight = 20;
                            checkbox.RunOnUiThread(() =>
                            {
                                //((CheckboxControl)checkbox.ToControl()).checkbox.Checked = false;
                            });
                        });
                completed = true;
            });
            checkbox.OnCheckedChanged = (s, e) =>
            {
                while (!completed)
                    Thread.Sleep(100);
                if (e.After)
                {
                    ViewModel = pe;
                    parent = pane.GetParent<EditView>();
                    parent.LayoutFill(ViewModel);
                    parent.Padding = 0;
                    pane.MinHeight = 200;
                }
                else
                {
                    //parent.Padding = 20;
                    ViewModel.RemoveFromParent();
                }
            };
            pane.MinHeight = 20;
            inputFormField.MinHeight = 20;
            return pane;
        }

        private AppViewBase CreateEventExpressionsControl(InputFormField inputFormField)
        {
            var view = (AppViewBase)inputFormField.Target;
            var expressions = view.EventsBindings.ContainsKey(inputFormField.Name) ?
                view.EventsBindings[inputFormField.Name] : new ChangingList<string>();
            inputFormField.Value = expressions;
            var primitiveCollection = new CollectionsView();
            primitiveCollection.OnAdded += (i, p) =>
            {
                this.Info(expressions);
                expressions.Add($"{p}");
            };
            primitiveCollection.OnValueChanged += (i, p) =>
            {
                this.Info(expressions);
                expressions[i] = $"{p}";
            };
            primitiveCollection.OnRemoved += (i, p) =>
            {
                expressions.RemoveAt(i);
            };
            return primitiveCollection;
        }

        private AppViewBase CreateSearch(InputFormField inputFormField)
        {
            inputFormField.MinHeight = 800;
            var attribute = inputFormField.GetInputAttribute();
            var entityType = attribute.Call("GetEntityType", new List<object>());
            var dbContextType = attribute.Call("GetDbContextType", new List<object>());
            var DbContextType = dbContextType.ToString().ToType();
            var EntityType = entityType.ToString().ToType();
            return SearchRepositoryView<DbContextBase, NamedObject>.CreateSearchRepository(DbContextType, EntityType, selected =>
            {

            });
        }

        /// <summary>
        /// Подтягивает актуальный словарь с модели (<see cref="InputFormField.Target"/>), чтобы поле и свойство не расходились.
        /// </summary>
        private static void SyncFieldDictionaryFromTarget(InputFormField view)
        {
            if (view?.Target == null || string.IsNullOrEmpty(view.Name))
                return;
            var prop = view.Target.GetType().GetProperty(view.Name);
            if (prop == null)
                return;
            var live = prop.GetValue(view.Target);
            if (live != null)
                view.Value = live;
        }

        /// <summary>
        /// Удаление ключа без <c>dynamic</c>: иначе DLR может выбрать <see cref="ICollection{T}.Remove"/> для
        /// <see cref="KeyValuePair{TKey,TValue}"/> вместо <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/>, исключение глотается — строка «возвращается».
        /// </summary>
        private static bool TryRemoveDictionaryKey(object dictionary, object key)
        {
            if (dictionary == null || key == null)
                return false;

            foreach (var iface in dictionary.GetType().GetInterfaces())
            {
                if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IDictionary<,>))
                    continue;
                var keyArg = iface.GetGenericArguments()[0];
                object keyForRemove = key;
                if (!keyArg.IsInstanceOfType(keyForRemove))
                {
                    try
                    {
                        keyForRemove = Convert.ChangeType(keyForRemove, Nullable.GetUnderlyingType(keyArg) ?? keyArg);
                    }
                    catch
                    {
                        continue;
                    }
                }
                var remove = iface.GetMethod("Remove", new[] { keyArg });
                if (remove == null)
                    continue;
                return (bool)remove.Invoke(dictionary, new[] { keyForRemove })!;
            }

            if (dictionary is IDictionary nd)
            {
                nd.Remove(key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Запись в словарь как <c>dict[k] = v</c> через индексатор <see cref="IDictionary{TKey,TValue}"/> (рефлексия).
        /// </summary>
        private static void SetDictionaryIndexerValue(object dictionary, object key, object value)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            static object Coerce(object obj, Type targetType)
            {
                if (obj == null)
                    return null;
                var t = Nullable.GetUnderlyingType(targetType) ?? targetType;
                if (t.IsInstanceOfType(obj))
                    return obj;
                return Convert.ChangeType(obj, t);
            }

            foreach (var iface in dictionary.GetType().GetInterfaces())
            {
                if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IDictionary<,>))
                    continue;
                var keyArg = iface.GetGenericArguments()[0];
                var valArg = iface.GetGenericArguments()[1];
                object keyForSet;
                try
                {
                    keyForSet = key == null ? null : Coerce(key, keyArg);
                }
                catch
                {
                    continue;
                }
                object valForSet;
                try
                {
                    valForSet = value == null && (!valArg.IsValueType || Nullable.GetUnderlyingType(valArg) != null)
                        ? null
                        : value == null
                            ? Activator.CreateInstance(Nullable.GetUnderlyingType(valArg) ?? valArg)
                            : Coerce(value, valArg);
                }
                catch
                {
                    continue;
                }
                var indexer = iface.GetProperty("Item", new[] { keyArg });
                if (indexer == null)
                    continue;
                indexer.SetValue(dictionary, valForSet, new[] { keyForSet });
                return;
            }

            if (dictionary is IDictionary nd)
            {
                nd[key] = value;
                return;
            }

            throw new InvalidOperationException(
                $"Тип {dictionary.GetType().GetTypeName()} не реализует IDictionary<,> и не является IDictionary.");
        }

        private AppViewBase CreateCollectionDictionary(InputFormField view)
        {
            var dictType = view.DataType.ToType();
            var idictIface = dictType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            if (idictIface is null && dictType != typeof(object))
                throw new ArgumentException(
                    $"Свойство «{view.Name}»: тип {view.DataType} не реализует IDictionary<,>.");


            var keyType = dictType == typeof(object) ? typeof(string) : idictIface.GetGenericArguments()[0];
            var valueType = dictType == typeof(object) ? typeof(string) : idictIface.GetGenericArguments()[1];

            if (view.Value is null)
                view.Value = dictType == typeof(object) ? new Dictionary<string, string>() : dictType.New();

            var root = new AppViewBase { MinHeight = 200 };
            var tools = new ToolsView();
            var table = new global::TableView
            {
                Title = string.IsNullOrWhiteSpace(view.Label) ? view.Name : view.Label,
                MinHeight = 170
            };
            table.VisibleColumns.Clear();
            table.VisibleColumns.AddRange(new[] { "Key", "Value" });
            table.Columns = new List<string> { "Key", "Value" };

            string FormatCell(object obj, Type t)
            {
                if (obj is null)
                    return "";
                if (t == typeof(string))
                    return (string)obj;
                var nt = Nullable.GetUnderlyingType(t) ?? t;
                if (nt.IsPrimitive || nt.IsEnum || nt == typeof(decimal))
                    return $"{obj}";
                try
                {
                    return obj.ToJson();
                }
                catch
                {
                    return $"{obj}";
                }
            }

            object ParseCell(string text, Type t)
            {
                if (t == typeof(string))
                    return text;
                var nt = Nullable.GetUnderlyingType(t) ?? t;
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (!nt.IsValueType)
                        return null;
                    if (Nullable.GetUnderlyingType(t) != null)
                        return null;
                    return Activator.CreateInstance(nt);
                }
                if (nt.IsEnum)
                    return Enum.Parse(nt, text, ignoreCase: true);
                try
                {
                    return Convert.ChangeType(text, nt);
                }
                catch
                {
                    return text.FromJson(t);
                }
            }

            void NotifyChange()
            {
                try
                {
                    view.OnInput(view.Value.ToJson());
                }
                catch
                {
                    view.OnInput($"{view.Value}");
                }
            }

            void ReloadFromDictionary()
            {
                SyncFieldDictionaryFromTarget(view);
                table.DataRows.Clear();
                foreach (dynamic e in (dynamic)view.Value)
                {
                    var ks = FormatCell(e.Key, keyType);
                    var vs = FormatCell(e.Value, valueType);
                    table.DataRows.Add(new KeyValuePair<string, string>(ks, vs));
                }
                if (table.DataRows.Count() == 0)
                {
                    table.DataRows.Add(new KeyValuePair<string, string>("", ""));
                }
            }

            ReloadFromDictionary();

            var deleteButton = new ToolsItemButtonView { Text = "удалить" };
            deleteButton.Enabled = false;
            table.DataRowsSelected.OnChanged += (_, __) =>
            {
                deleteButton.Enabled = table.DataRowsSelected.Count() > 0;
            };

            void OnAdd()
            {
                var keyBox = new TextBoxView();
                var valBox = new TextBoxView();
                var formHost = new AppViewBase();
                TextView keyText, valueText;
                formHost.LayoutTop(valBox);
                formHost.LayoutTop(valueText = new TextView { Text = "Значение" });
                formHost.LayoutTop(keyBox);
                formHost.LayoutTop(keyText = new TextView { Text = "Ключ" });


                var ok = new ButtonView { Text = "OK" };
                var cancel = new ButtonView { Text = "Отмена" };
                var buttons = new AppViewBase();
                buttons.LayoutLeft(ok);
                buttons.LayoutRight(cancel);
                formHost.LayoutBottom(buttons);

                dynamic f = null;
                var confirmed = false;
                ok.OnClick += (_, __) =>
                {
                    this.Info(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        key = keyBox.Text,
                        value = valBox.Text
                    }));
                    confirmed = true;
                    f?.Close();
                };
                cancel.OnClick += (_, __) => f?.Close();
                f = formHost.ToControl().RunAtForm(view.Label ?? "Пара ключ-значение");
                f.ShowDialog();
                if (!confirmed)
                    return;

                try
                {
                    SyncFieldDictionaryFromTarget(view);
                    var k = ParseCell(keyBox.Text, keyType);
                    var v = ParseCell(valBox.Text, valueType);
                    var idict = view.Value;
                    if ((bool)idict.Call("ContainsKey", new object[] { k }))
                    {

                    }
                    else
                    {
                        idict.Call("Add", new object[] { k, v });
                    }
                    ReloadFromDictionary();
                    NotifyChange();
                }
                catch (Exception ex)
                {
                    InfoDialog(view.Label ?? view.Name, ex.Message, "OK");
                }
            }

            void OnDelete()
            {
                SyncFieldDictionaryFromTarget(view);
                foreach (var row in table.DataRowsSelected.ToList())
                {
                    if (row is not KeyValuePair<string, string> kvp)
                        continue;
                    try
                    {
                        var k = ParseCell(kvp.Key, keyType);
                        TryRemoveDictionaryKey(view.Value, k);
                    }
                    catch
                    {
                        // пропуск строки при несовпадении типа ключа
                    }
                }
                this.Info(System.Text.Json.JsonSerializer.Serialize(view.Value));
                ReloadFromDictionary();
                NotifyChange();
                table.DataRowsSelected.Clear();
                deleteButton.Enabled = false;
                //if (table.control is DataTableControl dtc && !dtc.IsDisposed)
                //dtc.ClearRowSelection();
            }

            var addBtn = new ToolsItemButtonView { Text = "добавить" };
            addBtn.OnClick += (_, __) => OnAdd();
            addBtn.OnItemClicked += OnAdd;
            deleteButton.OnClick += (_, __) => OnDelete();
            deleteButton.OnItemClicked += OnDelete;
            tools.tools.Add(addBtn);
            tools.tools.Add(deleteButton);

            root.LayoutFill(table);
            root.LayoutTop(tools);
            return root;
        }

        private void InfoDialog(string v1, string message, string v2)
        {
            throw new NotImplementedException();
        }

        private AppViewBase CreateInputType(InputFormField inputFormField)
        {
            var combobox = new ComboboxView() { Text = inputFormField.Label };
            var inputTypes = ServiceFactory.Get()
                .GetTypesExtended<BaseInputAttribute>()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Select(t => t.GetTypeName())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            combobox.Options.AddRange(inputTypes);
            combobox.Value = inputTypes.First();
            var selected = $"{inputFormField.Value}";
            if (string.IsNullOrWhiteSpace(selected))
            {
                selected = inputFormField.Attributes.Keys
                    .FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
            }
            if (!string.IsNullOrWhiteSpace(selected) && inputTypes.Contains(selected))
            {
                combobox.Value = selected;
            }

            combobox.OnSelected += typeName =>
            {
                if (string.IsNullOrWhiteSpace(typeName))
                    return;
                inputFormField.Value = typeName;
                inputFormField.Attributes ??= new Dictionary<string, string>();
                foreach (var p in inputFormField.Attributes.Where(kv => kv.Key.ToType().IsExtends(typeof(BaseInputAttribute))))
                {
                    inputFormField.Attributes.Remove(p.Key);
                };
                inputFormField.Attributes[typeName] = "";
                inputFormField.OnInput?.Invoke(typeName);
            };

            return combobox;
        }

        /// <summary>
        /// Поле ввода «через конструктор»: выбор конкретного типа из наследников базового (как в <see cref="InputPanel"/> для <c>constructor&lt;T&gt;</c>),
        /// создание экземпляра и опционально редактирование через <see cref="EditView"/>.
        /// </summary>
        private AppViewBase CreateConstructor(InputFormField inputFormField)
        {
            const int rowHeight = 51;
            const int editorHeight = 300;
            const int fieldChrome = 52;

            var baseType = ResolveConstructorBaseType(inputFormField);
            if (baseType is null)
                throw new ArgumentException($"Для поля «{inputFormField.Name}» не удалось определить базовый тип для constructor (Type={inputFormField.Type}).");

            var extTypes = ServiceFactory.Get().GetTypesExtended(baseType)
                .Where(t => !t.IsAbstract && t.GetGenericArguments().Count() == 0)
                .ToList();
            var extNames = extTypes.Select(t => t.GetTypeName()).ToList();

            var root = new AppViewBase { AutoSize = false };
            var typeRow = new AppViewBase { AutoSize = false, MinHeight = rowHeight, Height = rowHeight };
            var editorHost = new AppViewBase { AutoSize = false, Visible = false, MinHeight = 0, Height = 0 };

            var combobox = new ComboboxView() { Text = inputFormField.Label };
            combobox.MinHeight = rowHeight;
            combobox.Height = rowHeight;
            combobox.AutoSize = false;
            foreach (var name in extNames)
                combobox.Options.Add(name);

            if (inputFormField.Value is not null)
            {
                var currentName = inputFormField.Value.GetType().GetTypeName();
                if (extNames.Contains(currentName))
                    combobox.Value = currentName;
            }

            EditView editor = null;

            void ApplyConstructorHeight()
            {
                var total = rowHeight + (editorHost.Visible ? editorHeight : 0);
                root.MinHeight = root.Height = total;
                typeRow.MinHeight = typeRow.Height = rowHeight;
                if (editorHost.Visible)
                    editorHost.MinHeight = editorHost.Height = editorHeight;
                inputFormField.AutoSize = false;
                inputFormField.MinHeight = inputFormField.Height = total + fieldChrome;
                //if (inputFormField.control is InputFormFieldControl fieldControl)
                //fieldControl.SyncContentHeight(inputFormField);
            }

            void ShowConstructorEditor(object instance)
            {
                if (editor is null)
                {
                    editor = EditView.CreateEditViewBase(instance, (success, model) =>
                    {
                        if (success && model is not null)
                        {
                            inputFormField.Value = model;
                            inputFormField.Target?.SetValue(inputFormField.Name, model);
                            try { inputFormField.OnInput?.Invoke(model.ToJson()); }
                            catch { inputFormField.OnInput?.Invoke($"{model.GetType().GetTypeName()}"); }
                        }
                    });
                    //editor.submit.ShowButtons = false;
                    editor.MinHeight = editorHeight;
                    editor.Height = editorHeight;
                    editor.Dock = "Fill";
                    editorHost.Controls.Clear();
                    editorHost.Controls.Add(editor);
                }
                else
                {
                    editor.BindEditControls(instance);
                }

                editorHost.Visible = true;
                ApplyConstructorHeight();
            }

            combobox.OnSelected += typeName =>
            {
                if (string.IsNullOrWhiteSpace(typeName))
                    return;

                try
                {
                    var instance = typeName.New();
                    if (instance is null)
                        return;
                    inputFormField.Value = instance;
                    inputFormField.Target?.SetValue(inputFormField.Name, instance);
                    ShowConstructorEditor(instance);
                }
                catch (Exception ex)
                {
                    inputFormField.Error($"Не удалось создать {typeName}: {ex.Message}");
                }
            };

            typeRow.Controls.Clear();
            combobox.Dock = "Fill";
            typeRow.Controls.Add(combobox);

            root.Controls.Clear();
            typeRow.Dock = "Top";
            editorHost.Dock = "Fill";
            root.Controls.Add(typeRow);
            root.Controls.Add(editorHost);
            ApplyConstructorHeight();

            if (inputFormField.Value is not null)
                ShowConstructorEditor(inputFormField.Value);

            return root;
        }

        /// <summary>
        /// Поле ввода «через конструктор»: выбор конкретного типа из наследников базового (как в <see cref="InputPanel"/> для <c>constructor&lt;T&gt;</c>),
        /// создание экземпляра и опционально редактирование через <see cref="EditView"/>.
        /// </summary>
        private AppViewBase CreateBuilder(InputFormField inputFormField)
        {
            Action<string> onresult = connectionString =>
            {

                inputFormField.Target.SetValue(inputFormField.Name, connectionString);

            };
            var view = new AppViewBase();
            var button = new ButtonView()
            {
                Text = "Редактировать",
                OnClick = (s, e) =>
                {
                    var attribute = inputFormField.GetInputAttribute().GetTypeName().ToType().New();
                    attribute.Call("BuildPropertyEditor", new List<object>() { inputFormField.Target, inputFormField.Name });
                    var view = new AppViewBase();
                }
            };
            view.LayoutFill(button);
            inputFormField.Control = view;
            return button;
        }

        private static Type ResolveConstructorBaseType(InputFormField inputFormField)
        {
            var t = inputFormField.Type ?? string.Empty;
            if (t.StartsWith("constructor", StringComparison.OrdinalIgnoreCase) && t.Contains('<') && t.Contains('>'))
            {
                var gentypeName = t.Substring(t.IndexOf("<", StringComparison.Ordinal) + 1,
                    t.IndexOf(">", StringComparison.Ordinal) - t.IndexOf("<", StringComparison.Ordinal) - 1);
                return gentypeName.ToType();
            }


            foreach (var kv in inputFormField.Attributes)
            {
                if (kv.Key.StartsWith("InputConstructorAttribute"))
                {
                    return kv.Key.ToType().GetGenericArguments()[0];
                }

            }


            return null;
        }

        private AppViewBase CreateMonth(InputFormField view)
        {
            var input = new InputDateTimeView();
            input.Mode = InputDateTimeViewMode.Month;
            input.Value = view.Value is null ? DateTime.Now : (DateTime)view.Value;
            input.OnValueChanged += (dt) => { if (dt is not null) view.OnInput(((DateTime)dt).ToString("yyyy-MM-dd")); };
            return input;
        }

        private AppViewBase CreateYear(InputFormField view)
        {
            var input = new InputDateTimeView();
            input.Mode = InputDateTimeViewMode.Year;
            input.Value = view.Value is null ? DateTime.Now : (DateTime)view.Value;
            input.OnValueChanged += (dt) => { if (dt is not null) view.OnInput(((DateTime)dt).ToString("yyyy-MM-dd")); };
            return input;
        }

        private AppViewBase CreateForm(InputFormField inputFormField)
        {
            return CreateObject(inputFormField);
        }

        private AppViewBase CreateRusword(InputFormField inputFormField)
        {
            return CreateText(inputFormField);
        }

        private AppViewBase CreateRustext(InputFormField inputFormField)
        {
            return CreateText(inputFormField);
        }

        private AppViewBase CreateEngtext(InputFormField inputFormField)
        {
            return CreateText(inputFormField);
        }

        private AppViewBase CreateEngword(InputFormField inputFormField)
        {
            return CreateText(inputFormField);
        }
    }
}
