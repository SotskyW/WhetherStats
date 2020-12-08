using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whether.Common;

namespace Whether.Providers
{
    public class MongoDBConnectionProvider : IConnectionStringProvider
    {
        public MongoDBConnectionProvider(IConfiguration config)
        {
            ConnectionString = config["Data:MongoConnection"];
        }
        public string ConnectionString { get; set; } = "";
        public string BaseFolder { get; set; }
    }
}
