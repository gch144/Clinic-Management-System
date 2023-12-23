using auth.Models.Domain;
using auth.Models.DTO;
using auth.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace auth.Controllers
{
    // [Area("admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IUserAuthenticationService _authService;
        private readonly DatabaseContext _context;
        private readonly UserManager<User> userManager;

        public AdminController(UserManager<User> userManager, IUserAuthenticationService authService, DatabaseContext context)
        {
            this.userManager = userManager;
            this._context = context;
            this._authService = authService;
        }



        private async Task VerifyUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.IsVerified = true;

                // Update the notification status if applicable
                if (user.Notification != null)
                {
                    user.Notification.IsNotified = true;
                }
                await userManager.UpdateSecurityStampAsync(user);
                await userManager.UpdateAsync(user);
            }
        }
        [HttpPost]
        public async Task<IActionResult> VerifyUser(string userId)
        {
            await VerifyUserAsync(userId);
            return RedirectToAction("Display"); // Redirect to the admin user display page
        }
        [HttpGet]
        public IActionResult Display()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }



        public IActionResult RegistrationDoctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrationDoctor(RegistrationDoctorModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            model.Role = "doctor";
            var result = await this._authService.RegisterDoctorAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Display));
            // return View(model);
        }
        /// <summary>
        /// Doctor Schedules Part start
        /// </summary>
        /// <param name</param>
        /// <returns></returns>
        public IActionResult AllDoctorSchedules()
        {
            // Retrieve a list of DoctorSchedule objects from the database
            var alldoctorSchedules = _context.DoctorSchedules
               .Include(ds => ds.Doctor).ToList();

            return View("DoctorSchedule/AllDoctorSchedules", alldoctorSchedules);
        }

        public IActionResult DisplayDoctorSchedules(string doctorId)
        {
            // Retrieve all doctors, including those without schedules
            var allDoctors = _context.Users
                .Where(user => user.Specialty_Doc != DoctorSpecialty.None) // Filter doctors only
                .ToList();

            // Retrieve doctor schedules for a specific doctor, including the doctor information
            var doctorSchedules = _context.DoctorSchedules
                .Include(ds => ds.Doctor).ToList();


            // Filter doctor schedules for the current week
            var currentDate = DateTime.Today;
            var startOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            var currentWeekSchedules = doctorSchedules
                .Where(ds => ds.DoctorDate >= startOfWeek && ds.DoctorDate <= endOfWeek)
                .ToList();
            var nextWeekSchedule = _context.DoctorSchedules
                .Where(s => s.DoctorDate >= startOfWeek.AddDays(7) && s.DoctorDate < endOfWeek.AddDays(7))
                .ToList();
            ViewData["CurrentWeekSchedules"] = currentWeekSchedules;
            ViewData["NextWeekSchedule"] = nextWeekSchedule;


            ViewData["AllTheDoctors"] = allDoctors.ToList();

            return View("DoctorSchedule/DisplayDoctorSchedules", doctorSchedules);
        }

        // public IActionResult Doctor
        // [HttpGet]
        public async Task<IActionResult> CreateDoctorSchedule(string doctorId)
        {

            ViewData.TryAdd("docToCreateScheduleFor", new());
            ViewData["docToCreateScheduleFor"] = await userManager.FindByIdAsync(doctorId);
            return View("DoctorSchedule/CreateDoctorSchedule");//RedirectToPage("/DoctorSchedule/CreateDoctorSchedule", new { doctorId = 3 });

        }

        [HttpPost]
        public IActionResult CreateDoctorSchedule(DoctorSchedule doctorSchedule)
        {


            // Handle invalid model state
            var doctorId = doctorSchedule.DoctorId; // Retrieve doctorI d from the submitted form
            var doctor = _context.Users.FirstOrDefault(u => u.Id == doctorId && u.Specialty_Doc != DoctorSpecialty.None);
            var existingSchedule = _context.DoctorSchedules
                .FirstOrDefault(s => s.DoctorId == doctorId &&
                                    ((doctorSchedule.StartTime >= s.StartTime && doctorSchedule.StartTime < s.EndTime) ||
                                    (doctorSchedule.EndTime > s.StartTime && doctorSchedule.EndTime <= s.EndTime) ||
                                    (doctorSchedule.StartTime <= s.StartTime && doctorSchedule.EndTime >= s.EndTime)));
            // System.pr

            if (existingSchedule != null)
            {
                TempData["ErrorMessage"] = "The doctor already has a schedule at the specified time.";
                // ModelState.AddModelError("", "The doctor already has a schedule at the specified time.");
                return RedirectToAction("CreateDoctorSchedule", new { doctorId });// Provide the name of your error view
            }
            // Console.WriteLine("Existing Schedule :" + existingSchedule);

            if (doctor == null)
            {
                // Handle invalid doctorId
                return NotFound();
            }

            _context.DoctorSchedules.Add(doctorSchedule);
            _context.SaveChanges();

            ViewData["Doctor"] = doctor;

            return RedirectToAction("DisplayDoctorSchedules");
            // return View("DoctorSchedule/DisplayDoctorSchedules");
        }
        [HttpPost]
        public IActionResult CopyScheduleForNextWeek(string doctorId, int scheduleId)
        {
            try
            {
                // Retrieve the existing schedule entry
                var existingSchedule = _context.DoctorSchedules
                    .Include(ds => ds.Doctor)
                    .FirstOrDefault(ds => ds.Id == scheduleId && ds.DoctorId == doctorId);

                if (existingSchedule == null)
                {
                    // Handle the case where the schedule entry is not found
                    return RedirectToAction("DisplayDoctorSchedules");
                }

                // Duplicate the schedule entry for the next week
                var nextWeekSchedule = new DoctorSchedule
                {
                    DoctorId = existingSchedule.DoctorId,
                    DoctorDate = existingSchedule.DoctorDate.AddDays(7),
                    StartTime = existingSchedule.StartTime,
                    EndTime = existingSchedule.EndTime

                };

                // Save the duplicated schedule for the next week
                _context.DoctorSchedules.Add(nextWeekSchedule);
                _context.SaveChanges();

                return RedirectToAction("DisplayDoctorSchedules");
            }
            catch (Exception ex)
            {
                // Handle exceptions, log errors, or redirect to an error page
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public IActionResult CopyCurrentWeekSchedules()
        {
            DateTime today = DateTime.Today;

            // Find the start of the week (Sunday)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            // Find the end of the week (Saturday) by adding 6 days
            DateTime endOfWeek = startOfWeek.AddDays(6);
            // Retrieve the current week's schedule for all doctors
            var currentWeekSchedule = _context.DoctorSchedules
                .Where(s => s.DoctorDate >= startOfWeek && s.DoctorDate <= endOfWeek)
                .ToList();

            if (currentWeekSchedule.Count == 0)
            {
                TempData["ErrorMessage"] = "No schedule found for the current week.";
                return RedirectToAction("DisplayDoctorSchedules"); // Redirect to the schedule display page
            }

            // Create a dictionary to store the cloned schedules for each doctor
            var clonedSchedulesByDoctor = new Dictionary<string, List<DoctorSchedule>>();

            // Iterate over all doctors
            var doctorIds = currentWeekSchedule.Select(s => s.DoctorId).Distinct();
            foreach (var doctorId in doctorIds)
            {
                // Clone the current week's schedule for the next week for the current doctor
                var nextWeekSchedule = currentWeekSchedule
                    .Where(s => s.DoctorId == doctorId)
                    .Select(schedule => new DoctorSchedule
                    {
                        DoctorId = schedule.DoctorId,
                        DoctorDate = schedule.DoctorDate.AddDays(7), // Increment by 7 days for the next week
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        // Copy other properties as needed
                    })
                    .ToList();


                clonedSchedulesByDoctor.Add(doctorId, nextWeekSchedule);
            }

            // Save the cloned schedules for the next week for all doctors
            foreach (var entry in clonedSchedulesByDoctor)
            {
                _context.DoctorSchedules.AddRange(entry.Value);
            }
            _context.SaveChanges();

            return RedirectToAction("DisplayDoctorSchedules"); // Redirect to the schedule display page
        }


        [HttpGet]
        public IActionResult EditDoctorSchedule(int scheduleId)
        {
            var doctorSchedule = _context.DoctorSchedules.Find(scheduleId);

            if (doctorSchedule == null)
            {
                return NotFound();

            }

            return View("DoctorSchedule/EditDoctorSchedule", doctorSchedule);
        }

        [HttpPost]
        public IActionResult EditDoctorSchedule(DoctorSchedule doctorSchedule)
        {

            var existingSchedule = _context.DoctorSchedules.Find(doctorSchedule.Id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            // Update the existing schedule with the new values
            existingSchedule.DoctorDate = doctorSchedule.DoctorDate;
            existingSchedule.StartTime = doctorSchedule.StartTime;
            existingSchedule.EndTime = doctorSchedule.EndTime;

            _context.SaveChanges();

            return RedirectToAction("DisplayDoctorSchedules");


        }
        [HttpPost]
        public IActionResult DeleteDoctorSchedule(int scheduleId)
        {
            var doctorSchedule = _context.DoctorSchedules.Find(scheduleId);

            if (doctorSchedule != null)
            {
                _context.DoctorSchedules.Remove(doctorSchedule);
                _context.SaveChanges();
            }

            return RedirectToAction("DisplayDoctorSchedules", new { doctorId = doctorSchedule?.DoctorId });
        }

    }

}