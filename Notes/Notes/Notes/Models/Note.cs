using System;

namespace Notes.Models
{
    public class Note
    {
        public string Filename { get; set; }
        public string ServiceName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
        public DateTime Date { get; set; }
    }
}
