using System;
using Interview_Suraj.Domain.Responses;
using Microsoft.EntityFrameworkCore;

namespace Interview_Suraj.Database
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext(DbContextOptions<CustomDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccountTransaction> AccountTransactions { get; set; }
    }
}
