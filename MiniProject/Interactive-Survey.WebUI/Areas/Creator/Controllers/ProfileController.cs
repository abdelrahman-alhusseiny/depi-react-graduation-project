using Interactive_Survey.DataAccess.Repository.IRepository;
using Interactive_Survey.Models.Models;
using Interactive_Survey.Models.ViewModels;
using Interactive_Survey.Utilities.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;

namespace Interactive_Survey.WebUI.Areas.Creator.Controllers
{
    [Authorize(Roles = SD.Role_Creator)]
    [Area("Creator")]
    [Route("Creator/Profile")]
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

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (creator == null)
            {
                return NotFound();
            }

            var vm = new CreatorVM
            {
                Creator = creator,
                DisplayImageUrl = string.IsNullOrEmpty(creator.ImageUrl) ? "/images/creators/default-creator.png" : creator.ImageUrl,
                AppUserEmail = creator.AppUser.Email
            };

            return View("Areas/Creator/Views/Profile/GetProfile.cshtml", vm);
        }

        [HttpGet("EditProfile")]
        public async Task<IActionResult> EditProfile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (creator == null)
            {
                return NotFound();
            }

            var vm = new CreatorVM
            {
                Creator = creator,
                GenderList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Male", Value = "Male" },
                    new SelectListItem { Text = "Female", Value = "Female" },
                },
                DisplayImageUrl = string.IsNullOrEmpty(creator.ImageUrl) ? "/images/creators/default-creator.png" : creator.ImageUrl,
                HasCustomImage = !string.IsNullOrEmpty(creator.ImageUrl),
            };

            return View("Areas/Creator/Views/Profile/EditProfile.cshtml", vm);
        }

        [HttpPost("EditProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileConfirmed([FromForm] CreatorVM vm)
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

                return View("Areas/Creator/Views/Profile/EditProfile.cshtml", vm);
            }

            if (vm.Creator.ImageFile != null && vm.Creator.ImageFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadPath = Path.Combine(wwwRootPath, "images", "creators");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (!string.IsNullOrEmpty(vm.Creator.ImageUrl))
                {
                    string oldImagePath = Path.Combine(
                        wwwRootPath, vm.Creator.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
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

            await _unitOfWork.Creator.UpdateAsync(vm.Creator);
            await _unitOfWork.SaveAsync();

            TempData["success"] = "Profile updated successfully.";
            return RedirectToAction("GetProfile", "Profile", new { area = "Creator" });
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

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (creator == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(creator.ImageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(
                    wwwRootPath, creator.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                creator.ImageUrl = null;
                await _unitOfWork.Creator.UpdateAsync(creator);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Image deleted successfully.";
            }

            return RedirectToAction("EditProfile", "Profile", new { area = "Creator" });
        }

        [HttpGet("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
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

            var vm = new CreatorVM
            {
                Creator = creator,
                DisplayImageUrl = string.IsNullOrEmpty(creator.ImageUrl) ? "/images/creators/default-creator.png" : creator.ImageUrl,
                AppUserEmail = creator.AppUser.Email
            };

            return View("Areas/Creator/Views/Profile/DeleteAccount.cshtml", vm);
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

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (creator == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(creator.ImageUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImagePath = Path.Combine(
                    wwwRootPath, creator.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.Creator.Remove(creator);
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

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id, includeProperties: "AppUser");
            if (creator == null)
            {
                return NotFound();
            }

            var vm = new ChangePasswordVM
            {
                AppUserEmail = creator.AppUser.Email
            };

            return View("Areas/Creator/Views/Profile/ChangeAccountPassword.cshtml", vm);
        }

        [HttpPost("ChangeAccountPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAccountPasswordConfirmed([FromForm] ChangePasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Areas/Creator/Views/Profile/ChangeAccountPassword.cshtml", vm);
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return NotFound();
            }

            var creator = await _unitOfWork.Creator.GetFirstOrDefaultAsync(
                c => c.AppUserId == appUser.Id);
            if (creator == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(creator.AppUser, vm.CurrentPassword, vm.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Areas/Creator/Views/Profile/ChangeAccountPassword.cshtml", vm);
            }

            await _signInManager.RefreshSignInAsync(creator.AppUser);

            TempData["success"] = "Password changed successfully.";
            return RedirectToAction("GetProfile", "Profile", new { area = "Creator" });
        }
    }
}
