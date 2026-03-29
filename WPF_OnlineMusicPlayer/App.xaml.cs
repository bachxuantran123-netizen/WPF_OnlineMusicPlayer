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
        // Hàm này sẽ tự động chạy ĐẦU TIÊN khi phần mềm vừa được bật lên
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Đánh thức lõi SQLite (Giữ lại từ bài học trước)
            SQLitePCL.Batteries.Init();

            // 2. SIÊU NĂNG LỰC: TỰ ĐỘNG XÂY DATABASE VÀ TẠO BẢNG NẾU CHƯA CÓ
            using (var db = new AppDbContext())
            {
                // Lệnh này sẽ tự động bê bản thiết kế (Migrations) vào thư mục bin/Debug và xây thành các bảng chuẩn chỉ!
                db.Database.Migrate();
            }
        }
    }
}
