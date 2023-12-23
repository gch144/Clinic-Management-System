using auth.Models.Domain;
using Microsoft.CodeAnalysis.Elfie.Model;

public class DoctorSchedule
{
    public int Id { get; set; }

    // Foreign key to link to the User (Doctor)
    public string DoctorId { get; set; }

    public DateTime DoctorDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }


    public virtual User Doctor { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
}
