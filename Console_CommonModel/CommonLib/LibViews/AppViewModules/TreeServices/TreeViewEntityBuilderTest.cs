using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices;

using pickpoint_delivery_service;


public class TreeViewEntityBuilderTest: TestingElement
{
    public TreeViewEntityBuilderTest(): base(AppProviderService.GetInstance())
    {
    }

    public override void OnTest()
    {
        //DeliveryDbContextInitiallizer.InitDatabase();
        TreeViewEntityBuilder<DeliveryDbContext> service = new();
        service.Join<MessageProtocol>( );
        service.Join<MessageProperty>();
        var view = service.Build();
        view.DesignMode = true;
        view.RunAtForm();
    }
}