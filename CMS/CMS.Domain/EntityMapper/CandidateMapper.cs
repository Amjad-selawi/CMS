using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.EntityMapper
{
    internal class CandidateMapper : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder
                .HasOne(p => p.CV)
                .WithMany()
                .HasForeignKey(p => p.CVAttachmentId);
            builder
                .HasOne(p=>p.Position)
                .WithMany(p=>p.Candidates)
                .HasForeignKey(p=>p.PositionId);
            builder
                .HasMany(p => p.Interviews)
                .WithOne(p => p.Candidate);

            builder.HasOne(p=>p.Company)
                .WithMany(p=>p.Candidates)
                .HasForeignKey(p=>p.CompanyId);
        }
    }
}
