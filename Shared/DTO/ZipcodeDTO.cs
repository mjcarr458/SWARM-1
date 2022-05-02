﻿using System;
namespace SWARM.Shared.DTO
{
    public class ZipcodeDTO
    {
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
