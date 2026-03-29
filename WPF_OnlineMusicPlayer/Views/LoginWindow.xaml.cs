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

            this.DataContext = vm;
        }
    }
}
