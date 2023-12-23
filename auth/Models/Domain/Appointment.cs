using auth.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
public class Appointment
{
    // [Key]
    public int Id { get; set; }
    public int? DoctorScheduleId { get; set; }
    public string DoctorId { get; set; }
    public string PatientId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? Description { get; set; }

    public virtual User Doctor { get; set; }
    public virtual User Patient { get; set; }
    public virtual DoctorSchedule DoctorSchedule { get; set; }
}

