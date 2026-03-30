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

        public ICommand LoadMusicCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand RecommendCommand { get; set; }

        public MainViewModel()
		{
			_apiService = new MusicService();
			Playlist = new ObservableCollection<MusicTrack>();

            LoadMusicCommand = new RelayCommand(async (o) =>
            {
                try
                {
                    var tracks = await _apiService.GetTrendingTracksAsync();

                    if (tracks == null || !tracks.Any())
                    {
                        MessageBox.Show("Không tìm thấy danh sách nhạc. Vui lòng thử lại sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    Playlist.Clear();
                    foreach (var track in tracks) Playlist.Add(track);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể tải dữ liệu: {ex.Message}", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

            SearchCommand = new RelayCommand(async (o) =>
            {
                if (string.IsNullOrWhiteSpace(SearchText)) return;
                try
                {
                    var tracks = await _apiService.SearchTracksAsync(SearchText);
                    Playlist.Clear();
                    foreach (var track in tracks) Playlist.Add(track);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi"); }
            });

            RecommendCommand = new RelayCommand(async (o) =>
            {
                string favoriteGenre = "pop";

                try
                {
                    using (var db = new AppDbContext())
                    {
                        var topGenre = db.ListeningHistories
                            .Where(h => h.UserId == CurrentUserId && h.Genre != null)
                            .GroupBy(h => h.Genre)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault();

                        if (!string.IsNullOrEmpty(topGenre))
                        {
                            favoriteGenre = topGenre;
                            MessageBox.Show($"Dựa vào lịch sử, có vẻ bạn rất thích nghe thể loại:\n{favoriteGenre}\n\nHệ thống đang tải các bài hát tương tự...", "Phân tích Gu âm nhạc");
                        }
                        else
                        {
                            MessageBox.Show("Bạn chưa nghe bài nào cả, hãy nghe vài bài để hệ thống phân tích nhé!", "Thông báo");
                            return;
                        }
                    }

                    var tracks = await _apiService.SearchTracksAsync(favoriteGenre);
                    Playlist.Clear();
                    foreach (var track in tracks) Playlist.Add(track);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi DB"); }
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
                    Genre = track.genre,
                    ListenedAt = DateTime.Now
                };

                db.ListeningHistories.Add(history);
                db.SaveChanges();
            }
        }
	}
}
