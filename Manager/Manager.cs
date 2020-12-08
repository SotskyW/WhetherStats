using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Whether;
using Whether.Common;
using Whether.Repository;

namespace Whether
{
    public class Manager
    {
        IRepository _repository;
        IWhetherLogger _logger;
        public Manager(IRepository repository, IWhetherLogger logger)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<WhetherStats> GetWhetherStats(DateTime date, string city, int daysInPast = 2, int daysInFuture = 15)
        {
            var result = new WhetherStats();
            var averageStats = new List<DayAverage>();
            int daysCount, yearEnd;
            DateTime dayStart, dayToAdd;
            CalculateRanges(date, daysInPast, daysInFuture, result, out daysCount, out yearEnd, out dayStart, out dayToAdd);

            var daysStats = await GetFeed(city, dayStart, yearEnd, daysCount);

            for (int year = dayStart.Year; year <= yearEnd; year++)
            {
                dayToAdd = new DateTime(year, dayStart.Month, dayStart.Day);

                var yearStat = new YeatStats() { Year = year };
                for (int i = 0; i < daysCount; i++)
                {

                    var dayStat = daysStats.FirstOrDefault(x => x.Date == dayToAdd && x.DayPart == DayPartEnum.Day) ?? new WhetherDay();
                    var morningStat = daysStats.FirstOrDefault(x => x.Date == dayToAdd && x.DayPart == DayPartEnum.Morning) ?? new WhetherDay();
                    var eveningStat = daysStats.FirstOrDefault(x => x.Date == dayToAdd && x.DayPart == DayPartEnum.Evening) ?? new WhetherDay();
                    var nightStat = daysStats.FirstOrDefault(x => x.Date == dayToAdd && x.DayPart == DayPartEnum.Night) ?? new WhetherDay();

                    var maxDayTemp = 0;
                    var minDayTemp = 0;
                    var maxMorningTemp = 0;
                    var minMorningTemp = 0;
                    var maxEveningTemp = 0;
                    var minEveningTemp = 0;
                    var minNightTemp = 0;
                    var maxNightTemp = 0;
                    if (!string.IsNullOrWhiteSpace(morningStat.Temperature))
                    {
                        maxMorningTemp = Convert.ToInt32(morningStat.Temperature.Split(' ').LastOrDefault());
                        minMorningTemp = Convert.ToInt32(morningStat.Temperature.Split(' ').FirstOrDefault());
                    }
                    if (!string.IsNullOrWhiteSpace(dayStat.Temperature))
                    {
                        maxDayTemp = Convert.ToInt32(dayStat.Temperature.Split(' ').LastOrDefault());
                        minDayTemp = Convert.ToInt32(dayStat.Temperature.Split(' ').FirstOrDefault());
                    }
                    if (!string.IsNullOrWhiteSpace(nightStat.Temperature))
                    {
                        maxNightTemp = Convert.ToInt32(nightStat.Temperature.Split(' ').LastOrDefault());
                        minNightTemp = Convert.ToInt32(nightStat.Temperature.Split(' ').FirstOrDefault());
                    }
                    if (!string.IsNullOrWhiteSpace(eveningStat.Temperature))
                    {
                        maxEveningTemp = Convert.ToInt32(eveningStat.Temperature.Split(' ').LastOrDefault());
                        minEveningTemp = Convert.ToInt32(eveningStat.Temperature.Split(' ').FirstOrDefault());
                    }
                    if (dayToAdd <= DateTime.UtcNow.Date)
                    {
                        var currentDayAverage = averageStats.FirstOrDefault(x => x.Day.Day == dayToAdd.Day && x.Day.Month == dayToAdd.Month);
                        if (currentDayAverage == null)
                        {
                            currentDayAverage = new DayAverage() { Day = dayToAdd };
                            averageStats.Add(currentDayAverage);
                        }

                        currentDayAverage.NumberOfDays++;
                        currentDayAverage.DayMaxTemp += maxDayTemp;
                        currentDayAverage.DayMinTemp += minDayTemp;
                        currentDayAverage.NightMaxTemp += maxNightTemp;
                        currentDayAverage.NightMinTemp += minNightTemp;
                        currentDayAverage.EveningMaxTemp += maxEveningTemp;
                        currentDayAverage.EveningMinTemp += minEveningTemp;
                        currentDayAverage.MorningMaxTemp += maxMorningTemp;
                        currentDayAverage.MorningMinTemp += minMorningTemp;
                        currentDayAverage.MorningClouds.AddOrUpdate(morningStat.Clouds, dayToAdd);
                        currentDayAverage.DayClouds.AddOrUpdate(dayStat.Clouds, dayToAdd);
                        currentDayAverage.EveningClouds.AddOrUpdate(eveningStat.Clouds, dayToAdd);
                        currentDayAverage.NightClouds.AddOrUpdate(nightStat.Clouds, dayToAdd);
                        
                        currentDayAverage.Preasures.AddOrUpdate(nightStat.Preasure, dayToAdd);
                        currentDayAverage.Preasures.AddOrUpdate(morningStat.Preasure, dayToAdd);
                        currentDayAverage.Preasures.AddOrUpdate(eveningStat.Preasure, dayToAdd);
                        currentDayAverage.Preasures.AddOrUpdate(dayStat.Preasure, dayToAdd);

                        currentDayAverage.Humidities.AddOrUpdate(nightStat.Humidity, dayToAdd);
                        currentDayAverage.Humidities.AddOrUpdate(morningStat.Humidity, dayToAdd);
                        currentDayAverage.Humidities.AddOrUpdate(eveningStat.Humidity, dayToAdd);
                        currentDayAverage.Humidities.AddOrUpdate(dayStat.Humidity, dayToAdd);

                        currentDayAverage.Winds.AddOrUpdate(nightStat.Wind, dayToAdd);
                        currentDayAverage.Winds.AddOrUpdate(morningStat.Wind, dayToAdd);
                        currentDayAverage.Winds.AddOrUpdate(eveningStat.Wind, dayToAdd);
                        currentDayAverage.Winds.AddOrUpdate(dayStat.Wind, dayToAdd);

                        currentDayAverage.WindDirections.AddOrUpdate(nightStat.WindDirection, dayToAdd);
                        currentDayAverage.WindDirections.AddOrUpdate(morningStat.WindDirection, dayToAdd);
                        currentDayAverage.WindDirections.AddOrUpdate(eveningStat.WindDirection, dayToAdd);
                        currentDayAverage.WindDirections.AddOrUpdate(dayStat.WindDirection, dayToAdd);
                    }
                    yearStat.DaysStat.Add(new DayStats
                    {
                        DayTemperature = $"{dayStat.Temperature}",
                        MaxDayTemp = maxDayTemp,
                        MinNightTemp = minNightTemp,
                        WeekDay = dayToAdd,
                        NightTemperature = $"{nightStat.Temperature}",
                        DayStat = dayStat,
                        MorningStat = morningStat,
                        EveningStat = eveningStat,
                        NightStat = nightStat,
                        WhetherIcons = new List<string> { morningStat.Clouds, dayStat.Clouds, eveningStat.Clouds }
                    });

                    dayToAdd = dayToAdd.AddDays(1);
                    if (dayToAdd.Year != year)
                    {
                        dayToAdd = dayToAdd.AddYears(-1);
                    }
                    if (dayToAdd.Month == 2 && dayToAdd.Day == 29)
                    {
                        dayToAdd = dayToAdd.AddDays(1);
                    }
                }
                result.YearsStats.Add(yearStat);
            }

            var averageYear = new YeatStats { Year = -1 };
            foreach (var average in averageStats)
            {
                var dayStat = new DayStats
                {
                    WeekDay = average.Day,
                    DayTemperature = Helper.ToTempStat(average.DayMinTemp / average.NumberOfDays, average.DayMaxTemp / average.NumberOfDays),
                    NightTemperature = Helper.ToTempStat(average.NightMinTemp / average.NumberOfDays, average.NightMaxTemp / average.NumberOfDays),
                };

                var dayTemperature = Helper.ToTempStat(average.DayMinTemp / average.NumberOfDays, average.DayMaxTemp / average.NumberOfDays);
                var nightTemperature = Helper.ToTempStat(average.NightMinTemp / average.NumberOfDays, average.NightMaxTemp / average.NumberOfDays);
                var morningTemperature = Helper.ToTempStat(average.MorningMinTemp / average.NumberOfDays, average.MorningMaxTemp / average.NumberOfDays);
                var eveningTemperature = Helper.ToTempStat(average.EveningMinTemp / average.NumberOfDays, average.EveningMaxTemp / average.NumberOfDays);

                var morningClouds = average.MorningClouds.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var dayClouds = average.DayClouds.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var eveningClouds = average.EveningClouds.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var nightClouds = average.NightClouds.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;

                var humidity = average.Humidities.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var preasure = average.Preasures.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var wind = average.Winds.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;
                var windDirection = average.WindDirections.OrderByDescending(x => x.Value.Item2).ThenByDescending(x => x.Value.Item1).FirstOrDefault().Key;


                dayStat.DayTemperature = dayTemperature;
                dayStat.NightTemperature = nightTemperature;
                dayStat.WhetherIcons = new List<string> { morningClouds, dayClouds, eveningClouds };
                dayStat.DayStat = new WhetherDay { Date = average.Day, DayPart = DayPartEnum.Day, Clouds = dayClouds, Temperature = dayTemperature, Humidity = humidity, Preasure = preasure, Wind = wind, WindDirection = windDirection };
                dayStat.EveningStat = new WhetherDay { Date = average.Day, DayPart = DayPartEnum.Evening, Clouds = eveningClouds, Temperature = eveningTemperature, Humidity = humidity, Preasure = preasure, Wind = wind, WindDirection = windDirection };
                dayStat.MorningStat = new WhetherDay { Date = average.Day, DayPart = DayPartEnum.Morning, Clouds = morningClouds, Temperature = morningTemperature, Humidity = humidity, Preasure = preasure, Wind = wind, WindDirection = windDirection };
                dayStat.NightStat = new WhetherDay { Date = average.Day, DayPart = DayPartEnum.Night, Clouds = nightClouds, Temperature = nightTemperature, Humidity = humidity, Preasure = preasure, Wind = wind, WindDirection = windDirection };


                averageYear.DaysStat.Add(dayStat);
            }
            result.YearsStats.Add(averageYear);

            return result;
        }

