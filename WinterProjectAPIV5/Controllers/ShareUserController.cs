using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterProjectAPIV5.DataTransferObjects;
using WinterProjectAPIV5.Functions;
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
                return Ok(string.Format("User with username: {0}, already exists", request.UserName));
            }
            
            //Check if the user with that email exists in the system and is blacklisted
            //Get the account
            List<ShareUser> ListOfUsers = await context.ShareUsers.Where(user => user.Email == request.Email).ToListAsync();

            foreach (ShareUser user in ListOfUsers)
            {
                if ((bool)user.IsBlacklisted)
                {
                    return Ok(string.Format("{0} has been blacklisted", request.Email));
                }
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
                IsAdmin = request.IsAdmin,
                Address = request.Address,
                QuestionId = request.QuestionID,
                SecurityAnswer = request.SecurityAnswer,
                IsDisabled = false,
                IsBlacklisted = false
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
                RecordToChange.Address = request.Address;
                RecordToChange.QuestionId = request.QuestionId;
                RecordToChange.SecurityAnswer = request.SecurityAnswer;
                RecordToChange.IsDisabled = request.IsDisabled;
                RecordToChange.IsBlacklisted = request.IsBlacklisted;
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
                Password = SingleUser.Password,
                Address = SingleUser.Address,
                QuestionID = SingleUser.QuestionId,
                SecurityAnswer = SingleUser.SecurityAnswer,
                IsDisabled = SingleUser.IsDisabled,
                IsBlacklisted = SingleUser.IsBlacklisted
            };

            return Ok(User);
        }

        [HttpGet("SearchShareUsers/{SearchString}")]
        public async Task<ActionResult<List<ShareUser>>> SearchForUsers(string SearchString)
        {
            List<ShareUser> SearchedUsers = await context.ShareUsers.Where(user => user.UserName.Contains(SearchString) || user.FirstName.Contains(SearchString) || user.LastName.Contains(SearchString)).ToListAsync();
            return Ok(SearchedUsers);
        }

        [HttpGet("GetAllUsersGroups/{UserID}")]
        public async Task<ActionResult<List<ShareGroup>>> GetAllUsersGroups(int UserID)
        {
            List<UserGroup> ListOfUsersGroups = await context.UserGroups
                    .Include(entry => entry.Group)
                    .Where(usergroup => usergroup.UserId == UserID).ToListAsync();

            return Ok(ListOfUsersGroups);
        }

        [HttpPut("DisableAccountOnID/{UserID}")]
        public async Task<ActionResult<string>> DisableAccountOnID(int UserID)
        {
            //Get the Account
            ShareUser account = await context.ShareUsers.FindAsync(UserID);

            if (account == null)
            {
                return Ok(string.Format("Account with ID: {0} not found", UserID));
            }
            
            //Edit the account
            account.IsDisabled = true;
            await context.SaveChangesAsync();
            
            return Ok("Account Disabled");
        }

        [HttpPut("BlacklistUserOnID/{UserID}")]
        public async Task<ActionResult<string>> BlacklistUserOnID(int UserID)
        {
            //get the account
            ShareUser account = await context.ShareUsers.FindAsync(UserID);

            if (account == null)
            {
                return Ok(string.Format("Account with ID: {0}", UserID));
            }
            
            //Edit the account

            account.IsBlacklisted = true;
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("ResetAccountPassword/{UserID}")]
        public async Task<ActionResult<string>> ResetAccountPassword(int UserID)
        {
            //Identify the account
            ShareUser TheAccount = await context.ShareUsers.FindAsync(UserID);

            if (TheAccount == null)
            {
                return NotFound(string.Format("Account with the UserID: {0} does not exist", UserID));
            }
            
            //Change the password
            string password = GenerateRandomString.CreateString(15);
            
            //Save it in the DB
            TheAccount.Password = password;
            await context.SaveChangesAsync();

            return Ok(password);

        }
        
        
        


    }
}
