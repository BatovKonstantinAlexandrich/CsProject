using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;

using Microsoft.EntityFrameworkCore.Storage;

using System.Text;

using static System.IO.Directory;

/// <summary>
/// Служба управления ресурсами
/// </summary>
public class FileDbService: IFileDbService
{
    private readonly FileDbContext _fileDbContext;


    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="fileDbContext"></param>
    public FileDbService(FileDbContext fileDbContext)
    {
        this._fileDbContext = fileDbContext;
    }

 
        

    public void DeleteDbFiles(int catalogId)
    {
        this._fileDbContext.FileResources.RemoveRange(this._fileDbContext.FileResources.Where(f => f.ParentId == catalogId));
        this._fileDbContext.SaveChanges();

        var fileCatalog = this._fileDbContext.FileResources.Where(f =>f.ParentId == catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }        
    }

    public void DeleteDbCatalogs(int catalogId)
    {
        this._fileDbContext.FileCatalogs.RemoveRange(this._fileDbContext.FileCatalogs.Where(f => f.ParentId == catalogId));
        this._fileDbContext.SaveChanges();
    }

    public void AddDbCatalogs(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        this._fileDbContext.FileCatalogs.AddRange(
            GetDirectories(catalogPath).Select(path => new FileCatalog()
            {
                Name = path.Substring(catalogPath.Length),
                ParentId = catalogId
            }));
        this._fileDbContext.SaveChanges();
    }

    public void AddDbFiles(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        this._fileDbContext.FileResources.AddRange(
            GetFiles(catalogPath).Select(path => new FileResource()
            {
                Name = path.Substring(catalogPath.Length),
                ParentId = catalogId,
                Mime = GetMimeForExt(GetFilePathExt(path)),
                Data = System.IO.File.ReadAllBytes(path),
                Created = System.IO.File.GetLastWriteTime(path)
            }));
        this._fileDbContext.SaveChangesWithLateIntegration();
    }

    private string GetMimeForExt(string ext)
    {
        switch(ext)
        {
            case "json":
            case "txt":
            case "xml":
            case "wsdl":
            case "cs":
                return $"text/{ext}";
            default: throw new UnsupportedException(ext);
        }
    }

    private string GetFilePathExt(string path)
    {
        if (!path.Contains("."))
            throw new ArgumentException($"Путь к файлу не содержит точки {path}");
        if (!path.Contains(Path.DirectorySeparatorChar))
            throw new ArgumentException($"Путь к файлу не разделителя {path}");
        var name = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        var ext = name.Substring(name.LastIndexOf(".") + 1);
        return ext.ToLower();
    }

    public void AddDbCatalogsAndFilesReqursive(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        DeleteDbFiles(catalogId);
        AddDbFiles(catalogId);
        foreach (var catalog in GetDirectories(catalogPath).Select(path => new FileCatalog()
        {
            Name = path.Substring(catalogPath.Length),
            ParentId = catalogId
        }))
        {
            this._fileDbContext.Add(catalog);
            this._fileDbContext.SaveChanges();
            AddDbCatalogsAndFilesReqursive(catalog.Id);
        }       
    }
     

    public void DeleteDriveFiles(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        foreach (var file in GetFiles(catalogPath))
        {
            System.IO.File.Delete(file);
        }
    }

    public void DeleteDriveCatalogs(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        foreach (var file in GetDirectories(catalogPath))
        {
            System.IO.Directory.Delete(file);
        }
    }

    public void AddDriveCatalogs(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        foreach (var catalog in this._fileDbContext.FileCatalogs.Where(c => c.ParentId==catalogId))
        {
            var creatingCatalogPath = Path.Combine(catalogPath, catalog.Name);    
            System.IO.Directory.CreateDirectory(creatingCatalogPath);
        }
    }

