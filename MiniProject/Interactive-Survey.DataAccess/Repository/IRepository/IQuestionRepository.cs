using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetFilteredQuestionsAsync(
            int? surveyId,
            string searchString);

        Task UpdateAsync(Question question);
    }
}
