using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movielogger.api.models;
using movielogger.api.models.requests.users;
using movielogger.api.validators;
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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var validator = new CreateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<UserDto>(request);
            var serviceResponse = await _usersService.CreateUserAsync(mappedRequest);
            
            return Ok();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            var validator = new UpdateUserRequestValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            var mappedRequest = _mapper.Map<UserDto>(request);
            var serviceResponse = await _usersService.UpdateUserAsync(userId, mappedRequest);
            
            return Ok();
        }
    }
}
