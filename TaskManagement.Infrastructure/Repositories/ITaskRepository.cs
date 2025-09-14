using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> AddAsync(TaskItem item);
        Task<bool> UpdateAsync(TaskItem item);
        Task<bool> DeleteAsync(int id);
        Task<bool> AddImagesAsync(long taskId, List<string> imagePaths);
        Task<bool> RemoveImageAsync(long imageId);
    }
}
