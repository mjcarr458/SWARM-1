using System;
namespace SWARM.Shared.DTO
{
    public class EnrollmentDTO
    {
        public int StudentId { get; set; }
        public int SecId { get; set; }
        public DateTime EnrollDate {get; set; }
        public byte? FinalGrade { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int SchoolId { get; set; }
    }
}
