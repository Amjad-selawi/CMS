using CMS.Domain.Entities;
using CMS.Domain.EntityMapper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain
{   
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CarrerOffer> CarrerOffers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Interviews> Interviews { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Templates> Templates { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CompanyMapper());
            builder.ApplyConfiguration(new CountryMapper());
            builder.ApplyConfiguration(new CandidateMapper());
            builder.ApplyConfiguration(new NotificationsMapper());
            builder.ApplyConfiguration(new InterviewsMapper());
            base.OnModelCreating(builder);
        }
    }
}
