using HurdaApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HurdaApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<HurdaItem> HurdaItems { get; set; }
}
