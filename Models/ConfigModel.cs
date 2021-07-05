using System.Runtime.InteropServices;

namespace DBPractice.Models
{
    public static class Config
    {
        static public int TEST = -2;//测试返回值
        static public int SUCCESS = 1;//成功
        static public int FAIL = -1;//失败
        static public string HOST = "loaclhost";
        static public string USERNAME = "C##PDCR";
        static public string PASSWORD = "Lty20001212";
        static public string SERVICENAME = "ORCL";
        static public string CONN = $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={HOST})(PORT=6001)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={SERVICENAME})));" +
                                    $"User Id={USERNAME};" +
                                    $"Password={PASSWORD};";
    }
}