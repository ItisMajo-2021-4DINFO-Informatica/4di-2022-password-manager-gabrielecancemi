using System;

namespace Notes.Models
{
    public class Note
    {

        public int ID { get; set; }
        public string SeviceName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public DateTime Date { get; set; }
    }
}
