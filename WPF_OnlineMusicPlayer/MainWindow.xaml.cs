using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_OnlineMusicPlayer.Models;
using WPF_OnlineMusicPlayer.ViewModels;

namespace WPF_OnlineMusicPlayer
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private bool _isDraggingSlider = false;
        private bool _isPlaying = false;
        public MainWindow(int userId)
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            if (DataContext is MainViewModel vm)
            {
                vm.CurrentUserId = userId;
            }
        }

        // KHI NGƯỜI DÙNG CLICK CHỌN BÀI HÁT TRONG DANH SÁCH
        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lấy dữ liệu từ Bộ não (ViewModel)
            if (DataContext is MainViewModel vm && vm.SelectedTrack != null)
            {
                try
                {
                    // 1. Kiểm tra ngay xem link nhạc có bị rỗng/lỗi từ máy chủ không
                    if (string.IsNullOrWhiteSpace(vm.SelectedTrack.audio))
                    {
                        MessageBox.Show("Rất tiếc! Bài hát này không được Apple cung cấp link nghe thử.");
                        return;
                    }

                    // 2. DỪNG bài cũ lại (Xóa sạch bộ đệm) trước khi nạp bài mới để tránh kẹt hệ thống
                    AudioPlayer.Stop();

                    string streamingUrl = vm.SelectedTrack.audio.Replace("https://", "http://");

                    // 3. Ép chuẩn đường dẫn (UriKind.Absolute) và Bắt đầu phát
                    AudioPlayer.Source = new Uri(streamingUrl, UriKind.Absolute);
                    AudioPlayer.Play();

                    _isPlaying = true;
                    btnPlayPause.Content = "⏸";

                    // 4. GỌI DATABASE LƯU LỊCH SỬ NGHE NHẠC LẠI
                    vm.SaveListeningHistory(vm.SelectedTrack);
                }
                catch (System.NullReferenceException)
                {
                    MessageBox.Show("LỖI HỆ ĐIỀU HÀNH");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi phát nhạc: {ex.Message}");
                }
            }
        }

        // ================= CÁC HÀM XỬ LÝ THANH TRƯỢT VÀ NÚT BẤM CƠ BẢN =================
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (AudioPlayer.NaturalDuration.HasTimeSpan && !_isDraggingSlider)
            {
                slTime.Value = AudioPlayer.Position.TotalSeconds;
                lblCurrentTime.Text = AudioPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void slTime_DragStart(object sender, System.Windows.Input.MouseButtonEventArgs e) { _isDraggingSlider = true; }
        private void slTime_DragEnd(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDraggingSlider = false;
            AudioPlayer.Position = TimeSpan.FromSeconds(slTime.Value);
        }

        private void AudioPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (AudioPlayer.NaturalDuration.HasTimeSpan)
            {
                slTime.Maximum = AudioPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                lblTotalTime.Text = AudioPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                _timer.Start();
            }
        }

        private void AudioPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            slTime.Value = 0;
            if (Playlist.SelectedIndex < Playlist.Items.Count - 1)
            {
                Playlist.SelectedIndex++;
            }
            else
            {
                _isPlaying = false;
                btnPlayPause.Content = "▶";
                AudioPlayer.Stop();
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedItem == null && Playlist.Items.Count > 0)
            {
                Playlist.SelectedIndex = 0;
                return;
            }

            if (_isPlaying) { AudioPlayer.Pause(); btnPlayPause.Content = "▶"; _isPlaying = false; }
            else if (AudioPlayer.Source != null) { AudioPlayer.Play(); btnPlayPause.Content = "⏸"; _isPlaying = true; }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedIndex < Playlist.Items.Count - 1) Playlist.SelectedIndex++;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.SelectedIndex > 0) Playlist.SelectedIndex--;
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            if (AudioPlayer.Source != null && AudioPlayer.NaturalDuration.HasTimeSpan) AudioPlayer.Position += TimeSpan.FromSeconds(10);
        }

        private void BtnRewind_Click(object sender, RoutedEventArgs e)
        {
            if (AudioPlayer.Source != null) AudioPlayer.Position -= TimeSpan.FromSeconds(10);
        }
    }
}