    public void AddDriveFiles(int catalogId)
    {
        var catalogPath = this.GetCatalogPath(catalogId);
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(catalogId));
        }
        foreach (var resource in this._fileDbContext.FileResources.Where(c => c.ParentId == catalogId))
        {
            var creatingCatalogPath = Path.Combine(catalogPath, resource.Name);
            System.IO.File.WriteAllBytes(creatingCatalogPath, resource.Data);
        }
    }

    public string GetCatalogPath(int? catalogId)
    {
        
        var drivePrefix = GetCurrentDirectory();
        if(catalogId is null)
        {
            return Path.Combine(GetCurrentDirectory(), "resources");
        }
        List<string> names = new();
        
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(catalogId);
        if (fileCatalog is null)
        {

            throw new ArgumentException(nameof(catalogId), $"Не найден каталог по идентификатору: {catalogId}\n" + _fileDbContext.FileCatalogs.ToList().ToJsonOnScreen());
        }
        while(fileCatalog is not null)
        {
            names.Add(fileCatalog.Name);
            fileCatalog = fileCatalog.ParentId is null? null : this._fileDbContext.FileCatalogs.Find(fileCatalog.ParentId);
        }
        names.Reverse();
        var sb = new StringBuilder();
        names.ForEach(name => { sb.Append(Path.DirectorySeparatorChar); sb.Append(name); });
        return drivePrefix + sb.ToString();


    }

    public string GetFilePath(int fileId)
    {
        var fileResource = this._fileDbContext.FileResources.Find(fileId);
        if(fileResource is null)
        {
            throw new ArgumentException(nameof(fileId), $"Не найден {nameof(FileResource)} с Id={fileId}");
        }
        var fileCatalog = this._fileDbContext.FileCatalogs.Find(fileResource.ParentId);
        if (fileCatalog is null)
        {
            throw new ArgumentException(nameof(fileId));
        }
        var catalogPath = GetCatalogPath((int)fileResource.ParentId);
        return Path.Combine(catalogPath, fileResource.Name);
    }

    public void InitCatalogs()
    {
        _fileDbContext.EnableFileSystemBinding = false;
        var rootPath = Path.Combine(GetCurrentDirectory(), "resources");
        if (!Exists(rootPath))
        {
            CreateDirectory(rootPath);
        }
        var rootCatalog = _fileDbContext.FileCatalogs.FirstOrDefault(c => c.Name == "resources");
        if(rootCatalog is null)
        {
            rootCatalog = new FileCatalog()
            {
                Name = "resources",
                Label = "Корень иерархии",
                Description = "Основной каталог"
            };
            _fileDbContext.Add(rootCatalog);
            _fileDbContext.SaveChanges();
        }
        _fileDbContext.EnableFileSystemBinding = true;
    }

    public int CreateCatalogs(string path)
    {
        var drive = GetCatalogPath(null);
        this.Debug(drive);
        this.Debug(path);
        if(path==drive)
        {
            return 0;
        }
        path = path.StartsWith(drive) ? path.Substring(drive.Length+1): path;
        var ids = path.Split(System.IO.Path.DirectorySeparatorChar).Where(id => !String.IsNullOrWhiteSpace(id) && id!=Path.DirectorySeparatorChar+"").ToList();
        var lastpath = $"{drive}";
        int? idParent = null;
        foreach(var id in ids)
        {
            lastpath += $"{System.IO.Path.DirectorySeparatorChar}{id}";
            if (!Exists(lastpath))
                CreateDirectory(lastpath);
            var c = this._fileDbContext.FileCatalogs.FirstOrDefault(c => c.ParentId == idParent && c.Name == id);
            if (c is not null)
            {
                idParent = c.Id;
                var ppath = GetCatalogPath(c.Id);
                if (!Exists(ppath))
                    CreateDirectory(ppath);
                continue;
            }

            FileCatalog f = null;
            this._fileDbContext.FileCatalogs.Add(f=new()
            {
                Name = id,
                Label = id,
                Description = id,
                ParentId = idParent,
            });
            this._fileDbContext.SaveChangesWithLateIntegration();
            var pppath = GetCatalogPath(f.Id);
            if (!Exists(pppath))
                CreateDirectory(pppath);
            idParent = f.Id;
        }
        return (int)idParent;
    }

    public int DeleteCatalogs(string path)
    {
        var drive = path.Substring(0, 2);
        path = path.Substring(2);
        var ids = path.Split(System.IO.Path.DirectorySeparatorChar);
        var lastpath = $"{drive}";
        int? idParent = null;
        foreach (var id in ids)
        {
            lastpath += $"{System.IO.Path.DirectorySeparatorChar}{id}";
            if (!Exists(lastpath))
                CreateDirectory(lastpath);
            var c = this._fileDbContext.FileCatalogs.FirstOrDefault(c => c.ParentId == idParent && c.Name == id);
            if (c is not null)
            {
                idParent = c.Id;
                continue;
            }
            FileCatalog f = null;
            this._fileDbContext.FileCatalogs.Add(f = new()
            {
                Name = id,
                ParentId = idParent,
            });
            this._fileDbContext.SaveChanges();
            idParent = f.Id;
        }
        return (int)idParent;
    }

  

    /// <summary>
    /// Выполняет сохранение каталога в базу данных
    /// </summary>
    /// <param name="fileCatalog"></param>
    //public void Import( string fileCatalog )
    //{
    //    try
    //    {            
    //        Dictionary<string, FileCatalog> catalogs = new Dictionary<string, FileCatalog>();
    //        fileCatalog.DoBroadcastToBrothers<string>((string p) => {
    //            //_context.
    //            FileCatalog catalog =
    //                catalogs[p.GetPath("\\")] = 
    //                    new FileCatalog(p.Name); 
    //            if(p.Parent != null)
    //            {
    //                catalog.Parent = catalogs[p.Parent.GetPath("\\")];
    //            }
    //            foreach(TypeFile file in p.Item.Values)
    //            {
    //                FileResource resource = new FileResource( );
    //                catalog.Files.Add(resource);
    //                _context.FilResources.Add(resource);
    //            }
    //            _context.FileCatalogs.Add(catalogs[p.GetPath("\\")]);
    //        });
    //        _context.SaveChanges();
    //    }
    //    catch(Exception ex)
    //    {
    //        this.Error(ex);
    //    }
    //}


} 