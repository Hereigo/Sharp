﻿using Calendarium.Models;

namespace Calendarium.Data
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

                    if (!context.CalEvents.Any())
                    {
                        foreach (CalEvent evt in new CalEvent[] { new(DateTime.Now, "Initial Demo.") { } })
                        {
                            context.CalEvents.Add(evt);
                        }
                        context.SaveChanges();
                    }

                    if (!context.Notes.Any())
                    {
                        context.Notes.Add(new Note() { SortNum = 0, Text = "Demo Note." });
                        context.SaveChanges();
                    }

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
