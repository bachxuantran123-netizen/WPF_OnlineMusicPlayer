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

        // Action này dùng để đóng cửa sổ Login sau khi đăng nhập thành công
        public Action CloseAction { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
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
                // Truy vấn CSDL bằng LINQ
                var user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
                if (user != null)
                {
                    // Chuyển sang màn hình chính và truyền ID người dùng vào
                    MainWindow mainWindow = new MainWindow(user.Id);
                    mainWindow.Show();
                    CloseAction?.Invoke(); // Đóng Form Login
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Đăng nhập thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Register(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Vui lòng điền thông tin để đăng ký!", "Lỗi");
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == Username))
                {
                    MessageBox.Show("Tên tài khoản đã tồn tại!", "Lỗi");
                    return;
                }

                db.Users.Add(new User { Username = Username, Password = Password });
                db.SaveChanges();
                MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay.", "Thành công");
            }
        }
    }
}
