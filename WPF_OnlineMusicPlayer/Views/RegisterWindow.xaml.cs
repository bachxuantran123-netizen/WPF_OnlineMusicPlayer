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
using System.Windows.Shapes;
using WPF_OnlineMusicPlayer.ViewModels;

namespace WPF_OnlineMusicPlayer.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            RegisterViewModel vm = new RegisterViewModel();
            vm.CloseAction = new Action(this.Close);
            this.DataContext = vm;
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegisterViewModel vm) vm.Password = txtPassword.Password;
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegisterViewModel vm) vm.ConfirmPassword = txtConfirmPassword.Password;
        }
    }
}
