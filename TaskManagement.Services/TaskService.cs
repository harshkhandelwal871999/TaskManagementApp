using Common;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repo;
        public TaskService(ITaskRepository repo) { _repo = repo; }

        public async Task<bool> AddImagesAsync(long taskId, List<string> imagePaths) => await _repo.AddImagesAsync(taskId, imagePaths);
        public async Task<bool> RemoveImageAsync(long imageId) => await _repo.RemoveImageAsync(imageId);

        public async Task<TaskItem> CreateAsync(TaskItem item) => await _repo.AddAsync(item);

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<IEnumerable<TaskItem>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<TaskItem?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<bool> MoveTaskAsync(int taskId, int status)
        {
            var t = await _repo.GetByIdAsync(taskId);
            if (t == null) return false;
            t.Status = (StatusEnum)status;
            return await _repo.UpdateAsync(t);
        }

        public async Task<bool> UpdateAsync(TaskItem item) => await _repo.UpdateAsync(item);
    }
}
