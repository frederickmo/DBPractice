using System.Runtime.InteropServices;
using System;

namespace DBPractice.Models
{

    public class User
    {
        public string id { get; set; }
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

    public class Carrier : User
    {
        
    }

    public class StationStaff : User
    {
        public string plantname { get; set; }
    }
    public class DutyArrange
    {
        public string watcher_id { get; set; }
        public string site_name { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
    }
    public class LoginRequest
    {
        public string userID { get; set; }
        public string password { get; set; }
    }

    public class LoginResponse
    {
        public int status { get; set; }
        public string role { get; set; }
        public string token { get; set; }
        public string loginMessage { get; set; }
    }
    //注册请求结构
    public class WCRegisterRequest
    {     
        public Watcher watcher { get; set; }   
    }
    
    public class GMRegisterRequest
    {      
        public GarbageMan garbageman { get; set; }      
    }
    public class CRRegisterRequest
    {      
        public Carrier transportpersonnel { get; set; }       
    }
    public class SSRegisterRequest
    {       
        public StationStaff stationstaff { get; set; }     
    }
    public class ADRegisterRequest
    {
        public  Administrator administrator { get; set; }
    }
    //注册返回结构
    public class RegisterResponse
    {
        public int status { get; set; }
        public string registerMessage { get; set; }
    }
    //更新请求结构
    public class WCUpdateRequest
    {
        public Watcher watcher { get; set; }
        public string password { get; set; }
    }

    public class GMUpdateRequest
    {
        public GarbageMan garbageman { get; set; }
        public string password { get; set; }
    }
    public class CRUpdateRequest
    {
        public Carrier transportpersonnel { get; set; }
        public string password { get; set; }
    }
    public class SSUpdateRequest
    {
        public StationStaff stationstaff { get; set; }
        public string password { get; set; }
    }
    public class ADUpdateRequest
    {
        public Administrator administrator { get; set; }
        public string password { get; set; }
    }
    //删除请求结构
    public class DeleteRequest
    {
        public string id { get; set; }
        public string password { get; set; }
    }
}