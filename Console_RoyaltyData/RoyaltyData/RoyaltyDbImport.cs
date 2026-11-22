using Console_RoyaltyData.RoyaltyImport.Access;
using Console_RoyaltyData.RoyaltyImport.Reports;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System.Data;
using System.Data.Common;
using System.Globalization;

using WebApi.Data;

using WebApiBuhReport.Models;
using WebApiBuhReport.Models.Access;
using WebApiBuhReport.Models.Royalty;

namespace Console_RoyaltyData.RoyaltyData
{
    public partial class RoyaltyDbImport
    {
        private ILogger<RoyaltyDbImport> logger = LoggerFactory.Create(options => { options.AddConsole(); }).CreateLogger<RoyaltyDbImport>();


        public async Task<List<StaffPerson>> GetStaffPersons()
        {
            List<StaffPerson> results = new();
            var sql = @"select p_name, p_surname, p_patronymic, ad_login from staff_persons p where p.activity is null and ad_login is not null and p_patronymic is not null";
            var helper = new MSSqlHelper();
            using (var reader = await helper.ExecuteReaderAsync(GetConnectionString(), sql, System.Data.CommandType.Text, parameters =>
            {
            }))
            {
                while (await reader.ReadAsync())
                {
                    var p_name = reader.GetString("p_name");
                    var p_surname = reader.GetString("p_surname");
                    var p_patronymic = reader.GetString("p_patronymic");
                    var ad_login = reader.GetString("ad_login");
                    results.Add(new()
                    {
                        p_name = p_name,
                        p_surname = p_surname,
                        p_patronymic = p_patronymic,
                        ad_login = ad_login
                    });
                }
            }
            return results;
        }
        /// <summary>
        /// Получение тэга, определяющего уровень доступа
        /// </summary>
        /// <param name="ad_login">логин из KeyCloak</param>
        public async Task<AccessTagResponse> GetAccessTag(string ad_login)
        {
            /// Full (CalcRoyalt) - Полный доступ -внесение данных, расчёт Роялти, просмотр результатов, формирование и рассылка документов-в дальнейшем
            /// Default (AccEa) - Частичный доступ - внесение показателей для определенных позиций, просмотр результатов-в дальнейшем
            /// Read (AccFr) - Только просмотр

            var procName = "[webapibuhreport].[Authorization_user]";
            string levels = "None";
            List<AccessTagItem> items = new();
            var helper = new MSSqlHelper();
            var connectionString = GetConnectionString();
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters => {
                parameters.Add(new("@user_name", ad_login));
            }))
            {
                while (await reader.ReadAsync())
                {
                    levels = reader.GetString("levels");
                    var id_dep = reader.GetInt32("id_dep");
                    var name_dep = reader.GetString("name_dep");
                    var id_owner = reader.GetInt32("id_owner");
                    var name_owner = reader.GetString("name_owner");
                    var inn_owner = reader.GetString("inn_owner");
                    var type_dep = reader.GetString("type_dep");
                    var type_shop = reader.GetString("type_shop");
                    var div_id = reader.GetInt32("division_id");
                    items.Add(new()
                    {
                        dep_id = id_dep,
                        dep_name = name_dep,
                        owner_id = id_owner,
                        owner_name = name_owner,
                        owner_inn = inn_owner,
                        type_dep = type_dep,
                        type_shop = type_shop,
                        div_id = div_id,
                    });
                }
            }
            return new AccessTagResponse()
            {
                access = levels,
                items = items
            };
        }

        private string GetConnectionString()
        {
            return "Server=srv-main.euroauto.acc;Initial Catalog=tradeservice;User ID=user_webapics;Password=9Mzn)5ji79tm)%Oi1s)4;Application Name=WebApi;TrustServerCertificate=true";
        }

        public async Task<List<FranshiseReportResult>> GetReportResult(int year, int month)
        {
            var endDate = new DateTime(year, month, 1).AddDays(-1);
            var beginDate = new DateTime(endDate.Year, endDate.Month, 1);
            var connectionString = GetDivConnectionSring();
            var procName = "[dbo].[stat_sp_as_final_calculftion_franchise]";
            var mssql = new MSSqlHelper();
            var result = new List<FranshiseReportResult>();

            using (var reader = await mssql.ExecuteReaderAsync(
                connectionString,
                procName,
                CommandType.StoredProcedure,
                parameters =>
                {
                    parameters.Add(new("@begin_date", beginDate));
                    parameters.Add(new("@end_date", endDate));
                }))
            {
                while (await reader.ReadAsync())
                {
                    result.Add(new FranshiseReportResult
                    {
                        div_id = reader.GetInt32("div_id"),
                        as_fr = reader.IsDBNull("as_fr") ? "" : reader.GetString("as_fr"),
                        div_name = reader.IsDBNull("div_name") ? "" : reader.GetString("div_name"),
                        yy = reader.GetInt32("yy"),
                        mm = reader.GetInt32("mm"),
                        sum_works = reader.GetDecimal(reader.GetOrdinal("sum_works")),
                        sum_parts = reader.GetDecimal(reader.GetOrdinal("sum_parts")),
                        cnt_run = reader.GetInt32("cnt_run"),
                        pret = reader.GetInt32("pret"),
                        pret_proc = reader.GetDouble(reader.GetOrdinal("pret_proc")),
                        sum_pur_other = reader.GetDecimal(reader.GetOrdinal("sum_pur_other")),
                        sum_pur_ea = reader.GetDecimal(reader.GetOrdinal("sum_pur_ea")),
                        sum_pur_all = reader.GetDecimal(reader.GetOrdinal("sum_pur_all")),
                        proc_pur = reader.GetDecimal(reader.GetOrdinal("proc_pur")),
                        sum_sale = reader.GetDecimal(reader.GetOrdinal("sum_sale")),
                        sum_sale_all = reader.GetDecimal(reader.GetOrdinal("sum_sale_all"))
                    });
                }
            }

            return result;
        }

        private string GetDivConnectionSring()
        {
            return "Server=srv-submain.euroauto.acc;Initial Catalog=tradeservice_div;User ID=user_webapics;Password=9Mzn)5ji79tm)%Oi1s)4;Application Name=WebApi;TrustServerCertificate=true";
        }

        public async Task<List<SalesSummRecord>> GetSalesSumms(int year, int month)
        {
            List<SalesSummRecord> results = new();
            var sql = @"DECLARE @date datetime = DATEFROMPARTS(@year, @month, 1)

                        DECLARE @begin_date datetime,
                                @end_date   datetime;

                        SET @begin_date = DATEADD(MONTH, DATEDIFF(MONTH, 0, @date) - 1, 0);
                        SET @end_date   = DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @date), 0));

                        DROP TABLE IF EXISTS #stat_final_calculftion_franchise

                        CREATE TABLE #stat_final_calculftion_franchise
                        (
	                        div_id		   INT           NULL,
                            as_fr          VARCHAR(50)   NULL,
                            div_name       VARCHAR(250)  NULL,
                            yy             INT           NULL,
                            mm             INT           NULL,
                            sum_works      MONEY         NULL,
                            sum_parts      MONEY         NULL,
                            cnt_run        INT           NULL,
                            pret           INT           NULL,
                            pret_proc      FLOAT         NULL,
                            sum_pur_other  MONEY         NULL,
                            sum_pur_ea     MONEY         NULL,
                            sum_pur_all    MONEY         NULL,
                            proc_pur       FLOAT         NULL,
                            sum_sale       MONEY         NULL,
                            sum_sale_all   MONEY         NULL
                        );

                        INSERT INTO #stat_final_calculftion_franchise
                        (
                            div_id, as_fr, div_name, yy, mm,
                            sum_works, sum_parts, cnt_run,
                            pret, pret_proc,
                            sum_pur_other, sum_pur_ea, sum_pur_all, proc_pur,
                            sum_sale, sum_sale_all
                        )
                        EXEC [SRV-SubMain.EUROAUTO.ACC].[tradeservice_div].[dbo].[stat_sp_as_final_calculftion_franchise]
                            @begin_date = @begin_date,
                            @end_date   = @end_date;

                        SELECT div_id, div_name, yy, mm, sum_pur_other, sum_pur_ea
                        FROM #stat_final_calculftion_franchise
                        WHERE sum_pur_other>0 or sum_pur_ea>0
                        ORDER BY div_id, div_name, yy, mm;";
            var helper = new MSSqlHelper();
            using (var reader = await helper.ExecuteReaderAsync(GetConnectionString(), sql, System.Data.CommandType.Text, parameters =>
            {
           
                parameters.Add(new("@month", month));
                parameters.Add(new("@year", year));
            }))
            {
                while (await reader.ReadAsync())
                {
                    var div_id = reader.GetInt32("div_id");
                    var sum_pur_other = reader.GetInt32("sum_pur_other");
                    var sum_pur_ea = reader.GetInt32("sum_pur_ea");
                    results.Add(new()
                    {
                        div_id = div_id,
                        sum_pur_ea = sum_pur_ea,
                        sum_pur_other = sum_pur_other
                    });
                }
            }
            return results;
        }

        /// <summary>
        /// Выборка основных параметров
        /// </summary>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        /// <param name="div_id">если 0, то выбираются все подразделения</param>        
        public async Task<List<RoyaltyMainSettingsItem>> GetRolayltyMainSettings(int month, int year, int div_id = 0)
        {
            List<RoyaltyMainSettingsItem> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSelect]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec {procName} @code=1, @division_id={div_id}, @month_rep={month}, @year_rep={year}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 1));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                while (await reader.ReadAsync())
                {
                    var division_id = reader.GetInt32("division_id");
                    var division_name = reader.GetString("division_name");
                    var owner_name = reader.GetString("owner_name");
                    double? nds_st = reader.IsDBNull("nds_st") ? null : reader.GetDouble("nds_st");
                    double? koef_roylty = reader.IsDBNull("koef_roylty") ? null : reader.GetDouble("koef_roylty");
                    double? koef_calc = reader.IsDBNull("koef_calc") ? null : reader.GetDouble("koef_calc");
                    results.Add(new()
                    {
                        division_id = division_id,
                        division_name = division_name,
                        owner_name = owner_name,
                        koef_roylty = koef_roylty,
                        koef_calc = koef_calc,
                        nds_st = nds_st
                    });
                }
                return results;
            }
        }


        /// <summary>
        /// Выборка параметров расчёта
        /// </summary>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        /// <param name="div_id">если 0, то выбираются все подразделения</param>        
        public async Task<List<RoyaltyCalculationSettingsItem>> GetRolayltyCalculationSettings(int month, int year, int div_id = 0)
        {
            List<RoyaltyCalculationSettingsItem> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSelect]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec {procName} @code=2, @division_id={div_id}, @month_rep={month}, @year_rep={year}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 2));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                while (await reader.ReadAsync())
                {
                    var division_id = reader.GetInt32("division_id");
                    var division_name = reader.GetString("division_name");
                    var owner_name = reader.GetString("owner_name");
                    double? koef_roylty = reader.IsDBNull("koef_roylty") ? null : reader.GetDouble("koef_roylty");
                    double? koef_calc = reader.IsDBNull("koef_calc") ? null : reader.GetDouble("koef_calc");
                    double? discount_for_buy = reader.IsDBNull("discount_for_buy") ? null : reader.GetDouble("discount_for_buy");
                    double? discount_loyalty = reader.IsDBNull("discount_loyalty") ? null : reader.GetDouble("discount_loyalty");
                    double? discount_for_buy_other = reader.IsDBNull("discount_for_buy_other") ? null : reader.GetDouble("discount_for_buy_other");
                    results.Add(new()
                    {
                        division_id = division_id,
                        division_name = division_name,
                        owner_name = owner_name,
                        koef_roylty = koef_roylty,
                        koef_calc = koef_calc,
                        discount_for_buy = discount_for_buy,
                        discount_for_buy_other = discount_for_buy_other,
                        discount_loyalty = discount_loyalty

                    });
                }
                return results;
            }
        }


        /// <summary>
        /// Сохранение основных параметров Роялти
        /// mount_rep - месяц
        /// year_rep - год
        /// koef_roylty - Роялти итоговый(расчётный)
        /// koef_calc - Коофицент для расчета
        /// nds_st - ставка НДС
        /// divisions - список подразделений
        /// </summary>       
        public async Task<SpExecuteResult> SetRolayltyMainSettings(int year, int month, float koef_calc, float koef_roylty, float nds_st, List<int> division_ids)
        {
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltySetting]";
            var connectionString = GetConnectionString();
            string param_setting = new
            {
                year_rep = year,
                month_rep = month,
                koef_roylty = koef_roylty,
                koef_calc = koef_calc,
                nds_st = nds_st,
                divisions = division_ids.Select(div_id => new { division_id = div_id }).ToList()
            }.ToJson();
            logger.LogInformation($"exec {procName} @code=1, @param_setting='{param_setting}'");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 1));
                parameters.Add(new("@param_setting", param_setting));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new()
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message")
                    };
                }
                else
                {
                    throw new Exception($"Процедура exec {procName} @code=1, @param_setting='{param_setting}' ничего не вернула.");
                }
            }
        }


        /// <summary>
        /// Сохранение параметров расчёта Роялти
        /// mount_rep - месяц
        /// year_rep - год
        /// nds_st - ставка НДС
        /// discount_for_buy - Скидка за закупки
        /// proc_commitment - Процент приверженцев(лояльность)
        /// proc_critics - Процент критиков(лояльность)
        /// proc_loyalty - Скидка лояльности
        /// divisions - список подразделений
        /// </summary>       
        public async Task<SpExecuteResult> SetRolayltyCalculationSettings(int year, int month, float discount_for_buy, float discount_for_buy_other, float discount_loyalty, List<int> division_ids)
        {
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltySetting]";
            var connectionString = GetConnectionString();
            string param_setting = new
            {
                year_rep = year,
                month_rep = month,
                discount_for_buy = discount_for_buy,
                discount_for_buy_other = discount_for_buy_other,
                discount_loyalty = discount_loyalty,
                divisions = division_ids.Select(div_id => new { division_id = div_id }).ToList()
            }.ToJson();
            logger.LogInformation($"exec {procName} @code=2, @param_setting='{param_setting}'");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 2));
                parameters.Add(new("@param_setting", param_setting));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new()
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message")
                    };
                }
                else
                {
                    throw new Exception($"Процедура exec {procName} @code=2, @param_setting='{param_setting}' ничего не вернула.");
                }
            }
            ;
        }


        /// <summary>
        /// Получение сумм оборотов
        /// </summary>
        /// <param name="year">год</param>
        /// <param name="month">месяц</param>
        /// <param name="div_id">подразделение или 0 - все</param>
        /// <returns></returns>
        public async Task<List<RoyaltyCashSummsItem>> GetRoyaltyCashSumms(int year, int month, int div_id = 0)
        {
            List<RoyaltyCashSummsItem> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSelect]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec {procName} @code=4, @division_id={div_id}, @month_rep={month}, @year_rep={year}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 4));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new()
                    {
                        division_id = reader.GetInt32("division_id"),
                        division_name = reader.IsDBNull("division_name") ? "" : reader.GetString("division_name"),
                        owner_name = reader.IsDBNull("owner_name") ? "" : reader.GetString("owner_name"),
                        sum_work_buh = reader.IsDBNull("sum_work_buh") ? 0 : reader.GetDouble("sum_work_buh"),
                        sum_part_buh = reader.IsDBNull("sum_part_buh") ? 0 : reader.GetDouble("sum_part_buh"),
                        not_edit = !reader.IsDBNull("notEdit") && Convert.ToBoolean(reader["notEdit"])
                    });
                }
                return results;
            }
        }

        /// <summary>
        /// Суммы закупок EA и сторонних поставщиков.
        /// RoyaltyReportSelect @code=5: division_id, sum_pur_ea, sum_pur_other.
        /// </summary>
        public async Task<List<RoyaltyPurSummsItem>> GetRoyaltyPurSumms(int year, int month, int div_id = 0)
        {
            List<RoyaltyPurSummsItem> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSelect]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec {procName} @code=5, @division_id={div_id}, @month_rep={month}, @year_rep={year}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 5));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new()
                    {
                        division_id = reader.GetInt32("division_id"),
                        sum_pur_ea = reader.GetDouble("sum_pur_ea"),
                        sum_pur_other = reader.GetDouble("sum_pur_other"),
                    });
                }
                return results;
            }
        }


        /// <summary>
        /// Сохранение сумм для оборотов, введенные бухгалтерами подразделений ЕА
        /// </summary>
        /// @code int=2,
        /// @division_id int - подразделение,
        /// @month_rep int - месяц отчета,
        /// @year_rep int - год отчета,
        /// @sum_work_buh float - сумма оборотов по услугам,
        /// @sum_part_buh float - сумма оборотов по запчастям
        public async Task<SpExecuteResult> SеtRoyaltyCashSumms(int year, int month, int div_id, float sum_work_buh, float sum_part_buh)
        {
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSetting]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec [webapibuhreport].[RoyaltyReportSetting] @code=2, @division_id={div_id}, @year_rep={year}, @month_rep={month}, @sum_work_buh={sum_work_buh}, @sum_part_buh={sum_part_buh}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 2));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
                parameters.Add(new("@sum_work_buh", sum_work_buh));
                parameters.Add(new("@sum_part_buh", sum_part_buh));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new()
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message")
                    };
                }
                else
                {
                    throw new Exception($"Процедура exec [webapibuhreport].[RoyaltyReportSetting] @code=2, @division_id={div_id}, @year_rep={year}, @month_rep={month}, @sum_work_buh={sum_work_buh}, @sum_part_buh={sum_part_buh} ничего не вернула.");
                }
            }
        }


        /// <summary>
        /// Установка/снятие признака "Согласовано"
        /// </summary>
        /// <param name="year">год</param>
        /// <param name="month">месяц</param>
        /// <param name="not_edit">согласовано или нет</param>
        /// <param name="division_ids">идентификаторы подразделений</param>
        public async Task<SpExecuteResult> SеtRoyaltyCashSummsEdit(int year, int month, bool not_edit, List<int> division_ids)
        {
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltySetting]";
            var connectionString = GetConnectionString();
            var param_setting = new
            {
                notEdit = not_edit ? 1 : 0,
                year_rep = year,
                month_rep = month,
                divisions = division_ids.Select(div_id => new { division_id = div_id }).ToList()
            }.ToJson();
            logger.LogInformation($"exec {procName} @code=3, @param_setting='{param_setting}'");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 3));
                parameters.Add(new("@param_setting", param_setting));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new()
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message")
                    };
                }
                else
                {
                    throw new Exception($"Процедура exec {procName} @code=3, @param_setting='{param_setting}' ничего не вернула.");
                }
            }
        }


        /// <summary>
        /// Формирование отчета Роялти по подразделению
        /// div_id - подразделение,
        /// month - месяц отчета,
        /// year - год отчета
        /// </summary>       
        public async Task<SpExecuteResult> CreateRoyaltyReport(int year, int month, int div_id)
        {

            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSetting]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec [webapibuhreport].[RoyaltyReportSetting] @code=1, @division_id={div_id}, @year_rep={year}, @month_rep={month}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 1));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new()
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message")
                    };
                }
                else
                {
                    throw new Exception($"Процедура exec [webapibuhreport].[RoyaltyReportSetting] @code=1, @division_id={div_id}, @year_rep={year}, @month_rep={month} ничего не вернула.");
                }
            }
            ;
        }


        /// <summary>
        /// Получение данных отчета по подразделению за месяц
        /// div_id - подразделение,
        /// month - месяц отчета,
        /// year - год отчета
        /// </summary>       
        public async Task<List<RoyaltyReportItem>> GetRoyaltyReport(int year, int month, int div_id)
        {
            List<RoyaltyReportItem> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyReportSelect]";
            var connectionString = GetConnectionString();
            logger.LogInformation($"exec {procName} @code=3, @division_id={div_id}, @month_rep={month}, @year_rep={year}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, System.Data.CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 3));
                parameters.Add(new("@division_id", div_id));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
            }))
            {
                while (await reader.ReadAsync())
                    results.Add(ReadRoyaltyReportItem(reader));
            }
            return results;
        }



        /// <summary>
        /// Получение данных отчета по списку подразделений.
        /// RoyaltyReportSelect не принимает @divison_list — фильтруем code=3.
        /// </summary>
        public async Task<List<RoyaltyReportItem>> GetRoyaltyReportByList(int year, int month, List<int> div_ids)
        {
            var results = await GetRoyaltyReport(year, month, 0);
            if (div_ids == null || div_ids.Count == 0)
                return results;
            var ids = div_ids.Where(id => id != 0).ToHashSet();
            if (ids.Count == 0)
                return results;
            return results.Where(item => ids.Contains(item.division_id)).ToList();
        }


        /// <summary>
        /// Данные для печати счетов. RoyaltyDocumentSelect @code=1.
        /// division_id: "0"/пусто — все; "613,621"; либо JSON.
        /// </summary>
        public async Task<List<RoyaltyInvoiceDocumentItem>> GetRoyaltyInvoiceDocuments(
            int year, int month, string? division_id = "0")
        {
            return await ReadRoyaltyDocuments(1, year, month, division_id, ReadInvoiceDocument);
        }


        /// <summary>
        /// Данные для печати справок. RoyaltyDocumentSelect @code=2.
        /// </summary>
        public async Task<List<RoyaltyCertificateDocumentItem>> GetRoyaltyCertificateDocuments(
            int year, int month, string? division_id = "0")
        {
            return await ReadRoyaltyDocuments(2, year, month, division_id, ReadCertificateDocument);
        }

        public static string ResolveDivisionIdParam(string? division_id, List<int>? division_ids)
        {
            if (division_ids != null && division_ids.Count > 0)
            {
                var joined = string.Join(",", division_ids.Where(id => id != 0).Distinct());
                return string.IsNullOrEmpty(joined) ? "0" : joined;
            }
            if (string.IsNullOrWhiteSpace(division_id))
                return "0";
            return division_id.Trim();
        }

        /// <summary>
        /// Карточки подразделений. RoyaltySelectDivisionParams @code=1.
        /// division_id = 0 и owner_id = 0 — все записи.
        /// </summary>
        public async Task<List<RoyaltyDivisionCard>> GetRoyaltyDivisions(int division_id = 0, int owner_id = 0)
        {
            List<RoyaltyDivisionCard> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltySelectDivisionParams]";
            var connectionString = GetConnectionString();
            string param_setting = new[] { new { division_id, owner_id } }.ToJson();
            logger.LogInformation($"exec {procName} @code=1, @param_setting='{param_setting}'");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 1));
                parameters.Add(new("@param_setting", param_setting));
            }))
            {
                while (await reader.ReadAsync())
                {
                    results.Add(new RoyaltyDivisionCard
                    {
                        division_id = reader.GetInt32("division_id"),
                        owner_id = ReadInt(reader, "id_owner", "owner_id"),
                        owner_name = reader.IsDBNull("owner_name") ? string.Empty : reader.GetString("owner_name"),
                        address_div = reader.IsDBNull("address_div") ? string.Empty : reader.GetString("address_div"),
                        email = reader.IsDBNull("email") ? string.Empty : reader.GetString("email"),
                        owner_inn = reader.IsDBNull("owner_inn") ? string.Empty : reader.GetString("owner_inn"),
                        owner_kpp = reader.IsDBNull("owner_kpp") ? string.Empty : reader.GetString("owner_kpp"),
                        owner_ogrn = reader.IsDBNull("owner_ogrn") ? string.Empty : reader.GetString("owner_ogrn"),
                        contract_num = ReadOptionalString(reader, "contract_num"),
                        contract_date = ReadOptionalDate(reader, "contract_date"),
                    });
                }
            }
            return results;
        }

        /// <summary>
        /// Вставка или обновление карточек. RoyaltyDivisionEdit @code=1.
        /// </summary>
        public async Task<SpExecuteResult> EditRoyaltyDivisions(IReadOnlyList<RoyaltyDivisionCard> cards)
        {
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyDivisionEdit]";
            var connectionString = GetConnectionString();
            string param_setting = cards.Select(card => new
            {
                card.division_id,
                card.owner_id,
                owner_name = card.owner_name ?? string.Empty,
                address_div = card.address_div ?? string.Empty,
                email = card.email ?? string.Empty,
                owner_inn = card.owner_inn ?? string.Empty,
                owner_kpp = card.owner_kpp ?? string.Empty,
                owner_ogrn = card.owner_ogrn ?? string.Empty,
                contract_num = card.contract_num ?? string.Empty,
                contract_date = FormatContractDateForDb(card.contract_date),
            }).ToJson();
            logger.LogInformation($"exec {procName} @code=1, @param_setting='{param_setting}'");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", 1));
                parameters.Add(new("@param_setting", param_setting));
            }))
            {
                if (await reader.ReadAsync())
                {
                    return new SpExecuteResult
                    {
                        error = reader.GetInt32("error"),
                        err_message = reader.IsDBNull("err_message") ? string.Empty : reader.GetString("err_message"),
                    };
                }
                throw new Exception($"Процедура exec {procName} @code=1 ничего не вернула.");
            }
        }

        private static bool HasColumn(DbDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string ReadOptionalString(DbDataReader reader, string name)
        {
            if (!HasColumn(reader, name) || reader.IsDBNull(name))
                return string.Empty;
            return reader.GetString(name);
        }

        private static int ReadInt(DbDataReader reader, params string[] names)
        {
            foreach (var name in names)
            {
                if (!HasColumn(reader, name) || reader.IsDBNull(name))
                    continue;
                return reader.GetInt32(name);
            }
            return 0;
        }

        /// <summary>
        /// В таблице contract_date — строка дд.мм.гггг, В API отдаём yyyy-MM-dd для даты
        /// </summary>
        private static string ReadOptionalDate(DbDataReader reader, string name)
        {
            if (!HasColumn(reader, name) || reader.IsDBNull(name))
                return string.Empty;
            var value = reader[name];
            if (value is DateTime date)
                return date.ToString("yyyy-MM-dd");
            return NormalizeContractDateFromDb(Convert.ToString(value));
        }

        private static readonly string[] ContractDateFormats = ["dd.MM.yyyy", "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss"];

        private static string NormalizeContractDateFromDb(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            text = text.Trim();
            if (DateTime.TryParseExact(text, ContractDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed.ToString("yyyy-MM-dd");
            return text;
        }

        private static string FormatContractDateForDb(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var text = value.Trim();
            if (DateTime.TryParseExact(text, ContractDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed.ToString("dd.MM.yyyy");
            return text;
        }

        private async Task<List<T>> ReadRoyaltyDocuments<T>(
            int code, int year, int month, string? division_id, Func<System.Data.Common.DbDataReader, T> map)
        {
            List<T> results = new();
            var helper = new MSSqlHelper();
            var procName = "[webapibuhreport].[RoyaltyDocumentSelect]";
            var connectionString = GetConnectionString();
            var divParam = string.IsNullOrWhiteSpace(division_id) ? "0" : division_id.Trim();
            logger.LogInformation($"exec {procName} @code={code}, @month_rep={month}, @year_rep={year}, @division_id={divParam}");
            using (var reader = await helper.ExecuteReaderAsync(connectionString, procName, CommandType.StoredProcedure, parameters =>
            {
                parameters.Add(new("@code", code));
                parameters.Add(new("@month_rep", month));
                parameters.Add(new("@year_rep", year));
                parameters.Add(new("@division_id", divParam));
            }))
            {
                while (await reader.ReadAsync())
                    results.Add(map(reader));
            }
            return results;
        }

        private static RoyaltyReportItem ReadRoyaltyReportItem(DbDataReader reader) => new()
        {
            division_id = reader.GetInt32("division_id"),
            division_name = reader.IsDBNull("division_name") ? null : reader.GetString("division_name"),
            owner_name = reader.IsDBNull("owner_name") ? null : reader.GetString("owner_name"),
            koef_roylty = reader.IsDBNull("koef_roylty") ? null : reader.GetDouble("koef_roylty"),
            koef_calc = reader.IsDBNull("koef_calc") ? null : reader.GetDouble("koef_calc"),
            nds_st = reader.IsDBNull("nds_st") ? null : reader.GetDouble("nds_st"),
            sum_proceeds = reader.IsDBNull("sum_proceeds") ? null : reader.GetDouble("sum_proceeds"),
            sum_work_buh = reader.IsDBNull("sum_work_buh") ? 0 : reader.GetDouble("sum_work_buh"),
            sum_part_buh = reader.IsDBNull("sum_part_buh") ? 0 : reader.GetDouble("sum_part_buh"),
            sum_pur_ea = reader.IsDBNull("sum_pur_ea") ? null : reader.GetDouble("sum_pur_ea"),
            sum_pur_other = reader.IsDBNull("sum_pur_other") ? null : reader.GetDouble("sum_pur_other"),
            sum_buy_ea = reader.IsDBNull("sum_buy_ea") ? null : reader.GetDouble("sum_buy_ea"),
            sum_buy_other = reader.IsDBNull("sum_buy_other") ? null : reader.GetDouble("sum_buy_other"),
            count_run = reader.IsDBNull("count_run") ? 0 : reader.GetInt32("count_run"),
            ratio_pretensions = reader.IsDBNull("ratio_pretensions") ? null : reader.GetDouble("ratio_pretensions"),
            ratio_pur_ea = reader.IsDBNull("ratio_pur_ea") ? null : reader.GetDouble("ratio_pur_ea"),
            sum_roylty = reader.IsDBNull("sum_roylty") ? null : reader.GetDouble("sum_roylty"),
            sum_roylty_nds = reader.IsDBNull("sum_roylty_nds") ? null : reader.GetDouble("sum_roylty_nds"),
            sum_smb = reader.IsDBNull("sum_smb") ? null : reader.GetDouble("sum_smb"),
            roylty_calc = reader.IsDBNull("roylty_calc") ? null : reader.GetDouble("roylty_calc"),
            index_loality = reader.IsDBNull("index_loality") ? null : reader.GetDouble("index_loality"),
            discount_for_buy = reader.IsDBNull("discount_for_buy") ? null : reader.GetDouble("discount_for_buy"),
            discount_for_buy_other = reader.IsDBNull("discount_for_buy_other") ? null : reader.GetDouble("discount_for_buy_other"),
            discount_loyalty = reader.IsDBNull("discount_loyalty") ? null : reader.GetDouble("discount_loyalty"),
            K2 = reader.IsDBNull("K2") ? null : reader.GetDouble("K2"),
            K3 = reader.IsDBNull("K3") ? null : reader.GetDouble("K3"),
            K4 = reader.IsDBNull("K4") ? null : reader.GetDouble("K4")
        };

        private static RoyaltyInvoiceDocumentItem ReadInvoiceDocument(DbDataReader reader) => new()
        {
            division_id = reader.GetInt32("division_id"),
            division_name = reader.IsDBNull("division_name") ? null : reader.GetString("division_name"),
            bank = reader.IsDBNull("bank") ? null : reader.GetString("bank"),
            bik = reader.IsDBNull("bik") ? null : reader.GetString("bik"),
            ks = reader.IsDBNull("ks") ? null : reader.GetString("ks"),
            rs = reader.IsDBNull("rs") ? null : reader.GetString("rs"),
            recipient_name = reader.IsDBNull("recipient_name") ? null : reader.GetString("recipient_name"),
            recipient_inn = reader.IsDBNull("recipient_inn") ? null : reader.GetString("recipient_inn"),
            recipient_kpp = reader.IsDBNull("recipient_kpp") ? null : reader.GetString("recipient_kpp"),
            recipient_address = reader.IsDBNull("recipient_address") ? null : reader.GetString("recipient_address"),
            recipient_phone = reader.IsDBNull("recipient_phone") ? null : reader.GetString("recipient_phone"),
            user_name = reader.IsDBNull("user_name") ? null : reader.GetString("user_name"),
            user_ogrn = reader.IsDBNull("user_ogrn") ? null : reader.GetString("user_ogrn"),
            user_inn = reader.IsDBNull("user_inn") ? null : reader.GetString("user_inn"),
            user_address = reader.IsDBNull("user_address") ? null : reader.GetString("user_address"),
            contract_num = reader.IsDBNull("contract_num") ? null : reader.GetString("contract_num"),
            contract_date = reader.IsDBNull("contract_date") ? null : reader.GetDateTime("contract_date"),
            invoice_num = reader.IsDBNull("invoice_num") ? null : reader.GetString("invoice_num"),
            invoice_date = reader.IsDBNull("invoice_date") ? null : reader.GetDateTime("invoice_date"),
            period_month = reader.IsDBNull("period_month") ? 0 : reader.GetInt32("period_month"),
            period_year = reader.IsDBNull("period_year") ? 0 : reader.GetInt32("period_year"),
            sum_roylty = reader.IsDBNull("sum_roylty") ? 0 : reader.GetDouble("sum_roylty"),
            nds_st = reader.IsDBNull("nds_st") ? 0 : reader.GetDouble("nds_st")
        };

        private static RoyaltyCertificateDocumentItem ReadCertificateDocument(DbDataReader reader) => new()
        {
            division_id = reader.GetInt32("division_id"),
            division_name = reader.IsDBNull("division_name") ? null : reader.GetString("division_name"),
            owner_name = reader.IsDBNull("owner_name") ? null : reader.GetString("owner_name"),
            owner_inn = reader.IsDBNull("owner_inn") ? null : reader.GetString("owner_inn"),
            org_name = reader.IsDBNull("org_name") ? null : reader.GetString("org_name"),
            org_inn = reader.IsDBNull("org_inn") ? null : reader.GetString("org_inn"),
            org_ogrn = reader.IsDBNull("org_ogrn") ? null : reader.GetString("org_ogrn"),
            org_address = reader.IsDBNull("org_address") ? null : reader.GetString("org_address"),
            contract_num = reader.IsDBNull("contract_num") ? null : reader.GetString("contract_num"),
            period_month = reader.IsDBNull("period_month") ? 0 : reader.GetInt32("period_month"),
            period_year = reader.IsDBNull("period_year") ? 0 : reader.GetInt32("period_year"),
            cert_date = reader.IsDBNull("cert_date") ? null : reader.GetDateTime("cert_date"),
            cert_day = reader.IsDBNull("cert_day") ? 0 : reader.GetInt32("cert_day"),
            cert_month = reader.IsDBNull("cert_month") ? 0 : reader.GetInt32("cert_month"),
            cert_year = reader.IsDBNull("cert_year") ? 0 : reader.GetInt32("cert_year"),
            sum_proceeds = reader.IsDBNull("sum_proceeds") ? 0 : reader.GetDouble("sum_proceeds"),
            koef_roylty = reader.IsDBNull("koef_roylty") ? 0 : reader.GetDouble("koef_roylty"),
            sum_buy_ea = reader.IsDBNull("sum_buy_ea") ? null : reader.GetDouble("sum_buy_ea"),
            sum_buy_other = reader.IsDBNull("sum_buy_other") ? null : reader.GetDouble("sum_buy_other"),
            sum_pur_ea = reader.IsDBNull("sum_pur_ea") ? null : reader.GetDouble("sum_pur_ea"),
            sum_pur_other = reader.IsDBNull("sum_pur_other") ? null : reader.GetDouble("sum_pur_other"),
            count_run = reader.IsDBNull("count_run") ? 0 : reader.GetInt32("count_run"),
            pretensions = reader.IsDBNull("pretensions") ? 0 : Convert.ToInt32(reader["pretensions"]),
            ratio_pretensions = reader.IsDBNull("ratio_pretensions") ? 0 : reader.GetDouble("ratio_pretensions"),
            K2 = reader.IsDBNull("K2") ? 0 : reader.GetDouble("K2"),
            K3 = reader.IsDBNull("K3") ? 0 : reader.GetDouble("K3"),
            K4 = reader.IsDBNull("K4") ? 0 : reader.GetDouble("K4"),
            sum_roylty = reader.IsDBNull("sum_roylty") ? 0 : reader.GetDouble("sum_roylty"),
            nds_st = reader.IsDBNull("nds_st") ? 0 : reader.GetDouble("nds_st")
        };
    }


    public static class Extensions
    {
        public static string ToJson(this object target)
        {
            return JsonConvert.SerializeObject(target);
        }
    }
}
