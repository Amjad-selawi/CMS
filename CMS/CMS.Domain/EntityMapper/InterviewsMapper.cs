using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.EntityMapper
{
    public class InterviewsMapper : IEntityTypeConfiguration<Interviews>
    {
        public void Configure(EntityTypeBuilder<Interviews> builder)
        {

            builder.HasOne(p => p.Candidate)
                .WithMany(p => p.Interviews)
                .HasForeignKey(p => p.CandidateId);

            builder.HasOne(p => p.Position)
              .WithMany(p => p.Interviews)
              .HasForeignKey(p => p.PositionId);

            builder.HasOne(p => p.Status)
                .WithMany(p => p.Interviews)
                .HasForeignKey(p => p.StatusId);

            builder.HasOne(p=>p.Interviewer)
                .WithMany()
                .HasForeignKey(p=>p.InterviewerId);

            builder.HasOne(P => P.Attachment)
                .WithMany()
                .HasForeignKey(P => P.AttachmentId).HasConstraintName("FK_Interviews_Attachments");
             

        }
    }
}
