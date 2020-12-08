using System;
using System.Collections.Generic;
using System.Text;
using Whether.Common;

namespace Whether.Repository
{
    public abstract class BaseRepository: IRepository
    {
        protected IWhetherLogger _logger;
        protected IConnectionStringProvider _connectionProvider;
        public BaseRepository(IConnectionStringProvider connectionProvider, IWhetherLogger logger)
        {
            _connectionProvider = connectionProvider;
            _logger = logger;
        }

        public abstract List<WhetherDay> GetDayWhethere(string city, DateTime day);

        public abstract void UpsertWhetherStat(List<WhetherDay> stats);
       
    }
}
