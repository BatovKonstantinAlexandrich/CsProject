using Console_RoyaltyData.RoyaltyEntities;
using Console_RoyaltyData.RoyaltyImport.Reports;
using Console_RoyaltyData.RoyaltyServices;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Console_RoyaltyData.RoyaltyData
{
    public class RoyaltyDbInitiallizer
    {
        private ILogger<RoyaltyDbInitiallizer> logger = LoggerFactory.Create(options => { options.AddConsole(); }).CreateLogger<RoyaltyDbInitiallizer>();

        private RoyaltyDbImport dbImport = new();
       

        public async Task Init(RoyaltyDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var accessReponse = await dbImport.GetAccessTag("batov.ka");
            var divionsResults = await dbImport.GetRoyaltyDivisions( );
            
            Dictionary<(int, int), List<FranshiseReportResult>> reportResults = new();
            if (dbContext.Employees.Count() == 0)
            {
                var staff = await dbImport.GetStaffPersons();
                dbContext.Employees.AddRange(staff.Select(s => new Employee()
                {
                    AdLogin = s.ad_login,
                    Name = s.p_name,
                    Surname = s.p_surname,
                    Patronymic = s.p_patronymic
                }).ToList());
                await dbContext.SaveChangesAsync();
            }
            if (dbContext.Roles.Count()==0)
            {
                dbContext.Roles.Add(new()
                {
                    Name = "DepartmentBuh",
                    Description = "Бухгалтер подразделения предоставляет сведения по финансовым показателям деятельности предпрятия"
                });
                dbContext.Roles.Add(new()
                {
                    Name = "EuroautoBuh",
                    Description = "Бухгалтер евроавто принимает документы от бухгалтера подразделений"
                });
                await dbContext.SaveChangesAsync();
            }
            if (dbContext.Reports.Count()==0)
            {                          
                for (int i = 1; i <= 10; i++)
                {
                    logger.LogInformation($"{i}.{2026}");
                    reportResults[(2026, i)] = await dbImport.GetReportResult(2026, i);
                }                                     
            }
            
            foreach (var divisionItem in divionsResults)
            {
                var accessItem = accessReponse.items.FirstOrDefault(item => item.div_id== divisionItem.division_id);
                if (accessItem is null)
                    continue;

                var dep = dbContext.Departments.FirstOrDefault(item => item.Name == accessItem.dep_name);
                if (dep is null)
                {
                    Organization organization = null;
                    dbContext.Organizations.Add(organization=new()
                    {
                        Name = accessItem.owner_name,
                        Inn = divisionItem.owner_inn,
                        Ogrn = divisionItem.owner_ogrn,
                        Kpp = divisionItem.owner_kpp
                    });
                    dbContext.Departments.Add(dep = new()
                    {
                        Name = accessItem.dep_name,
                        Address = divisionItem.address_div,
                        Organization = organization,
                        DivisionId = divisionItem.division_id,
                        OwnerId = accessItem.owner_id,
                        DivisionType = accessItem.type_dep ?? string.Empty,
                        IsEa = RoyaltyFormula.IsInternalFranchise(accessItem.type_dep)
                    });
                    var contractDate = DateTime.Now.AddMonths(-12);
                    dbContext.Contracts.Add(new()
                    {
                        ContractNumber = $"{(dbContext.Contracts.Count()+1)}-{contractDate.Month}/{contractDate.Year}",
                        Organization = organization
                    });
                    await dbContext.SaveChangesAsync();

                    if (dbContext.Reports.Any(r => r.DepartmentId==dep.Id))
                    {
                        continue;
                    }
                    foreach(var kv in reportResults)
                    {
                        var reports = kv.Value.Where(p => p.div_id== divisionItem.division_id);
                        if(reports is not null && reports.Count()>0)
                        {
                            dbContext.Reports.Add(new()
                            {
                                Department = dep,
                                CountRunOrders = reports.Sum(rep => rep.cnt_run),
                                CountPretentionOrders = reports.Sum(rep => rep.pret),
                                PartsAmount = reports.Sum(rep => rep.sum_parts),
                                WorksAmount = reports.Sum(rep => rep.sum_works),
                                ReportYear = kv.Key.Item1,
                                ReportMonth = kv.Key.Item2,
                                PurchaseEa = reports.Sum(rep => rep.sum_pur_ea),
                                PurchaseOthers = reports.Sum(rep => rep.sum_pur_other)
                            });
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    dep.DivisionId = divisionItem.division_id;
                    dep.OwnerId = accessItem.owner_id;
                    dep.DivisionType = accessItem.type_dep ?? string.Empty;
                    dep.IsEa = RoyaltyFormula.IsInternalFranchise(accessItem.type_dep);
                }
            }
            await dbContext.SaveChangesAsync();


        }
    }
}
