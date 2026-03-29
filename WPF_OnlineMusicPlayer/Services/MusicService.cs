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
        private readonly string[] _genres = { "pop", "rock", "lofi", "chill", "acoustic", "edm", "kpop" };

        public async Task<List<MusicTrack>> GetTrendingTracksAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                Random rnd = new Random();
                string randomGenre = _genres[rnd.Next(_genres.Length)];

                string url = $"https://itunes.apple.com/search?term={randomGenre}&limit=100&media=music";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    MusicResponse data = JsonConvert.DeserializeObject<MusicResponse>(jsonResponse);

                    if (data?.results != null)
                    {
                        foreach (var track in data.results.Where(t => !string.IsNullOrEmpty(t.image)))
                        {
                            track.image = track.image.Replace("100x100bb", "400x400bb");
                        }
                    }

                    return data?.results ?? new List<MusicTrack>();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi máy chủ: {ex.Message}");
                }
            }
        }
        public async Task<List<MusicTrack>> SearchTracksAsync(string keyword)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                // Uri.EscapeDataString giúp chuyển dấu cách thành %20 để link web không bị lỗi
                string safeKeyword = Uri.EscapeDataString(keyword);
                string url = $"https://itunes.apple.com/search?term={safeKeyword}&limit=50&media=music";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    MusicResponse data = JsonConvert.DeserializeObject<MusicResponse>(jsonResponse);

                    if (data?.results != null)
                    {
                        foreach (var track in data.results.Where(t => !string.IsNullOrEmpty(t.image)))
                        {
                            track.image = track.image.Replace("100x100bb", "400x400bb");
                        }
                    }
                    return data?.results ?? new List<MusicTrack>();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi tìm kiếm: {ex.Message}");
                }
            }
        }
    }
}
