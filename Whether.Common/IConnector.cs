using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whether
{
    public interface IConnector
    {
        Task<List<WhetherDay>> GetWhetherOnDay(DateTime day, string city);
    }
}
