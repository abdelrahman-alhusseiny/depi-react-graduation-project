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
    [Route("Creator/Survey")]
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

        [HttpGet("GetAllSurveys")]
        public async Task<IActionResult> GetAllSurveys(
            [FromQuery] string searchString = "",
            [FromQuery] string currentSort = "",
            [FromQuery] int currentPage = 1)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (creator == null)
            {
                return NotFound();
            }

            int pageSize = 5;
            var (surveys, totalItems) =
                await _unitOfWork.Survey.GetFilteredSurveysAsync(
                    creator.CreatorId, searchString, currentSort, currentPage, pageSize);

            var surveyRows = new List<SurveyRowVM>();
            foreach (var s in surveys)
            {
                surveyRows.Add(new SurveyRowVM
                {
                    Survey = s,
                });
            }

            var vm = new SurveyListVM
            {
                Surveys = surveyRows,
                SourceView = "GetAllSurveys",

                SearchString = searchString,

                CurrentSort = currentSort,
                TitleSort = currentSort == "title" ? "title_desc" : "title",
                CreatedAtSort = currentSort == "created" ? "created_desc" : "created",

                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };

            return View("Areas/Creator/Views/Survey/GetAllSurveys.cshtml", vm);
        }

        [HttpGet("CreateSurvey")]
        public IActionResult CreateSurvey([FromQuery] SurveyVM vm)
        {
            return View("Areas/Creator/Views/Survey/CreateSurvey.cshtml", vm);
        }

        [HttpPost("CreateSurvey")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSurveyConfirmed([FromForm] SurveyVM vm)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (creator == null)
            {
                return NotFound();
            }

            if (await _unitOfWork.Survey.IsTitleExistsAsync(vm.Survey.Title))
            {
                ModelState.AddModelError("Survey.Title", "This survey title already exists");
            }

            if (!ModelState.IsValid)
            {
                return View("Areas/Creator/Views/Survey/CreateSurvey.cshtml", vm);
            }

            vm.Survey.CreatorId = creator.CreatorId;

            await _unitOfWork.Survey.AddAsync(vm.Survey);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Survey created successfully.";
            return RedirectToAction("GetAllSurveys", "Survey", new { area = "Creator" });
        }

        [HttpGet("EditSurvey/{id:int}")]
        public async Task<IActionResult> EditSurvey(
            [FromRoute] int id, [FromQuery] SurveyVM vm)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(
                s => s.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }

            vm.Survey = survey;
            return View("Areas/Creator/Views/Survey/EditSurvey.cshtml", vm);
        }

        [HttpPost("EditSurvey/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSurveyConfirmed([FromForm] SurveyVM vm)
        {
            if (await _unitOfWork.Survey.IsTitleExistsAsync(
                vm.Survey.Title, vm.Survey.SurveyId))
            {
                ModelState.AddModelError("Survey.Title", "This survey title already exists");
            }

            if (!ModelState.IsValid)
            {
                return View("Areas/Creator/Views/Survey/EditSurvey.cshtml", vm);
            }

            await _unitOfWork.Survey.UpdateAsync(vm.Survey);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Survey updated successfully.";
            return RedirectToAction(vm.SourceView, "Survey", new
            {
                area = "Creator",
                id = vm.Survey.SurveyId,
                searchString = vm.SearchString,
                currentSort = vm.CurrentSort,
                currentPage = vm.CurrentPage
            });
        }

        [HttpGet("DeleteSurvey/{id:int}")]
        public async Task<IActionResult> DeleteSurvey([FromRoute] int id, [FromQuery] SurveyVM vm)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(
                s => s.SurveyId == id);
            if (survey == null)
            {
                return NotFound();
            }

            vm.Survey = survey;
            return View("Areas/Creator/Views/Survey/DeleteSurvey.cshtml", vm);
        }

        [HttpPost("DeleteSurvey/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurveyConfirmed([FromForm] SurveyVM vm)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(
                s => s.SurveyId == vm.Survey.SurveyId);
            if (survey == null)
            {
                return NotFound();
            }

            _unitOfWork.Survey.Remove(survey);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Survey has been deleted successfully.";
            return RedirectToAction("GetAllSurveys", "Survey", new
            {
                area = "Creator",
                searchString = vm.SearchString,
                currentSort = vm.CurrentSort,
                currentPage = vm.CurrentPage
            });
        }

        [HttpGet("GenerateLink/{surveyId:int}")]
        public async Task<IActionResult> GenerateLink(int surveyId)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var respondentLink = Url.Action(
                "TakeSurvey",
                "Survey",
                new { area = "Respondent", surveyId = surveyId },
                protocol: Request.Scheme
            );

            ViewBag.RespondentLink = respondentLink;
            return View("Areas/Creator/Views/Survey/GenerateLink.cshtml");
        }

        [HttpGet("SurveyResults/{surveyId:int}")]
        public async Task<IActionResult> SurveyResults(int surveyId)
        {
            var survey = await _unitOfWork.Survey.GetFirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var questions = await _unitOfWork.Question.GetFilteredQuestionsAsync(surveyId, "");
            var questionResults = new List<QuestionResultVM>();

            foreach (var q in questions)
            {
                var options = await _unitOfWork.Option.GetFilteredOptionsAsync(q.QuestionId);

                var optionResults = new List<OptionResultVM>();
                foreach (var opt in options)
                {
                    var answersCount = await _unitOfWork.Answer.CountAsync(a => a.OptionId == opt.OptionId);

                    optionResults.Add(new OptionResultVM
                    {
                        Option = opt,
                        AnswersCount = answersCount
                    });
                }

                questionResults.Add(new QuestionResultVM
                {
                    Question = q,
                    OptionsResults = optionResults
                });
            }

            var vm = new SurveyResultsVM
            {
                Survey = survey,
                QuestionsResults = questionResults
            };

            return View("Areas/Creator/Views/Survey/SurveyResults.cshtml", vm);
        }
    }
}

