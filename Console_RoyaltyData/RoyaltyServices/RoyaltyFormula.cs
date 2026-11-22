namespace Console_RoyaltyData.RoyaltyServices
{
    /// <summary>
    /// Расчёт роялти по правилам [webapibuhreport].[RoyaltyReportSetting] @code = 1.
    /// </summary>
    public static class RoyaltyFormula
    {
        public static bool IsInternalFranchise(string? divisionType)
        {
            if (string.IsNullOrEmpty(divisionType))
                return false;
            if (divisionType.StartsWith("EA_", StringComparison.Ordinal))
                return true;
            return divisionType.Contains("euroauto", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Проверки процедуры до выборки оборотов. Последнее несработавшее условие задаёт сообщение.
        /// </summary>
        public static string? Validate(
            bool isEa,
            decimal koefRoyalty,
            decimal koefCalc,
            decimal ndsSt,
            decimal sumWorkBuh,
            decimal sumPartBuh)
        {
            string? message = null;
            if (koefRoyalty == 0)
                message = "Не установлен коэффициент Роялти";
            if (koefCalc == 0)
                message = "Не установлен коэффициент рассчета";
            if (ndsSt == 0)
                message = "Не установлен НДС";
            if (isEa && (sumWorkBuh == 0 || sumPartBuh == 0))
                message = "Не введена валовая выручка";
            return message;
        }

        public static RoyaltyComputation Compute(RoyaltyComputationInput input)
        {
            var sumWork = input.SumWork;
            var sumPart = input.SumPart;
            if (input.IsEa)
            {
                sumWork = input.SumWorkBuh;
                sumPart = input.SumPartBuh;
            }

            decimal procRoyalty;
            if (input.IsEa && input.DiscountForBuy > 0)
            {
                procRoyalty = input.DiscountLoyalty > 0
                    ? input.KoefRoyalty * (100m - input.DiscountForBuy) / 100m * (100m - input.DiscountLoyalty) / 100m
                    : input.KoefRoyalty * (100m - input.DiscountForBuy) / 100m;
            }
            else if (input.IsEa)
            {
                procRoyalty = input.KoefRoyalty;
            }
            else
            {
                // В процедуре для подразделения не EA коэффициент переводится в долю до формулы суммы.
                procRoyalty = input.KoefRoyalty / 100m;
            }

            var buyTotal = input.SumBuyOther + input.SumBuyEa;
            var ratioPurEa = Round(input.SumBuyEa / (buyTotal == 0 ? 1m : buyTotal), 2);
            var royaltyBase = (sumWork + sumPart) * procRoyalty / 100m;
            var pretensionPercent = input.CountRun > 0
                ? input.Pretensions * 100m / input.CountRun
                : 0m;

            return new RoyaltyComputation
            {
                SumWorkUsed = sumWork,
                SumPartUsed = sumPart,
                SumProceeds = sumWork + sumPart,
                SumPurEa = input.SumBuyEa,
                SumPurOther = input.SumBuyOther,
                SumBuyEa = input.SumBuyEa,
                SumBuyOther = input.SumBuyOther,
                RatioPurEa = ratioPurEa,
                RatioPretensions = input.Pretensions,
                SumRoyalty = Round(royaltyBase, 2),
                SumRoyaltyNds = Round(royaltyBase * input.NdsSt / input.KoefCalc, 2),
                SumSmb = input.SumSmb,
                CountRun = input.CountRun,
                IndexLoyalty = Round(ratioPurEa * 100m - pretensionPercent, 0),
                K2 = CommitmentCoefficient(ratioPurEa),
                K3 = CriticsCoefficient(input.CountRun, input.Pretensions),
                K4 = 0,
                RoyaltyCalc = procRoyalty,
                KoefRoyalty = input.KoefRoyalty,
                KoefCalc = input.KoefCalc,
                NdsSt = input.NdsSt
            };
        }

        /// <summary>Коэффициент приверженности. ratioPurEa — доля закупок EA от 0 до 1.</summary>
        public static decimal CommitmentCoefficient(decimal ratioPurEa)
        {
            if (ratioPurEa < 0.25m)
                return 3m;
            if (ratioPurEa >= 0.25m && ratioPurEa <= 0.30m)
                return 2m;
            if (ratioPurEa >= 0.31m && ratioPurEa <= 0.35m)
                return 1m;
            return 0m;
        }

        /// <summary>Коэффициент критиков по доле обоснованных претензий к заказ-нарядам.</summary>
        public static decimal CriticsCoefficient(int countRun, int pretensions)
        {
            if (countRun <= 0)
                return 0m;
            var percent = pretensions * 100m / countRun;
            if (percent <= 0.75m)
                return 0m;
            if (percent <= 1.00m)
                return 1m;
            if (percent <= 1.25m)
                return 2m;
            return 3m;
        }

        private static decimal Round(decimal value, int digits) =>
            Math.Round(value, digits, MidpointRounding.AwayFromZero);
    }

    public sealed class RoyaltyComputationInput
    {
        public bool IsEa { get; init; }
        public decimal KoefRoyalty { get; init; }
        public decimal KoefCalc { get; init; }
        public decimal NdsSt { get; init; }
        public decimal DiscountForBuy { get; init; }
        public decimal DiscountLoyalty { get; init; }
        public decimal SumWork { get; init; }
        public decimal SumPart { get; init; }
        public decimal SumWorkBuh { get; init; }
        public decimal SumPartBuh { get; init; }
        public decimal SumBuyEa { get; init; }
        public decimal SumBuyOther { get; init; }
        public int CountRun { get; init; }
        public int Pretensions { get; init; }
        public decimal SumSmb { get; init; }
    }

    public sealed class RoyaltyComputation
    {
        public decimal SumWorkUsed { get; init; }
        public decimal SumPartUsed { get; init; }
        public decimal SumProceeds { get; init; }
        public decimal SumPurEa { get; init; }
        public decimal SumPurOther { get; init; }
        public decimal SumBuyEa { get; init; }
        public decimal SumBuyOther { get; init; }
        public decimal RatioPurEa { get; init; }
        public decimal RatioPretensions { get; init; }
        public decimal SumRoyalty { get; init; }
        public decimal SumRoyaltyNds { get; init; }
        public decimal SumSmb { get; init; }
        public int CountRun { get; init; }
        public decimal IndexLoyalty { get; init; }
        public decimal K2 { get; init; }
        public decimal K3 { get; init; }
        public decimal K4 { get; init; }
        public decimal RoyaltyCalc { get; init; }
        public decimal KoefRoyalty { get; init; }
        public decimal KoefCalc { get; init; }
        public decimal NdsSt { get; init; }
    }
}
