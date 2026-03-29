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
    public class JamendoService
    {
        private readonly string _clientId = "0871f9e6";

        public async Task<List<JamendoTrack>> GetTrendingTracksAsync()
        {
            // 1. Ép giao thức bảo mật và BỎ QUA KIỂM TRA CHỨNG CHỈ SSL (Cực kỳ quan trọng)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // 2. Dạy C# giải nén và BỎ QUA PROXY HỆ THỐNG
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseProxy = false // Không đi qua trạm kiểm soát ngầm của Windows
            };

            using (HttpClient client = new HttpClient(handler))
            {
                // 3. Mặt nạ trình duyệt
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.Timeout = TimeSpan.FromSeconds(15);

                string url = $"https://api.jamendo.com/v3.0/tracks/?client_id={_clientId}&format=json&limit=20&include=musicinfo&imagesize=200";

                try
                {
                    // Thêm ConfigureAwait(false) để chống kẹt luồng UI
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Máy chủ Jamendo báo lỗi: {response.StatusCode}");
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JamendoResponse data = JsonConvert.DeserializeObject<JamendoResponse>(jsonResponse);

                    return data?.results ?? new List<JamendoTrack>();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
