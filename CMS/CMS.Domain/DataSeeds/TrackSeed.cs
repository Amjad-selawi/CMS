using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.DataSeeds
{
    public class TrackSeed : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasData(
                new Track { Id = 1, Name = ".NET" },
                new Track { Id = 2, Name = "QC" },
                new Track { Id = 3, Name = "BA" },
                new Track { Id = 4, Name = "PM" },
                new Track { Id = 5, Name = "IT" },
                new Track { Id = 6, Name = "Frontend" }
            ) ;
        }
    }
}
