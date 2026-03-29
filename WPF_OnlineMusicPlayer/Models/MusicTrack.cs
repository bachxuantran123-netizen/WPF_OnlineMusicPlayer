using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_OnlineMusicPlayer.Models
{
    public class MusicResponse
    {
        [JsonProperty("results")]
        public List<MusicTrack> results { get; set; }
    }

    public class MusicTrack
    {
        [JsonProperty("trackId")]
        public string id { get; set; }

        [JsonProperty("trackName")]
        public string name { get; set; }

        [JsonProperty("artistName")]
        public string artist_name { get; set; }

        [JsonProperty("primaryGenreName")]
        public string genre { get; set; }

        [JsonProperty("artworkUrl100")]
        public string image { get; set; }

        [JsonProperty("previewUrl")]
        public string audio { get; set; }
    }
}