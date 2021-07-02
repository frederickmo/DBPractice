using System.Runtime.InteropServices;

namespace DBPractice.Models
{

    public class User
    {
        public string usernane { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string phonenumber { get; set; }
    }
    public class Administrator:User
    {
    }
    public class GarbageMan:User
    {
        public int credit { get; set; }
        public string address { get; set; }
    }

    public class Watcher:User
    {
        public string address { get; set; }
    }

    public class TransportPersonnel : User
    {
        
    }

    public class StationStaff : User
    {
        public string address { get; set; }
    }
    
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public int status { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }

    public class WCRegisterRequest
    {
        // GarbageMan.credit必须为0
        public Watcher watcher { get; set; } 
    }
    
    public class GMRegisterRequest
    {
        // GarbageMan.credit必须为0
        public GarbageMan garbageman { get; set; } 
    }
    public class TPRegisterRequest
    {
        // GarbageMan.credit必须为0
        public TransportPersonnel transportpersonnel { get; set; } 
    }
    public class SSRegisterRequest
    {
        // GarbageMan.credit必须为0
        public StationStaff stationstaff { get; set; } 
    }
    public class RegisterResponse
    {
        public int status { get; set; }
    }

    public class UpdateRequest
    {
        // user.credit必须为0
        public string token { get; set; }
        public User user { get; set; }
    }
    
    
}