using System;

namespace ADO.Repos.Models
{
    public struct ReleaseId
    {
        private readonly string _name;

        public ReleaseId(string name)
        {
            _name = name;
        }

        public string GetId() => $"release/{_name}";
    }
}