using Interactive_Survey.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Data.Configurations
{
    public class CreatorConfiguration : IEntityTypeConfiguration<Creator>
    {
        public void Configure(EntityTypeBuilder<Creator> builder)
        {
            builder.HasOne(c => c.AppUser)
                    .WithOne(u => u.Creator)
                    .HasForeignKey<Creator>(c => c.AppUserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
