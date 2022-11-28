using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using WinterProjectAPIV5.DataTransferObjects;
using WinterProjectAPIV5.Models;


namespace WinterProjectAPIV5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly PaymentApidb2Context context;
        private static Dictionary<int, string> TokenDictionary = new Dictionary<int, string>();

        public MainController(PaymentApidb2Context context)
        {
            this.context = context;
        }

        [HttpGet("IsOnline")]
        public bool ApplicationIsOnline()
        {
            return true;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> LoginForToken(GetEncodingDto request)
        {
            string toEncode = request.Username + request.Password;
            string EncodedValue = Functions.Base64.Encode(toEncode);

            List<ShareUser> UsersList = await context.ShareUsers.Where(user => user.UserName == request.Username && user.Password == request.Password).ToListAsync();
            if (UsersList.Count == 0)
            {
                return NotFound("Invalid user details");
            }
            ShareUser user = UsersList.First();

            //Check if the key is already present in the dictionary
            if (TokenDictionary.ContainsKey(user.UserId))
            {
                return Ok("Already logged in");
            }

            TokenDictionary.Add(user.UserId, EncodedValue);

            return Ok(EncodedValue);
        }

        [HttpGet("GetTokenOnUserID/{UserID}")]
        public async Task<ActionResult<string>> GetTokenOnUserID(int UserID)
        {
            if (TokenDictionary.ContainsKey(UserID))
            {
                return Ok(TokenDictionary[UserID]);
            }
            else
            {
                return NotFound("Not Present in the Dictionary");
            }
        }

        [HttpGet("TestGetPDFFile")]
        public async Task<ActionResult<Byte[]>> TestGetPDFFile()
        {
            string FilePath = "C:\\Users\\allan\\OneDrive\\Desktop\\Allan's Resume.pdf";
            byte[] PDFBytes = System.IO.File.ReadAllBytes(FilePath);
            return PDFBytes;
        }
    }
}
