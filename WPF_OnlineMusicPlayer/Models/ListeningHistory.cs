using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_OnlineMusicPlayer.Models
{
    public class ListeningHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TrackId { get; set; }
        public string TrackName { get; set; }
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public DateTime ListenedAt { get; set; }
    }
}
