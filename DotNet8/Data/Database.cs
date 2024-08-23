using DotNet8.Models;

namespace DotNet8.Data
{
    internal static class Database
    {
        internal static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    context.Database.EnsureCreated();

                    if (context.CalEvents.Any())
                    {
                        return; // DB has been seeded already.
                    }

                    foreach (CalEvent evt in new CalEvent[] {
                        new(DateTime.Now, "Initial Demo."){},
                    })
                    {
                        context.CalEvents.Add(evt);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }
    }
}
