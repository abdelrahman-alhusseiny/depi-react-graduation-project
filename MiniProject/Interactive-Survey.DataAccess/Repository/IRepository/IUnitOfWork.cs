using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Survey.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICreatorRepository Creator { get; }
        IRespondentRepository Respondent { get; }
        ISurveyRepository Survey { get; }
        IQuestionRepository Question { get; }
        IOptionRepository Option { get; }
        IAnswerRepository Answer { get; }

        Task SaveAsync();
    }
}
