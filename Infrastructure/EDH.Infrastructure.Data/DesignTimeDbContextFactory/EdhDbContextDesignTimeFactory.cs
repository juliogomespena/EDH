using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using EDH.Infrastructure.Data.ApplicationDbContext;

namespace EDH.Infrastructure.Data.DesignTimeDbContextFactory;

public sealed class EdhDbContextDesignTimeFactory : IDesignTimeDbContextFactory<EdhDbContext>
{
	public EdhDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<EdhDbContext>();
		optionsBuilder.UseSqlite("Data Source=EDH.db");

		return new EdhDbContext(optionsBuilder.Options);
	}
}