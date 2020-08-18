using DrPet.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Models
{
    public class AnimalViewModel : Animal //TODO: ERGUNTAR O PORQUE DO IMAGEFILE NAO ESTAR DENTRO DO ANIMAL
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
