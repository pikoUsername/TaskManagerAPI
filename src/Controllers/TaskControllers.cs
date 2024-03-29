﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using TaskManager.Database;
using TaskManager.Database.Models;
using TaskManager.Schemas;

namespace TaskManager.Controllers
{
    [SwaggerTag("tasks")]
    [Route("api/task/")]
    [ApiController]
    public class TaskControllers : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public TaskControllers(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpGet("{id}", Name = "get-task")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<TaskModel>> GetTask(Guid id)
        {
            var task = await _context.Tasks
                .Include(x => x.Tags)
                .Include(x => x.Project)
                .Include(x => x.AssignedUser)
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                return NotFound(new JsonResult("Не найдено") { StatusCode = 401 });

            return Ok(task);
        }

        [HttpGet(Name = "get-tasks")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<List<TaskModel>>> GetTasks([FromQuery] GetTasksScheme model)
        {
            List<TaskModel> tasks;
            var baseRequest = _context.Tasks
                .Include(x => x.Tags)
                .Include(x => x.Project)
                .Include(x => x.AssignedUser)
                .Include(x => x.CreatedBy);
            if (model.UserTasks)
            {
                var email = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                tasks = await baseRequest
                    .Where(x => x.AssignedUser.Id == user.Id || x.CreatedBy.Id == user.Id)
                    .ToListAsync();
            }
            else 
            {
                tasks = await baseRequest
                    .Where(
                        x => x.Project.Id == model.ProjectId
                    )
                    .ToListAsync();
            }
            return Ok(tasks);
        }

        [HttpPost(Name = "create-task")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<TaskModel>> CreateTask([FromBody] CreateTaskSchema model)
        {   
            var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == model.ProjectId);
            if (project == null)
                return NotFound(new JsonResult("Проект на найден") { StatusCode = 401 });
            var createdBy = await _context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
            if (createdBy == null)
                throw new Exception("Что то пошло очень не так, авториазация сломалась"); 

            var task = new TaskModel()
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                StartedAt = model.StartsAt != null ? (DateTime)model.StartsAt : DateTime.UtcNow,
                EndsAt = model.EndsAt != null ? (DateTime)model.EndsAt : DateTime.UtcNow.AddDays(7),
                Project = project, 
                CreatedBy = createdBy,
                Tags = new List<TaskTag>()
            }; 
            if (model.AssignToUserId != null)
            {
                var assignUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.AssignToUserId);
                if (assignUser == null)
                    return NotFound(new JsonResult("Пользватель на найден") { StatusCode = 401 });
                task.AssignedUser = assignUser;
            }

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return Ok(task); 
        }

        [HttpPost("{id}", Name = "assign-user-to-task")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<TaskModel>> AssignUserToTask(Guid id, [FromBody] AssignUserScheme model)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return NotFound(new JsonResult("Задача не найдена") { StatusCode = 404 });
            }

            var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (selectedUser == null)
            {
                return NotFound(new JsonResult("Пользователь не найден") { StatusCode = 404 });
            }
            task.AssignedUser = selectedUser;
            _context.Update(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpPatch("{id}", Name = "update-task")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<TaskModel>> UpdateTask(Guid id, [FromBody] UpdateTaskScheme model)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return NotFound(new JsonResult("Задача не найдена") { StatusCode = 404 });
            }

            if (!string.IsNullOrEmpty(model.Title))
            {
                task.Title = model.Title;
            }
            if (!string.IsNullOrEmpty(model.Description))
            {
                task.Description = model.Description;
            }
            if (!string.IsNullOrEmpty(model.TaskStatus))
            {
                task.Status = model.TaskStatus;
            }

            _context.Update(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}", Name = "delete-task")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<TaskModel>> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
            {
                return NotFound(new JsonResult("Задача не найдена") { StatusCode = 404 });
            }
            _context.Remove(task);
            await _context.SaveChangesAsync();  

            return Ok(task);
        }
    }
}
