using System;
using System.Runtime.InteropServices;
using DBPractice.Models;
namespace DBPractice.Models
{
    public class Garbage
    {
        public string id { get; set; }
        public string type { get; set; }
        public char result { get; set; }
        public DateTime finishtime { get; set; }
    }

    public class ThrowGarbage
    {
        public string username { get; set; }
        public string garbageid { get; set; }
        public char status { get; set; }
        public DateTime throwtime { get; set; }
    }

    public class TransportStart
    {
        public string garbageid { get; set; }
        public string truckid { get; set; }
        public DateTime starttime { get; set; }
    }

    public class TransportEnd
    {
        public string garbageid { get; set; }
        public string truckid { get; set; }
        public DateTime endtime { get; set; }
    }
    
}