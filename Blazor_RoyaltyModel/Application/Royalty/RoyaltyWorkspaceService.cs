using Console_RoyaltyData.RoyaltyData;
using Console_RoyaltyData.RoyaltyEntities;
using Console_RoyaltyData.RoyaltyServices;

using Microsoft.EntityFrameworkCore;

namespace Blazor_SiteModel.Application.Royalty;

public sealed class RoyaltyWorkspaceService(RoyaltyDbContext db, RoyaltyFullAccessService royalty)
{
    private static readonly string[] MonthNames =
    [
        "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
        "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
    ];

    public static IReadOnlyList<int> Years
    {
        get
        {
            var current = DateTime.Today.Year;
            return Enumerable.Range(2020, current - 2020 + 1).ToList();
        }
    }

    public static IReadOnlyList<int> Months => Enumerable.Range(1, 12).ToList();

    public static string MonthLabel(int month) =>
        month is >= 1 and <= 12 ? MonthNames[month - 1] : month.ToString();

    public static (int Year, int Month) DefaultPeriod()
    {
        var previous = DateTime.Today.AddMonths(-1);
        return (previous.Year, previous.Month);
    }

    public async Task<List<RoyaltyDepartmentRow>> GetDepartmentRowsAsync(
        bool internalFranchise,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var departments = await QueryDepartments(internalFranchise)
            .Include(item => item.Organization)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        var departmentIds = departments.Select(item => item.Id).ToList();
        var reports = await db.Reports
            .Where(item => departmentIds.Contains(item.DepartmentId) && item.ReportYear == year && item.ReportMonth == month)
            .ToListAsync(cancellationToken);
        var monthData = await db.RoyaltyMonthData
            .Where(item => departmentIds.Contains(item.DepartmentId) && item.ReportYear == year && item.ReportMonth == month)
            .ToListAsync(cancellationToken);
        var settings = await db.RoyaltyCalculations
            .Where(item => departmentIds.Contains(item.DepartmentId) && item.ReportYear == year && item.ReportMonth == month)
            .ToListAsync(cancellationToken);

        var reportByDep = reports.GroupBy(item => item.DepartmentId).ToDictionary(g => g.Key, g => g.ToList());
        var monthByDep = monthData.ToDictionary(item => item.DepartmentId);
        var settingsByDep = settings.ToDictionary(item => item.DepartmentId);

        return departments.Select(department =>
        {
            reportByDep.TryGetValue(department.Id, out var depReports);
            depReports ??= [];
            //monthByDep.TryGetValue(department.Id, out var data);
            //settingsByDep.TryGetValue(department.Id, out var calc);

            var purchaseEa = depReports.Sum(item => item.PurchaseEa);
            var purchaseOther = depReports.Sum(item => item.PurchaseOthers);
            var countRun = depReports.Sum(item => item.CountRunOrders);
            var pretensions = depReports.Sum(item => item.CountPretentionOrders);

            return new RoyaltyDepartmentRow
            {
                DepartmentId = department.Id,
                Name = department.Name,
                TypeShop = ShopType(department),
                OwnerName = department.Organization?.Name ?? "",
                OwnerInn = department.Organization?.Inn ?? "",
                Address = department.Address ?? "",
                Ogrn = department.Organization?.Ogrn ?? "",
                Kpp = department.Organization?.Kpp ?? "",
                IsInternal = IsInternal(department),
                PurchaseEa = purchaseEa,
                PurchaseOther = purchaseOther,
                PurchaseRatio = Ratio(purchaseEa, purchaseEa + purchaseOther),
                CountRun = countRun,
                Pretensions = pretensions,
                PretensionRatio = countRun == 0 ? 0 : Math.Round(pretensions * 100m / countRun, 2),
                //ServiceTurnover = data?.SumWorkBuh ?? 0,
                //PartsTurnover = data?.SumPartBuh ?? 0,
                //KoefCalc = calc?.KoefCalc ?? 0,
                //KoefRoyalty = calc?.KoefRoyalty ?? 0,
                //NdsSt = calc?.NdsSt ?? 0,
                //DiscountForBuy = calc?.DiscountForBuy ?? 0,
                //DiscountLoyalty = calc?.DiscountLoyalty ?? 0,
                //Approved = (data?.SumWorkBuh ?? 0) > 0 && (data?.SumPartBuh ?? 0) > 0
            };
        }).ToList();
    }

