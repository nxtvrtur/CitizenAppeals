using Microsoft.EntityFrameworkCore;

namespace CitizenAppeals.Server.Model
{
    public partial class CitizenAppealsContext : DbContext
    {
        public CitizenAppealsContext(DbContextOptions<CitizenAppealsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appeal> Appeals { get; set; }
        public virtual DbSet<Citizen> Citizens { get; set; }
        public virtual DbSet<Executor> Executors { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appeal>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Appeals__3214EC07C4B983AD");

                entity.Property(e => e.AppealDate).HasColumnType("date");
                entity.Property(e => e.AppealLink).HasMaxLength(200);
                entity.Property(e => e.AppealNumber).HasMaxLength(50);
                entity.Property(e => e.Result).HasMaxLength(20);

                entity.HasOne(d => d.Citizen).WithMany(p => p.Appeals)
                    .HasForeignKey(d => d.CitizenId)
                    .HasConstraintName("FK__Appeals__Citizen__286302EC");

                entity.HasMany(d => d.Executors).WithMany(p => p.Appeals)
                    .UsingEntity<Dictionary<string, object>>(
                        "AppealExecutors",
                        r => r.HasOne<Executor>().WithMany()
                            .HasForeignKey("ExecutorId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__AppealExe__Execu__2D27B809"),
                        l => l.HasOne<Appeal>().WithMany()
                            .HasForeignKey("AppealId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK__AppealExe__Appea__2C3393D0"),
                        j =>
                        {
                            j.HasKey("AppealId", "ExecutorId").HasName("PK__AppealEx__B206D7DB0C5BFB46");
                        });
            });

            modelBuilder.Entity<Citizen>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Citizens__3214EC07C8EB05ED");

                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.MiddleName).HasMaxLength(100);
            });

            modelBuilder.Entity<Executor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Executor__3214EC07D4A03FE0");

                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.MiddleName).HasMaxLength(100);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07CE4F9FF3");

                entity.HasIndex(e => e.Name, "UQ__Roles__737584F6CCD8CB72").IsUnique();

                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07BE46DFB8");

                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4971E31AB").IsUnique();

                entity.Property(e => e.PasswordHash).HasMaxLength(256);
                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__RoleId__33D4B598");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}