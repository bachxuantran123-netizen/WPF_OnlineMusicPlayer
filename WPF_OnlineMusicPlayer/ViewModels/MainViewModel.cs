using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_OnlineMusicPlayer.Core;
using WPF_OnlineMusicPlayer.Data;
using WPF_OnlineMusicPlayer.Models;
using WPF_OnlineMusicPlayer.Services;

namespace WPF_OnlineMusicPlayer.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private readonly MusicService _apiService;

		public ObservableCollection<MusicTrack> Playlist { get; set; }
		public int CurrentUserId { get; set; }

		private MusicTrack _selectedTrack;
		public MusicTrack SelectedTrack
		{
			get => _selectedTrack;
			set { _selectedTrack = value; OnPropertyChanged(); }
		}
        private MusicTrack _currentTrack;
        public MusicTrack CurrentTrack
        {
            get => _currentTrack;
            set { _currentTrack = value; OnPropertyChanged(); }
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        // Lệnh bấm nút tải nhạc
        public ICommand LoadMusicCommand { get; set; }

		public MainViewModel()
		{
			_apiService = new MusicService();
			Playlist = new ObservableCollection<MusicTrack>();

            // Định nghĩa hành động khi bấm nút Tải Nhạc
            LoadMusicCommand = new RelayCommand(async (o) =>
            {
                System.Windows.MessageBox.Show("1. NÚT ĐÃ HOẠT ĐỘNG! Bắt đầu chuẩn bị lên mạng...");
                try
                {
                    var tracks = await _apiService.GetTrendingTracksAsync();
                    System.Windows.MessageBox.Show($"2. THÀNH CÔNG! Mạng đã trả về {tracks?.Count} bài hát!");
                    if (tracks == null || tracks.Count == 0)
                    {
                        System.Windows.MessageBox.Show("3. Lỗi: Mạng có kết nối nhưng danh sách nhạc trống không!");
                        return;
                    }
                    Playlist.Clear(); // Xóa list cũ
                    foreach (var track in tracks)
                    {
                        Playlist.Add(track); // Đổ từng bài hát mới vào
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show($"Lỗi không tải được nhạc:\n{ex.Message}");
                }
            });
        }
        public void SaveListeningHistory(MusicTrack track)
        {
            if (track == null) return;

            using (var db = new AppDbContext())
            {
                var history = new ListeningHistory
                {
                    UserId = CurrentUserId,
                    TrackId = track.id,
                    TrackName = track.name,
                    ArtistName = track.artist_name,
                    ListenedAt = System.DateTime.Now
                };

				db.ListeningHistories.Add(history);
				db.SaveChanges();
			}
		}
	}
}
