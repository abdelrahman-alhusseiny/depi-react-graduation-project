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
    public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
    {
        public void Configure(EntityTypeBuilder<Survey> builder)
        {
            builder.HasOne(s => s.Creator)
                   .WithMany(c => c.Surveys)
                   .HasForeignKey(s => s.CreatorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
