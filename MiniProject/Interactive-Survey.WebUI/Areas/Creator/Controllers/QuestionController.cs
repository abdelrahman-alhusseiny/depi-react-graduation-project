using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Interactive_Survey.Models.ViewModels;
using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Interactive_Survey.WebUI.Areas.Creator.Controllers
{
    [Authorize(Roles = SD.Role_Creator)]
    [Area("Creator")]
    [Route("Creator/Question")]
    public class QuestionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuestionController(
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

        [HttpGet("GetAllQuestions/{surveyId:int}")]
        public async Task<IActionResult> GetAllQuestions(
            [FromRoute] int surveyId,
            [FromQuery] string searchString = "")
        {
            var questions =
                await _unitOfWork.Question.GetFilteredQuestionsAsync(
                    surveyId, searchString);

            var questionRows = new List<QuestionRowVM>();
            foreach (var q in questions)
            {
                questionRows.Add(new QuestionRowVM
                {
                    Question = q,
                    Options = await _unitOfWork.Option.GetFilteredOptionsAsync(q.QuestionId)
                });
            }

            var vm = new QuestionListVM
            {
                Questions = questionRows,
                SurveyId = surveyId,
                SearchString = searchString
            };

            return View("Areas/Creator/Views/Question/GetAllQuestions.cshtml", vm);
        }

        [HttpGet("CreateQuestion/{surveyId:int}")]
        public IActionResult CreateQuestion([FromRoute] int surveyId)
        {
            var vm = new QuestionVM
            {
                SurveyId = surveyId,
                Question = new Question(),
                Options = new List<Option>
                {
                    new Option(),
                    new Option(),
                    new Option(),
                    new Option()
                }
            };

            return View("Areas/Creator/Views/Question/CreateQuestion.cshtml", vm);
        }

        [HttpPost("CreateQuestion/{surveyId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion([FromRoute] int surveyId, [FromForm] QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Areas/Creator/Views/Question/CreateQuestion.cshtml", vm);
            }

            vm.Question.SurveyId = surveyId;

            await _unitOfWork.Question.AddAsync(vm.Question);
            await _unitOfWork.SaveAsync();

            foreach (var option in vm.Options)
            {
                option.QuestionId = vm.Question.QuestionId;
                await _unitOfWork.Option.AddAsync(option);
            }
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Question created successfully.";
            return RedirectToAction("GetAllQuestions", "Question", new { area = "Creator", surveyId = surveyId });
        }

        [HttpGet("EditQuestion/{questionId:int}")]
        public async Task<IActionResult> EditQuestion([FromRoute] int questionId)
        {
            var question = await _unitOfWork.Question.GetFirstOrDefaultAsync(q => q.QuestionId == questionId);
            if (question == null)
            {
                return NotFound();
            }

            var options = await _unitOfWork.Option.GetFilteredOptionsAsync(questionId);

            var vm = new QuestionVM
            {
                Question = question,
                Options = options.ToList(),
                SurveyId = question.SurveyId
            };

            return View("Areas/Creator/Views/Question/EditQuestion.cshtml", vm);
        }

        [HttpPost("EditQuestion/{questionId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion([FromRoute] int questionId, [FromForm] QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Areas/Creator/Views/Question/EditQuestion.cshtml", vm);
            }

            var existingQuestion = await _unitOfWork.Question.GetFirstOrDefaultAsync(q => q.QuestionId == vm.Question.QuestionId);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            existingQuestion.QuestionText = vm.Question.QuestionText;
            existingQuestion.SurveyId = vm.SurveyId;

            await _unitOfWork.Question.UpdateAsync(existingQuestion);
            await _unitOfWork.SaveAsync();

            var existingOptions = await _unitOfWork.Option.GetFilteredOptionsAsync(questionId);
            foreach (var opt in existingOptions)
            {
                _unitOfWork.Option.Remove(opt);
            }

            foreach (var option in vm.Options)
            {
                option.QuestionId = existingQuestion.QuestionId;
                await _unitOfWork.Option.AddAsync(option);
            }

            await _unitOfWork.SaveAsync();

            TempData["success"] = "Question updated successfully.";
            return RedirectToAction("GetAllQuestions", "Question", new { area = "Creator", surveyId = vm.SurveyId });
        }

        [HttpGet("DeleteQuestion/{questionId:int}")]
        public async Task<IActionResult> DeleteQuestion([FromRoute] int questionId)
        {
            var question = await _unitOfWork.Question.GetFirstOrDefaultAsync(q => q.QuestionId == questionId);
            if (question == null)
            {
                return NotFound();
            }

            var options = await _unitOfWork.Option.GetFilteredOptionsAsync(questionId);

            var vm = new QuestionVM
            {
                Question = question,
                Options = options.ToList(),
                SurveyId = question.SurveyId
            };

            return View("Areas/Creator/Views/Question/DeleteQuestion.cshtml", vm);
        }

        [HttpPost("DeleteQuestion/{questionId:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed([FromRoute] int questionId)
        {
            var question = await _unitOfWork.Question.GetFirstOrDefaultAsync(q => q.QuestionId == questionId);
            if (question == null)
            {
                return NotFound();
            }

            var options = await _unitOfWork.Option.GetFilteredOptionsAsync(questionId);
            foreach (var opt in options)
            {
                _unitOfWork.Option.Remove(opt);
            }

            _unitOfWork.Question.Remove(question);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Question deleted successfully.";
            return RedirectToAction("GetAllQuestions", "Question", new { area = "Creator", surveyId = question.SurveyId });
        }













    }
}
