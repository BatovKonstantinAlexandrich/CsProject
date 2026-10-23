
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


public class ReportDesigner: BaseService
{
 
    public List<TypeInfo> Controllers { get; set; }
    public List<TypeInfo> TagHelpers { get; set; }
    public List<TypeInfo> ViewComponents { get; set; }
    public ViewItemSet _tabs { get; set; }

    private readonly ApplicationPartManager _partManager;
    private readonly ApplicationDbContext _context;
    public Tree _root;
    public Tree _palette;
    public ViewItem _selected;
    private Form _designer;
    public Form _selector;
    public ViewOptions _customizer;
  

    public Form _datamanager { get; private set; }

    public ReportDesigner( ApplicationPartManager partManager, ApplicationDbContext context )
    {
        _partManager = partManager;



        _context = context;
        InitComponents();
        InitPalette(); 
    }

    public Tree InitComponents()
    {

        
        _selected = new Pane() { Item = new Button() };
        
        _designer = (Form)new Form(_selected) { Title = "Параметры представления" }.SetDesignMode();
        _selector = (Form)new Form(_selected) { Title = "Выбор компонента" }.SetDesignMode();
       // _datamanager = (Form)new Form(_datasource) { Title = "Набор данных" }.SetDesignMode();
        _customizer = new Form(((Pane)_selected).Item) { Title = "Свойства компонента" }.SetEditMode();
        ((ViewItem)_customizer).Bind("Item", _selected, "Item");
        _designer.Bind("Item", _selected, "Item");
        _selected.OnEvent += (message) =>
        {
            ((Form)_customizer).Item = ((Pane)_selected).Item;
            ((Form)_customizer).Update(ReflectionService.GetOwnPropertyNames(((Pane)_selected).Item.GetType()).ToArray());
            _customizer.Changed = true;
        };
        ((ViewItem)_customizer).AddPropertyChangeListener("Item",(e)=> {
            Writing.ToConsole("Свойства должны поменяться");
            ((Form)_customizer).PropertyNames = ReflectionService.GetOwnPropertyNames(e.After.GetType()).ToArray();
            _customizer.Changed = true;
        });
        _tabs = new ViewItemSet();
        _tabs.Items.Add(_datamanager);
        _tabs.Items.Add(_selector);
        _tabs.Items.Add((Form)_customizer);
        _root = new Tree()
        {
            Item = new NamedObject()
            {
                Name = "Новый отчёт",
                Item = _customizer
            }
        };
        return _root;
    }


    public object InitPalette()
    {
        _palette = new Tree()
        {
            Item = null
        };
        _palette.Item = new NamedObject()
        {
            Name = "Панель инструментов"
        };
        Init(_partManager );
        Init(_palette);
        return _palette;
    }


    private void Init(ApplicationPartManager _partManager)
    {
        var tagHelperFeature = new TagHelperFeature();
        _partManager.PopulateFeature(tagHelperFeature);
        TagHelpers = tagHelperFeature.TagHelpers.ToList();

        var viewComponentFeature = new ViewComponentFeature();
        _partManager.PopulateFeature(viewComponentFeature);
        ViewComponents = viewComponentFeature.ViewComponents.ToList();

       
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);
        Controllers = controllerFeature.Controllers.ToList();
         
    }


    public Tree GetViewComponents() {
        Tree root = new Tree() { };
        Tree vcs = new Tree("ViewComponentFeature") { };
        var vcf = new ViewComponentFeature();
        _partManager.PopulateFeature(vcf);
        foreach (var vc in vcf.ViewComponents.ToList())
        {
            var item = new NamedObject() {
                Name = vc.Name
            };
            new Tree(vcs, item);
        }
        Tree cs = new Tree("ControllerFeature") { };
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);
        foreach (var vc in controllerFeature.Controllers.ToList())
        {
            var item = new NamedObject() {
                Name = vc.Name
            };
            new Tree(cs, item);
        }
        vcs.SetParent(root);
        cs.SetParent(root);
        return root;
    }
    public string Init(Tree root)
    {
        var controllerFeature = new ViewComponentFeature();
        _partManager.PopulateFeature(controllerFeature);
        foreach(var vc in controllerFeature.ViewComponents.ToList())
        {
            var item = new NamedObject()
            {
                Name = vc.Name
            };
            new Tree(root, item);            
        }
        
        return "";
    }
   /* public string Init(Node root)
    {
         
      
        root.Item = new NamedObject()
        {
            Name = "Инструменты"

        };
        root.Children.Clear();


        var pctrl = new Node()
        {
            Parent = root,
            Title = "Контроллеры MVC"
        };
 
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);
        Controllers = controllerFeature.Controllers.ToList();
        Controllers.ForEach((ctrl) =>
        {
            new Node()
            {
                Parent = pctrl,
                Title = ctrl.Name
            };
            
        });



     
        var datasets = new Node()
        {
            Parent = root,
            Title = "Наборы данных"
        };
        var webpages = new Node()
        {
            Parent = root,
            Title = "Аналитические модели"
        };
     
        return "";
    }
 
 */
}

