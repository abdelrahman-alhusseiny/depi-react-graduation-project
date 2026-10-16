using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<int> CountAsync(Expression<Func<Answer, bool>> predicate);

        Task UpdateAsync(Answer answer);
    }
}
