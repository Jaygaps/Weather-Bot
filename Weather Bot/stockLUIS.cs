using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather_Bot
{
    public class stockLUIS
    {
        public string query { get; set; }
        public Topscoringintent[] intents { get; set; }
    }
    public class Rootobject
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public object[] entities { get; set; }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

}