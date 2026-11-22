using Console_RoyaltyData.RoyaltyEntities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_RoyaltyData.RoyaltyData;

using WebApiBuhReport.Models;

namespace Console_RoyaltyData.RoyaltyServices
{
    public class RoyaltyFullAccessService(RoyaltyDbContext royaltyDbContext)
    {
        
        public List<Department> GetDepartments()
        {
            return royaltyDbContext.Departments.Include(d => d.Organization).ToList();
        }

        public int UpdateDepartment(Department department, Organization organization)
        {
            royaltyDbContext.Update(department);
            royaltyDbContext.Update(organization);
            return royaltyDbContext.SaveChanges();
        }

        public List<Report> GetReports(int year, int month)
        {
            return royaltyDbContext.Reports.Include(d => d.Department).ThenInclude(d => d.Organization).Where(rep => rep.ReportYear==year && rep.ReportMonth == month).ToList();
        }

        /// <summary>
        /// Расчёт роялти подразделения за месяц.
        /// Повторяет [webapibuhreport].[RoyaltyReportSetting] @code = 1:
        /// коэффициенты из RoyaltyCalculations, обороты и претензии из Reports,
        /// для внутренней франшизы EA база роялти — суммы бухгалтера.
        /// </summary>
        /// <param name="sumWorkBuh">Оборот по услугам. Вместе с sumPartBuh больше нуля сохраняется до расчёта.</param>
        /// <param name="sumPartBuh">Оборот по запчастям.</param>
        public SpExecuteResult CalculateRoyalty(int departmentId, int year, int month, decimal sumWorkBuh = 0, decimal sumPartBuh = 0)
        {
            var department = royaltyDbContext.Departments.FirstOrDefault(item => item.Id == departmentId);
            if (department is null)
            {
                return new SpExecuteResult
                {
                    error = 1,
                    err_message = "Подразделение не найдено"
                };
            }

            var settings = royaltyDbContext.RoyaltyCalculations.FirstOrDefault(item =>
                item.DepartmentId == departmentId && item.ReportYear == year && item.ReportMonth == month);

            var ownerId = settings?.OwnerId ?? 0;
            if (ownerId == 0)
                ownerId = department.OwnerId;

            var koefRoyalty = settings?.KoefRoyalty ?? 0m;
            var koefCalc = settings?.KoefCalc ?? 0m;
            var ndsSt = settings?.NdsSt ?? 0m;
            var discountForBuy = settings?.DiscountForBuy ?? 0m;
            var discountLoyalty = settings?.DiscountLoyalty ?? 0m;
            var isEa = string.IsNullOrEmpty(department.DivisionType)
                ? department.IsEa
                : RoyaltyFormula.IsInternalFranchise(department.DivisionType);

            var monthData = royaltyDbContext.RoyaltyMonthData.FirstOrDefault(item =>
                item.DepartmentId == departmentId && item.ReportYear == year && item.ReportMonth == month);

            if (sumWorkBuh > 0 && sumPartBuh > 0)
            {
                monthData ??= AddMonthData(departmentId, year, month, ownerId);
                monthData.SumWorkBuh = sumWorkBuh;
                monthData.SumPartBuh = sumPartBuh;
                monthData.OwnerId = ownerId;
                royaltyDbContext.SaveChanges();
            }

            var storedWorkBuh = monthData?.SumWorkBuh ?? 0m;
            var storedPartBuh = monthData?.SumPartBuh ?? 0m;
            var validationMessage = RoyaltyFormula.Validate(isEa, koefRoyalty, koefCalc, ndsSt, storedWorkBuh, storedPartBuh);
            if (validationMessage is not null)
            {
                return new SpExecuteResult
                {
                    error = 1,
                    err_message = validationMessage
                };
            }

            var reports = royaltyDbContext.Reports
                .Where(item => item.DepartmentId == departmentId && item.ReportYear == year && item.ReportMonth == month)
                .ToList();
            if (reports.Count == 0)
            {
                return new SpExecuteResult
                {
                    error = 0,
                    err_message = "Успех"
                };
            }

            var computation = RoyaltyFormula.Compute(new RoyaltyComputationInput
            {
                IsEa = isEa,
                KoefRoyalty = koefRoyalty,
                KoefCalc = koefCalc,
                NdsSt = ndsSt,
                DiscountForBuy = discountForBuy,
                DiscountLoyalty = discountLoyalty,
                SumWork = reports.Sum(item => item.WorksAmount),
                SumPart = reports.Sum(item => item.PartsAmount),
                SumWorkBuh = storedWorkBuh,
                SumPartBuh = storedPartBuh,
                SumBuyEa = reports.Sum(item => item.PurchaseEa),
                SumBuyOther = reports.Sum(item => item.PurchaseOthers),
                CountRun = reports.Sum(item => item.CountRunOrders),
                Pretensions = reports.Sum(item => item.CountPretentionOrders),
                SumSmb = 0
            });

            monthData ??= AddMonthData(departmentId, year, month, ownerId);
            ApplyComputation(monthData, computation, ownerId);

            try
            {
                royaltyDbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return new SpExecuteResult
                {
                    error = 1,
                    err_message = "Ошибка"
                };
            }

            return new SpExecuteResult
            {
                error = 0,
                err_message = "Успех"
            };
        }

        private RoyaltyMonthData AddMonthData(int departmentId, int year, int month, int ownerId)
        {
            var monthData = new RoyaltyMonthData
            {
                DepartmentId = departmentId,
                ReportYear = year,
                ReportMonth = month,
                OwnerId = ownerId
            };
            royaltyDbContext.RoyaltyMonthData.Add(monthData);
            return monthData;
        }

        private static void ApplyComputation(RoyaltyMonthData monthData, RoyaltyComputation computation, int ownerId)
        {
            monthData.OwnerId = ownerId;
            monthData.KoefRoyalty = computation.KoefRoyalty;
            monthData.KoefCalc = computation.KoefCalc;
            monthData.NdsSt = computation.NdsSt;
            monthData.SumProceeds = computation.SumProceeds;
            monthData.SumPurEa = computation.SumPurEa;
            monthData.SumPurOther = computation.SumPurOther;
            monthData.SumBuyEa = computation.SumBuyEa;
            monthData.SumBuyOther = computation.SumBuyOther;
            monthData.RatioPurEa = computation.RatioPurEa;
            monthData.RatioPretensions = computation.RatioPretensions;
            monthData.SumRoyalty = computation.SumRoyalty;
            monthData.SumRoyaltyNds = computation.SumRoyaltyNds;
            monthData.SumSmb = computation.SumSmb;
            monthData.SumWorkBuh = computation.SumWorkUsed;
            monthData.SumPartBuh = computation.SumPartUsed;
            monthData.CountRun = computation.CountRun;
            monthData.IndexLoyalty = computation.IndexLoyalty;
            monthData.K2 = computation.K2;
            monthData.K3 = computation.K3;
            monthData.K4 = computation.K4;
            monthData.RoyaltyCalc = computation.RoyaltyCalc;
        }
    }
}
