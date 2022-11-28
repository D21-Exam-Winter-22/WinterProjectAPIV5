using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterProjectAPIV5.DataTransferObjects;
using WinterProjectAPIV5.Models;

namespace WinterProjectAPIV5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareUserController : ControllerBase
    {
        private readonly PaymentApidb2Context context;

        public ShareUserController(PaymentApidb2Context context)
        {
            this.context = context;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<ShareUser>>> GetUsers()
        {
            var users = await context.ShareUsers.ToListAsync();
            if (users.Count == 0)
            {
                return NotFound(users);
            }
            return Ok(users);
        }

        [HttpGet("GetUserByID/{ID}")]
        public async Task<ActionResult<ShareUser>> GetUserOnID(int ID)
        {
            ShareUser SearchedUser = context.ShareUsers.Find(ID);
            if (SearchedUser == null)
            {
                return NotFound(SearchedUser);
            }
            return Ok(SearchedUser);
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<List<ShareUser>>> CreateShareUser(CreateShareUserDto request)
        {

            //Check if the username already exists in the DB
            List<ShareUser> ExistingUsers = await context.ShareUsers.Where(User => User.UserName == request.UserName).ToListAsync();
            if (ExistingUsers.Count > 0)
            {
                return Conflict();
            }

            //Create the user to insert
            ShareUser UserToInsert = new ShareUser
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                IsAdmin = request.IsAdmin
            };

            //Insert the user
            await context.ShareUsers.AddAsync(UserToInsert);
            await context.SaveChangesAsync();

            return await GetUsers();
        }

        //Update details of a user
        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ShareUser>> UpdateUserDetails(ShareUser request)
        {
            ShareUser RecordToChange = context.ShareUsers.Find(request.UserId);
            if (RecordToChange != null)
            {
                RecordToChange.UserName = request.UserName;
                RecordToChange.PhoneNumber = request.PhoneNumber;
                RecordToChange.FirstName = request.FirstName;
                RecordToChange.LastName = request.LastName;
                RecordToChange.Email = request.Email;
                RecordToChange.Password = request.Password;
            }
            else
            {
                return NotFound(RecordToChange);
            }
            await context.SaveChangesAsync();
            return Ok(RecordToChange);
        }

        [HttpGet("GetDetailsOnUsername/{UserName}")]
        public async Task<ActionResult<CreateShareUserDto>> GetLogInDetails(string UserName)
        {
            List<ShareUser> UsersList = await context.ShareUsers.Where(User => User.UserName == UserName).ToListAsync();

            ShareUser SingleUser = UsersList.First();
            CreateShareUserDto User = null;

            if (UsersList.Count == 0)
            {
                return NotFound(User);
            }

            User = new CreateShareUserDto
            {
                UserName = SingleUser.UserName,
                PhoneNumber = SingleUser.PhoneNumber,
                FirstName = SingleUser.FirstName,
                LastName = SingleUser.LastName,
                Email = SingleUser.Email,
                IsAdmin = SingleUser.IsAdmin,
                Password = SingleUser.Password
            };

            return Ok(User);
        }

        [HttpGet("SearchShareUsers/{SearchString}")]
        public async Task<ActionResult<List<ShareUser>>> SearchForUsers(string SearchString)
        {
            List<ShareUser> SearchedUsers = await context.ShareUsers.Where(user => user.UserName.Contains(SearchString) || user.FirstName.Contains(SearchString) || user.LastName.Contains(SearchString)).ToListAsync();
            return Ok(SearchedUsers);
        }


    }
}
