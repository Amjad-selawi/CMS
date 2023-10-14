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
                .HasOne(candidate => candidate.CV)
                .WithMany()
                .HasForeignKey(candidate => candidate.CVAttachmentId);
            builder
                .HasOne(candidate => candidate.Position)
                .WithMany(position=>position.Candidates)
                .HasForeignKey(candidate => candidate.PositionId);
            //builder
            //    .HasOne(candidate => candidate.Country)
            //    .WithMany()
            //    .HasForeignKey(candidate => candidate.CountryId)
            //    .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasMany(candidate => candidate.Interviews)
                .WithOne(interview => interview.Candidate);

            builder.HasOne(candidate => candidate.Company)
                .WithMany(company=>company.Candidates)
                .HasForeignKey(candidate => candidate.CompanyId);
        }
    }
}
