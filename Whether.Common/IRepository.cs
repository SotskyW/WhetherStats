using System;
using System.Collections.Generic;

namespace Whether.Repository
{
    public interface IRepository
    {
        List<WhetherDay> GetDayWhethere(string city, DateTime day);

        void UpsertWhetherStat(List<WhetherDay> stats);
    }
}
