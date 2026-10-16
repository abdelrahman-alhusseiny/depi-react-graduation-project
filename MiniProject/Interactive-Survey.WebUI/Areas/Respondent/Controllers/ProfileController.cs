using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Interactive_Survey.Models.ViewModels;
using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Interactive_Survey.WebUI.Areas.Respondent.Controllers
{
    [Authorize(Roles = SD.Role_Respondent)]
    [Area("Respondent")]
    [Route("Respondent/Profile")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
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

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (respondent == null)
            {
                return NotFound();
            }

            var vm = new RespondentVM
            {
                Respondent = respondent,
                DisplayImageUrl = string.IsNullOrEmpty(respondent.ImageUrl) ? "/images/respondents/default-respondent.png" : respondent.ImageUrl,
                AppUserEmail = respondent.AppUser.Email
            };

            return View("Areas/Respondent/Views/Profile/GetProfile.cshtml", vm);
        }

        [HttpGet("EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (respondent == null)
            {
                return NotFound();
            }

            var vm = new RespondentVM
            {
                Respondent = respondent,
                GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                },
                DisplayImageUrl = string.IsNullOrEmpty(respondent.ImageUrl) ? "/images/respondents/default-respondent.png" : respondent.ImageUrl,
                HasCustomImage = !string.IsNullOrEmpty(respondent.ImageUrl),
            };

            return View("Areas/Respondent/Views/Profile/EditProfile.cshtml", vm);
        }

        [HttpPost("EditProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileConfirmed([FromForm] RespondentVM vm)
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

                return View("Areas/Respondent/Views/Profile/EditProfile.cshtml", vm);
            }

            if (vm.Respondent.ImageFile != null && vm.Respondent.ImageFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "images", "respondents");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (!string.IsNullOrEmpty(vm.Respondent.ImageUrl))
                {
                    string oldImagePath = Path.Combine(
                        wwwRootPath, vm.Respondent.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
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

            await _unitOfWork.Respondent.UpdateAsync(vm.Respondent);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Profile updated successfully.";
            return RedirectToAction("GetProfile", "Profile", new { area = "Respondent" });
        }

        [HttpPost("DeleteProfileImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (respondent == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(respondent.ImageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(
                    wwwRootPath, respondent.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                respondent.ImageUrl = null;
                await _unitOfWork.Respondent.UpdateAsync(respondent);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Image deleted successfully.";
            }

            return RedirectToAction("EditProfile", "Profile", new { area = "Respondent" });
        }

        [HttpGet("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (respondent == null)
            {
                return NotFound();
            }

            var vm = new RespondentVM
            {
                Respondent = respondent,
                DisplayImageUrl = string.IsNullOrEmpty(respondent.ImageUrl) ? "/images/respondents/default-respondent.png" : respondent.ImageUrl,
                AppUserEmail = respondent.AppUser.Email
            };

            return View("Areas/Respondent/Views/Profile/DeleteAccount.cshtml", vm);
        }

        [HttpPost("DeleteAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (respondent == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(respondent.ImageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(
                    wwwRootPath, respondent.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Respondent.Remove(respondent);
            await _userManager.DeleteAsync(appUser);
            await _unitOfWork.SaveAsync();
            await _signInManager.SignOutAsync();

            TempData["success"] = "Your account has been deleted successfully.";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet("ChangeAccountPassword")]
        public async Task<IActionResult> ChangeAccountPassword()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (respondent == null)
            {
                return NotFound();
            }

            var vm = new ChangePasswordVM
            {
                AppUserEmail = respondent.AppUser.Email
            };

            return View("Areas/Respondent/Views/Profile/ChangeAccountPassword.cshtml", vm);
        }

        [HttpPost("ChangeAccountPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAccountPasswordConfirmed([FromForm] ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Areas/Respondent/Views/Profile/ChangeAccountPassword.cshtml", vm);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var respondent = await _unitOfWork.Respondent.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (respondent == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(respondent.AppUser, vm.CurrentPassword, vm.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Areas/Respondent/Views/Profile/ChangeAccountPassword.cshtml", vm);
            }

            await _signInManager.RefreshSignInAsync(respondent.AppUser);

            TempData["success"] = "Password changed successfully.";
            return RedirectToAction("GetProfile", "Profile", new { area = "Respondent" });
        }
    }
}
