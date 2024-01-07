using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.EntityMapper
{
    public class TrackMapper : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasMany(t => t.Candidates).WithOne(t => t.Track);
            builder.HasMany(t => t.Interviews).WithOne(t => t.Track);


        }
    }
}
