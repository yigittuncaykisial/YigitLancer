using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Name = "admin", Surname = "admin", IsAdmin = true,Age=25, UserId= 90, UserName="admin", UserPassword="123456"}
                );



            // Job - User & Category ilişkileri
            modelBuilder.Entity<Jobs>()
    .HasOne(j => j.User)
    .WithMany(u => u.Jobs)
    .HasForeignKey(j => j.UserId)
    .OnDelete(DeleteBehavior.Restrict); ;

            modelBuilder.Entity<Jobs>()
    .HasOne(j => j.PurchasedByUser)
    .WithMany() // User tarafında koleksiyon tutmuyorsan boş bırak
    .HasForeignKey(j => j.PurchasedByUserId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Jobs>()
                .HasOne(j => j.Category)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
    .HasMany(u => u.PurchasedJobs)
    .WithOne(j => j.PurchasedByUser)
    .HasForeignKey(j => j.PurchasedByUserId)
    .OnDelete(DeleteBehavior.NoAction);


            // Review ilişkileri
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Freelancer)
                .WithMany()
                .HasForeignKey(r => r.FreelancerId)
                .OnDelete(DeleteBehavior.NoAction); // mutlaka NoAction

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
       .HasOne(r => r.Job)
       .WithMany()
       .HasForeignKey(r => r.JobId)
       .OnDelete(DeleteBehavior.Cascade);
            ;
            // Conversation
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Job)
                      .WithMany()
                      .HasForeignKey(c => c.JobId)           // JobId nullable
                      .OnDelete(DeleteBehavior.Cascade);      // Job silinirse bağlı sohbetler silinebilir

                entity.HasOne(c => c.Buyer)
                      .WithMany()
                      .HasForeignKey(c => c.BuyerUserId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(c => c.Freelancer)
                      .WithMany()
                      .HasForeignKey(c => c.FreelancerUserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Message
            modelBuilder.Entity<Message>(e =>
            {
                e.HasKey(m => m.Id);

                e.HasOne(m => m.Conversation)
                 .WithMany(c => c.Messages)
                 .HasForeignKey(m => m.ConversationId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(m => m.Sender)
                 .WithMany()
                 .HasForeignKey(m => m.SenderUserId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.Property(m => m.Text)
                 .HasMaxLength(2000)
                 .IsRequired();
            });
            // PurchaseRequests (kullanıyorsan)
            modelBuilder.Entity<PurchaseRequest>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.Job)
                      .WithMany(/* j => j.PurchaseRequests */)
                      .HasForeignKey(p => p.JobId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Buyer)
                      .WithMany(/* u => u.PurchaseRequestsAsBuyer */)
                      .HasForeignKey(p => p.BuyerUserId)
                      .OnDelete(DeleteBehavior.NoAction);


            });
            modelBuilder.Entity<Conversation>()
    .HasIndex(c => new { c.BuyerUserId, c.FreelancerUserId, c.JobId })
    .IsUnique();
            modelBuilder.Entity<Jobs>()
    .HasIndex(j => j.PurchasedByUserId);


            base.OnModelCreating(modelBuilder);
        }
    }
}
