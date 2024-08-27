﻿using DotNet8.DbLogger;
using DotNet8.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CalEvent> CalEvents { get; set; } = null!;

        public DbSet<CalEventCategory> CalEventCategories { get; set; } = null!;

        public DbSet<Error> Errors { get; set; } = null!;
    }
}
