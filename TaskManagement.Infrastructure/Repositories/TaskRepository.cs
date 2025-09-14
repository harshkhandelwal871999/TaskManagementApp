using Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;
        public TaskRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            var tasks = new Dictionary<long, TaskItem>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT t.Id AS TaskId, t.Name, t.Description, t.Deadline, t.IsFavorite, t.Status, t.CreatedAt AS TaskCreatedAt, t.UpdatedAt,
                           i.Id AS ImageId, i.ImagePath, i.CreatedAt AS ImageCreatedAt
                    FROM Tasks t
                    LEFT JOIN TaskImages i ON t.Id = i.TaskId
                    WHERE t.isdeleted=0
                    ORDER BY t.IsFavorite DESC, i.Id ASC";

                using var cmd = new SqlCommand(query, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    long taskId = (long)reader["TaskId"];

                    if (!tasks.TryGetValue(taskId, out var task))
                    {
                        task = new TaskItem
                        {
                            Id = taskId,
                            Name = (string)reader["Name"],
                            Description = reader["Description"] as string,
                            Deadline = reader["Deadline"] as DateTime?,
                            IsFavorite = (bool)reader["IsFavorite"],
                            Status = (StatusEnum)(byte)reader["Status"],
                            CreatedAt = (DateTime)reader["TaskCreatedAt"],
                            UpdatedAt = (DateTime)(reader["UpdatedAt"])
                        };
                        tasks[taskId] = task;
                    }

                    if (reader["ImageId"] != DBNull.Value)
                    {
                        task.Images.Add(new TaskImages
                        {
                            Id = (long)reader["ImageId"],
                            TaskId = taskId,
                            ImagePath = (string)reader["ImagePath"],
                            CreatedAt = (DateTime)reader["ImageCreatedAt"]
                        });
                    }
                }
            }
            return tasks.Values.ToList();
        }
    
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            TaskItem? task = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    SELECT t.Id AS TaskId, t.Name, t.Description, t.Deadline, t.IsFavorite, t.Status, t.CreatedAt AS TaskCreatedAt, t.UpdatedAt,
                           i.Id AS ImageId, i.ImagePath, i.CreatedAt AS ImageCreatedAt
                    FROM Tasks t
                    LEFT JOIN TaskImages i ON t.Id = i.TaskId
                    WHERE t.Id = @TaskId and t.Isdeleted=0";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TaskId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (task == null)
                            {
                                task = new TaskItem
                                {
                                    Id = (long)reader["TaskId"],
                                    Name = (string)reader["Name"],
                                    Description = reader["Description"] as string,
                                    Deadline = reader["Deadline"] as DateTime?,
                                    IsFavorite = (bool)reader["IsFavorite"],
                                    Status = (StatusEnum)(byte)reader["Status"],
                                    CreatedAt = (DateTime)reader["TaskCreatedAt"],
                                    UpdatedAt = (DateTime)(reader["UpdatedAt"])
                                };
                            }

                            if (reader["ImageId"] != DBNull.Value)
                            {
                                task.Images.Add(new TaskImages
                                {
                                    Id = (long)reader["ImageId"],
                                    TaskId = task.Id,
                                    ImagePath = (string)reader["ImagePath"],
                                    CreatedAt = (DateTime)reader["ImageCreatedAt"]
                                });
                            }
                        }
                    }
                }
            }

            return task;
        }

        public async Task<TaskItem> AddAsync(TaskItem item)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Tasks (Name, Description, Deadline, IsFavorite,Status, IsDeleted,  CreatedAt, UpdatedAt) " +
                "OUTPUT INSERTED.Id VALUES (@Name,@Description,@Deadline,@IsFavorite,@Status,0, @CreatedAt, @updatedAt)", conn);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Deadline", (object?)item.Deadline ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFavorite", item.IsFavorite);
            cmd.Parameters.AddWithValue("@Status", StatusEnum.TODO);
            cmd.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);
            await conn.OpenAsync();
            item.Id = (long)await cmd.ExecuteScalarAsync();
            return item;
        }

        public async Task<bool> UpdateAsync(TaskItem item)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "UPDATE Tasks SET Name=@Name, Description=@Description, Deadline=@Deadline, Status= @Status, IsFavorite=@IsFavorite WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", item.Id);
            cmd.Parameters.AddWithValue("@Name", item.Name);
            cmd.Parameters.AddWithValue("@Description", (object?)item.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Deadline", (object?)item.Deadline ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFavorite", item.IsFavorite);
            cmd.Parameters.AddWithValue("@Status", item.Status);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("UPDATE Tasks SET IsDeleted = 1 WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AddImagesAsync(long taskId, List<string> imagePaths)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // Check if task exists
            using var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Tasks WHERE Id = @TaskId", conn);
            checkCmd.Parameters.AddWithValue("@TaskId", taskId);
            var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;
            if (!exists) return false;

            foreach (var path in imagePaths)
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO TaskImages (TaskId, ImagePath) VALUES (@TaskId, @ImagePath)", conn);
                cmd.Parameters.AddWithValue("@TaskId", taskId);
                cmd.Parameters.AddWithValue("@ImagePath", path);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        // Remove an image by ID
        public async Task<bool> RemoveImageAsync(long imageId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("DELETE FROM TaskImages WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", imageId);
            int affected = await cmd.ExecuteNonQueryAsync();

            return affected > 0;
        }
    }
}
