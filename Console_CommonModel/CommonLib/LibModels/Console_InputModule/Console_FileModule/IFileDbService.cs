public interface IFileDbService
{
    public int CreateCatalogs(string path);
    public int DeleteCatalogs(string path);
    public void DeleteDbFiles(int catalogId);
    public void DeleteDbCatalogs(int catalogId);
    public void AddDbCatalogs(int catalogId);
    public void AddDbFiles(int catalogId);
    public void DeleteDriveFiles(int catalogId);
    public void DeleteDriveCatalogs(int catalogId);
    public void AddDriveCatalogs(int catalogId);
    public void AddDriveFiles(int catalogId);
    public string GetCatalogPath(int? catalogId);
    public string GetFilePath(int fileId);
    public void InitCatalogs();

}
