using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.dal.dtos;
using movielogger.services.interfaces;

namespace movielogger.api.controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var serviceResponse = await _usersService.GetAllUsersAsync();
            var mappedResponse = _mapper.Map<IList<UserDto>>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpGet("{userId}")]
        public async Task <IActionResult> GetUserById(int userId)
        {
            var serviceResponse = await _usersService.GetUserByIdAsync(userId);
            var mappedResponse = _mapper.Map<UserDto>(serviceResponse);
            
            return Ok(mappedResponse);
        }

        [HttpPost]
        public IActionResult RegisterUser([FromBody] RegisterUserRequest request)
        {
            return Ok();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            return Ok();
        }
    }
}
