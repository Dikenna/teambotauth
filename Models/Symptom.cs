using System;
using System.Collections.Generic;

namespace AuthenticationBot.Models
{
    public class Symptom
    {
        public string name { get; set; }

        public string description { get; set; }

        public string type { get; set; }

        public List<string> subSymptomIds { get; set; }

        public List<string> dataCollectors { get; set; }

        public List<string> analyzers { get; set; }

        public string workflowId { get; set; }

        public InputCard inputCard { get; set; }

        public List<string> tags { get; set; }

        public string id { get; set; }

        public string etag { get; set; }
    }
}
