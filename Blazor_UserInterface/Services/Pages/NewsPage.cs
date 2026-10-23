using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.TreeEditor.Services.Pages
{
   
    
    public class NewsPage
    {
        public static int Counter = StartupApplication.AddScopedConfiguration<NewsPage>();
        public InputFormModel Model { get; set; }
        public NewsPage(IServiceProvider provider)
        {
            this.Model = new InputFormModel(provider.GetService<APIAuthorization>().Verify().Settings);
        }
        
    }

    

}
