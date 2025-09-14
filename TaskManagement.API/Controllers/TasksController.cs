using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _svc;
        public TasksController(ITaskService svc) { _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var t = await _svc.GetByIdAsync(id);
            if (t == null) return NotFound();
            return Ok(t);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskItem item)
        {
            var created = await _svc.CreateAsync(item);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem item)
        {
            if (id != item.Id) return BadRequest();
            var ok = await _svc.UpdateAsync(item);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _svc.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/move/{columnId}")]
        public async Task<IActionResult> Move(int id, int columnId)
        {
            var ok = await _svc.MoveTaskAsync(id, columnId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // POST: api/TaskImages/{taskId}
        // Add one or multiple images to a task
        [HttpPost("{taskId}")]
        public async Task<IActionResult> AddImages(long taskId, [FromBody] List<string> imagePaths)
        {
            if (imagePaths == null || !imagePaths.Any())
                return BadRequest("No images provided.");

            var success = await _svc.AddImagesAsync(taskId, imagePaths);
            if (!success)
                return NotFound($"Task with ID {taskId} not found.");

            return Ok("Images added successfully.");
        }

        // DELETE: api/TaskImages/{imageId}
        // Remove a single image
        [HttpDelete("remove/{imageId}")]
        public async Task<IActionResult> RemoveImage(long imageId)
        {
            var success = await _svc.RemoveImageAsync(imageId);
            if (!success)
                return NotFound($"Image with ID {imageId} not found.");

            return Ok("Image removed successfully.");
        }
    }
}
