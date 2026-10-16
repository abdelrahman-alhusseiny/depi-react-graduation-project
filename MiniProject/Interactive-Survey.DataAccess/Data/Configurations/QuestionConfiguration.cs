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
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasOne(q => q.Survey)
                   .WithMany(s =>s.Questions)
                   .HasForeignKey(q => q.SurveyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
