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
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasOne(a => a.Option)
                   .WithMany(o => o.Answers)
                   .HasForeignKey(a => a.OptionId)
                   .OnDelete(DeleteBehavior.Cascade); // keep cascade here

            builder.HasOne(a => a.Respondent)
                   .WithMany(r => r.Answers)
                   .HasForeignKey(a => a.RespondentId)
                   .OnDelete(DeleteBehavior.Restrict); // prevent multiple cascade paths
        }
    }
}
