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
    public class RegisterViewModel : ObservableObject
    {
        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private string _confirmPassword;
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _phoneNumber;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; set; }
        public ICommand CancelCommand { get; set; } 

        public Action CloseAction { get; set; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
            CancelCommand = new RelayCommand((o) => CloseAction?.Invoke());
        }

        private void Register(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Vui lòng điền đầy đủ tất cả thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Email.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new AppDbContext())
            {
                if (db.Users.Any(u => u.Username == Username))
                {
                    MessageBox.Show("Tên tài khoản này đã có người sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                db.Users.Add(new User
                {
                    Username = Username,
                    Password = Password,
                    Email = Email,
                    PhoneNumber = PhoneNumber
                });
                db.SaveChanges();

                MessageBox.Show("Đăng ký thành công! Bạn có thể dùng tài khoản này để đăng nhập ngay bây giờ.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke();
            }
        }
    }
}
