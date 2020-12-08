using System;
using System.Collections.Generic;
using System.Text;

namespace Whether.Common
{
    public interface IConnectionStringProvider
    {
        string ConnectionString { get; set; }

        string BaseFolder { get; set; }
    }
}
