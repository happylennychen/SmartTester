using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTesterLib;
using SmartTesterLib.DataAccess;

namespace SmartTesterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutomatorController : ControllerBase
    {
        private readonly Automator _automator; 
        private readonly ChamberRepository _chamberRepository;
        private readonly TesterRepository _testerRepository;
        public AutomatorController(Automator automator, ChamberRepository chamberRepository, TesterRepository testerRepository)
        {
            _automator = automator;
            _chamberRepository = chamberRepository;
            _testerRepository = testerRepository;
        }    
        
        // POST api/automator/start
        [HttpPost("start")]
        public IActionResult StartChamber([FromBody] int chamberID)
        {
            IChamber chamber = _chamberRepository.GetChamberById(1);
            chamber.Assamble();
            ITester tester = _testerRepository.GetTesterById(1);
            tester.Assamble();
            List<IChamber> chambers = new List<IChamber> { chamber};
            Task task = _automator.AsyncStartChambers(chambers);

            return Ok("Chamber started successfully.");
        }
    }
}
