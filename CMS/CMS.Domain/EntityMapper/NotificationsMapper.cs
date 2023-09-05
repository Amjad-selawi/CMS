using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.EntityMapper
{
    public class NotificationsMapper : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder
                .HasOne(p => p.Templates)
                .WithMany(p => p.Notifications)
                .HasForeignKey(p => p.TemplatesId);
        }
    }
}
