using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Whether.Common;

namespace Whether.Repository
{
    public class FileSystemRepository : BaseRepository
    {
        public List<WhetherDay> loadedRecords = new List<WhetherDay>();

        public FileSystemRepository(IConnectionStringProvider connectionProvider, IWhetherLogger logger) : base(connectionProvider, logger)
        {
        }
        public override List<WhetherDay> GetDayWhethere(string city, DateTime day)
        {
            try
            {
                if (!loadedRecords.Any(x => x.Date.Year == day.Year && x.City == city))
                {
                    loadedRecords.AddRange(GetWhethereYear(city, day.Year));
                }
                return loadedRecords.Where(x => x.Date == day && x.City == city).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new List<WhetherDay>();
            }
        }

        private List<WhetherDay> GetWhethereYear(string city, int year)
        {
            var fileName = $@"{_connectionProvider.BaseFolder}\Wether_{city}_{year}.csv";

            if (File.Exists(fileName))
            {
                using (var reader = new StreamReader(fileName))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        var res = csv.GetRecords<WhetherDay>().Select(x => { x.City = string.IsNullOrEmpty(x.City) ? city : x.City; return x; }).ToList();
                        return res;
                    }
                }
            }
            return new List<WhetherDay>();
        }

        public override void UpsertWhetherStat(List<WhetherDay> stats)
        {
            try
            {
                foreach (var city in stats.Select(x => x.City).Distinct())
                {
                    foreach (var year in stats.Select(x => x.Date.Year).Distinct())
                    {
                        var storedStats = GetWhethereYear(city, year);
                        storedStats.AddRange(stats.Where(x => !storedStats.Any(y => y.City == x.City && y.Date == x.Date)));
                        var fileName = $@"{_connectionProvider.BaseFolder}\Wether_{city}_{year}.csv";

                        using (TextWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                        {
                            var csv = new CsvWriter(writer);
                            csv.WriteRecords(storedStats.OrderBy(x => x.Date)); // where values implements IEnumerable
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
