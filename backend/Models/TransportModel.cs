using System;
namespace DBPractice.Models
{
    /*
     * 这些行为产生之后对trash状态的转变，应该用trigger实现吧。
     */

    public class ThrowRequest
    {
        public string gid { get; set; } //垃圾编号
        public string bid { get; set; } //垃圾桶编号
    }

    /*
     * 垃圾的运输应该分为开始和结束
     * 垃圾运输开始之后不应该立刻存进数据库里
     * 而是应该放在本程序里（比如放进list啥的）
     * 在运输到站之后补全信息再写入
     * 运输开始由运输员决定
     * 但是运输结束只能由垃圾站的staff决定
     */
    public class Transport
    {
        public string trans_id { get; set; }
        public string dustbin_id { get; set; }
        public string truck_id { get; set; }//垃圾车编号

        public string carrier_id { get; set; }

        public string plant_name { get; set; }//垃圾处理站
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
  
    }
    public class TransportStart
    {    
        public string truck_id { get; set; }//垃圾车编号
        public string dustbin_id { get; set; }
    }
    /*
     * Finish请求发过来，所有pid是本垃圾站的都更新状态
     */
    public class TransportEnd
    {
        public string truck_id { get; set; }//垃圾车编号
        public string plant_name { get; set; }//垃圾处理站
    }
    
}