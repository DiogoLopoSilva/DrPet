﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Doctor : Human
    {
        public int SpecializationId { get; set; }

        public Specialization Specialization { get; set; }

    }
}
