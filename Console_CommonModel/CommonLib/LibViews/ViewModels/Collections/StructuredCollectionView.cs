
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[Label("Модель представления коллекции элементов")]
public class StructuredCollectionView: AppView, IEditable
{
    
    [NotNullNotEmpty("Необходимо задать значение свойсва ItemType")]
    [Label("Type")]
    public Type Type { get; set; }
    
    [Label("Items List")]
    public List<ViewItem> ItemsList { get; set; } = new List<ViewItem>();


    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Create")]
    public Func<object, object> Create { get; set; }

    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Search Control")]
    public SearchProductView SearchControl { get; set; }



    public StructuredCollectionView()
    {        
        
    }


    /// <summary>
    /// Поисковая строка
    /// </summary>
    /// <returns></returns>
    public SearchProductView CreateSearch()
    {          
        if(SearchControl == null)
        {
            var search = new SearchProductView()
            {
                AtInput = (search) => {
                    var dataset = GetDatasetFromItems();
                    var results = MyExpression.Search(dataset, Type.Name, search.Query);
                    ItemsList.ForEach(p => {
                        bool visible = results.Contains(p.Dataset);
                        if (p.Visible != visible)
                        {
                            if (p.Selected)
                            {
                                p.Selected = false;
                            }
                            p.Visible = visible;
                            p.Changed = true;
                        }
                    });
                    return new
                    {
                        Query = search.Query,
                        Keywords = MyExpression.GetKeywords(dataset, Type.Name, search.Query)
                    };
                },
                AtSearch = (search) => {
                    var dataset = GetDatasetFromItems();
                    var results = MyExpression.Search(dataset, Type.Name, search.Query);
                    ItemsList.ForEach(p => {
                        bool visible = results.Contains(p.Dataset);
                        if( p.Visible != visible)
                        {
                            if (p.Selected)
                            {
                                p.Selected = false;
                            }
                            p.Visible = visible;
                            
                        }
                        p.Changed = true;
                    });
                    return new
                    {
                        Status = "Success"
                    };
                }
            };
            search.Changed = false;
            SearchControl = search;
        }        
        return SearchControl;
    }

  
    /// <summary>
    /// Извлекает элементы данных
    /// </summary>
    /// <returns></returns>
    public List<object> GetDatasetFromItems()
    {
        List<object> dataset = new List<object>();
        ItemsList.ForEach((p) => {
            dataset.Add(p.Dataset);
        });
        return dataset;
    }


    /// <summary>
    /// Присоединяет панель инструментов: "Редактирование коллекций"
    /// </summary>
    public void AddEditTools()
    {
        var ctrl = this;
        var createButton = new ButtonView()
        {
            Text = "Создать",
            Icon = "add",
            OnClick = (s, e) => {
                object item = ctrl.Create==null? ReflectionService.CreateWithDefaultConstructor<object>(Type): ctrl.Create(ctrl);
                var form = new FormViewModel(item, ReflectionService.GetPropertyNames(item.GetType()).ToArray());
                form.Edited = true;
                var createButton = new ButtonView()
                {
                     
                    Text = "Сохранить",
                    OnClick = (s, e) =>
                    {
                        var entity = ((BaseEntity)form.Item);
                        entity.Create();
                        ItemsList.Add(new ListItemView() { /*DataSet = entity*/ });
                        this.LeftPane.Clear();
                        this.Changed = true;                     
                        this.LeftPane.Changed = true;
                    }
                };
                createButton.Enabled = false;
                createButton.Bind("Enabled", form, "IsValid");
                form.Buttons.Add(createButton);
                this.LeftPane.Item = form;
                this.LeftPane.Changed = true;
            }
        };
        createButton.OnClick += (s, e) =>
        {
            createButton.Enabled = createButton.Enabled ? false : true;
        };



        //кнопка редактирования
        ButtonView editButton = null;
        editButton = new ButtonView()
        {
            Text = "Редактировать",
            Icon = "edit",
            OnClick = (s, e) => {

                var selectedListItem = this.SelectionModel.FirstOrDefault();
                if (selectedListItem == null)
                {
                    throw new Exception("Не найден выбранный элемент");
                }
                var item = selectedListItem.Dataset;
                //object item = 
                var form = new FormViewModel(item, ReflectionService.GetPropertyNames(item.GetType()).ToArray());
                var createButton = new ButtonView()
                {
                    Text = "Сохранить",
                    OnClick = (s, e) =>
                    {
                        var entity = ((BaseEntity)form.Item);
                        entity.Update();
                        this.LeftPane.Clear();
                        this.Changed = true;
                    }
                };
                var closeButton = new ButtonView()
                {
                    Text = "Закрыть",
                    OnClick = (s, e) =>
                    {
                        this.LeftPane.Clear();
                        this.Changed = true;
                    }
                };
                createButton.Enabled = false;
                createButton.Bind("Enabled", form, "IsValid");
                form.Buttons.Add(createButton);
                form.Buttons.Add(closeButton);
                this.LeftPane.Item = form;
                this.LeftPane.Changed = true;
            }
        };

        this.OnEvent += (message) =>
        {
            if (message is PropertyChangedMessage)
            {
                PropertyChangedMessage propertyChangedMessage = (PropertyChangedMessage)message;
                if (propertyChangedMessage.Property == "Selected")
                {
                    if (this.LeftPane.Clear() != null)
                    {
                        this.Changed = true;
                    }
                }
            }
        };
        this.OnEvent += (message) =>
        {
            if (message is PropertyChangedMessage)
            {
                PropertyChangedMessage propertyChangedMessage = (PropertyChangedMessage)message;
                if (propertyChangedMessage.Property == "Selected")
                {
                    editButton.Enabled = (this.SelectionModel.Count() == 1);
                }
            }
        };
        editButton.Enabled = false;

        ButtonView deleteButton = null;
        deleteButton = new ButtonView()
        {
            Text = "Удалить",
            Icon = "remove",
            OnClick = (s, e) => {
                foreach (var listitem in this.SelectionModel.ToList())
                {
                    BaseEntity entity = ((BaseEntity)listitem.Dataset);
                    ItemsList.Remove(listitem);
                    this.SelectionModel.Remove(listitem);
                    entity.Delete();
                }
                editButton.Enabled = deleteButton.Enabled = false;
                ctrl.Changed = true;
            }
        };
        deleteButton.Enabled = true;
        this.OnEvent += (message) =>
        {
            if (message is PropertyChangedMessage)
            {
                PropertyChangedMessage propertyChangedMessage = (PropertyChangedMessage)message;
                if (propertyChangedMessage.Property == "Selected")
                {
                    deleteButton.Enabled = (this.SelectionModel.Count() > 0);
                }
            }
        };
        createButton.FontSize = 14;
        editButton.FontSize = 14;
        deleteButton.FontSize = 14;

        deleteButton.Enabled = false;

        createButton.OnClick(createButton, new EventArgs());
        ((ViewItem)(this.LeftPane)).Visible = false;
        this.TopMenu.Append(createButton);
        this.TopMenu.Append(editButton);
        this.TopMenu.Append(deleteButton);
    }
}
