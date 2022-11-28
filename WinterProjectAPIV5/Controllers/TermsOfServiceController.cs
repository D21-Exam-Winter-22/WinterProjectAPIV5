using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WinterProjectAPIV5.Models;

namespace WinterProjectAPIV5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermsOfServiceController : ControllerBase
    {
        private readonly PaymentApidb2Context context;

        public TermsOfServiceController(PaymentApidb2Context context)
        {
            this.context = context;
        }
    }
}
