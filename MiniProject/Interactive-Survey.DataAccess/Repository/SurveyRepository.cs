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
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        private readonly ApplicationDbContext _db;
        public SurveyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Survey>, int)> GetFilteredSurveysAsync(
            int? creatorId,
            string searchString,
            string currentSort,
            int currentPage,
            int pageSize)
        {
            var query = _db.Surveys
                .Include(s => s.Questions)
                .AsQueryable();

            if (creatorId.HasValue)
            {
                query = query.Where(s => s.CreatorId == creatorId.Value);
            }

                if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(
                    s => s.Title.Contains(searchString) ||
                    s.Description.Contains(searchString));
            }

            query = currentSort switch
            {
                "title" => query.OrderBy(s => s.Title),
                "title_desc" => query.OrderByDescending(s => s.Title),
                "created" => query.OrderBy(s => s.CreatedAt),
                "created_desc" => query.OrderByDescending(s => s.CreatedAt),
                _ => query.OrderBy(s => s.Title)
            };

            int totalItems = await query.CountAsync();

            var surveys = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (surveys, totalItems);
        }

        public async Task<bool> IsTitleExistsAsync(
            string title, int? excludeId = null)
        {
            return await _db.Surveys.AnyAsync(
                s => s.Title == title &&
                (!excludeId.HasValue || s.SurveyId != excludeId.Value));
        }

        public async Task UpdateAsync(Survey survey)
        {
            var objFromDb = await _db.Surveys.FirstOrDefaultAsync(
                s => s.SurveyId == survey.SurveyId);

            if (objFromDb != null)
            {
                objFromDb.Title = survey.Title;
                objFromDb.Description = survey.Description;
            }
        }
    }
}
