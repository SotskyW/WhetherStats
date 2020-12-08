using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Whether.ServiceModels
{
    public class WhetherStatsServiceModel
    {
        public WhetherStatsServiceModel(WhetherStats dto)
        {
            foreach (var day in dto.DaysRange)
            {
                DaysRange.Add(new DayModel { DayTitle = day.ToString("dd MMM"), IsCurrentDay = day.Day == DateTime.Today.Day && day.Month == DateTime.Today.Month });
            }

            foreach (var yearStatDto in dto.YearsStats)
            {
                var yearStat = new YeatStatsModel { Year = yearStatDto.Year < 0 ? "Average" : $"{yearStatDto.Year}" };
                foreach (var stat in yearStatDto.DaysStat)
                {
                    var dayStatModel = new DayStatsModel
                    {
                        DayTemperature = stat.DayTemperature,
                        NightTemperature = stat.NightTemperature,
                        DayClass = stat.MaxDayTemp > 25 ? "hot" : stat.MinNightTemp < -10 ? "cold" : "",
                        WeekDay = stat.WeekDay.ToString("ddd"),
                        WeekEndClass = stat.WeekDay.DayOfWeek == DayOfWeek.Saturday || stat.WeekDay.DayOfWeek == DayOfWeek.Sunday ? "week-end" : "",
                        IsCurrentDay = stat.WeekDay == DateTime.Today,
                        WhetherIcons = string.Join(" ", stat.WhetherIcons),
                       
                    };
                    dayStatModel.Details.Add(stat.NightStat);
                    dayStatModel.Details.Add(stat.DayStat);
                    dayStatModel.Details.Add(stat.MorningStat);
                    dayStatModel.Details.Add(stat.EveningStat);


                    yearStat.DaysStat.Add(
                       dayStatModel);
                }
                YearsStats.Add(yearStat);
            }
        }
        public List<DayModel> DaysRange { get; set; } = new List<DayModel>();
        public List<YeatStatsModel> YearsStats { get; set; } = new List<YeatStatsModel>();

    }

    public class YeatStatsModel
    {
        public string Year { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
        public List<DayStatsModel> DaysStat { get; set; } = new List<DayStatsModel>();
    }

    public class DayModel
    {
        public string DayTitle { get; set; }
        public bool IsCurrentDay { get; set; }

    }
    public class DayStatsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DayTemperature { get; set; }
        public string NightTemperature { get; set; }
        public string WeekDay { get; set; }
        public string WeekEndClass { get; set; }
        public string DayClass { get; set; }
        public bool IsCurrentDay { get; set; }
        public string WhetherIcons { get; set; }
        public List<WhetherDay> Details { get; set; } = new List<WhetherDay>(4);

    }


}
