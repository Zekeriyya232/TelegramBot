using Microsoft.EntityFrameworkCore;

namespace TelegramBot.Entities
{
    public class DatabaseContext:DbContext
    {

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }  

        public DbSet<ChatMembersDB> ChatMembers { get; set; }

        public DbSet<UserDB> Users { get; set; }

        public DbSet<TaskDB> Tasks { get; set; }

        public DbSet<MembersDB> Members { get; set; }

        public DbSet<TaskMember> taskMembers { get; set; }

        public DbSet<CategoryDB> categories { get; set; }

        public DbSet<projectStatus> projectStatus { get; set; }

        public DbSet<roleDB> roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //One-to-one relationship


            modelBuilder.Entity<MembersDB>()
                .HasOne(e => e.User)
                .WithOne(e => e.Members)
                .HasForeignKey<UserDB>(e => e.memberId)
                .IsRequired(false);

            // Many-to-Many relationship
            modelBuilder.Entity<TaskMember>()
                .HasKey(tm => new { tm.TaskId, tm.MemberId });

            modelBuilder.Entity<TaskMember>()
                .HasOne(tm => tm.Task)
                .WithMany(t => t.TaskMembers)
                .HasForeignKey(tm => tm.TaskId);

            modelBuilder.Entity<TaskMember>()
                .HasOne(tm => tm.Member)
                .WithMany(m => m.TaskMembers)
                .HasForeignKey(tm => tm.MemberId);

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<MembersDB>()
        .Property(e => e.startingJob)
        .HasColumnType("timestamp without time zone");


            modelBuilder.Entity<TaskDB>()
        .Property(e => e.startingTime)
        .HasColumnType("timestamp without time zone");

            modelBuilder.Entity<TaskDB>()
       .Property(e => e.endingTime)
       .HasColumnType("timestamp without time zone");
            
            modelBuilder.Entity<TaskDB>()
       .Property(e => e.creationTime)
       .HasColumnType("timestamp without time zone");

        }

    }
}
