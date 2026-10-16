using Interactive_Survey.DataAccess.Data;
using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository
{
   
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        private readonly ApplicationDbContext _db;
        public AnswerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<int> CountAsync(Expression<Func<Answer, bool>> predicate)
        {
            return await _db.Answers.CountAsync(predicate);
        }

        public async Task UpdateAsync(Answer answer)
        {
            var objFromDb = await _db.Answers.FirstOrDefaultAsync(
                p => p.AnswerId == answer.AnswerId);

            if (objFromDb != null)
            {
            }
        }
    }
}
