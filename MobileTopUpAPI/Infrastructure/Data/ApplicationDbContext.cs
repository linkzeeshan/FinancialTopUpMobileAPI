using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MobileTopUpAPI.Domain.Entities;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<Balance> Balances { get; set; }

        public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

        public virtual DbSet<TopUpTransaction> TopUpTransactions { get; set; }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<Balance>(entity =>
            {
                entity.HasOne(d => d.User).WithMany(p => p.Balances)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Balance_User");
            });
           

            builder.Entity<Beneficiary>(entity =>
            {
                entity.HasOne(d => d.User).WithMany(p => p.Beneficiaries)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Beneficiary_User");
            });

            builder.Entity<TopUpTransaction>(entity =>
            {
                entity.HasOne(d => d.Beneficiary).WithMany(p => p.TopUpTransactions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TopUpTransaction_Beneficiary");

                entity.HasOne(d => d.User).WithMany(p => p.TopUpTransactions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TopUpTransaction_User");
            });

            this.SeedDataInMemory(builder);

        }
        private void SeedDataInMemory(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Balance>().HasData(new Balance { Id = 1, UserId = 1, Amount = 500 });
            modelBuilder.Entity<Balance>().HasData(new Balance { Id = 2, UserId = 2, Amount = 1000 });

            

            modelBuilder.Entity<Beneficiary>().HasData(new Beneficiary { Id = 1, UserId = 1, Nickname = "Beneficiary 1", IsActive = true });
            modelBuilder.Entity<Beneficiary>().HasData(new Beneficiary { Id = 2, UserId = 1, Nickname = "Beneficiary 2", IsActive = true });

            modelBuilder.Entity<TopUpTransaction>().HasData(new TopUpTransaction { Id = 1, UserId = 1, BeneficiaryId = 1, Amount = 50, Charge = 1, TotalAmount = 51, TransactionDate = DateTime.Now });
            modelBuilder.Entity<TopUpTransaction>().HasData(new TopUpTransaction { Id = 2, UserId = 1, BeneficiaryId = 2, Amount = 100, Charge = 1, TotalAmount = 101, TransactionDate = DateTime.Now });

            modelBuilder.Entity<User>().HasData(new User { Id = 1, UserName = "Zeeshan Ayyub", PhoneNumber = "00971521339907", IsVerified = true });
            modelBuilder.Entity<User>().HasData(new User { Id = 2, UserName = "Ayyub", PhoneNumber = "009715031685", IsVerified = false });

        }
        
        private void SeedData(ApplicationDbContext dbContext)
        {
            // Check if any records exist in the Users table
            if (!dbContext.Users.Any())
            {
                // Add default user records
                dbContext.Users.AddRange(
                    new User { Id = 1, IsVerified = true }, // Example record
                    new User { Id = 2, IsVerified = false } // Example record
                );
            }

            // Check if any records exist in the Beneficiaries table
            if (!dbContext.Beneficiaries.Any())
            {
                // Add default beneficiary records
                dbContext.Beneficiaries.AddRange(
                    new Beneficiary { Id = 1, UserId = 1, Nickname = "Beneficiary 1", IsActive = true }, // Example record
                    new Beneficiary { Id = 2, UserId = 1, Nickname = "Beneficiary 2", IsActive = true } // Example record
                );
            }

            // Check if any records exist in the TopUpTransactions table
            if (!dbContext.TopUpTransactions.Any())
            {
                // Add default top-up transaction records
                dbContext.TopUpTransactions.AddRange(
                    new TopUpTransaction { Id = 1, UserId = 1, BeneficiaryId = 1, Amount = 50, Charge = 1, TotalAmount = 51, TransactionDate = DateTime.Now }, // Example record
                    new TopUpTransaction { Id = 2, UserId = 1, BeneficiaryId = 2, Amount = 100, Charge = 1, TotalAmount = 101, TransactionDate = DateTime.Now } // Example record
                );
            }
            // Check if any records exist in the Balance table
            if (!dbContext.Balances.Any())
            {
                // Add default balance records
                dbContext.Balances.AddRange(
                    new Balance { UserId = 1, Amount = 500 }, // Example record
                    new Balance { UserId = 2, Amount = 1000 } // Example record
                );
            }
            // Save changes to the database
            dbContext.SaveChanges();
        }
    }
}
