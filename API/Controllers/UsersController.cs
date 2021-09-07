using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entitites;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetUsers()
        {
            var users = await _usersRepository.GetUsersDtoAsync();
            return Ok(users);
        }


        [HttpGet("{username}")]
        public async Task<ActionResult<AppUserDto>> GetUser(string username)
        {
            return await _usersRepository.GetUserDtoByUsernameAsync(username);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(AppUserUpdateDto appUserUpdateDto)
        {

            // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _usersRepository.GetUserByUsernameAsync(appUserUpdateDto.userName);

            _mapper.Map(appUserUpdateDto, user);
            _usersRepository.Update(user);

            if (await _usersRepository.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Failed to update user");
            }
        }

        [HttpPost("password")]
        public async Task<ActionResult> UpdateUserPassword(LoginDto loginDto)
        {

            var user = await _usersRepository.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                return BadRequest("Inavlid username");
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return BadRequest("Inavlid password");
                }
            }

            using var hmacNew = new HMACSHA512();

            user.PasswordHash = hmacNew.ComputeHash(Encoding.UTF8.GetBytes(loginDto.NewPassword));
            user.PasswordSalt = hmacNew.Key;

            _usersRepository.Update(user);

            if (await _usersRepository.SaveAllAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Failed to update user");
            }
        }

    }
}