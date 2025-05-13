using BookManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
    }
}
