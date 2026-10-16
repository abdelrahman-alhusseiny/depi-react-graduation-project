using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Interactive_Survey.Models.ViewModels;
using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Survey.WebUI.Areas.Respondent.Controllers
{
    [Authorize(Roles = SD.Role_Respondent)]
    [Area("Respondent")]
    [Route("Respondent/Survey")]
    public class SurveyController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SurveyController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("TakeSurvey/{surveyId:int}")]
        public async Task<IActionResult> TakeSurvey([FromRoute] int surveyId)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var questions = await _unitOfWork.Question.GetFilteredQuestionsAsync(surveyId, "");
            var questionRows = new List<QuestionRowVM>();

            foreach (var q in questions)
            {
                questionRows.Add(new QuestionRowVM
                {
                    Question = q,
                    Options = await _unitOfWork.Option.GetFilteredOptionsAsync(q.QuestionId)
                });
            }

            var vm = new RespondentSurveyVM
            {
                Survey = survey,
                Questions = questionRows
            };

            return View("Areas/Respondent/Views/Survey/TakeSurvey.cshtml", vm);
        }

        [HttpPost("TakeSurvey/{surveyId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeSurveyConfirmation([FromRoute] int surveyId, [FromForm] RespondentSurveyVM vm)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Unauthorized();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(r => r.AppUserId == appUser.Id);
            if (respondent == null)
            {
                return NotFound("Respondent not found.");
            }

            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
            {
                return NotFound("Survey not found.");
            }

            foreach (var entry in vm.Answers)
            {
                var questionId = entry.Key;
                var answerVm = entry.Value;

                var option = await _unitOfWork.Option.GetFirstOrDefaultAsync(o => o.OptionId == answerVm);
                if (option == null)
                {
                    continue; 
                }

                var newAnswer = new Answer
                {
                    RespondentId = respondent.RespondentId,
                    OptionId = option.OptionId
                };

                await _unitOfWork.Answer.AddAsync(newAnswer);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Survey submitted successfully.";
            return RedirectToAction("ThankYou", "Survey", new { area = "Respondent" });
        }


        [HttpGet("ThankYou")]
        public IActionResult ThankYou()
        {
            return View("Areas/Respondent/Views/Survey/ThankYou.cshtml");
        }
    }
}

