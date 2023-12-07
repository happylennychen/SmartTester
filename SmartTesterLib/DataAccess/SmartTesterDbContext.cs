using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SmartTesterLib.DataAccess
{
    public class SmartTesterDbContext : DbContext
    {
        public SmartTesterDbContext(DbContextOptions<SmartTesterDbContext> options) : base(options)
        {
        }
        public DbSet<DebugTester> Testers { get; set; }
        public DbSet<DebugChamber> Chambers { get; set; }
        public DbSet<DebugChannel> Channels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DebugTester>()
                .Ignore(t => t.Executor)
                .Ignore(t => t.Channels);
            modelBuilder.Entity<DebugChamber>()
                .Ignore(c => c.Executor)
                .Ignore(c => c.PairedChannels)
                .Ignore(c => c.TempScheduler)
                .Ignore(c => c.TestScheduler);
            modelBuilder.Entity<DebugChannel>()
                .Ignore(c => c.CurrentStep)
                .Ignore(c => c.ContainingChamber)
                .Ignore(c => c.Recipe)
                .Ignore(c => c.Status)
                .Ignore(c => c.Tester);
            modelBuilder.Entity<DebugChannel>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<DebugChannel>()
                .Property<int>("TesterId"); // 影子属性

            modelBuilder.Entity<DebugChannel>()
                .HasOne<DebugTester>() // 使用具体类
                .WithMany()
                .HasForeignKey("TesterId");
        }
    }
}