        private static void CalculateRanges(DateTime date, int daysInPast, int daysInFuture, WhetherStats result, out int daysCount, out int yearEnd, out DateTime dayStart, out DateTime dayToAdd)
        {
            var numberYearsInThePast = -6;
            daysCount = daysInPast + daysInFuture;
            yearEnd = date.Year;
            if (date.AddDays(-daysInPast) > DateTime.UtcNow.Date)
            {
                yearEnd--;
            }
            dayStart = date.AddYears(numberYearsInThePast);
            dayStart = dayStart.AddDays(-daysInPast);

            if (dayStart.Month == 2 && dayStart.Day == 29)
            {
                dayStart = dayStart.AddDays(-1);
            }

            dayToAdd = dayStart;
            for (int i = 0; i < daysCount; i++)
            {
                result.DaysRange.Add(dayToAdd);
                dayToAdd = dayToAdd.AddDays(1);
                if (dayToAdd.Month == 2 && dayToAdd.Day == 29)
                {
                    dayToAdd = dayToAdd.AddDays(1);
                }
            }
        }

        private async Task<List<WhetherDay>> GetFeed(string city, DateTime dayStart, int yearEnd, int daysCount)
        {
            var connector = ConnectorsFactory.GetConnectorByCity(_logger);
            var daysStats = new List<WhetherDay>();
            var tasks = new List<Task<List<WhetherDay>>>();
            var newDates = new List<WhetherDay>();

            for (int year = dayStart.Year; year <= yearEnd; year++)
            {
                var extraDays = 0;
                if (DateTime.IsLeapYear(year))
                {
                    extraDays = 1;
                }

                var day = DateTime.SpecifyKind( new DateTime(year, dayStart.Month, dayStart.Day), DateTimeKind.Utc);
                for (int i = 0; i < daysCount + extraDays; i++)
                {
                    if (day.Year != year)
                    {
                        day = day.AddYears(-1);
                    }
                    if (day > DateTime.UtcNow.Date)
                    {
                        day = day.AddDays(1);
                        continue;
                    }
                    var storedDayWhether = _repository.GetDayWhethere(city, day);
                    if (storedDayWhether.Any())
                    {
                        daysStats.AddRange(storedDayWhether);
                    }
                    else
                    {
                        tasks.Add(connector.GetWhetherOnDay(day, city));
                    }
                    day = day.AddDays(1);
                }
            }


            Task.WaitAll(tasks.ToArray());
            foreach (var task in tasks)
            {
                var whetherFromFeed = await task;
                daysStats.AddRange(whetherFromFeed);
                newDates.AddRange(whetherFromFeed);
            }

            if (newDates.Any())
            {
                _repository.UpsertWhetherStat(newDates);
            }

            return daysStats;
        }
    }

}
