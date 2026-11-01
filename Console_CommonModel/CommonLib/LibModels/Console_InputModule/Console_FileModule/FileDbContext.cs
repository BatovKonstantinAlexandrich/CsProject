using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;


[Label("Контекст данных файлового хранлища")]
public class FileDbContext: DbContextBase, IFileDbService
{
    [Label("Включить интеграцию с файловой системой")]
    [NotMapped]
    public bool EnableFileSystemBinding { get; set; } = true;

    public virtual DbSet<FileCatalog> FileCatalogs { get; set; }
    public virtual DbSet<FileResource> FileResources { get; set; }
    

    [NotMapped]
    private IFileDbService fileDbService;


    public FileDbContext() {        
        this.fileDbService = new FileDbService(this);        
    }

    [OnDataChanged<FileCatalog>()]
    public void OnFileCatalogChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        if (!this.EnableDataChangedAttributes)
            return;
        var before = primary.New<FileCatalog>();
        var after = data.New<FileCatalog>();
        switch (state)
        {
            case EntityState.Added:
                {
                    if (after.ParentId is null)
                        return;
                    var beforeCatalogPath = this.GetCatalogPath((int)after.ParentId);
                    this.CreateCatalogs(beforeCatalogPath);
                    break;
                }
            case EntityState.Deleted:
                {
                    if (before.ParentId is null)
                        return;
                    var afterCatalogPath = this.GetCatalogPath((int)before.ParentId);
                    this.DeleteCatalogs(afterCatalogPath);
                    break;
                }
            case EntityState.Modified:
                {
                    if (before.ParentId is not null)
                    {
                        var beforeCatalogPath = this.GetCatalogPath((int)before.ParentId);
                        this.DeleteCatalogs(beforeCatalogPath);
                    }
                    if (after.ParentId is not null)
                    {
                        var afterCatalogPath = this.GetCatalogPath((int)after.ParentId);
                        this.CreateCatalogs(afterCatalogPath);
                    }
                    break;
                }
            default: throw new UnsupportedException(state.ToString());
        }
    }

    [OnDataChanged<FileResource>()]
    public void OnFileResourceChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        if (!this.EnableDataChangedAttributes)
            return;
        var before = primary.New<FileResource>();
        var after = data.New<FileResource>();
        switch(state)
        {
            case EntityState.Added:
                {
                    var catalogPath = this.GetCatalogPath((int)after.ParentId);
                    var filePath = Path.Combine(catalogPath, after.Name);                    
                    System.IO.File.WriteAllBytes(filePath, after.Data);                    
                    break;
                }
            case EntityState.Deleted:
                {
                    var filePath = this.GetFilePath(before.Id);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    break;
                }
            case EntityState.Modified:
                {
                    var beforeFilePath = this.GetFilePath(before.Id);
                    if (System.IO.File.Exists(beforeFilePath))
                    {
                        System.IO.File.Delete(beforeFilePath);
                    }
                    var catalogPath = this.GetCatalogPath((int)after.ParentId);
                    var filePath = Path.Combine(catalogPath, after.Name);
                    
                    System.IO.File.WriteAllBytes(filePath, after.Data);
                    
                    break;                    
                }
            default: throw new UnsupportedException(state.ToString());
        }
    }


    public FileCatalog GetRoot()
    {
        var rootCatalog = this.FileCatalogs.FirstOrDefault(c => c.ParentId == null);
        if(rootCatalog is null)
        {
            InitCatalogs();
        }
        rootCatalog = this.FileCatalogs.FirstOrDefault(c => c.ParentId == null);
        return rootCatalog;
    }

    public FileCatalog GetFileCatalog(List<string> path)
    {
        FileCatalog rootCatalog = GetRoot();
        FileCatalog pCatalog = rootCatalog;
        foreach (var name in path)
        {
            var pc = FileCatalogs.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
            if(pc is null)
            {
                FileCatalogs.Add(pc=new FileCatalog()
                {
                    Name = name,                    
                    Parent = pCatalog
                });                                
            }
            pCatalog = pc;
        }
        this.SaveChangesWithLateIntegration();
        return pCatalog;
    }

    public FileDbContext(DbContextOptions options) : base(options)
    {
        this.Database.EnsureCreated();
        this.fileDbService = new FileDbService(this);
        
    }

    public void DeleteDbFiles(int catalogId)
    {
        ((IFileDbService)fileDbService).DeleteDbFiles(catalogId);
    }

    public void DeleteDbCatalogs(int catalogId)
    {
        ((IFileDbService)fileDbService).DeleteDbCatalogs(catalogId);
    }

    public void AddDbCatalogs(int catalogId)
    {
        ((IFileDbService)fileDbService).AddDbCatalogs(catalogId);
    }

    public void AddDbFiles(int catalogId)
    {
        ((IFileDbService)fileDbService).AddDbFiles(catalogId);
    }

    public void DeleteDriveFiles(int catalogId)
    {
        ((IFileDbService)fileDbService).DeleteDriveFiles(catalogId);
    }

    public void DeleteDriveCatalogs(int catalogId)
    {
        ((IFileDbService)fileDbService).DeleteDriveCatalogs(catalogId);
    }

    public void AddDriveCatalogs(int catalogId)
    {
        ((IFileDbService)fileDbService).AddDriveCatalogs(catalogId);
    }

    public void AddDriveFiles(int catalogId)
    {
        ((IFileDbService)fileDbService).AddDriveFiles(catalogId);
    }

    public string GetCatalogPath(int? catalogId)
    {
        return ((IFileDbService)fileDbService).GetCatalogPath(catalogId);
    }

    public string GetFilePath(int fileId)
    {
        return ((IFileDbService)fileDbService).GetFilePath(fileId);
    }

    public void InitCatalogs()
    {
        fileDbService.InitCatalogs();
    }

    public int CreateCatalogs(string path)
    {
        return fileDbService.CreateCatalogs(path);
    }

    public int CreateCatalogs(int fileCatalogId)
    {
        var path = GetCatalogPath(fileCatalogId);
        return fileDbService.CreateCatalogs(path);
    }

    public int DeleteCatalogs(string path)
    {
        return fileDbService.DeleteCatalogs(path);
    }
}