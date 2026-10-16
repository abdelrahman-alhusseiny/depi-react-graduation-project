using Interactive_Survey.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{
    public interface IOptionRepository : IRepository<Option>
    {
        Task<IEnumerable<Option>> GetFilteredOptionsAsync(
            int? questionId);
        Task UpdateAsync(Option option);
    }
}
