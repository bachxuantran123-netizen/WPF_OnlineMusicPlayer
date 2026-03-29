using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_OnlineMusicPlayer.Models;

namespace WPF_OnlineMusicPlayer.Services
{
    public class MusicService
    {
        public async Task<List<MusicTrack>> GetTrendingTracksAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                string[] genres = { "pop", "rock", "lofi", "chill", "acoustic", "edm", "kpop" };

                Random rnd = new Random();
                string randomGenre = genres[rnd.Next(genres.Length)];

                string url = $"https://itunes.apple.com/search?term={randomGenre}&limit=100&media=music";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    MusicResponse data = JsonConvert.DeserializeObject<MusicResponse>(jsonResponse);

                    // Phóng to ảnh bìa từ 100x100 lên 400x400 cho giao diện đẹp long lanh
                    if (data?.results != null)
                    {
                        foreach (var track in data.results)
                        {
                            if (!string.IsNullOrEmpty(track.image))
                            {
                                track.image = track.image.Replace("100x100bb", "400x400bb");
                            }
                        }
                    }

                    return data?.results ?? new List<MusicTrack>();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi kết nối máy chủ Apple: " + ex.Message);
                }
            }
        }
    }
}
