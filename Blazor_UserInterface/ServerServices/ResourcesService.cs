 
 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
 
/// <summary>
/// Служба управления ресурсами
/// </summary>
public class ResourcesService: BaseService
{
    private readonly ApplicationDbContext _context;


    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="context"></param>
    public ResourcesService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Вывод сообщения в консоль
    /// </summary>
    /// <param name="message"></param>
    protected void Info(object item) {
        string message = $"[{GetType().Name}][{DateTime.Now}] => {item}";
        Debug.WriteLine(message);
        Console.WriteLine(message);
    }



    /// <summary>
    /// Выполняет сохранение файлов каталога в базу данных
    /// </summary>  
    public void Save( string package, string path, string filter="*.*" )
    {
        try
        {
            TypeCatalog catalog = new TypeCatalog(@"D:\", "commands");
            catalog.DoBroadcastToHierElements((p) => {
                Console.WriteLine(((TypeCatalog)p).Name);
            });
            Dictionary<string, FileCatalog> catalogs = new Dictionary<string, FileCatalog>();
            catalog.DoBroadcastToBrothers<TypeCatalog>((TypeCatalog p) => {                
                FileCatalog catalog =
                    catalogs[p.GetPath("\\")] = 
                        new FileCatalog(p.Name); 
                if(p.Parent != null)
                {
                    catalog.Parent = catalogs[p.Parent.GetPath("\\")];
                }
                foreach(TypeFile file in p.Item.Values)
                {
                    FileResource resource = new FileResource(file);
                    catalog.Files.Add(resource);
                    _context.FilResources.Add(resource);
                }
                _context.FileCatalogs.Add(catalogs[p.GetPath("\\")]);
            });
            if (_context.SaveChanges() <=0 )
            {
                throw new Exception();
            }
        }
        catch(Exception ex)
        {
            Info(ex);
        }
    }
} 