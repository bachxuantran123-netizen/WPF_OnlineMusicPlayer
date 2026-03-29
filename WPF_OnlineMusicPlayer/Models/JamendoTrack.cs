using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_OnlineMusicPlayer.Models
{
    public class JamendoResponse
    {
        public List<JamendoTrack> results { get; set; }
    }

    public class JamendoTrack
    {
        public string id { get; set; }
        public string name { get; set; }
        public string artist_name { get; set; }
        public string image { get; set; }
        public string audio { get; set; }
    }
}
