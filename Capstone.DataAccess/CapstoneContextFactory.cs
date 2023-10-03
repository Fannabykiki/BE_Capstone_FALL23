using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Capstone.DataAccess
{
    public class CapstoneContextFactory : IDesignTimeDbContextFactory<CapstoneContext>
    {
        public CapstoneContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CapstoneContext>();
            optionsBuilder.UseSqlServer("Server=devtasker.database.windows.net;uid=devtasker;Password=PhanNam.2001;Database=Capstone;Trusted_Connection=False;Encrypt=True");
            return new CapstoneContext(optionsBuilder.Options);
        }
    }
}
