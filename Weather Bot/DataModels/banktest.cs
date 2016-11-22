using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather_Bot.DataModels
{
    public class banktest
    {

        [JsonProperty(PropertyName = "ID")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "FirstName")]
        public double FirstName { get; set; }

        [JsonProperty(PropertyName = "UpdateDat")]
        public DateTime Updatedat { get; set; }

        [JsonProperty(PropertyName = "CurrentAccount")]
        public double CurrentAccount { get; set; }

        [JsonProperty(PropertyName = "SeriousSaver")]
        public double SeriousSaver { get; set; }

        [JsonProperty(PropertyName = "Version")]
        public double Version { get; set; }

        [JsonProperty(PropertyName = "CreateDat")]
        public DateTime CreateDat { get; set; }

        [JsonProperty(PropertyName = "Deleted")]
        public double Deleted { get; set; }
    }
}