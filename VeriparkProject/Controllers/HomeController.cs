using Coreplus.Sample.Api.Services;
using Coreplus.Sample.Api.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VeriparkProject.Models;
using static Coreplus.Sample.Api.Services.PractitionerService;

namespace VeriparkProject.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
     
        public ActionResult Index()
        {

            PractitionerService s = new PractitionerService();
            var a = s.GetPractitionersTest();


            return View(a);
        }

        public async Task<IActionResult> EmployeeReportIndex(int PractionerID)
        {
            TempData["PRACTIONER_ID"] = PractionerID;
            CustomModelReportFinal model = new CustomModelReportFinal();
            model.practionId = PractionerID;
            return View("Views/EmployeeReport.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> SearchRequest(CustomModelReportFinal Model)
        {
      
            
            
            var startDate = Model.startDate;
            var endDate = Model.endDate;
            PractitionerService s = new PractitionerService();

          

            CustomModelReportFinal model = new CustomModelReportFinal
            {
                practionId = Model.practionId,
            startDate = startDate,
                endDate= endDate,
                CustomModelReportFinalList = s.GetPractitionersReport(Model.practionId, startDate, endDate)
            };
           
            //saad


            return View("Views/EmployeeReport.cshtml", model);
        
        }
    }
}