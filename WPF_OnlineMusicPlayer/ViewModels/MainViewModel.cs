using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

		public ICommand LoadMusicCommand { get; set; }

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
					foreach (var track in tracks)
					{
						Playlist.Add(track);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Không thể tải dữ liệu: {ex.Message}", "Lỗi kết nối", MessageBoxButton.OK, MessageBoxImage.Error);
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
					ListenedAt = DateTime.Now
				};

				db.ListeningHistories.Add(history);
				db.SaveChanges();
			}
		}
	}
}
