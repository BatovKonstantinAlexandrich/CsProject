using BlazorErp.Models;
using BlazorErp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.EnsureCreatedAsync();
        await EnsureSchemaUpgradesAsync(context);

        foreach (var role in RoleNames.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await SeedUsersAsync(userManager);
        await SeedCatalogAsync(context);
        await EnsureSupplierDataAsync(context, userManager);
        await EnsureAutoServiceDataAsync(context);
        await EnsureCashierDemoDataAsync(context, userManager);
        await EnsureMarketingDemoDataAsync(context);
        await EnsureReportsDemoDataAsync(context);
        await SeedNewsAsync(context, userManager);
    }

    private static async Task EnsureSchemaUpgradesAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS StoreProducts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StoreId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                FOREIGN KEY (StoreId) REFERENCES Stores(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_StoreProducts_StoreId_ProductId
            ON StoreProducts(StoreId, ProductId);
            """);

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Stores ADD COLUMN Description TEXT NULL");
        }
        catch { }

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Stores ADD COLUMN ImageUrl TEXT NULL");
        }
        catch { }

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS StoreSellerRegistrationRequests (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                StoreId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                ProcessedAt TEXT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (StoreId) REFERENCES Stores(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS StoreSellers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                StoreId INTEGER NOT NULL,
                AssignedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (StoreId) REFERENCES Stores(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_StoreSellers_UserId
            ON StoreSellers(UserId);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS StoreCashierRegistrationRequests (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                StoreId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                ProcessedAt TEXT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (StoreId) REFERENCES Stores(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS StoreCashiers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                StoreId INTEGER NOT NULL,
                AssignedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (StoreId) REFERENCES Stores(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_StoreCashiers_UserId
            ON StoreCashiers(UserId);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServices (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT NULL,
                Description TEXT NULL,
                Phone TEXT NULL,
                OwnerId TEXT NOT NULL,
                FOREIGN KEY (OwnerId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServiceOfferings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AutoServiceId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Description TEXT NULL,
                Price REAL NOT NULL,
                DurationMinutes INTEGER NOT NULL,
                IsActive INTEGER NOT NULL,
                FOREIGN KEY (AutoServiceId) REFERENCES AutoServices(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServiceStaffMembers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AutoServiceId INTEGER NOT NULL,
                FullName TEXT NOT NULL,
                Position TEXT NOT NULL,
                Phone TEXT NULL,
                Email TEXT NULL,
                IsActive INTEGER NOT NULL,
                FOREIGN KEY (AutoServiceId) REFERENCES AutoServices(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServiceAppointments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AutoServiceId INTEGER NOT NULL,
                OfferingId INTEGER NULL,
                StaffMemberId INTEGER NULL,
                ClientName TEXT NOT NULL,
                ClientPhone TEXT NULL,
                ScheduledAt TEXT NOT NULL,
                DurationMinutes INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                Notes TEXT NULL,
                FOREIGN KEY (AutoServiceId) REFERENCES AutoServices(Id),
                FOREIGN KEY (OfferingId) REFERENCES AutoServiceOfferings(Id),
                FOREIGN KEY (StaffMemberId) REFERENCES AutoServiceStaffMembers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServiceEmployeeRegistrationRequests (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                AutoServiceId INTEGER NOT NULL,
                StaffRole TEXT NOT NULL,
                Status INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                ProcessedAt TEXT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (AutoServiceId) REFERENCES AutoServices(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS AutoServiceEmployees (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                AutoServiceId INTEGER NOT NULL,
                StaffRole TEXT NOT NULL,
                AssignedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (AutoServiceId) REFERENCES AutoServices(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_AutoServiceEmployees_UserId
            ON AutoServiceEmployees(UserId);
            """);

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Categories ADD COLUMN ParentId INTEGER NULL");
        }
        catch { }

        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "ALTER TABLE Products ADD COLUMN BrandId INTEGER NULL");
        }
        catch { }

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS Brands (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS Suppliers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                ContactInfo TEXT NULL,
                OwnerId TEXT NOT NULL,
                FOREIGN KEY (OwnerId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SupplierBrands (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SupplierId INTEGER NOT NULL,
                BrandId INTEGER NOT NULL,
                FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
                FOREIGN KEY (BrandId) REFERENCES Brands(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_SupplierBrands_SupplierId_BrandId
            ON SupplierBrands(SupplierId, BrandId);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SupplierProducts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SupplierId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                StockQuantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_SupplierProducts_SupplierId_ProductId
            ON SupplierProducts(SupplierId, ProductId);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SupplierOrders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StoreId INTEGER NOT NULL,
                SupplierId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                TotalAmount REAL NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (StoreId) REFERENCES Stores(Id),
                FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SupplierOrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SupplierOrderId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                FOREIGN KEY (SupplierOrderId) REFERENCES SupplierOrders(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS Feedbacks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Subject TEXT NOT NULL,
                Body TEXT NOT NULL,
                UserId TEXT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS NewsItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Content TEXT NOT NULL,
                AuthorId TEXT NOT NULL,
                PublishedAt TEXT NOT NULL,
                IsPublished INTEGER NOT NULL,
                FOREIGN KEY (AuthorId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS NewsAttachments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NewsItemId INTEGER NOT NULL,
                OriginalFileName TEXT NOT NULL,
                StoredFileName TEXT NOT NULL,
                ContentType TEXT NOT NULL,
                MediaType INTEGER NOT NULL,
                FileSize INTEGER NOT NULL,
                FOREIGN KEY (NewsItemId) REFERENCES NewsItems(Id)
            );
            """);

        await EnsureCashierSchemaAsync(context);
    }

    private static async Task EnsureCashierSchemaAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SaleBaskets (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderUid TEXT NOT NULL,
                BasketNumber TEXT NOT NULL,
                StoreId INTEGER NOT NULL,
                CashierUserId TEXT NULL,
                ClientName TEXT NOT NULL,
                ClientPhone TEXT NULL,
                ClientEmail TEXT NULL,
                Status INTEGER NOT NULL,
                TotalAmount REAL NOT NULL,
                PaidAmount REAL NOT NULL,
                RefundSum REAL NOT NULL,
                IsReceiptPrinted INTEGER NOT NULL,
                OrderId INTEGER NULL,
                CreatedAt TEXT NOT NULL,
                PaidAt TEXT NULL,
                FOREIGN KEY (StoreId) REFERENCES Stores(Id),
                FOREIGN KEY (CashierUserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (OrderId) REFERENCES Orders(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_SaleBaskets_BasketNumber ON SaleBaskets(BasketNumber);
            CREATE UNIQUE INDEX IF NOT EXISTS IX_SaleBaskets_OrderUid ON SaleBaskets(OrderUid);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS SaleBasketItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SaleBasketId INTEGER NOT NULL,
                ProductId INTEGER NOT NULL,
                ProductName TEXT NOT NULL,
                Quantity INTEGER NOT NULL,
                UnitPrice REAL NOT NULL,
                LineTotal REAL NOT NULL,
                FOREIGN KEY (SaleBasketId) REFERENCES SaleBaskets(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS CashPayments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SaleBasketId INTEGER NULL,
                OrderId INTEGER NULL,
                ExternalPaymentId TEXT NOT NULL,
                PaymentUrl TEXT NULL,
                PhoneNumber TEXT NULL,
                Amount REAL NOT NULL,
                RefundSum REAL NOT NULL,
                Status INTEGER NOT NULL,
                Method INTEGER NOT NULL,
                PaymentSystemName TEXT NULL,
                ProcessedByUserId TEXT NULL,
                CreatedAt TEXT NOT NULL,
                CompletedAt TEXT NULL,
                StatusDate TEXT NULL,
                FOREIGN KEY (SaleBasketId) REFERENCES SaleBaskets(Id),
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (ProcessedByUserId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_CashPayments_ExternalPaymentId ON CashPayments(ExternalPaymentId);
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS CashRegisterEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StoreId INTEGER NOT NULL,
                SaleBasketId INTEGER NULL,
                OrderId INTEGER NULL,
                CashPaymentId INTEGER NULL,
                Amount REAL NOT NULL,
                ActionType INTEGER NOT NULL,
                PaymentMethod INTEGER NOT NULL,
                ReceiptNumber TEXT NOT NULL,
                CashierUserId TEXT NOT NULL,
                Notes TEXT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (StoreId) REFERENCES Stores(Id),
                FOREIGN KEY (SaleBasketId) REFERENCES SaleBaskets(Id),
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (CashPaymentId) REFERENCES CashPayments(Id),
                FOREIGN KEY (CashierUserId) REFERENCES AspNetUsers(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS PaymentLinkLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SaleBasketId INTEGER NULL,
                OrderId INTEGER NULL,
                CashPaymentId INTEGER NULL,
                Channel INTEGER NOT NULL,
                Destination TEXT NOT NULL,
                PaymentUrl TEXT NOT NULL,
                IsSuccess INTEGER NOT NULL,
                ErrorMessage TEXT NULL,
                SentByUserId TEXT NOT NULL,
                SentAt TEXT NOT NULL,
                FOREIGN KEY (SaleBasketId) REFERENCES SaleBaskets(Id),
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (CashPaymentId) REFERENCES CashPayments(Id),
                FOREIGN KEY (SentByUserId) REFERENCES AspNetUsers(Id)
            );
            """);

        foreach (var column in new[]
        {
            "ALTER TABLE Payments ADD COLUMN OrderUid TEXT NULL",
            "ALTER TABLE Payments ADD COLUMN RefundSum REAL NOT NULL DEFAULT 0",
            "ALTER TABLE Payments ADD COLUMN PaymentUrl TEXT NULL",
            "ALTER TABLE Payments ADD COLUMN PhoneNumber TEXT NULL",
            "ALTER TABLE Payments ADD COLUMN ProcessedByUserId TEXT NULL",
            "ALTER TABLE Payments ADD COLUMN PaymentSourceId INTEGER NULL",
            "ALTER TABLE Payments ADD COLUMN PaymentSystemName TEXT NULL"
        })
        {
            try { await context.Database.ExecuteSqlRawAsync(column); }
            catch { }
        }

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS MailingConfigurations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SettingsName TEXT NOT NULL,
                MarketerName TEXT NOT NULL,
                Status INTEGER NOT NULL,
                MailingStartDate TEXT NULL,
                AudienceTotal INTEGER NOT NULL,
                AudienceSent INTEGER NOT NULL,
                TimeoutMinutes INTEGER NOT NULL,
                ExportFileName TEXT NULL,
                LastEditDate TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS MailingTestAudiences (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MailingConfigurationId INTEGER NOT NULL,
                Phone TEXT NOT NULL,
                MessengersOrder TEXT NOT NULL,
                FallbackToSms INTEGER NOT NULL,
                IsActive INTEGER NOT NULL,
                FOREIGN KEY (MailingConfigurationId) REFERENCES MailingConfigurations(Id)
            );
            """);

        await EnsureReportsSchemaAsync(context);
    }

    private static async Task EnsureReportsSchemaAsync(ApplicationDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ReportGroups (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ReportProcedures (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                GroupId INTEGER NOT NULL,
                ProcedureName TEXT NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT NULL,
                DiagramType INTEGER NOT NULL,
                ChartXField TEXT NULL,
                ChartYField TEXT NULL,
                IsHidden INTEGER NOT NULL,
                FOREIGN KEY (GroupId) REFERENCES ReportGroups(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ReportParameters (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReportId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Title TEXT NOT NULL,
                Type INTEGER NOT NULL,
                DefaultValue TEXT NULL,
                LookupValues TEXT NULL,
                SortOrder INTEGER NOT NULL,
                FOREIGN KEY (ReportId) REFERENCES ReportProcedures(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ReportColumns (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ReportId INTEGER NOT NULL,
                ColumnName TEXT NOT NULL,
                Title TEXT NOT NULL,
                Format TEXT NULL,
                ShowSum INTEGER NOT NULL,
                SortOrder INTEGER NOT NULL,
                FOREIGN KEY (ReportId) REFERENCES ReportProcedures(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS ReportUserFavorites (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                ReportId INTEGER NOT NULL,
                FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
                FOREIGN KEY (ReportId) REFERENCES ReportProcedures(Id)
            );
            """);

        await context.Database.ExecuteSqlRawAsync("""
            CREATE UNIQUE INDEX IF NOT EXISTS IX_ReportUserFavorites_UserId_ReportId
            ON ReportUserFavorites(UserId, ReportId);
            """);
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserIfNotExists(userManager, "client@demo.ru", "Client123!", "Иван Клиентов", RoleNames.Client);
        await CreateUserIfNotExists(userManager, "transport@demo.ru", "Transport123!", "ТК Быстрая Доставка", RoleNames.TransportCompany);
        await CreateUserIfNotExists(userManager, "store@demo.ru", "Store123!", "Пётр Магазинов", RoleNames.StoreOwner);
        await CreateUserIfNotExists(userManager, "store2@demo.ru", "Store123!", "Анна Ритейл", RoleNames.StoreOwner);
        await CreateUserIfNotExists(userManager, "cashier@demo.ru", "Cashier123!", "Ольга Кассирова", RoleNames.Cashier);
        await CreateUserIfNotExists(userManager, "supplier@demo.ru", "Supplier123!", "ООО ТехОпт", RoleNames.Supplier);
        await CreateUserIfNotExists(userManager, "supplier2@demo.ru", "Supplier123!", "Одежда Опт", RoleNames.Supplier);
        await CreateUserIfNotExists(userManager, "autoservice@demo.ru", "Auto123!", "Сергей Автосервисов", RoleNames.AutoServiceOwner);
        await CreateUserIfNotExists(userManager, "admin@demo.ru", "Admin123!", "Администратор", RoleNames.Admin);
        await CreateUserIfNotExists(userManager, "developer@demo.ru", "Dev123!", "Разработчик", RoleNames.Developer);
        await CreateUserIfNotExists(userManager, "support@demo.ru", "Support123!", "Алексей Поддержкин", RoleNames.TechnicalSupport);
        await CreateUserIfNotExists(userManager, "callcenter@demo.ru", "CallCenter123!", "Мария Операторова", RoleNames.CallCenter);
        await CreateUserIfNotExists(userManager, "marketing@demo.ru", "Marketing123!", "Елена Маркетова", RoleNames.Marketing);
        await CreateUserIfNotExists(userManager, "analyst@demo.ru", "Analyst123!", "Игорь Аналитиков", RoleNames.Analyst);
    }

    private static async Task CreateUserIfNotExists(
        UserManager<ApplicationUser> userManager,
        string email,
        string password,
        string fullName,
        string role)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }

    private static async Task SeedCatalogAsync(ApplicationDbContext context)
    {
        if (!await context.Categories.AnyAsync())
        {
            await SeedFullCatalogAsync(context);
            return;
        }

        await UpgradeFlatCatalogToHierarchyAsync(context);
        await EnsureMobilePhonesCatalogAsync(context);
        await EnsureProductImagesAsync(context);
        await EnsureStoresAndInventoryAsync(context);
        await EnsureSampleProductReviewsAsync(context);
        await EnsureBrandsOnProductsAsync(context);
    }

    private static async Task EnsureBrandsOnProductsAsync(ApplicationDbContext context)
    {
        if (!await context.Brands.AnyAsync())
        {
            context.Brands.AddRange(
                new Brand { Name = "TechPro" },
                new Brand { Name = "AudioMax" },
                new Brand { Name = "DenimCo" },
                new Brand { Name = "WinterWear" },
                new Brand { Name = "ArabicaPlus" },
                new Brand { Name = "OliveGold" });
            await context.SaveChangesAsync();
        }

        var brands = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);
        var products = await context.Products.Where(p => p.BrandId == null).ToListAsync();

        foreach (var product in products)
        {
            product.BrandId = product.Name switch
            {
                var n when n.Contains("Смартфон") || n.Contains("Ноутбук") => brands["TechPro"],
                var n when n.Contains("Наушник") => brands["AudioMax"],
                var n when n.Contains("Джинсы") => brands["DenimCo"],
                var n when n.Contains("Куртка") => brands["WinterWear"],
                var n when n.Contains("Кофе") => brands["ArabicaPlus"],
                var n when n.Contains("масло") => brands["OliveGold"],
                _ => product.BrandId
            };
        }

        if (products.Count > 0)
            await context.SaveChangesAsync();
    }

    private static async Task EnsureSupplierDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        await EnsureBrandsOnProductsAsync(context);

        var supplierUser = await userManager.FindByEmailAsync("supplier@demo.ru");
        var supplierUser2 = await userManager.FindByEmailAsync("supplier2@demo.ru");

        if (supplierUser != null && !await context.Suppliers.AnyAsync(s => s.OwnerId == supplierUser.Id))
        {
            var supplier = new Supplier
            {
                Name = "ООО «ТехОпт»",
                ContactInfo = "opt@tech.demo.ru, +7 (495) 111-22-33",
                OwnerId = supplierUser.Id
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var techPro = await context.Brands.FirstAsync(b => b.Name == "TechPro");
            var audioMax = await context.Brands.FirstAsync(b => b.Name == "AudioMax");
            context.SupplierBrands.AddRange(
                new SupplierBrand { SupplierId = supplier.Id, BrandId = techPro.Id },
                new SupplierBrand { SupplierId = supplier.Id, BrandId = audioMax.Id });
            await context.SaveChangesAsync();
        }

        if (supplierUser2 != null && !await context.Suppliers.AnyAsync(s => s.OwnerId == supplierUser2.Id))
        {
            var supplier = new Supplier
            {
                Name = "Одежда Опт",
                ContactInfo = "sales@clothes.demo.ru",
                OwnerId = supplierUser2.Id
            };
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();

            var denim = await context.Brands.FirstAsync(b => b.Name == "DenimCo");
            var winter = await context.Brands.FirstAsync(b => b.Name == "WinterWear");
            context.SupplierBrands.AddRange(
                new SupplierBrand { SupplierId = supplier.Id, BrandId = denim.Id },
                new SupplierBrand { SupplierId = supplier.Id, BrandId = winter.Id });
            await context.SaveChangesAsync();
        }

        if (await context.SupplierProducts.AnyAsync())
            return;

        var suppliers = await context.Suppliers.Include(s => s.Brands).ToListAsync();
        var products = await context.Products.Include(p => p.Brand).Where(p => p.BrandId != null).ToListAsync();

        foreach (var supplier in suppliers)
        {
            var brandIds = supplier.Brands.Select(b => b.BrandId).ToHashSet();
            foreach (var product in products.Where(p => brandIds.Contains(p.BrandId!.Value)))
            {
                context.SupplierProducts.Add(new SupplierProduct
                {
                    SupplierId = supplier.Id,
                    ProductId = product.Id,
                    StockQuantity = 40 + product.Id * 5,
                    UnitPrice = Math.Round(product.Price * 0.85m, 0)
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedFullCatalogAsync(ApplicationDbContext context)
    {
        await EnsureBrandsOnProductsAsync(context);
        var brands = await context.Brands.ToDictionaryAsync(b => b.Name, b => b.Id);

        var electronics = new Category { Name = "Электроника", Description = "Гаджеты и техника" };
        var clothing = new Category { Name = "Одежда", Description = "Мужская и женская одежда" };
        var food = new Category { Name = "Продукты", Description = "Продукты питания" };
        context.Categories.AddRange(electronics, clothing, food);
        await context.SaveChangesAsync();

        var phones = new Category { Name = MobilePhoneCatalog.CategoryName, ParentId = electronics.Id, Description = "Мобильные телефоны и планшеты" };
        var audio = new Category { Name = "Аудио", ParentId = electronics.Id };
        var computers = new Category { Name = "Компьютеры", ParentId = electronics.Id };
        var outerwear = new Category { Name = "Верхняя одежда", ParentId = clothing.Id };
        var jeans = new Category { Name = "Джинсы и брюки", ParentId = clothing.Id };
        var drinks = new Category { Name = "Напитки", ParentId = food.Id };
        var grocery = new Category { Name = "Бакалея", ParentId = food.Id };
        context.Categories.AddRange(phones, audio, computers, outerwear, jeans, drinks, grocery);
        await context.SaveChangesAsync();

        context.Products.AddRange(
            CreateProduct("Наушники Pro", "Беспроводные с шумоподавлением", 8990, 120, audio.Id, brands["AudioMax"], "006493"),
            CreateProduct("Ноутбук Ultra", "14\", 16 ГБ RAM", 89990, 25, computers.Id, brands["TechPro"], "625B71"),
            CreateProduct("Джинсы Classic", "Прямой крой, хлопок", 3990, 200, jeans.Id, brands["DenimCo"], "7D5260"),
            CreateProduct("Куртка зимняя", "Утеплённая, водонепроницаемая", 12990, 80, outerwear.Id, brands["WinterWear"], "386A20"),
            CreateProduct("Кофе Arabica", "Зерновой, 1 кг", 1490, 300, drinks.Id, brands["ArabicaPlus"], "984061"),
            CreateProduct("Оливковое масло", "Extra Virgin, 0.5 л", 890, 150, grocery.Id, brands["OliveGold"], "006874")
        );
        await context.SaveChangesAsync();

        await EnsureMobilePhonesCatalogAsync(context);
        await EnsureStoresAndInventoryAsync(context);
        await EnsureSampleProductReviewsAsync(context);
    }

    private static Product CreateProduct(string name, string description, decimal price, int stock, int categoryId, int brandId, string color) =>
        new()
        {
            Name = name,
            Description = description,
            Price = price,
            StockQuantity = stock,
            CategoryId = categoryId,
            BrandId = brandId,
            ImageUrl = $"https://placehold.co/480x320/{color}/FFFFFF/png?text={Uri.EscapeDataString(name)}"
        };

    private static async Task UpgradeFlatCatalogToHierarchyAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync(c => c.ParentId != null))
            return;

        var roots = await context.Categories.OrderBy(c => c.Id).Take(3).ToListAsync();
        if (roots.Count < 3)
            return;

        var electronics = roots[0];
        var clothing = roots[1];
        var food = roots[2];

        var phones = new Category { Name = MobilePhoneCatalog.CategoryName, ParentId = electronics.Id, Description = "Мобильные телефоны и планшеты" };
        var audio = new Category { Name = "Аудио", ParentId = electronics.Id };
        var computers = new Category { Name = "Компьютеры", ParentId = electronics.Id };
        var outerwear = new Category { Name = "Верхняя одежда", ParentId = clothing.Id };
        var jeans = new Category { Name = "Джинсы и брюки", ParentId = clothing.Id };
        var drinks = new Category { Name = "Напитки", ParentId = food.Id };
        var grocery = new Category { Name = "Бакалея", ParentId = food.Id };
        context.Categories.AddRange(phones, audio, computers, outerwear, jeans, drinks, grocery);
        await context.SaveChangesAsync();

        var products = await context.Products.ToListAsync();
        foreach (var product in products)
        {
            product.CategoryId = product.Name switch
            {
                var n when n.Contains("Смартфон") || n.Contains("XOOM") || n.Contains("DROID") || n.Contains("Galaxy") || n.Contains("Nexus") => phones.Id,
                var n when n.Contains("Наушник") => audio.Id,
                var n when n.Contains("Ноутбук") => computers.Id,
                var n when n.Contains("Джинсы") => jeans.Id,
                var n when n.Contains("Куртка") => outerwear.Id,
                var n when n.Contains("Кофе") => drinks.Id,
                var n when n.Contains("масло") => grocery.Id,
                _ => product.CategoryId
            };
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureMobilePhonesCatalogAsync(ApplicationDbContext context)
    {
        var electronics = await context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Электроника" && c.ParentId == null);

        if (electronics == null)
            return;

        var phonesCategory = await context.Categories
            .FirstOrDefaultAsync(c => c.ParentId == electronics.Id &&
                (c.Name == MobilePhoneCatalog.CategoryName || c.Name == "Смартфоны"));

        if (phonesCategory == null)
        {
            phonesCategory = new Category
            {
                Name = MobilePhoneCatalog.CategoryName,
                ParentId = electronics.Id,
                Description = "Мобильные телефоны и планшеты"
            };
            context.Categories.Add(phonesCategory);
            await context.SaveChangesAsync();
        }
        else if (phonesCategory.Name != MobilePhoneCatalog.CategoryName)
        {
            phonesCategory.Name = MobilePhoneCatalog.CategoryName;
            phonesCategory.Description ??= "Мобильные телефоны и планшеты";
            await context.SaveChangesAsync();
        }

        var placeholder = await context.Products.FirstOrDefaultAsync(p => p.Name == "Смартфон X1");
        if (placeholder != null && placeholder.IsActive)
        {
            placeholder.IsActive = false;
            await context.SaveChangesAsync();
        }

        var existingNames = await context.Products
            .Where(p => p.CategoryId == phonesCategory.Id)
            .Select(p => p.Name)
            .ToListAsync();

        var added = false;
        foreach (var phone in MobilePhoneCatalog.Phones)
        {
            if (existingNames.Contains(phone.Name))
                continue;

            var brandId = await EnsureBrandAsync(context, phone.Brand);
            context.Products.Add(new Product
            {
                Name = phone.Name,
                Description = phone.Description,
                Price = phone.Price,
                StockQuantity = 30 + MobilePhoneCatalog.Phones.ToList().IndexOf(phone) * 3,
                CategoryId = phonesCategory.Id,
                BrandId = brandId,
                ImageUrl = phone.ImageUrl,
                IsActive = true
            });
            added = true;
        }

        if (added)
            await context.SaveChangesAsync();

        var phonesInCategory = await context.Products
            .Where(p => p.CategoryId == phonesCategory.Id)
            .ToListAsync();

        foreach (var product in phonesInCategory)
        {
            var seed = MobilePhoneCatalog.Phones.FirstOrDefault(p => p.Name == product.Name);
            if (seed == null)
                continue;

            if (string.IsNullOrWhiteSpace(product.ImageUrl) ||
                product.ImageUrl.StartsWith("https://placehold.co", StringComparison.OrdinalIgnoreCase))
            {
                product.ImageUrl = seed.ImageUrl;
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task<int> EnsureBrandAsync(ApplicationDbContext context, string brandName)
    {
        var brand = await context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);
        if (brand != null)
            return brand.Id;

        brand = new Brand { Name = brandName };
        context.Brands.Add(brand);
        await context.SaveChangesAsync();
        return brand.Id;
    }

    private static async Task EnsureProductImagesAsync(ApplicationDbContext context)
    {
        var products = await context.Products.Where(p => p.ImageUrl == null || p.ImageUrl == "").ToListAsync();
        if (products.Count == 0)
            return;

        var palette = new[] { "6750A4", "006493", "386A20", "7D5260", "625B71", "984061", "006874" };
        for (var i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var color = palette[i % palette.Length];
            p.ImageUrl = $"https://placehold.co/480x320/{color}/FFFFFF/png?text={Uri.EscapeDataString(p.Name)}";
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureStoresAndInventoryAsync(ApplicationDbContext context)
    {
        var storeOwner = await context.Users.FirstOrDefaultAsync(u => u.Email == "store@demo.ru");
        var storeOwner2 = await context.Users.FirstOrDefaultAsync(u => u.Email == "store2@demo.ru");

        if (storeOwner != null && !await context.Stores.AnyAsync(s => s.OwnerId == storeOwner.Id))
        {
            context.Stores.Add(new Store
            {
                Name = "Магазин «У Пятра»",
                Address = "ул. Торговая, 15",
                Description = "Сеть бытовой техники и электроники с доставкой по городу.",
                OwnerId = storeOwner.Id
            });
        }

        if (storeOwner2 != null && !await context.Stores.AnyAsync(s => s.OwnerId == storeOwner2.Id))
        {
            context.Stores.Add(new Store
            {
                Name = "Сеть «Ритейл+»",
                Address = "пр. Центральный, 42",
                Description = "Розничный магазин техники и аксессуаров.",
                OwnerId = storeOwner2.Id
            });
        }

        await context.SaveChangesAsync();

        if (await context.StoreProducts.AnyAsync())
            return;

        var stores = await context.Stores.OrderBy(s => s.Id).ToListAsync();
        var products = await context.Products.OrderBy(p => p.Id).ToListAsync();
        if (stores.Count == 0 || products.Count == 0)
            return;

        var assignments = new (int storeIndex, int productIndex, int qty)[]
        {
            (0, 0, 12), (0, 1, 25), (0, 2, 8), (0, 5, 40),
            (1, 0, 6), (1, 1, 30), (1, 3, 50), (1, 4, 15), (1, 6, 60)
        };

        if (stores.Count == 1)
        {
            foreach (var product in products)
            {
                context.StoreProducts.Add(new StoreProduct
                {
                    StoreId = stores[0].Id,
                    ProductId = product.Id,
                    Quantity = 10 + product.Id * 3
                });
            }
        }
        else
        {
            foreach (var (storeIndex, productIndex, qty) in assignments)
            {
                if (storeIndex >= stores.Count || productIndex >= products.Count)
                    continue;

                context.StoreProducts.Add(new StoreProduct
                {
                    StoreId = stores[storeIndex].Id,
                    ProductId = products[productIndex].Id,
                    Quantity = qty
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureCashierDemoDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        var cashier = await userManager.FindByEmailAsync("cashier@demo.ru");
        var store = await context.Stores.FirstOrDefaultAsync(s => s.Name == "Магазин «У Пятра»");
        if (cashier == null || store == null)
            return;

        if (!await context.StoreCashiers.AnyAsync(c => c.UserId == cashier.Id))
        {
            context.StoreCashiers.Add(new StoreCashier
            {
                UserId = cashier.Id,
                StoreId = store.Id,
                AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        if (await context.SaleBaskets.AnyAsync(b => b.StoreId == store.Id))
            return;

        var product = await context.StoreProducts
            .Include(sp => sp.Product)
            .Where(sp => sp.StoreId == store.Id && sp.Quantity > 0)
            .Select(sp => sp.Product)
            .FirstOrDefaultAsync();

        if (product == null)
            return;

        var basket = new SaleBasket
        {
            OrderUid = Guid.NewGuid(),
            BasketNumber = $"B-{DateTime.UtcNow:yyyyMMdd}-0001",
            StoreId = store.Id,
            CashierUserId = cashier.Id,
            ClientName = "Демо клиент",
            ClientPhone = "79001234567",
            ClientEmail = "client@demo.ru",
            Status = SaleBasketStatus.Open,
            TotalAmount = product.Price * 2,
            Items =
            [
                new SaleBasketItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = 2,
                    UnitPrice = product.Price,
                    LineTotal = product.Price * 2
                }
            ]
        };

        context.SaleBaskets.Add(basket);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureReportsDemoDataAsync(ApplicationDbContext context)
    {
        if (await context.ReportGroups.AnyAsync())
            return;

        var sales = new ReportGroup { Name = "Продажи" };
        var warehouse = new ReportGroup { Name = "Склад" };
        var cashier = new ReportGroup { Name = "Касса" };
        var orders = new ReportGroup { Name = "Заказы" };
        context.ReportGroups.AddRange(sales, warehouse, cashier, orders);
        await context.SaveChangesAsync();

        var salesByDay = new ReportProcedure
        {
            GroupId = sales.Id,
            ProcedureName = "stat_store_sales_by_day",
            Title = "Продажи по дням",
            Description = "Количество заказов и выручка по дням за период (аналог stat SP в Reps).",
            DiagramType = ReportDiagramType.Bar,
            ChartXField = "report_date",
            ChartYField = "revenue",
            Parameters =
            [
                new ReportParameter { Name = "dateFrom", Title = "Дата с", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd"), SortOrder = 1 },
                new ReportParameter { Name = "dateTo", Title = "Дата по", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.ToString("yyyy-MM-dd"), SortOrder = 2 }
            ],
            Columns =
            [
                new ReportColumn { ColumnName = "report_date", Title = "Дата", Format = "date", SortOrder = 1 },
                new ReportColumn { ColumnName = "orders_count", Title = "Заказов", Format = "int", ShowSum = true, SortOrder = 2 },
                new ReportColumn { ColumnName = "revenue", Title = "Выручка", Format = "money", ShowSum = true, SortOrder = 3 }
            ]
        };

        var topProducts = new ReportProcedure
        {
            GroupId = sales.Id,
            ProcedureName = "stat_top_products",
            Title = "Топ товаров по выручке",
            Description = "Рейтинг товаров магазина по сумме продаж.",
            DiagramType = ReportDiagramType.None,
            Parameters =
            [
                new ReportParameter { Name = "topN", Title = "Кол-во позиций", Type = ReportParameterType.Integer, DefaultValue = "10", SortOrder = 1 }
            ],
            Columns =
            [
                new ReportColumn { ColumnName = "rank", Title = "№", Format = "int", SortOrder = 1 },
                new ReportColumn { ColumnName = "product_name", Title = "Товар", SortOrder = 2 },
                new ReportColumn { ColumnName = "quantity", Title = "Кол-во", Format = "int", ShowSum = true, SortOrder = 3 },
                new ReportColumn { ColumnName = "amount", Title = "Сумма", Format = "money", ShowSum = true, SortOrder = 4 }
            ]
        };

        var inventory = new ReportProcedure
        {
            GroupId = warehouse.Id,
            ProcedureName = "stat_inventory",
            Title = "Остатки на складе магазина",
            Description = "Текущие остатки и оценка стоимости запасов.",
            DiagramType = ReportDiagramType.None,
            Columns =
            [
                new ReportColumn { ColumnName = "product_name", Title = "Товар", SortOrder = 1 },
                new ReportColumn { ColumnName = "brand", Title = "Бренд", SortOrder = 2 },
                new ReportColumn { ColumnName = "quantity", Title = "Остаток", Format = "int", ShowSum = true, SortOrder = 3 },
                new ReportColumn { ColumnName = "price", Title = "Цена", Format = "money", SortOrder = 4 },
                new ReportColumn { ColumnName = "stock_value", Title = "Сумма", Format = "money", ShowSum = true, SortOrder = 5 }
            ]
        };

        var cashierPayments = new ReportProcedure
        {
            GroupId = cashier.Id,
            ProcedureName = "stat_cashier_payments",
            Title = "Кассовые операции по дням",
            Description = "Корзины кассы: оплаты и возвраты за период.",
            DiagramType = ReportDiagramType.Line,
            ChartXField = "report_date",
            ChartYField = "paid_amount",
            Parameters =
            [
                new ReportParameter { Name = "dateFrom", Title = "Дата с", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"), SortOrder = 1 },
                new ReportParameter { Name = "dateTo", Title = "Дата по", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.ToString("yyyy-MM-dd"), SortOrder = 2 }
            ],
            Columns =
            [
                new ReportColumn { ColumnName = "report_date", Title = "Дата", Format = "date", SortOrder = 1 },
                new ReportColumn { ColumnName = "baskets_count", Title = "Корзин", Format = "int", ShowSum = true, SortOrder = 2 },
                new ReportColumn { ColumnName = "paid_amount", Title = "Оплачено", Format = "money", ShowSum = true, SortOrder = 3 },
                new ReportColumn { ColumnName = "refund_sum", Title = "Возвраты", Format = "money", ShowSum = true, SortOrder = 4 }
            ]
        };

        var ordersByStatus = new ReportProcedure
        {
            GroupId = orders.Id,
            ProcedureName = "stat_orders_by_status",
            Title = "Заказы по статусам",
            Description = "Распределение заказов магазина по статусам за период.",
            DiagramType = ReportDiagramType.Pie,
            ChartXField = "status_name",
            ChartYField = "orders_count",
            Parameters =
            [
                new ReportParameter { Name = "dateFrom", Title = "Дата с", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd"), SortOrder = 1 },
                new ReportParameter { Name = "dateTo", Title = "Дата по", Type = ReportParameterType.Date, DefaultValue = DateTime.UtcNow.ToString("yyyy-MM-dd"), SortOrder = 2 }
            ],
            Columns =
            [
                new ReportColumn { ColumnName = "status_name", Title = "Статус", SortOrder = 1 },
                new ReportColumn { ColumnName = "orders_count", Title = "Заказов", Format = "int", ShowSum = true, SortOrder = 2 },
                new ReportColumn { ColumnName = "total_amount", Title = "Сумма", Format = "money", ShowSum = true, SortOrder = 3 }
            ]
        };

        context.ReportProcedures.AddRange(salesByDay, topProducts, inventory, cashierPayments, ordersByStatus);
        await context.SaveChangesAsync();

        var store = await context.Stores.FirstOrDefaultAsync(s => s.Name == "Магазин «У Пятра»");
        if (store == null)
            return;

        var ordersWithoutStore = await context.Orders.Where(o => o.StoreId == null).ToListAsync();
        foreach (var order in ordersWithoutStore)
            order.StoreId = store.Id;

        if (ordersWithoutStore.Count > 0)
            await context.SaveChangesAsync();

        if (!await context.SaleBaskets.AnyAsync(b => b.Status == SaleBasketStatus.Paid))
        {
            var product = await context.Products.FirstOrDefaultAsync();
            if (product != null)
            {
                context.SaleBaskets.Add(new SaleBasket
                {
                    OrderUid = Guid.NewGuid(),
                    BasketNumber = $"B-{DateTime.UtcNow:yyyyMMdd}-PAID1",
                    StoreId = store.Id,
                    ClientName = "Клиент кассы",
                    Status = SaleBasketStatus.Paid,
                    TotalAmount = product.Price,
                    PaidAmount = product.Price,
                    PaidAt = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Items =
                    [
                        new SaleBasketItem
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            Quantity = 1,
                            UnitPrice = product.Price,
                            LineTotal = product.Price
                        }
                    ]
                });
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task EnsureMarketingDemoDataAsync(ApplicationDbContext context)
    {
        if (await context.MailingConfigurations.AnyAsync())
            return;

        var mailing = new MailingConfiguration
        {
            SettingsName = "Акция «Весна 2026»",
            MarketerName = "Midvix #1042",
            Status = MailingConfigurationStatus.Active,
            MailingStartDate = DateTime.UtcNow.Date,
            AudienceTotal = 12500,
            AudienceSent = 8300,
            TimeoutMinutes = 120,
            ExportFileName = "mailing_spring_2026.csv",
            TestAudience =
            [
                new MailingTestAudience
                {
                    Phone = "79001234567",
                    MessengersOrder = "telegram,whatsapp",
                    FallbackToSms = true,
                    IsActive = true
                }
            ]
        };

        context.MailingConfigurations.Add(mailing);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureSampleProductReviewsAsync(ApplicationDbContext context)
    {
        if (await context.Reviews.AnyAsync())
            return;

        var client = await context.Users.FirstOrDefaultAsync(u => u.Email == "client@demo.ru");
        var products = await context.Products.OrderBy(p => p.Id).Take(4).ToListAsync();
        if (client == null || products.Count == 0)
            return;

        var sampleReviews = new[]
        {
            (Rating: 5, Comment: "Отличное качество, быстрая доставка!"),
            (Rating: 4, Comment: "Хороший товар, но упаковка могла быть лучше."),
            (Rating: 5, Comment: "Покупаю уже второй раз, рекомендую."),
            (Rating: 3, Comment: "Нормально за свои деньги.")
        };

        for (var i = 0; i < products.Count; i++)
        {
            var product = products[i];
            var review = sampleReviews[i];

            var order = new Order
            {
                OrderNumber = $"DEMO-{1000 + i}",
                ClientId = client.Id,
                Status = OrderStatus.Delivered,
                DeliveryMethod = DeliveryMethod.Courier,
                DeliveryAddress = "ул. Демо, 1",
                TotalAmount = product.Price,
                CreatedAt = DateTime.UtcNow.AddDays(-14 + i),
                DeliveredAt = DateTime.UtcNow.AddDays(-7 + i),
                Items =
                [
                    new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        UnitPrice = product.Price
                    }
                ]
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            context.Reviews.Add(new Review
            {
                OrderId = order.Id,
                AuthorId = client.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = DateTime.UtcNow.AddDays(-5 + i)
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedNewsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.NewsItems.AnyAsync())
            return;

        var admin = await userManager.FindByEmailAsync("admin@demo.ru");
        if (admin == null)
            return;

        context.NewsItems.AddRange(
            new NewsItem
            {
                Title = "Добро пожаловать в BlazorErp!",
                Content = "Система управления заказами, доставкой и продажами запущена. Зарегистрируйтесь и выберите роль: клиент, транспортная компания или владелец магазина.",
                AuthorId = admin.Id,
                PublishedAt = DateTime.UtcNow.AddDays(-3)
            },
            new NewsItem
            {
                Title = "Оплата через СБП",
                Content = "Теперь клиенты могут оплачивать заказы через Систему быстрых платежей (СБП). Выберите СБП при оформлении заказа.",
                AuthorId = admin.Id,
                PublishedAt = DateTime.UtcNow.AddDays(-1)
            },
            new NewsItem
            {
                Title = "Двухфакторная аутентификация",
                Content = "В личном кабинете доступна настройка 2FA через приложение-аутентификатор для повышения безопасности аккаунта.",
                AuthorId = admin.Id,
                PublishedAt = DateTime.UtcNow
            }
        );
        await context.SaveChangesAsync();
    }

    private static async Task EnsureAutoServiceDataAsync(ApplicationDbContext context)
    {
        var owner = await context.Users.FirstOrDefaultAsync(u => u.Email == "autoservice@demo.ru");
        if (owner == null)
            return;

        var autoService = await context.AutoServices.FirstOrDefaultAsync(s => s.OwnerId == owner.Id);
        if (autoService == null)
        {
            autoService = new AutoService
            {
                Name = "Автосервис «Мастер»",
                Address = "ул. Гаражная, 7",
                Description = "Ремонт и обслуживание легковых автомобилей.",
                Phone = "+7 (495) 123-45-67",
                OwnerId = owner.Id
            };
            context.AutoServices.Add(autoService);
            await context.SaveChangesAsync();
        }

        if (!await context.AutoServiceOfferings.AnyAsync(o => o.AutoServiceId == autoService.Id))
        {
            context.AutoServiceOfferings.AddRange(
                new AutoServiceOffering
                {
                    AutoServiceId = autoService.Id,
                    Name = "Замена масла",
                    Description = "Масло и фильтр включены",
                    Price = 2500,
                    DurationMinutes = 45
                },
                new AutoServiceOffering
                {
                    AutoServiceId = autoService.Id,
                    Name = "Диагностика двигателя",
                    Description = "Компьютерная диагностика",
                    Price = 1500,
                    DurationMinutes = 30
                },
                new AutoServiceOffering
                {
                    AutoServiceId = autoService.Id,
                    Name = "Развал-схождение",
                    Price = 3500,
                    DurationMinutes = 60
                });
            await context.SaveChangesAsync();
        }

        if (!await context.AutoServiceStaffMembers.AnyAsync(s => s.AutoServiceId == autoService.Id))
        {
            context.AutoServiceStaffMembers.AddRange(
                new AutoServiceStaffMember
                {
                    AutoServiceId = autoService.Id,
                    FullName = "Алексей Механиков",
                    Position = "Механик",
                    Phone = "+7 (900) 111-22-33"
                },
                new AutoServiceStaffMember
                {
                    AutoServiceId = autoService.Id,
                    FullName = "Дмитрий Приёмщик",
                    Position = "Мастер-приёмщик",
                    Phone = "+7 (900) 444-55-66"
                });
            await context.SaveChangesAsync();
        }

        if (await context.AutoServiceAppointments.AnyAsync(a => a.AutoServiceId == autoService.Id))
            return;

        var offerings = await context.AutoServiceOfferings
            .Where(o => o.AutoServiceId == autoService.Id)
            .OrderBy(o => o.Id)
            .ToListAsync();
        var staff = await context.AutoServiceStaffMembers
            .Where(s => s.AutoServiceId == autoService.Id)
            .OrderBy(s => s.Id)
            .ToListAsync();

        if (offerings.Count == 0)
            return;

        var tomorrow = DateTime.UtcNow.Date.AddDays(1).AddHours(10);
        context.AutoServiceAppointments.AddRange(
            new AutoServiceAppointment
            {
                AutoServiceId = autoService.Id,
                OfferingId = offerings[0].Id,
                StaffMemberId = staff.FirstOrDefault()?.Id,
                ClientName = "Иван Петров",
                ClientPhone = "+7 (900) 777-88-99",
                ScheduledAt = tomorrow,
                DurationMinutes = offerings[0].DurationMinutes,
                Status = AutoServiceAppointmentStatus.Scheduled
            },
            new AutoServiceAppointment
            {
                AutoServiceId = autoService.Id,
                OfferingId = offerings.Count > 1 ? offerings[1].Id : offerings[0].Id,
                StaffMemberId = staff.Skip(1).FirstOrDefault()?.Id,
                ClientName = "Мария Сидорова",
                ClientPhone = "+7 (900) 222-33-44",
                ScheduledAt = tomorrow.AddHours(2),
                DurationMinutes = offerings.Count > 1 ? offerings[1].DurationMinutes : 60,
                Status = AutoServiceAppointmentStatus.Scheduled
            });

        await context.SaveChangesAsync();
    }
}
