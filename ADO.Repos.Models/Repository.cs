using System;

namespace ADO.Repos.Models
{
    public class Repository
    {
        public string Url {get; set; }
        public string Name { get; set; }    
        public bool MainIsAheadOfDev { get; set; }
        public Branch Main { get; set; }
        public Branch Dev { get; set; }
        public Branch Test { get; set; }
        public Branch Master { get; set; }
    }
}
