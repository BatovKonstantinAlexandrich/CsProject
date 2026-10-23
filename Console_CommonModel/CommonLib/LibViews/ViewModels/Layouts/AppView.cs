using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;


/// <summary>
/// Основная форма приложения
/// </summary>
[Label("Основная форма приложения")]
[NotInput]
public class AppView : TypeView
{    
    [Label("Заголовок")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string Chapter { get => Get<string>("Chapter"); set => Set<string>("Chapter", value); }

    [Label("Верняя панель")]
    [InputObject]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public FlexContainer TopMenu = new FlexContainer();

    [Label("Панель инструментов")]
    [InputObject]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public FlexContainer TopToolbar = new FlexContainer();

    [Label("Нижняя панель")]
    [InputObject]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public FlexContainer BottomToolbox = new FlexContainer();

    [Label("Левая панель")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public TypeView LeftPane { get; set; } = new TypeView() { Dock = "Left" };

    [Label("Правая панель")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public TypeView RightPane { get; set; } = new TypeView() { Dock = "Right" };


    public AppView(): base()
    {                 
        /*Append(Left = LeftPane);
        Append(Right = RightPane);
        Append(Top = new Pane() {
            Dock = "Top",
            Item = TopMenu
        });
        Append(Bottom = new Pane() {
            Dock = "Bottom",
            Item = BottomToolbox
        });*/
        this.EnableChangeSupport = true;
        this.OnEvent+=(eventMessage)=>{
            if( eventMessage is PropertyChangedMessage ){
                if( ((PropertyChangedMessage)eventMessage).Property == "Item"){
                    //this.Info("Item changed");
                }
            }
        };
        EnableChangeSupport = true;
        Chapter = this.GetType().Name;
    }




    /// Окно настрйоки стиля элемента
    public void ShowPropertiesCustomizer()
    {
        FormViewModel form = GetPropertiesCustomizer();         
        LeftPane.Item = form;
        LeftPane.Changed = true;
    }

    private FormViewModel GetPropertiesCustomizer()
    {
        EnableChangeSupport = true;
        var ctrl = this;       
        var form = new FormViewModel();
        form.StructuresAreEditable = true;
        if(this.Item != null){
            form.Item = this.Item;
            form.Update(form.Item.GetType());
            form.Chapter = this.Item.GetType().GetLabel();
        }            
        form.Changed = true;
        LeftPane.Changed = true;                     
        form.EnableChangeSupport = true;
        //form.Bind("Item",ctrl,"Item");        
        return form;
    }


    /// Окно настройки параметров связывания с данными
    public void ShowDataBindingsCustomizer()
    {
        FormViewModel form = GetDataBindingsCustomizer();

        LeftPane.Item = form;
        LeftPane.Changed = true;
    }


    private FormViewModel GetDataBindingsCustomizer()
    {        
        var form = new FormViewModel();            
        form.StructuresAreEditable = true;
        if(this.Item!=null)  {
            form.Item = this.Item;
            form.Chapter = this.Item.GetType().Name;
            form.Chapter = typeof(ViewDataBuilder).GetLabel();
            form.Update(typeof(ViewDataBuilder));      
        }
            
        form.Changed = false;
        LeftPane.Changed = true;        
      
        return form;
    }





    /// Окно настрйоки стиля элемента
    public void ShowStyleCustomizer()
    {
        FormViewModel form = GetStyleCustomizer();

        LeftPane.Item = form;
        LeftPane.Changed = true;
    }


    private FormViewModel GetStyleCustomizer()
    {
     
        var form = new FormViewModel();            
        form.StructuresAreEditable = false;
        if(this.Item!=null)  {
            form.Item = this.Item;
            form.Chapter = this.Item.GetType().Name;          
        }
        form.Chapter = typeof(StyledItem).GetLabel();
        form.Update(typeof(StyledItem));
        form.Changed = false;
        LeftPane.Changed = true;
      
        return form;
    }




 


    private bool IsCollection()
    {
        return this.IsExtendedFrom("StructuredCollection");
    }
}