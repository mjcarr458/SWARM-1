using System;
namespace SWARM.Shared.DTO
{
    public class SchoolDTO
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
