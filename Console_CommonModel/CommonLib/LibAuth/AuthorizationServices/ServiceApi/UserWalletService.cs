namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationServices.ServiceApi
{
    public class UserWalletService(DbContextUser db)
    {


        public void ChangeWallet(int userid, float value)
        {
            var user = db.UserContexts.Find(userid);
            var wallet = db.UserWallets.Find(user.WalletId);
            wallet.Amount += value;
            db.TransferTransactions.Add(new()
            {
                SourceWalletId = userid,
                TargetWalletId = userid,
                TransactionAmount = value
            });
            db.SaveChanges();
        }
    }
}