    public async Task<List<CompanyCardView>> GetCompanyCardsAsync(
        bool internalFranchise,
        CancellationToken cancellationToken = default)
    {
        var departments = await QueryDepartments(internalFranchise)
            .Include(item => item.Organization)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);

        var orgIds = departments.Select(item => item.OrganizationId).Distinct().ToList();
        var contracts = await db.Contracts
            .Where(item => orgIds.Contains(item.OrganizationId))
            .ToListAsync(cancellationToken);
        var contractByOrg = contracts
            .GroupBy(item => item.OrganizationId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(item => item.ContractDate).First());

        var rates = await db.RoyaltyCalculations
            .Where(item => item.KoefRoyalty > 0)
            .GroupBy(item => item.DepartmentId)
            .Select(g => new { DepartmentId = g.Key, Rate = g.Max(item => item.KoefRoyalty) })
            .ToListAsync(cancellationToken);
        var rateByDep = rates.ToDictionary(item => item.DepartmentId, item => item.Rate);

        return departments.Select(department =>
        {
            contractByOrg.TryGetValue(department.OrganizationId, out var contract);
            rateByDep.TryGetValue(department.Id, out var rate);
            var org = department.Organization;
            var inn = org?.Inn ?? "";
            var kpp = org?.Kpp ?? "";
            var ogrn = org?.Ogrn ?? "";
            var ownerName = org?.Name ?? "";
            var isIp = inn.Length == 12 || ownerName.StartsWith("ИП", StringComparison.OrdinalIgnoreCase);
            var filled = !string.IsNullOrWhiteSpace(ownerName)
                && !string.IsNullOrWhiteSpace(department.Address)
                && !string.IsNullOrWhiteSpace(inn)
                && !string.IsNullOrWhiteSpace(ogrn)
                && (isIp || !string.IsNullOrWhiteSpace(kpp));

            return new CompanyCardView
            {
                DepartmentId = department.Id,
                OrganizationId = department.OrganizationId,
                Title = string.IsNullOrWhiteSpace(ownerName) ? department.Name : ownerName,
                Subtitle = department.Name,
                Inn = inn,
                Kpp = kpp,
                Ogrn = ogrn,
                Address = department.Address ?? "",
                OwnerName = ownerName,
                ContractNumber = contract?.ContractNumber ?? "",
                ContractDate = contract?.ContractDate,
                IsInternal = IsInternal(department),
                IsIp = isIp,
                Filled = filled,
                BaseRate = rate == 0 ? null : rate
            };
        }).ToList();
    }

    public async Task SaveTurnoverAsync(int departmentId, int year, int month, decimal service, decimal parts)
    {
        royalty.CalculateRoyalty(departmentId, year, month, service, parts);
        await Task.CompletedTask;
    }

    public string Calculate(int departmentId, int year, int month, decimal service, decimal parts)
    {
        var result = royalty.CalculateRoyalty(departmentId, year, month, service, parts);
        return result.err_message ?? "";
    }

    public async Task SaveMainSettingsAsync(
        IEnumerable<RoyaltyDepartmentRow> rows,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        foreach (var row in rows)
        {
            var settings = await db.RoyaltyCalculations.FirstOrDefaultAsync(item =>
                item.DepartmentId == row.DepartmentId && item.ReportYear == year && item.ReportMonth == month,
                cancellationToken);

            if (settings is null)
            {
                settings = new RoyaltyCalculation
                {
                    DepartmentId = row.DepartmentId,
                    ReportYear = year,
                    ReportMonth = month
                };
                db.RoyaltyCalculations.Add(settings);
            }

            settings.KoefCalc = row.KoefCalc;
            settings.KoefRoyalty = row.KoefRoyalty;
            settings.NdsSt = row.NdsSt;
            settings.DiscountForBuy = row.DiscountForBuy;
            settings.DiscountLoyalty = row.DiscountLoyalty;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveCompanyCardAsync(CompanyCardView card, CancellationToken cancellationToken = default)
    {
        var department = await db.Departments.Include(item => item.Organization)
            .FirstOrDefaultAsync(item => item.Id == card.DepartmentId, cancellationToken);
        if (department is null)
            return;

        department.Name = card.Subtitle;
        department.Address = card.Address;
        if (department.Organization is not null)
        {
            department.Organization.Name = card.OwnerName;
            department.Organization.Inn = card.Inn;
            department.Organization.Ogrn = card.Ogrn;
        }

        var contract = await db.Contracts
            .Where(item => item.OrganizationId == department.OrganizationId)
            .OrderByDescending(item => item.ContractDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(card.ContractNumber) || card.ContractDate is not null)
        {
            if (contract is null)
            {
                contract = new Contract
                {
                    OrganizationId = department.OrganizationId,
                    ContractDate = card.ContractDate ?? DateTime.Today
                };
                db.Contracts.Add(contract);
            }

            contract.ContractNumber = card.ContractNumber;
            if (card.ContractDate is not null)
                contract.ContractDate = card.ContractDate.Value;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ReceivedDocumentRow>> GetReceivedDocumentsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var rows = await GetDepartmentRowsAsync(false, year, month, cancellationToken);
        var internalRows = await GetDepartmentRowsAsync(true, year, month, cancellationToken);
        return rows.Concat(internalRows)
            .Select(item => new ReceivedDocumentRow
            {
                DepartmentId = item.DepartmentId,
                Name = item.Name,
                TypeShop = item.TypeShop,
                OwnerName = item.OwnerName,
                Year = year,
                Month = month,
                HasInvoice = item.ServiceTurnover > 0 || item.PartsTurnover > 0 || item.PurchaseEa > 0
            })
            .Where(item => item.HasInvoice)
            .OrderBy(item => item.Name)
            .ToList();
    }

    private IQueryable<Department> QueryDepartments(bool internalFranchise)
    {
        var query = db.Departments.AsQueryable();
        return internalFranchise
            ? query.Where(item => item.IsEa || item.DivisionType.StartsWith("EA_") || item.DivisionType.Contains("euroauto"))
            : query.Where(item => !item.IsEa && !item.DivisionType.StartsWith("EA_") && !item.DivisionType.Contains("euroauto"));
    }

    private static bool IsInternal(Department department) =>
        department.IsEa || RoyaltyFormula.IsInternalFranchise(department.DivisionType);

    private static string ShopType(Department department) =>
        IsInternal(department) ? "внутренняя" : "внешняя";

    private static decimal Ratio(decimal part, decimal total) =>
        total == 0 ? 0 : Math.Round(part / total, 2);
}

public sealed class RoyaltyDepartmentRow
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = "";
    public string TypeShop { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string OwnerInn { get; set; } = "";
    public string Address { get; set; } = "";
    public string Ogrn { get; set; } = "";
    public string Kpp { get; set; } = "";
    public bool IsInternal { get; set; }
    public decimal PurchaseEa { get; set; }
    public decimal PurchaseOther { get; set; }
    public decimal PurchaseRatio { get; set; }
    public int CountRun { get; set; }
    public int Pretensions { get; set; }
    public decimal PretensionRatio { get; set; }
    public decimal ServiceTurnover { get; set; }
    public decimal PartsTurnover { get; set; }
    public decimal KoefCalc { get; set; }
    public decimal KoefRoyalty { get; set; }
    public decimal NdsSt { get; set; }
    public decimal DiscountForBuy { get; set; }
    public decimal DiscountLoyalty { get; set; }
    public bool Approved { get; set; }
    public bool Selected { get; set; }
}

public sealed class CompanyCardView
{
    public int DepartmentId { get; set; }
    public int OrganizationId { get; set; }
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Inn { get; set; } = "";
    public string Kpp { get; set; } = "";
    public string Ogrn { get; set; } = "";
    public string Address { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string ContractNumber { get; set; } = "";
    public DateTime? ContractDate { get; set; }
    public bool IsInternal { get; set; }
    public bool IsIp { get; set; }
    public bool Filled { get; set; }
    public decimal? BaseRate { get; set; }
}

public sealed class ReceivedDocumentRow
{
    public int DepartmentId { get; set; }
    public string Name { get; set; } = "";
    public string TypeShop { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public int Year { get; set; }
    public int Month { get; set; }
    public bool HasInvoice { get; set; }
}
