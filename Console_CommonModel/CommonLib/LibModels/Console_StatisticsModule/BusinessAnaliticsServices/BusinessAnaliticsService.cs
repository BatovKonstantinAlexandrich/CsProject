using System.Data;
using System.Globalization;

/// <summary>
/// Служба доступа к аналитическим данным.
/// </summary>
public class BusinessAnaliticsService : BusinessContextAnaliticsService<BusinessDbContext>
{
    public BusinessAnaliticsService()
    {
    }

    public BusinessAnaliticsService(BusinessDbContext businessDataModel) : base(businessDataModel)
    {
    }

    public List<BusinessData> GetBusinessData(BusinessResource resource, List<BusinessIndicator> indicators)
    {
        return this.businessDataModel.BusinessData.Where(data => data.BusinessResourceID == resource.Id && indicators.Select(i=>i.Id).Contains(data.BusinessIndicatorID)).ToList();
    }

    public List<DateTime> GetBeginDates(int resourceId, int granularityId, List<int> indicatorIds)
    {
        var bas = this;
        var indicators = indicatorIds.Select(i => bas.GetIndicatorById(i)).ToList();
        var resource = bas.GetResourceById(resourceId);
        var data = bas.GetBusinessData(resource, indicators);
        var dates = data.Where(d => d.GranularityID == granularityId).Select(d => d.BeginDate).ToList();
        return dates;
    }
}








