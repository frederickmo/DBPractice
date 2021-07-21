namespace DBPractice.Models
{
    public class TrashCan
    {
        public string id { get; set; }
        public string type { get; set; }
        public char condition { get; set; }
        public string siteName { get; set; }
       
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
        public string carrierID { get; set; }
    }
    public class Plant
    {
        public string name { get; set; }
        public string address { get; set; }
    }

    public class Response
    {
        public int status { get; set; }
    }
    public class AddResponse:Response
    {
        public string addMessage { get; set; }
    }

    public class DeleteResponse:Response
    {
        public string deleteMessage { get; set; }
    }

    public class UpdateResponse : Response
    {
        public string updateMessage { get; set; }
    }
}