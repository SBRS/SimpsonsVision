using System;
using Newtonsoft.Json;

namespace SimpsonsVision.DataModels
{
    public class SimpsonsVisionTable
    {
		[JsonProperty(PropertyName = "Id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName = "Tag")]
		public string Tag { get; set; }

		[JsonProperty(PropertyName = "Prediction")]
		public double Prediction { get; set; }

		[JsonProperty(PropertyName = "Longitude")]
		public float Longitude { get; set; }

		[JsonProperty(PropertyName = "Latitude")]
		public float Latitude { get; set; }
    }
}