public class BusinessContextAnaliticsService<TBusinessContext> 
    where TBusinessContext: BusinessDbContext
{
    protected readonly BusinessDbContext businessDataModel;

    public BusinessContextAnaliticsService( ): this(new BusinessDbContext()){}
    public BusinessContextAnaliticsService(BusinessDbContext businessDataModel)
    {
        this.businessDataModel = businessDataModel;
    }



    public IEnumerable<TModel> Get<TModel>() where TModel : class                
        => this.businessDataModel.Get<TModel>();
    public IEnumerable<BusinessDatasource> GetDatasources() => Get<BusinessDatasource>();

    public BusinessIndicator GetIndicatorById(int id)
        => this.businessDataModel.BusinessIndicators.FirstOrDefault(i => i.Id == id);
    public BusinessIndicator GetIndicatorByName(string name)
        => this.businessDataModel.BusinessIndicators.FirstOrDefault(i => i.Name.ToLower() == name.ToLower());   
    public IEnumerable<BusinessDataset> GetDatasets() 
        => businessDataModel.BusinessDatasets.ToList();





    [Label("Получение набора для линейной диаграммы." +
        "Со статистическими показателями")]    
    public Dictionary<string, List<float?>> GetLineSeriesData(int resourceId, int datasetId, string beginDate, int granularity)
    {
        Dictionary<string, List<float?>> result = new();
            
        var dataset = GetDatasetValues(resourceId,datasetId,beginDate,granularity);
        var indicator_ids = dataset.Select(p => p.BusinessIndicatorID).ToList();
        var indicators = businessDataModel.BusinessIndicators.Where( ind => indicator_ids.Contains(ind.Id)).Select( ind => new
        {
            Title = ind.Name,
            Subtitle = ind.Description,
            Id = ind.Id
        } ).ToList();

        foreach(var indicator in indicators)
        {
            result[indicator.Title] = dataset.Where(data => data.BusinessIndicatorID == indicator.Id).Select(data => data.IndValue).ToList();                
        }
        return result;
    }




    public BusinessResource GetResourceById(int id) => businessDataModel.BusinessResources.Find(id);
    public Dictionary<string, IEnumerable<BusinessData>> GetDatasetReport(int datasetId, string beginDate, int granularity)
    {
        Dictionary<string, IEnumerable<BusinessData>> result = new();
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        this.Info(businessDataModel.BusinessData.ToList().ToJsonOnScreen());
    var data = this.businessDataModel.BusinessData.ToList();//.Where(data => data.BeginDate == bdate && data.BusinessDatasetID == datasetId && data.GranularityID == granularity);
        foreach (var res in data.Select(p => p.BusinessResourceID).Distinct().ToHashSet())
        {
            result[res.ToString()] = data.Where(d => d.BusinessResourceID == res).ToList();
        }
        if (result.Count() == 0)
            throw new ArgumentException("Критерии выборки данных неверны");
        return result;
    }
    public Dictionary<string, IEnumerable<BusinessData>> GetIndicatorsReport(int indicatorId, string beginDate, int granularity)
    {
        Dictionary<string, IEnumerable<BusinessData>> result = new();
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        List<BusinessData> data = this.businessDataModel.BusinessData.Where(data => data.BeginDate == bdate && data.BusinessIndicatorID == indicatorId && data.GranularityID == granularity).ToList();
        foreach (var res in data.Select(data => data.BusinessResourceID).Distinct().ToHashSet())
        {
            result[res.ToString()] = data.Where(d => d.BusinessResourceID == res).ToList();
        }
        return result;
    }


public IEnumerable<BusinessResource> GetResourcesFromData(string beginDate)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        var resourcesIds = this.businessDataModel.BusinessData.Where(data => data.BeginDate == bdate).Select(data => data.BusinessResourceID).ToHashSet();
        return this.businessDataModel.BusinessResources.Where(res => resourcesIds.Contains(res.Id)).ToList();             
    }

    public IEnumerable<BusinessIndicator> GetIndicatorsFromData(string beginDate)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        var resourcesIds = this.businessDataModel.BusinessData.Where(data => data.BeginDate == bdate).Select(data => data.BusinessIndicatorID).ToHashSet();
        return this.businessDataModel.BusinessIndicators.Where(res => resourcesIds.Contains(res.Id)).ToList();
    }

    public IEnumerable<BusinessDataset> GetDatasetsFromData(string beginDate)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        var resourcesIds = this.businessDataModel.BusinessData.Where(data => data.BeginDate == bdate).Select(data => data.BusinessDatasetID).ToHashSet();
        return this.businessDataModel.BusinessDatasets.Where(res => resourcesIds.Contains(res.Id)).ToList();
    }



    public IEnumerable<BusinessData> GetIndicatorValues(int indicatorId, int datasetId, string beginDate, int granularity)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        return businessDataModel.BusinessData.Where(data => data.BusinessIndicatorID == indicatorId && data.BusinessDatasetID == datasetId && data.BeginDate == bdate && data.GranularityID == granularity);
    }
    public IEnumerable<BusinessData> GetDatasetValues(int resourceId, int datasetId, string beginDate, int granularity)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
    
        return businessDataModel.BusinessData.Where(data => data.BusinessResourceID == resourceId && data.BusinessDatasetID == datasetId && data.BeginDate == bdate && data.GranularityID == granularity);
    }

    public IEnumerable<BusinessData> GetDatasetValues( int datasetId, string beginDate, int granularity)
    {
        var bdate = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        return businessDataModel.BusinessData.Where(data => data.BusinessDatasetID == datasetId && data.BeginDate == bdate && data.GranularityID == granularity);
    }


    public int AddData(int resourceId, int indicatorId, int datasetId, string beginDate, int granularity, float value)
    {
        System.Console.WriteLine($"exec insert_primary_data @BusinessResourceID={resourceId},@BusinessDatasetID={datasetId},@BusinessIndicatorID={indicatorId},@BeginDate={beginDate},@GranularityID={granularity},@IndValue={value}");
        var bd = DateTime.ParseExact(beginDate, "yyyy-MM-dd", new CultureInfo("us"));
        BusinessData newData = new()
        {
            BeginDate = bd,
            BusinessDatasetID = datasetId,
            GranularityID = granularity,
            BusinessIndicatorID = indicatorId,
            BusinessResourceID = resourceId,
            Changed = DateTime.Now,
            IndValue = value
        };
        newData.EnsureIsValide();
        businessDataModel.BusinessData.Add(newData);
        return businessDataModel.SaveChanges();
    }


    public BusinessDatasource AddDatasource( string name, string desription, string connectionString )
    {
        BusinessDatasource target = new()
        {
            Name = name,
            Description = desription,
            ConnectionString = connectionString
        };
        businessDataModel.BusinessDatasources.Add( target );
        businessDataModel.SaveChanges();
        return target;
    }


    public BusinessIndicator AddIndicator(string label, string name, string desription)
    {
        BusinessIndicator target = new()
        {
            Label = label,
            Name = name,
            Description = desription
        };
        businessDataModel.BusinessIndicators.Add(target);
        businessDataModel.SaveChanges();
        return target;
    }

    public string GetConnectionString(string name)
    {

        var datasource = businessDataModel.BusinessDatasources.FirstOrDefault(ds => ds.Name.ToLower() == name.ToLower());
        if(datasource is null)
        {
            throw new ArgumentException("name", "Значение в аргументе name не существующее");
        }
        return datasource.ConnectionString;
    }

    public IEnumerable<BusinessIndicator> GetIndicators()
    {
        return businessDataModel.BusinessIndicators.ToList();
    }


        
    public IEnumerable<BusinessResource> GetResources()
    {
        return businessDataModel.BusinessResources.ToList();            
    }

    public BusinessDataset GetDatasetByName(string name)
    {
        var res = businessDataModel.BusinessDatasets.FirstOrDefault(ds => ds.Name.ToLower() == name.ToLower());          
        return res;
    }
}
