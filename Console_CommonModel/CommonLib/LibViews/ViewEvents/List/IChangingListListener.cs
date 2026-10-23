namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{
    public interface IChangingListListener<TItem>
    {
        public void OnAdded(TItem service);
        public void OnRemoved(TItem service);

    }









    /*
    public class ExntesibleControl<TControl>: ColumnsView where TControl: Control
    {

    }





    public class IExtensibleService<TControl> 
    {
        private readonly ChangingList _services = new();
    }


    public interface IExtensibleControl<TControl> 
    {
        public ChangingList<IExtensibleService<TControl>> Services { get; set; }
        
        public void OnInit(TControl ctrl);        
    }

    public class ExtensiblePanel: Panel, IExtensibleControl<Panel>
    {
        public ChangingList<IExtensibleService<Panel>> Services { get; set; }

        public ExtensiblePanel( )
        {
            this.OnInit(this);
        }

        public void OnAdded(IExtensibleService<Panel> service)
        {
            throw new NotImplementedException();
        }

        public void OnRemoved(IExtensibleService<Panel> service)
        {
            throw new NotImplementedException();
        }

        public void OnInit(Panel ctrl)
        {
            
            Services = new();
            
            
        }
    }*/
}
