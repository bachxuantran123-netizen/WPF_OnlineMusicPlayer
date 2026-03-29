using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_OnlineMusicPlayer.ViewModels;

namespace WPF_OnlineMusicPlayer.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            LoginViewModel vm = new LoginViewModel();
            vm.CloseAction = new Action(this.Close);
            vm.OpenRegisterAction = new Action(() =>
            {
                RegisterWindow regWindow = new RegisterWindow();
                regWindow.ShowDialog(); // ShowDialog để chặn tương tác form Login lúc đang đăng ký
            });
            this.DataContext = vm;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Ép kiểu DataContext lấy ra Bộ não ViewModel
            if (this.DataContext is LoginViewModel vm)
            {
                // Truyền mật khẩu thực tế vào thẳng ViewModel
                vm.Password = txtPassword.Password;
            }
        }
    }
}
