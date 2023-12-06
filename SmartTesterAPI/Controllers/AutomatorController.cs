using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTesterLib;

namespace SmartTesterAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutomatorController : ControllerBase
    {
        private readonly Automator _automator; 
        public AutomatorController(Automator automator)
        {
            _automator = automator;
        }    
        
        // POST api/automator/start
        [HttpPost("start")]
        public IActionResult StartChamber([FromBody] int chamberID)
        {
            IChamber chamber = new PUL80Chamber
            {
                Name = chamberID.ToString()
            };
            List<IChamber> chambers = new List<IChamber> { chamber};
            Task task = _automator.AsyncStartChambers(chambers);

            return Ok("Chamber started successfully.");
        }
    }
}
