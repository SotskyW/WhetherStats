using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whether.Common;

namespace Whether.Providers
{
    public class FileSystemConnectionProvider : IConnectionStringProvider
    {
        public FileSystemConnectionProvider(IWebHostEnvironment env)
        {
            BaseFolder = $@"{env.ContentRootPath}\App_Data";
        }
        public string ConnectionString { get; set; } = "";
        public string BaseFolder { get; set; }
    }
}
