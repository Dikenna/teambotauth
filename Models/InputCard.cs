using System;
using System.Collections.Generic;

namespace AuthenticationBot.Models
{
    public class InputCard
    {
        public string type { get; set; }

        public double version { get; set; }

        public List<Body> body { get; set; }


    }
}
