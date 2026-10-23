
public interface IBusinessDbContextInitiallizer<TBusinessDbContext>
    where TBusinessDbContext: BusinessDbContext
{
    IEnumerable<BusinessResource> GetItDepartmentRoles();
    int InitBusinessData(TBusinessDbContext db);
    int InitBusinessDatasets(TBusinessDbContext context);
    int InitBusinessDatasources(TBusinessDbContext context);
    int InitBusinessFunctions(TBusinessDbContext db);
    int InitBusinessIndicators(TBusinessDbContext db);
    int InitBusinessOLAP(TBusinessDbContext db);
    int InitBusinessReports(TBusinessDbContext db);
    int InitBusinessResource(TBusinessDbContext context);
    int InitBusinessResources(TBusinessDbContext db);
    int InitGranularities(TBusinessDbContext db);
    void InitMessageAttributes(TBusinessDbContext db);
    int InitOdbcBusinessDatasources(TBusinessDbContext db);
    int InitRatingBusinessDatasets(TBusinessDbContext db);
}