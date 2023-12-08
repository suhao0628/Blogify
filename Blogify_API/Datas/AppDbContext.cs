using Blogify_API.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Blogify_API.Datas
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
