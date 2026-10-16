using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Interactive_Survey.Models.ViewModels;
using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Interactive_Survey.WebUI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
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

        [HttpGet("ChooseRole")]
        public IActionResult ChooseRole()
        {
            var vm = new ChooseRoleVM
            {
                CreatorImageUrl = "/images/identity/creator.png",
                RespondentImageUrl = "/images/identity/respondent.png"
            };

            return View("Areas/Identity/Views/Account/ChooseRole.cshtml", vm);
        }

        [HttpGet("RegisterCreator")]
        public IActionResult RegisterCreator()
        {
            var vm = new RegisterCreatorVM();
            vm.GenderList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Male", Value = "Male" },
            new SelectListItem { Text = "Female", Value = "Female" },
        };

            return View("Areas/Identity/Views/Account/RegisterCreator.cshtml", vm);
        }

        [HttpPost("RegisterCreator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCreatorConfirmed([FromForm] RegisterCreatorVM vm)
        {
            if (vm.Creator.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                string extension =
                    Path.GetExtension(vm.Creator.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError(
                        "Creator.ImageFile", "Only jpg, jpeg, png images are allowed");

                if (vm.Creator.ImageFile.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError(
                        "Creator.ImageFile", "Image size must be less than 2MB");
            }

            if (!ModelState.IsValid)
            {
                vm.GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                };

                return View("Areas/Identity/Views/Account/RegisterCreator.cshtml", vm);
            }

            var appUser = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email
            };

            var result = await _userManager.CreateAsync(appUser, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                vm.GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                };

                return View("Areas/Identity/Views/Account/RegisterCreator.cshtml", vm);
            }

            await _userManager.AddToRoleAsync(appUser, SD.Role_Creator);

            if (vm.Creator.ImageFile != null && vm.Creator.ImageFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "images", "creators");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(vm.Creator.ImageFile.FileName);
                string fullPath = Path.Combine(uploadPath, fileName + extension);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.Creator.ImageFile.CopyToAsync(fileStream);
                }

                vm.Creator.ImageUrl = "/images/creators/" + fileName + extension;
            }

            vm.Creator.AppUserId = appUser.Id;

            await _unitOfWork.Creator.AddAsync(vm.Creator);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Creator account registered successfully.";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpGet("RegisterRespondent")]
        public IActionResult RegisterRespondent()
        {
            var vm = new RegisterRespondentVM();
            vm.GenderList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Male", Value = "Male" },
            new SelectListItem { Text = "Female", Value = "Female" },
        };

            return View("Areas/Identity/Views/Account/RegisterRespondent.cshtml", vm);
        }

        [HttpPost("RegisterRespondent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterRespondentConfirmed([FromForm] RegisterRespondentVM vm)
        {
            if (vm.Respondent.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                string extension =
                    Path.GetExtension(vm.Respondent.ImageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    ModelState.AddModelError(
                        "Respondent.ImageFile", "Only jpg, jpeg, png images are allowed");

                if (vm.Respondent.ImageFile.Length > 2 * 1024 * 1024)
                    ModelState.AddModelError(
                        "Respondent.ImageFile", "Image size must be less than 2MB");
            }

            if (!ModelState.IsValid)
            {
                vm.GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                };

                return View("Areas/Identity/Views/Account/RegisterRespondent.cshtml", vm);
            }

            var appUser = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email
            };

            var result = await _userManager.CreateAsync(appUser, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                vm.GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                };

                return View("Areas/Identity/Views/Account/RegisterRespondent.cshtml", vm);
            }

            await _userManager.AddToRoleAsync(appUser, SD.Role_Respondent);

            if (vm.Respondent.ImageFile != null && vm.Respondent.ImageFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "images", "respondents");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Guid.NewGuid().ToString("N");
                string extension = Path.GetExtension(vm.Respondent.ImageFile.FileName);
                string fullPath = Path.Combine(uploadPath, fileName + extension);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.Respondent.ImageFile.CopyToAsync(fileStream);
                }

                vm.Respondent.ImageUrl = "/images/respondents/" + fileName + extension;
            }

            vm.Respondent.AppUserId = appUser.Id;

            await _unitOfWork.Respondent.AddAsync(vm.Respondent);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Respondent account registered successfully.";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            var vm = new LoginVM { ReturnUrl = returnUrl };
            return View("Areas/Identity/Views/Account/Login.cshtml", vm);
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginConfirmed([FromForm] LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Areas/Identity/Views/Account/Login.cshtml", vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View("Areas/Identity/Views/Account/Login.cshtml", vm);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                vm.Password,
                vm.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                TempData["success"] = "You have logged in successfully.";
                return await RedirectToLocal(vm.ReturnUrl, user);
            }
            else
            {
                if (result.IsLockedOut)
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    ModelState.AddModelError(string.Empty,
                        $"Your account is locked until {lockoutEnd?.LocalDateTime}");

                    return View("Areas/Identity/Views/Account/Login.cshtml", vm);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View("Areas/Identity/Views/Account/Login.cshtml", vm);
            }
        }

        private async Task<IActionResult> RedirectToLocal(
            string? returnUrl, ApplicationUser user)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(SD.Role_Creator))
                return RedirectToAction("Index", "Dashboard", new { area = "Creator" });
            if (roles.Contains(SD.Role_Respondent))
                return RedirectToAction("Index", "Dashboard", new { area = "Respondent" });

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("Areas/Identity/Views/Account/AccessDenied.cshtml");
        }
    }
}
