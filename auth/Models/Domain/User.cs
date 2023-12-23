
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using auth.Models.Domain;

namespace auth.Models.Domain
{
    public enum DoctorSpecialty : int
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "Home")]
        Home,
        [Display(Name = "ENT")]
        ENT,
        [Display(Name = "Dermatologist")]
        Dermatologist,
        [Display(Name = "Ophtalmologist")]
        Ophtalmologist,
        [Display(Name = "Neurologist")]
        Neurologist,
        [Display(Name = "Orthopedist")]
        Orthopedist,
        [Display(Name = "Pediatrician")]
        Pediatrician
    }
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public UserNotification Notification { get; set; }
        public DoctorSpecialty Specialty_Doc { get; set; }
        public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; }
        // public virtual ICollection<Appointment> Appointments { get; set; }
    }


}
