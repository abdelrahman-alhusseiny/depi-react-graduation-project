using Interactive_Survey.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Data.Configurations
{
    public class RespondentConfiguration : IEntityTypeConfiguration<Respondent>
    {
        public void Configure(EntityTypeBuilder<Respondent> builder)
        {
            builder.HasOne(r => r.AppUser)
                    .WithOne(u => u.Respondent)
                    .HasForeignKey<Respondent>(r => r.AppUserId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
