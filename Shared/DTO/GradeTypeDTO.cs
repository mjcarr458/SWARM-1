﻿using System;
namespace SWARM.Shared.DTO
{
    public class GradeTypeDTO
    {
        public int SchoolId { get; set; }
        public string GradeTypeCode { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
