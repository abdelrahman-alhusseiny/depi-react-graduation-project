using Interactive_Survey.DataAccess.Data;
using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository
{
    public class RespondentRepository : Repository<Respondent>, IRespondentRepository
    {
        private readonly ApplicationDbContext _db;
        public RespondentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Respondent respondent)
        {
            var objFromDb = await _db.Respondents.FirstOrDefaultAsync(
                r => r.RespondentId == respondent.RespondentId);

            if (objFromDb != null)
            {
                objFromDb.FullName = respondent.FullName;
                objFromDb.PhoneNumber = respondent.PhoneNumber;
                objFromDb.Address = respondent.Address;
                objFromDb.Gender = respondent.Gender;
                objFromDb.DateOfBirth = respondent.DateOfBirth;
                objFromDb.ImageUrl = respondent.ImageUrl;
            }
        }
    }
}
