using Interactive_Survey.DataAccess.Data;
using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private readonly ApplicationDbContext _db;
        public QuestionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Question>> GetFilteredQuestionsAsync(
            int? surveyId,
            string searchString)
        {
            var query = _db.Questions
                .Include(q => q.Options)
                .AsQueryable();

            if (surveyId.HasValue)
            {
                query = query.Where(p => p.SurveyId == surveyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(
                    p => p.QuestionText.Contains(searchString) ||
                    p.QuestionText.Contains(searchString));
            }

            var questions = await query.ToListAsync();

            return questions;
        }

        public async Task UpdateAsync(Question question)
        {
            var objFromDb = await _db.Questions.FirstOrDefaultAsync(
                p => p.QuestionId == question.QuestionId);

            if (objFromDb != null)
            {
                objFromDb.QuestionText = question.QuestionText;
                //objFromDb.OrderIndex = question.OrderIndex;
            }
        }
    }
}
