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
        public Specialization Specialization { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } 

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get { return this.StartTime.AddMinutes(30); } }

        public string DoctorNotes { get; set; }

        public string ClientDescription { get; set; }

        [NotMapped]
        public bool IsBlock { get; set; }

        [NotMapped]
        public bool IsReadonly { get; set; }

        public bool IsDeleted { get; set; }

        public string Subject
        {
            get
            {
                if (this.Specialization != null)
                {
                    return this.Specialization.Name;
                }
                return "Not Available";
            }
        }
    }
}
