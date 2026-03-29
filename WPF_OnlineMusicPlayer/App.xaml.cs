using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF_OnlineMusicPlayer.Data;
using Microsoft.EntityFrameworkCore;

namespace WPF_OnlineMusicPlayer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SQLitePCL.Batteries.Init();

            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
            }
        }
    }
}
