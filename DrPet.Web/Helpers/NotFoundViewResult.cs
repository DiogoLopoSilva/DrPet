using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DrPet.Web.Helpers
{
    public class NotFoundViewResult :ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound; //TODO PERGUNTAR O PORQUE DE TER DE PASSAR O CODIGO
        } 
    }
}
