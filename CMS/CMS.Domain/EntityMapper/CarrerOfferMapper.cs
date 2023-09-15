using CMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Domain.EntityMapper
{
    internal class CarrerOfferMapper : IEntityTypeConfiguration<CarrerOffer>
    {
        public void Configure(EntityTypeBuilder<CarrerOffer> builder)
        {
            builder
                .HasOne(o => o.Position)
                .WithMany(p=>p.Offers)
                .HasForeignKey(o => o.PositionId);
        }
    }
}
