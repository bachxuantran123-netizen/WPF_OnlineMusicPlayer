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
        private readonly DispatcherTimer _timer;
        private bool _isDraggingSlider = false;
        private bool _isPlaying = false;

        public MainWindow(int userId)
        {
            InitializeComponent();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += Timer_Tick;

            if (DataContext is MainViewModel vm)
            {
                vm.CurrentUserId = userId;
            }
        }

        private void Playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedTrack != null)
            {
                try
                {
                    vm.CurrentTrack = vm.SelectedTrack;

                    if (string.IsNullOrWhiteSpace(vm.CurrentTrack.audio))
                    {
                        MessageBox.Show("Bài hát này hiện không có bản nghe thử.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    AudioPlayer.Stop();

                    // Bypass HTTPS issue for MediaElement streaming
                    string streamingUrl = vm.CurrentTrack.audio.Replace("https://", "http://");

                    AudioPlayer.Source = new Uri(streamingUrl, UriKind.Absolute);
                    AudioPlayer.Play();

                    _isPlaying = true;
                    btnPlayPause.Content = "⏸";

                    vm.SaveListeningHistory(vm.CurrentTrack);
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi phát nhạc. Vui lòng kiểm tra lại kết nối mạng hoặc thiết bị âm thanh.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                    btnPlayPause.Content = "▶";
                    _isPlaying = false;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (AudioPlayer.NaturalDuration.HasTimeSpan && !_isDraggingSlider)
            {
                slTime.Value = AudioPlayer.Position.TotalSeconds;
                lblCurrentTime.Text = AudioPlayer.Position.ToString(@"mm\:ss");
            }
        }

        private void slTime_DragStart(object sender, MouseButtonEventArgs e) => _isDraggingSlider = true;

        private void slTime_DragEnd(object sender, MouseButtonEventArgs e)
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

            if (_isPlaying)
            {
                AudioPlayer.Pause();
                btnPlayPause.Content = "▶";
                _isPlaying = false;
            }
            else if (AudioPlayer.Source != null)
            {
                AudioPlayer.Play();
                btnPlayPause.Content = "⏸";
                _isPlaying = true;
            }
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
            if (AudioPlayer.Source != null && AudioPlayer.NaturalDuration.HasTimeSpan)
                AudioPlayer.Position += TimeSpan.FromSeconds(10);
        }

        private void BtnRewind_Click(object sender, RoutedEventArgs e)
        {
            if (AudioPlayer.Source != null)
                AudioPlayer.Position -= TimeSpan.FromSeconds(10);
        }
    }
}