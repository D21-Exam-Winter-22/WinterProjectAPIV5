using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WinterProjectAPIV5.Models;

namespace WinterProjectAPIV5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityQuestionController : ControllerBase
    {
        private readonly PaymentApidb2Context context;

        public SecurityQuestionController(PaymentApidb2Context context)
        {
            this.context = context;
        }
    }
}
