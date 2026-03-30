using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_OnlineMusicPlayer.Core;
using WPF_OnlineMusicPlayer.Data;
using WPF_OnlineMusicPlayer.Models;
using WPF_OnlineMusicPlayer.Views;

namespace WPF_OnlineMusicPlayer.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public Action CloseAction { get; set; }
        public Action OpenRegisterAction { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand((o) => {
                OpenRegisterAction?.Invoke();
            });
        }

        private void Login(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tài khoản và Mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
                if (user != null)
                {
                    MainWindow mainWindow = new MainWindow(user.Id);
                    mainWindow.Show();
                    CloseAction?.Invoke();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
