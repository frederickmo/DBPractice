namespace DBPractice.Models
{
    public class TrashCan
    {
        public string id { get; set; }
        public string type { get; set; }
        public char condition { get; set; }
    }

    public class BinSite
    {
        public string name { get; set; }
        public string location { get; set; }
    }
    
    public class Truck
    {
        public string id { get; set; }
        public char condition { get; set; }
    }

    public class Plant
    {
         public string id { get; set; }
         public string address { get; set; }
    }

    public class Response
    {
        public int Status { get; set; }
    }
    public class AddResponse:Response
    {
    }

    public class DeleteResponse:Response
    {
    }

    public class UpdateResponse : Response
    {
        
    }
}