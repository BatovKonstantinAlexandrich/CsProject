using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Console;
using static Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using pickpoint_delivery_service;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi
{
    public interface IDeliveryDbContextInitiallizer
    {



        public int InitCustomers(DeliveryDbContext context, int customersCount);
        public int InitHolders(DeliveryDbContext context);
        public int InitProducts(DeliveryDbContext context, string contentPath);
    }

}
