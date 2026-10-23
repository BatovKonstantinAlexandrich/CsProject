/// <summary>
/// Модель представления "Выпадающий список"
/// </summary>
[Icon("apps")]
[Label("Меню с выпадающим списком")]
public class DropDownMenu: DropDown
{
    
    [Label("Links")]
    public List<ViewNode> Links { get; set; }
   
    public DropDownMenu() : base( )
    {
        Label = "eckumoc";
        //Links = new UserRolesService(new ApplicationDbContext()).GetUserRoleNavigation("Dev");
        Changed = false;
    }

    public DropDownMenu(string name, ILink item, TypeNode<ILink> parent) : base()
    {
        Label = name;
        Item = item;
        Changed = false;
    }
}

