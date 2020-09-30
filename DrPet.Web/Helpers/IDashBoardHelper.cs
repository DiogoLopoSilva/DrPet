using DrPet.Web.Data.Entities;
using DrPet.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public interface IDashBoardHelper
    {
        //ChangeUserViewModel ToChangeUserViewModel(Human human);
        DashBoardViewModel GetDashBoardInfo();
    }
}
