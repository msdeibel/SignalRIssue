using BackendService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterValuesController : ControllerBase
    {
        private IHubContext<ValueHub.ValueHub> _valuesHub;
        private readonly ISendValuesService _sendValueService;

        public RegisterValuesController(IHubContext<ValueHub.ValueHub> hubContext, ISendValuesService sendValuesService)
        {
            _valuesHub = hubContext;
            _sendValueService = sendValuesService;
        }

        // GET: api/LiveVariableValues
        public void LiveVariableValues()
        {
            _sendValueService.LiveVariableValues();
        }
    } 
}
