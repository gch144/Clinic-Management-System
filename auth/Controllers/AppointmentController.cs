using auth.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
// using System.DateAndTime; 
namespace auth.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> userManager;

        public AppointmentController(UserManager<User> userManager, DatabaseContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }
        [Authorize(Roles = "patient")]
        public IActionResult SearchDoctor()
        {
            // You can perform any necessary setup here before rendering the view
            return View("SearchDoctor");
        }
        [Authorize(Roles = "patient")]
        [HttpGet]
        public IActionResult SearchDoctorsBySpecialization([FromQuery(Name = "Specialty_Doc")] int specialization)
        {

            // Console.WriteLine($"Received specialization: {specialization}");

            var doctors = _context.Users
                .Where(u => u.Specialty_Doc != DoctorSpecialty.None && (int)u.Specialty_Doc == specialization)
                .ToList();

            return View("Doctors", doctors);
        }
        [Authorize(Roles = "patient")]
        public async Task<IActionResult> DisplayDoctorAvaibilty(string doctorId)
        {
            // ViewData.TryAdd("docToCreateScheduleFor", new());
            ViewData["DoctorDetails"] = await userManager.FindByIdAsync(doctorId);
            var doctors = _context.DoctorSchedules
                .Include(d => d.Doctor)
                .Where(d => d.DoctorId == doctorId)
                .ToList();

            return View("DisplayDoctorAvaibilty", doctors);
        }


        [Authorize(Roles = "patient")]
        [HttpPost]
        public IActionResult DisplayAvailableTimeSlots(string doctorId, DateTime doctordate, TimeOnly startTime, TimeOnly endTime)
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            // Retrieve the doctor's schedule
            var doctorSchedule = _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId && ds.DoctorDate == doctordate && ds.StartTime == startTime && ds.EndTime == endTime)
                .FirstOrDefault();
            // Console.WriteLine($"doctorId: {doctorId}, doctordate: {doctordate}, startTime: {startTime}, endTime: {endTime}");

            if (doctorSchedule == null)
            {
                // Handle case where the doctor's schedule is not found
                return RedirectToAction("Error");
            }

            // Generate available time slots based on the doctor's schedule
            var availableTimeSlots = GenerateAvailableTimeSlots(doctorSchedule);

            return View(availableTimeSlots);
        }

        private List<Appointment> GenerateAvailableTimeSlots(DoctorSchedule doctorSchedule)
        {
            var availableTimeSlots = new List<Appointment>();

            // Calculate time slots based on the doctor's schedule
            DateTime testDate = doctorSchedule.DoctorDate;
            DateOnly currentDate = DateOnly.FromDateTime(testDate);
            TimeOnly currentTime = doctorSchedule.StartTime;
            TimeOnly endTime = doctorSchedule.EndTime;

            while (currentTime.AddMinutes(15) <= endTime)
            {
                var timeSlot = new Appointment
                {
                    DoctorId = doctorSchedule.DoctorId,
                    AppointmentDate = currentDate,
                    StartTime = currentTime,
                    EndTime = currentTime.AddMinutes(15),

                };

                availableTimeSlots.Add(timeSlot);

                currentTime = currentTime.AddMinutes(15);
            }

            return availableTimeSlots;
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public IActionResult BookAppointment(string doctorId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime)
        {


            var patientId = GetCurrentUserId().Result;

            var scheduleId = _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId && s.DoctorDate == appointmentDate.ToDateTime(TimeOnly.Parse("12:00 AM")))
                .Select(s => s.Id)
                .FirstOrDefault();
            var isSlotAvailable = IsAppointmentSlotAvailable(doctorId, appointmentDate, startTime);

            if (!isSlotAvailable)
            {
                TempData["ErrorMessage"] = "This slot is already booked. Please try a different slot.";
                return RedirectToAction("DisplayAvailableTimeSlots", new { doctorId });
            }


            // Create a new appointment
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                DoctorScheduleId = scheduleId,
                PatientId = patientId,
                AppointmentDate = appointmentDate,
                StartTime = startTime,
                EndTime = endTime,
                Description = "Doctor need to write....",
            };


            _context.Appointments.Add(appointment);
            _context.SaveChanges();


            return RedirectToAction("DisplayPatientAppointments");
        }

        [Authorize(Roles = "patient")]
        [HttpPost]
        public IActionResult CancelAppointment(int appointmentId)
        {
            var patientId = GetCurrentUserId().Result;

            // Find the appointment in the database
            var appointment = _context.Appointments
                .FirstOrDefault(a => a.Id == appointmentId && a.PatientId == patientId);

            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();

            }
            else
            {
                TempData["Message"] = "Appointment not found or you don't have permission to cancel.";
            }

            return RedirectToAction("DisplayPatientAppointments");
        }


        private bool IsAppointmentSlotAvailable(string doctorId, DateOnly appointmentDate, TimeOnly startTime)
        {
            // Check if there is an existing appointment for the same doctor, date, and time
            return !_context.Appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate &&
                (a.StartTime == startTime)
            );
        }
        [Authorize(Roles = "patient")]
        public IActionResult DisplayPatientAppointments()
        {

            // Get the current user's ID (patient ID)
            var patientId = GetCurrentUserId().Result;

            // Retrieve appointments for the current patient
            var patientAppointments = _context.Appointments
                .Where(appointment => appointment.PatientId == patientId)
                .Include(appointment => appointment.Doctor)  // Include related doctor information if needed
                .ToList();

            return View(patientAppointments);
        }
        [Authorize(Roles = "doctor")]
        public IActionResult DisplayDoctorAppointments()
        {
            // Get the current user's ID (doctor ID)
            var doctorId = GetCurrentUserId().Result;

            // Retrieve appointments for the current doctor
            var doctorAppointments = _context.Appointments
                .Where(appointment => appointment.DoctorId == doctorId)
                .Include(appointment => appointment.Patient)  // Include related patient information if needed
                .ToList();

            return View(doctorAppointments);
        }
        [Authorize(Roles = "doctor")]
        [HttpGet]
        public IActionResult EditAppointment(int appointmentId)
        {
            // Get the appointment from the database
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == appointmentId);

            // Check if the appointment exists and belongs to the current doctor
            if (appointment == null)
            {
                return NotFound(); // Or handle unauthorized access as needed
            }

            return View("EditAppointment", appointment);
        }

        [Authorize(Roles = "doctor")]
        [HttpPost]
        public IActionResult EditAppointment(Appointment editedAppointment)
        {

            // Update only the description
            var existingAppointment = _context.Appointments.FirstOrDefault(a => a.Id == editedAppointment.Id);
            if (existingAppointment == null)
            {
                return NotFound();
            }
            existingAppointment.Description = editedAppointment.Description;
            // Save changes to the database
            _context.SaveChanges();


            return RedirectToAction("DisplayDoctorAppointments");
        }
        private async Task<string> GetCurrentUserId()
        {

            return (await userManager.FindByNameAsync(User.Identity.Name)).Id;
        }
    }

}