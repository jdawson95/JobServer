using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using WorkerService.Jobs;

namespace JobServer.Controllers
{
    [Route("/api/testjob")]
    public class TestJobController : Controller
    {
        private ITestJob TestJob;
        public TestJobController(ITestJob TestJob)
        {
            this.TestJob = TestJob;
        }

        [HttpGet]
        public IActionResult Index()
        {
            BackgroundJob.Enqueue(() => this.TestJob.RunTestJob());


            return Ok();
        }
    }
}