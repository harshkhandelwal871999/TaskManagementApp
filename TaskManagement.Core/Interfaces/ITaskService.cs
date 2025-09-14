using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> CreateAsync(TaskItem item);
        Task<bool> UpdateAsync(TaskItem item);
        Task<bool> DeleteAsync(int id);
        Task<bool> MoveTaskAsync(int taskId, int toColumnId);
        Task<bool> AddImagesAsync(long taskId, List<string> imagePaths);
        Task<bool> RemoveImageAsync(long imageId);
    }
}
