using Interactive_Survey.DataAccess.Data;
using Interactive_Survey.DataAccess.Repository.IRepository;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICreatorRepository Creator { get; private set; }
        public IRespondentRepository Respondent { get; private set; }
        public ISurveyRepository Survey { get; private set; }
        public IQuestionRepository Question { get; private set; }
        public IOptionRepository Option { get; private set; }
        public IAnswerRepository Answer { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Creator = new CreatorRepository(_db);
            Respondent = new RespondentRepository(_db);
            Survey = new SurveyRepository(_db);
            Question = new QuestionRepository(_db);
            Option = new OptionRepository(_db);
            Answer = new AnswerRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
