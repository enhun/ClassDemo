using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Model_CoreFirst_Home.Models
{
    public class GuestBookContext : DbContext
    {
        public GuestBookContext(DbContextOptions<GuestBookContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<ReBook> ReBook { get; set; }


        public virtual DbSet<Login> Login { get; set; }
    }
}
