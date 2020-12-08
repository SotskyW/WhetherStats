using CsvHelper.Configuration.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Whether.Common;

namespace Whether
{

    public class WhetherStats
    {
        public List<DateTime> DaysRange { get; set; } = new List<DateTime>();
        public List<YeatStats> YearsStats { get; set; } = new List<YeatStats>();
    }

    public class DayAverage
    {
        public DateTime Day { get; set; }

        public int NumberOfDays { get; set; }

        public int DayMaxTemp { get; set; }

        public int DayMinTemp { get; set; }

        public int NightMaxTemp { get; set; }

        public int NightMinTemp { get; set; }
        public int EveningMaxTemp { get; set; }

        public int EveningMinTemp { get; set; }
        public int MorningMaxTemp { get; set; }

        public int MorningMinTemp { get; set; }

        public Dictionary<string, Tuple<DateTime, int>> Preasures { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();
        public Dictionary<string, Tuple<DateTime, int>> Humidities { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();
        public Dictionary<string, Tuple<DateTime, int>> Winds { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();
        public Dictionary<string, Tuple<DateTime, int>> WindDirections { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();

        public Dictionary<string, Tuple<DateTime, int>> MorningClouds { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();

        public Dictionary<string, Tuple<DateTime, int>> DayClouds { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();

        public Dictionary<string, Tuple<DateTime, int>> EveningClouds { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();
        public Dictionary<string, Tuple<DateTime, int>> NightClouds { get; set; } = new Dictionary<string, Tuple<DateTime, int>>();
    }

    public class YeatStats
    {
        public int Year { get; set; }
        public List<DayStats> DaysStat { get; set; } = new List<DayStats>();
    }

    public class DayStats
    {
        public string DayTemperature { get; set; }
        public int MaxDayTemp { get; set; }
        public int MinNightTemp { get; set; }
        public DateTime WeekDay { get; set; }
        public List<string> WhetherIcons { get; set; }
        public string NightTemperature { get; set; }
        public WhetherDay NightStat { get; set; }
        public WhetherDay MorningStat { get; set; }
        public WhetherDay DayStat { get; set; }
        public WhetherDay EveningStat { get; set; }
    }
    public class WhetherDay
    {
        public DateTime Date { get; set; }
        public DayPartEnum DayPart { get; set; }
        public string DayPartName => DayPart.ToString();
        public string Temperature { get; set; }
        public string Preasure { get; set; }
        public string Clouds { get; set; }
        public string Humidity { get; set; }
        public string Wind { get; set; }
        [Ignore]
        public int Year { get; set; }
        public string WindDirection { get; set; }
        [Ignore]
        public string City { get; set; }
        [Ignore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

    }

}
