﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Database;
using TaskManager.Database.Models;
using TaskManager.Schemas;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManager.Controllers
{
    [SwaggerTag("team")]
    [Route("api/team/")]
    [ApiController]
    public class TeamControllers : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public TeamControllers(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "get-teams-all")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetTeamsAll([FromQuery] GetTeamsSchema model)
        {
            List<Team> teams;
            if (model.UserId == null)
            {
                teams = await _context.Teams.ToListAsync();
            } else
            {
                // TODO: make users specific get teams 
                teams = await _context.Teams.ToListAsync();
            }

            return Ok(new JsonResult(teams));  
        }

        [HttpPost(Name = "create-team")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamSchema model)
        {
            var ownerUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (ownerUser == null)
            {
                return NotFound(new JsonResult("Not found") { StatusCode = 400 });
            }
            Team team = new Team()
            {
                Name = model.Name,
                Groups = new List<Group>(), 
                CreatedBy = ownerUser,
            };

            Group defaultGroup = new Group()
            {
                Owner = ownerUser, 
                Role = GroupRoles.employee,
                Users = new List<UserModel>()
            };

            Group ownerGroup = new Group()
            {
                Role = GroupRoles.employee,
                Owner = ownerUser,
                Users = new List<UserModel>()
            };
            foreach (var userId in model.UserIds)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    defaultGroup.Users.Add(user);
                }
            }
            await _context.Groups.AddAsync(defaultGroup);
            await _context.Groups.AddAsync(ownerGroup);

            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            team.Groups.Add(defaultGroup);
            team.Groups.Add(ownerGroup);

            _context.Teams.Update(team);
            await _context.SaveChangesAsync(); 

            return Ok(team); 
        }

        [HttpGet("{id}", Name = "get-team")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetTeam(Guid id)
        {
            var team = await _context.Groups.FirstOrDefaultAsync(x => x.Id == id); 
            return Ok(new JsonResult(team));
        }
    }
}
