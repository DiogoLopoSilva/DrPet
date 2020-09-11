using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        [Required]
        public Client Client { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        public Doctor Doctor { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Status { get; set; }     

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } //TODO MUDAR PARA LOCAL TIME        

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get { return this.StartTime.AddMinutes(30); } } //TODO MUDAR PARA LOCAL TIME

        public string DoctorNotes { get; set; }

        public string ClientDescription { get; set; }

        [NotMapped]
        public bool IsBlock { get; set; }

        [NotMapped]
        public bool IsReadonly { get; set; }           
    }
}
