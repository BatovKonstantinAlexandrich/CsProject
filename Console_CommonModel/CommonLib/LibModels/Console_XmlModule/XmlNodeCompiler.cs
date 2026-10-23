using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;



namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule;

[Label("XmlNodeCompiler: построение XML-узлов")]
[Description("Проверка API XmlNodeCompiler для создания и разбора XML-дерева компонентов.")]
public class XmlNodeCompilerTest : TestingElement
{
    public override void OnTest()
    {
        LogMessage("Тест XmlNodeCompiler не реализован (заглушка)");
        throw new NotImplementedException();
    }
}


/// <summary>
/// Компиляция XML в программный интерфейс
/// </summary>
public class XmlNodeCompiler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public static XmlNode<ComponentModel> CreateXmlNode(string tag, Dictionary<string, string> attributes)
    {
        var component = new ComponentModel();
        component.Parameters.Tag = tag;
        component.Parameters.Attrs = new(attributes);
        var pnode = new XmlNode<ComponentModel>(tag, component, null);
        return pnode;
    }

    public static XmlNode<ComponentModel> CreateXmlNodeForObject(object ptarget)
    {
        var tag = ptarget.GetTypeName().ToKebabStyle("-");
        var attrs = ptarget.ToTextDictionary();
        return CreateXmlNode(tag, attrs);
    }

    public static XmlNode<ComponentModel> CreateXmlNode(ViewItem view)
    {
        var p = view;
        var model = new ComponentModel();
        model.Parameters = new();
        model.Parameters.Tag = view.GetTypeName().ToKebabStyle("-");
        var xhildNodeProperties =
            view.GetProperties().Where(p => p.GetPropertyAttributes().ContainsKey(nameof(XmlChildNodeAttribute))).Select(p => p.Name).ToList();
        var kvs = view.DataState
            .Where(kv => !xhildNodeProperties.Contains(kv.Key))
            .Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value?.ToString())).ToList();
        model.Parameters.Attrs = new(kvs);




        var node = new XmlNode<ComponentModel>(view.GetTypeName(), model, null);
        foreach (var kv in view.DataState
            .Where(kv => xhildNodeProperties.Contains(kv.Key)))
        {
            var pkv = view.GetProperty(kv.Key);
            if (pkv is null)
            {
                continue;
            }
            var pnode = ((ViewItem)pkv).ToXmlNode();
            pnode.Item.Parameters.Attrs.Add("Name", kv.Key);
            //pnode.Item.Parameters.Attrs.Add("Type", pnode.Item.Parameters.Tag);
            //pnode.Item.Parameters.Tag = kv.Key;            
            node.Append(pnode);
        }

        var expectedProperties = xhildNodeProperties.Except(view.DataState.Keys);
        foreach (var prop in expectedProperties)
        {
            var pkv = view.GetProperty(prop);
            if (pkv is null)
            {
                continue;
            }

            if (pkv is ViewItem expectedChild)
            {
                var expectedNode = expectedChild.ToXmlNode();
                expectedNode.Item.Parameters.Attrs.Add("Name", prop);
                node.Append(expectedNode);
                continue;
            }

            if (pkv is System.Collections.IEnumerable expectedItems && pkv is not string)
            {
                var wrapperTag = prop.ToKebabStyle("-");
                var wrapperNode = new XmlNode<ComponentModel>(
                    wrapperTag,
                    new ComponentModel()
                    {
                        Parameters = new ComponentParams()
                        {
                            Tag = wrapperTag,
                            Attrs = new()
                        }
                    },
                    null);

                foreach (var expectedItem in expectedItems)
                {
                    if (expectedItem is ViewItem expectedItemView)
                        wrapperNode.Append(expectedItemView.ToXmlNode());
                }

                if (wrapperNode.HierElements.Count > 0)
                    node.Append(wrapperNode);
            }

        }

        foreach (ViewItem next in view.Controls)
        {
            var pchild = CreateXmlNode(next);
            node.Append(pchild);
        }
        return node;
    }

    public static string GetXml(ViewItem view)
    {
        return GetXml(CreateXmlNode(view));
    }

    public static string GetXml(ReportComponent view)
    {
        var root = new XmlNode<ComponentModel>("data-report", new ComponentModel()
        {
            Parameters = new()
            {
                Tag = "data-report"
            }
        }, null);
        var datasources = new XmlNode<ComponentModel>("data-sources", new ComponentModel()
        {
            Parameters = new()
            {
                Tag = "data-sources",
                Attrs = new()
                {

                }
            }
        }, root);

        foreach (var datasource in view.DataSources)
        {
            var ds = CreateXmlNodeForObject(datasource);
            datasources.Append(ds);
        }
        var datasets = new XmlNode<ComponentModel>("data-sets", new ComponentModel()
        {
            Parameters = new()
            {
                Tag = "data-sets",
                Attrs = new()
                {

                }
            }
        }, root);
        foreach (var dataset in view.DataSets)
        {
            var ds = CreateXmlNodeForObject(dataset);
            datasets.Append(ds);
        }
        var body = new XmlNode<ComponentModel>("body", new ComponentModel()
        {
            Parameters = new()
            {
                Tag = "body",
                Attrs = new()
                {

                }
            }
        }, root);
        foreach (ViewItem pview in view.Body.Controls)
        {
            var pchild = CreateXmlNode(pview);
            pchild.Parent = body;
        }
        return GetXml(root);
    }

    public static string GetXml(object target)
    {
        return GetXml(CreateXmlNodeForObject(target));
    }

    public static string GetXml(XmlNode<ComponentModel> view)
    {
        string xml = "";
        var level = view.GetLevel();
        foreach (var kv in view.HierElements)
        {
            xml += GetXml(kv.Value);
        }
        return view.Item.GetXml(xml, level);
    }


    public static ReportComponent CompileXmlFile(string reportXml)
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load(reportXml);

        // получим корневой элемент        
        XmlNode pnode = xDoc.DocumentElement;
        Console.WriteLine(pnode.OuterXml);
        var compiler = new XmlNodeCompiler();
        var root = compiler.Compile(pnode, null, true);
        var report = new ReportComponent(root);
        report.OnInit();
        return report;
    }

    public static ReportComponent CompileXml(string xml)
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(xml);

        // получим корневой элемент        
        XmlNode pnode = xDoc.DocumentElement;
        Console.WriteLine(pnode.OuterXml);
        var compiler = new XmlNodeCompiler();
        var root = compiler.Compile(pnode, null, true);
        var report = new ReportComponent(root);
        report.OnInit();
        return report;
    }

    private readonly IBlazorComponentFactory _componentFactory;
    private DataReport root;

    public XmlNodeCompiler() : this(new XmlComponentsFactory())
    {
    }

    public XmlNodeCompiler(IBlazorComponentFactory componentFactory)
    {

        _componentFactory = componentFactory;
    }


    public XmlNode<ComponentModel> Compile(string xml, bool isRoot = true)
    {
        using (var reader = new StringReader(xml))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(reader);
            var rootView = new AppViewBase();
            var rootNode = rootView.ToXmlNode();
            rootNode.Item.Target = rootView;
            // получим корневой элемент
            XmlNode pnode = xDoc.DocumentElement;
            return Compile(pnode, rootNode, isRoot);
        }
    }

    public bool EnableValidation { get; set; } = false;
    public XmlNode<ComponentModel> Compile(XmlNode pnode, XmlNode<ComponentModel> parent, bool isRoot)
    {
        if (isRoot == false)
        {
            if (pnode == null)
            {
                throw new ArgumentException("pnode");
            }
            if (parent == null)
            {
                throw new ArgumentException("parent");
            }
        }

        ComponentParams inputParams = ParseCompileParams(pnode);
        ComponentModel parentComponent = parent != null ? parent.Item : null;
        //this.Info("Компиляция "+inputParams.Tag+ " уровень "+ (parent is null? 0 :(parent.GetLevel()+1)));
        //this.Info(inputParams.Attrs);


        if (inputParams.Tag == "menu-view")
        {
            this.Info(inputParams.Attrs);
        }
        if (inputParams.Tag == "tree-view")
        {
            int x = 0;
        }



        XmlNode<ComponentModel> compiled = null;
        if (pnode != null)
        {
            if (isRoot)
            {
                root = null;
            }
            //ComponentParams inputParams = ParseCompileParams(pnode);
            //ComponentModel parentComponent = parent != null ? parent.Item: null;
            object targetComponent = _componentFactory.Create(inputParams, parentComponent, root);
            if (EnableValidation)
            {
                targetComponent.EnsureIsValide();
            }
            ComponentModel CurrentComponent = new ComponentModel()
            {
                Parameters = inputParams,

            };
            CurrentComponent.SetTarget(targetComponent);
            compiled = new XmlNode<ComponentModel>(pnode.GetHashCode() + "", CurrentComponent, parent);
            int level = compiled.GetLevel();
            for (int i = 0; i < level; i++)
            {
                Console.Write("   ");
            }
            Console.WriteLine(targetComponent.GetTypeName());
            if (isRoot)
            {
                root = (DataReport)CurrentComponent.GetTarget();
            }
            for (int i = 0; i < pnode.ChildNodes.Count; i++)
            {
                XmlNode node = pnode.ChildNodes.Item(i);
                //compiled.Append();
                Compile(node, compiled, false);
            }
        }

        return compiled;
    }



    private ComponentParams ParseCompileParams(XmlNode pnode)
    {
        string tag = pnode.Name;
        string log = pnode.Name;
        var attrs = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        if (pnode.Attributes != null)
        {
            for (int i = 0; i < pnode.Attributes.Count; i++)
            {
                attrs[ToCapitalStyle(pnode.Attributes.Item(i).Name)] = pnode.Attributes.Item(i).Value;
                log += " " + pnode.Attributes.Item(i).Name + "=" + pnode.Attributes.Item(i).Value;
            }
        }
        string text = "";
        if (pnode.ChildNodes.Count == 0)
        {
            text = pnode.InnerText;
        }
        return new ComponentParams()
        {
            Tag = tag,
            Attrs = new(attrs),
            Text = text
        };
    }

    private string ToCapitalStyle(string name) => name.ToCapitalStyle();
}
