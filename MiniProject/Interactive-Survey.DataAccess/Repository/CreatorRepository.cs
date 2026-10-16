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
    public class CreatorRepository : Repository<Creator>, ICreatorRepository
    {
        private readonly ApplicationDbContext _db;
        public CreatorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Creator creator)
        {
            var objFromDb = await _db.Creators.FirstOrDefaultAsync(
                c => c.CreatorId == creator.CreatorId);

            if (objFromDb != null)
            {
                objFromDb.FullName = creator.FullName;
                objFromDb.PhoneNumber = creator.PhoneNumber;
                objFromDb.Address = creator.Address;
                objFromDb.Gender = creator.Gender;
                objFromDb.DateOfBirth = creator.DateOfBirth;
                objFromDb.ImageUrl = creator.ImageUrl;
            }
        }
    }
}
