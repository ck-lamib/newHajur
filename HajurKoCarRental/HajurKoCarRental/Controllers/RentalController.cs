using HajurKoCarRental.Areas.Identity.Data;
using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HajurKoCarRental.Controllers
{
    public class RentalController : Controller
    {
        private readonly HajurKoCarRentalDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<HajurKoCarRentalUser> _userManager;

        public RentalController(HajurKoCarRentalDbContext dbContext, IWebHostEnvironment webHostEnvironment, UserManager<HajurKoCarRentalUser> userManager)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Rental/Rent/5
        public IActionResult Rent(int id)
        {
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect the user to the login page
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Check if the logged in user has uploaded the documents
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            // Check if the user has uploaded the documents
            if (user.DrivingLicenseFileName == null || user.CitizenshipFileName == null)
            {
                // Redirect the user to upload the documents
                return RedirectToAction("UploadDocuments");
            }

            // Get the car info by ID
            var car = _dbContext.CarInfo.FirstOrDefault(c => c.id == id);

            if (car == null)
            {
                return NotFound();
            }


          

            return View(car);
        }

        // POST: Rental/Rent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(DateTime date, int id)
        {
            

            // Check if the logged in user has uploaded the documents
            var user = await _userManager.GetUserAsync(User);

           

            if (ModelState.IsValid)
            {
                var car = _dbContext.CarInfo.First(x=> x.id ==id);

                car.is_available = false;

                
                Rental rental = new Rental();
                // Update rental properties
                rental.UserID = user.Id;
                rental.Fee = CalculateRentalFee(id, user.is_RegularCustomer, User.IsInRole(UserRoles.Staff));
                rental.RentalStatus = 0;
                rental.CarID = id;
                rental.AuthorizedBy = "none";
                rental.date = date;
                rental.CreatedAt = DateTime.Now;
               

                _dbContext.Rental.Add(rental);
                await _dbContext.SaveChangesAsync();

               

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        private decimal CalculateRentalFee(int carId, bool isRegularCustomer, bool isStaff)
        {
            var car = _dbContext.CarInfo.FirstOrDefault(c => c.id == carId);

            if (car == null)
            {
                throw new InvalidOperationException("Invalid car ID.");
            }

            decimal rentalFee = car.RentPrice;

            if (isRegularCustomer)
            {
                rentalFee *= 0.9m; // 10% discount for regular customers
            }

            if (isStaff)
            {
                rentalFee *= 0.75m; // 25% discount for staff members
            }

            return rentalFee;
        }

        private void NotifyCustomer(Rental rental)
        {
        }


        // GET: Rental/Confirmation
        public IActionResult Confirmation()
        {
            return View();
        }

        // GET: Rental/UploadDocuments
        [Authorize]
        public IActionResult UploadDocuments()
        {
            // Display the view to upload the documents
            return View();
        }

        // POST: Rental/UploadDocuments
        [HttpPost]
        public async Task<IActionResult> UploadDocuments(DocumentUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save the driving license file
                if (model.DrivingLicense != null && model.DrivingLicense.Length > 0)
                {


                    string drivingLicenseFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.DrivingLicense.FileName);
                    string drivingLicenseFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", drivingLicenseFileName);

                    using (var stream = new FileStream(drivingLicenseFilePath, FileMode.Create))
                    {
                        await model.DrivingLicense.CopyToAsync(stream);
                    }

                    // Update the user's driving license details in the database
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId);
                    user.DrivingLicenseFileName = drivingLicenseFileName;
                    Console.WriteLine("update");
                    await _userManager.UpdateAsync(user);
                    user.DrivingLicenseFileName = drivingLicenseFileName;
                    Console.WriteLine("update");
                }

                // Save the citizen paper file
                if (model.CitizenPaper != null && model.CitizenPaper.Length > 0)
                {
                    string citizenPaperFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CitizenPaper.FileName);
                    string citizenPaperFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", citizenPaperFileName);

                    using (var stream = new FileStream(citizenPaperFilePath, FileMode.Create))
                    {
                        await model.CitizenPaper.CopyToAsync(stream);
                    }

                    // Update the user's citizen paper details in the database
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId);
                    user.CitizenshipFileName = citizenPaperFileName;
                    
                    await _userManager.UpdateAsync(user);
                }

                return RedirectToAction("Index", "Home");
            }

            // If the model is invalid, return the view with the model errors
            return View(model);
        }


        // GET: Rental/ViewRequestedRental
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RentalHistory()
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Get the rentals for the logged-in user
            var rentals = _dbContext.Rental
                .Include(r => r.CarInfo)
                .Where(r => r.UserID == user.Id)
                .ToList();

            return View(rentals);
        }

        // GET: Rental/Cancel/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Cancel(int id)
        {
            // Get the rental by ID
            var rental = await _dbContext.Rental
                .Include(r => r.CarInfo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            // Check if the rental is in a cancelable state (status 0 or 1)
            if (rental.RentalStatus == 0 || rental.RentalStatus == 1)
            {
                return View(rental);
            }

            return RedirectToAction("RentalHistory");
        }


        // POST: Rental/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            // Get the rental by ID
            var rental = await _dbContext.Rental.FindAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            // Check if the rental is in a cancelable state (status 0 or 1)
            if (rental.RentalStatus == 0 || rental.RentalStatus == 1)
            {
                // Delete the rental record from the database
                _dbContext.Rental.Remove(rental);
                await _dbContext.SaveChangesAsync();

                // Update the availability of the rented car
                var car = await _dbContext.CarInfo.FindAsync(rental.CarID);
                if (car != null)
                {
                    car.is_available = true;
                    _dbContext.CarInfo.Update(car);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction("RentalHistory");
            }

            return RedirectToAction("Index", "Home");
        }



    }


}