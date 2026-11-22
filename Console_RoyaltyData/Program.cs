using Console_CommonModel;

using Console_RoyaltyData.RoyaltyData;

try
{
    CommonProgram.Run<Program>(new string[] { "--no-force" });

    Console.WriteLine("Started");
    RoyaltyDbInitiallizer royaltyDbInitiallizer = new();
    using (var db = new RoyaltyDbContext())
    {     
        await royaltyDbInitiallizer.Init(db);
    }
    Console.WriteLine("Completed");
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}