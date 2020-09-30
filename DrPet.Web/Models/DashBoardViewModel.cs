using DrPet.Web.Data.Entities;
using System.Collections.Generic;

namespace DrPet.Web.Models
{
    public class DashBoardViewModel
    {
        public int Admins { get; set; }

        public int Clients { get; set; }

        public int Doctors { get; set; }

        public int Animals { get; set; }

        public int Appointments { get; set; }

        public List<DoughnutData> ChartData { get; set; }

        public List<Client> LatestClients { get; set; }
    }
}
