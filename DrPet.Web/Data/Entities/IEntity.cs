﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public interface IEntity
    {
       int Id { get; set; }

        bool IsDeleted { get; set; }
    }
}
