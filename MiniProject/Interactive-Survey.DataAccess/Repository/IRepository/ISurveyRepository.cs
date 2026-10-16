using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{
    public interface ISurveyRepository : IRepository<Survey>
    {
        Task<(IEnumerable<Survey>, int)> GetFilteredSurveysAsync(
            int? creatorId,
            string searchString,
            string currentSort,
            int currentPage,
            int pageSize);

        Task<bool> IsTitleExistsAsync(string title, int? excludeId = null);

        Task UpdateAsync(Survey survey);
    }
}
