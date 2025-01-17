﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vigen_Repository.Models;
using Microsoft.EntityFrameworkCore;
using Vigen_Repository.Email;

namespace Vigen_Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly vigendbContext _context;
        public UserController(vigendbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<User>>> getUsers()
        {
            List<User> users = await _context.Users.ToListAsync();
            if (users.Count == 0) return NoContent();
            return Ok(users);
        }

        [HttpGet("{user}/{password}")]
        public async Task<ActionResult<Object>> loginUser(string user, string password)
        {
            User? userObject = await _context.Users.FindAsync(user);
            if(userObject?.Password != password)
            {
                return null;
            }
            return new {name=userObject.Name };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> getUser(string id)
        {
            User? user = await _context.Users.FindAsync(id);
            if(user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> postUser(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                Send send = new Send();
                var res = send.enviar(user.Email, user.Code);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("succesfull");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(string id, User user)
        {
            if(id!=user.Identification) return BadRequest("El id no concide");
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(user);
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            try
            {
                User? user = await _context.Users.FindAsync(id);
                if(user==null) return NotFound();
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}
