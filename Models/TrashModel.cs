using System;
namespace DBPractice.Models
{
    public class Garbage
    {
        public string gar_id { get; set; }
        public string type { get; set; }
        public string user_id { get; set; }
        public DateTime created_time { get; set; }
    }
    public class DueTime
    {
        public string year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
    }
    public class ViolateRecord
    {
        public string user_id { get; set; }
        public string watcher_id { get; set; }
        public string reason { get; set; }
        public int punishment { get; set; }
        public DateTime violate_time { get; set; }
    }

    public class Interact
    {
        public string carrier_id { get; set; }
        public string staff_id { get; set; }
        public string interact_time { get; set; }
        public char interact_result { get; set; }
    }
}