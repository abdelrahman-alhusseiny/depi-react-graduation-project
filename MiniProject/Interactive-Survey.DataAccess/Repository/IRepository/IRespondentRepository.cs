using Interactive_Survey.Models.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{

    public interface IRespondentRepository : IRepository<Respondent>
    {
        Task UpdateAsync(Respondent respondent);
    }
}
