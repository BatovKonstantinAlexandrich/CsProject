using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

 
public class ApplicationSignleton
{
    public static int Counter = StartupApplication.AddSingletonConfiguration<ApplicationSignleton>();

    private readonly NavigationManager _navigationManager;
    private readonly ILogger<ApplicationSignleton> _logger;

    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    public ApplicationSignleton( 
            ILogger<ApplicationSignleton> logger, 
            NavigationManager navigationManager )
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += this.LocationChanged;
        this.LocationChanged += LogLocationChanges;
        _logger = logger;
    }

    public void LogLocationChanges(object sender, LocationChangedEventArgs evt)
    {
        _logger.LogInformation(evt.Location);
            
    }
}
 