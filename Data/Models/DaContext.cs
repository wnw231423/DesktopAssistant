using Data.Models.Table;
using Data.Models.Todo;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

// "Da" stands for "Desk Assistant".
public class DaContext: DbContext
{
    // 以下DbSet对应数据库里的表
    public DbSet<Course> Courses { get; set; }  // 课程表
    public DbSet<TodoItem> TodoItems { get; set; } // 待办事项表
    
    // 数据库的路径
    public string DbPath { get; }

    public DaContext()
    {
        // 获取当前用户的本地应用程序数据目录
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // 令数据库文件存放在本地应用程序数据目录下, windows环境下为: C:\Users\<username>\AppData\Local
        DbPath = System.IO.Path.Combine(path, "DesktopAssistant.db");
    }
    
    // 让EF生成一个数据库文件在前面指定的数据库目录下
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}