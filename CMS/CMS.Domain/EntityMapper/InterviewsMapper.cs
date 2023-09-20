﻿using CMS.Domain.Entities;
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

            builder.HasOne(interview => interview.Candidate)
                .WithMany(candidate => candidate.Interviews)
                .HasForeignKey(interview => interview.CandidateId);

            builder.HasOne(interview => interview.Position)
              .WithMany(position => position.Interviews)
              .HasForeignKey(interview => interview.PositionId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(interview => interview.Interviewer)
              .WithMany()
              .HasForeignKey(interview => interview.InterviewerId);

            builder.HasOne(interview => interview.Status)
                .WithMany(status => status.Interviews)
                .HasForeignKey(interview => interview.StatusId);

            builder.HasOne(interview => interview.Interviewer)
                .WithMany()
                .HasForeignKey(interview => interview.InterviewerId);

            builder.HasOne(interview => interview.Attachment)
                .WithMany()
                .HasForeignKey(interview => interview.AttachmentId)
                .HasConstraintName("FK_Interviews_Attachments");
             

        }
    }
}
