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
    public class OptionRepository : Repository<Option>, IOptionRepository
    {
        private readonly ApplicationDbContext _db;
        public OptionRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Option>> GetFilteredOptionsAsync(
            int? questionId)
        {
            var query = _db.Options
                .Include(o => o.Answers)
                .AsQueryable();

            if (questionId.HasValue)
            {
                query = query.Where(o => o.QuestionId == questionId.Value);
            }

            var options = await query.ToListAsync();

            return options;
        }

        public async Task UpdateAsync(Option option)
        {
            var objFromDb = await _db.Options.FirstOrDefaultAsync(
                p => p.OptionId == option.OptionId);

            if (objFromDb != null)
            {
                objFromDb.OptionText = option.OptionText;
                //objFromDb.OrderIndex = option.OrderIndex;
            }
        }
    }
}
