using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kata.ToDo.WebApi.Data;
using Kata.ToDo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kata.ToDo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ToDoController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoModel>>> Get()
        {
            var notes = await _dbContext
                .ToDos
                .AsNoTracking()
                .ToListAsync();

            return Ok(notes.Select(Map));
        }


        [HttpPost]
        public async Task<ActionResult<ToDoModel>> Post([FromBody]ToDoRequestModel request)
        {
            var newNoteResult = await _dbContext
                .ToDos
                .AddAsync(new Data.Entities.ToDo
                {
                    Text = request.Text
                });

            await _dbContext.SaveChangesAsync();

            return Ok(Map(newNoteResult.Entity));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDoModel>> Put([FromRoute]Guid id, [FromBody]ToDoRequestModel request)
        {
            var existingNote = await _dbContext
                .ToDos
                .FindAsync(id);

            if (existingNote == null)
            {
                return NotFound($"Note {id} does not exist");
            }
            
            existingNote.Text = request.Text;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var note = await _dbContext.ToDos.FindAsync(id);
            if (note == null)
            {
                return NotFound($"Note {id} does not exist");
            }
            _dbContext.ToDos.Remove(note);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static ToDoModel Map(Data.Entities.ToDo todo)
        {
            return new ToDoModel()
            {
                Id = todo.Id,
                Text = todo.Text
            };
        }
    }
}
