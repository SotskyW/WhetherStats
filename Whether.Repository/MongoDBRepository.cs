using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Whether.Common;

namespace Whether.Repository
{
    public class MongoDBRepository : BaseRepository
    {
        public List<WhetherDay> loadedRecords = new List<WhetherDay>();
        public MongoDBRepository(IConnectionStringProvider connectionProvider, IWhetherLogger logger) : base(connectionProvider, logger)
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
            var dbClient = new MongoClient(_connectionProvider.ConnectionString);
            var database = dbClient.GetDatabase("whether");
            var collection = database.GetCollection<WhetherDay>("wherether-state");

            var filter = Builders<WhetherDay>.Filter.Where(x => x.City == city && x.Year == year);
            return collection.Find(filter).ToList();
        }

        public override void UpsertWhetherStat(List<WhetherDay> newStats)
        {
            try
            {
                var dbClient = new MongoClient(_connectionProvider.ConnectionString);
                var database = dbClient.GetDatabase("whether");
                var collection = database.GetCollection<WhetherDay>("wherether-state");
                collection.InsertMany(newStats);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
