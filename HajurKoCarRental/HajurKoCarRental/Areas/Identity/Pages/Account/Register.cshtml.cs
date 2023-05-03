using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.IO;
using HajurKoCarRental.Areas.Identity.Data;
using HajurKoCarRental.Models;
using System.Net.Mail;
using System.Net;

namespace HajurKoCarRental.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<HajurKoCarRentalUser> _signInManager;
        private readonly UserManager<HajurKoCarRentalUser> _userManager;
        private readonly IUserStore<HajurKoCarRentalUser> _userStore;
        private readonly IUserEmailStore<HajurKoCarRentalUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long MaxFileSize = (long)(1.5 * 1024 * 1024); // 1.5 MB


        public RegisterModel(
            UserManager<HajurKoCarRentalUser> userManager,
            IUserStore<HajurKoCarRentalUser> userStore,
            SignInManager<HajurKoCarRentalUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetUniqueFileName(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var randomString = Path.GetRandomFileName().Replace(".", "");
            var uniqueFileName = $"{fileNameWithoutExtension}_{timestamp}_{randomString}{extension}";
            return uniqueFileName;
        }
        private bool IsSupportedFileType(string fileName, string[] supportedExtensions)
        {
            var fileExtension = Path.GetExtension(fileName);
            return supportedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(250, ErrorMessage = "The {0} must be at least {2} and should not exceed {1} characters", MinimumLength = 3)]
            [Display(Name = "Name")]
            public string UserName { get; set; }



            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }


            [Required]
            [StringLength(250, ErrorMessage = "Enter your valid address", MinimumLength = 2)]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [StringLength(250, ErrorMessage = "Enter your valid Phone number", MinimumLength = 2)]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            [DataType(DataType.Upload)]
            [Display(Name = "Driving Licence Document")]
            public IFormFile DrivingLicense { get; set; }


            [DataType(DataType.Upload)]
            [Display(Name = "Citizenship Document")]
            public IFormFile Citizenship { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {

            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Check driving license file size and format
            if (Input.DrivingLicense != null && Input.DrivingLicense.Length > MaxFileSize)
            {
                ModelState.AddModelError(string.Empty, "The driving license file size should not exceed 1.5 MB.");
            }

            if (Input.DrivingLicense != null && !IsSupportedFileType(Input.DrivingLicense.FileName, new[] { ".pdf", ".png" }))
            {
                ModelState.AddModelError(string.Empty, "The driving license should be in PDF or PNG format.");
            }

            // Check citizen paper file size and format
            if (Input.Citizenship != null && Input.Citizenship.Length > MaxFileSize)
            {
                ModelState.AddModelError(string.Empty, "The citizen paper file size should not exceed 1.5 MB.");
            }

            if (Input.Citizenship != null && !IsSupportedFileType(Input.Citizenship.FileName, new[] { ".pdf", ".png" }))
            {
                ModelState.AddModelError(string.Empty, "The citizen paper should be in PDF or PNG format.");
            }


            
                var user = CreateUser();

                user.FullName = Input.UserName;
                user.Address = Input.Address;
                user.PhoneNumber = Input.PhoneNumber;
                user.DrivingLicense = null;
                user.Citizenship = null;
                user.CitizenshipFileName = null;
                user.DrivingLicenseFileName = null;
                

                // Upload driving license scan
                if (Input.DrivingLicense != null && Input.DrivingLicense.Length > 0)
                {
                    if (Input.DrivingLicense.Length <= MaxFileSize)
                    {
                        // Generate a unique filename for the driving license
                        var drivingLicenseFileName = GetUniqueFileName(Input.DrivingLicense.FileName);
                        var drivingLicenseFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", drivingLicenseFileName);

                        // Save the driving license file
                        using (var fileStream = new FileStream(drivingLicenseFilePath, FileMode.Create))
                        {
                            await Input.DrivingLicense.CopyToAsync(fileStream);
                        }
                        var license = new byte[Input.DrivingLicense.Length];

                        await Input.DrivingLicense.OpenReadStream().ReadAsync(user.DrivingLicense);
                    

                    // Set the user's driving license properties
                    user.DrivingLicense = license;
                    user.DrivingLicenseFileName = drivingLicenseFileName;
                    
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The driving license file size exceeds the maximum limit.");
                        return Page();
                    }
                }

                // Upload citizen paper scan
                if (Input.Citizenship != null && Input.Citizenship.Length > 0)
                {
                    if (Input.Citizenship.Length <= MaxFileSize)
                    {
                        // Generate a unique filename for the citizen paper
                        var citizenPaperFileName = GetUniqueFileName(Input.Citizenship.FileName);

                        var citizenPaperFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", citizenPaperFileName);

                        // Save the citizen paper file
                        using (var fileStream = new FileStream(citizenPaperFilePath, FileMode.Create))
                        {
                            await Input.Citizenship.CopyToAsync(fileStream);
                        }

                        var CitizenPaper = new byte[Input.Citizenship.Length];

                        
                        await Input.Citizenship.OpenReadStream().ReadAsync(user.Citizenship);

                        // Set the user's citizen paper properties
                        user.Citizenship = CitizenPaper;
                        user.CitizenshipFileName = citizenPaperFileName;
                       
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The citizen paper file size exceeds the maximum limit.");
                        return Page();
                    }
                }






                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Assign the "Customer" role to the registered user
                    await _userManager.AddToRoleAsync(user, UserRoles.Customer);

                    _logger.LogInformation("User created a new account with password.");
                    

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                await SendEmailAsync(Input.Email, "Confirm your email for Hamro Car Rental",
                    $"Your are invited to be a user of Hamro Car Rental. Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.\n");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private async Task<bool> SendEmailAsync(string email, string subject, string callbackLink)
        {
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                msg.From = new MailAddress("khattribimal06@gmail.com");
                msg.Subject = subject;
                msg.To.Add(email);
                msg.IsBodyHtml = true;
                msg.Body = callbackLink;

                client.Credentials = new NetworkCredential("khattribimal06@gmail.com", "bdrlbagnwtjfkyfh");
                client.EnableSsl = true;
                client.Send(msg);

                return true;

            }
            catch
            {
                return false;
            }
        }
        private HajurKoCarRentalUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<HajurKoCarRentalUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(HajurKoCarRentalUser)}'. " +
                    $"Ensure that '{nameof(HajurKoCarRentalUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<HajurKoCarRentalUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<HajurKoCarRentalUser>)_userStore;
        }
    }
}
