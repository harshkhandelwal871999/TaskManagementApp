using System.Threading.Tasks;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Entities;
using TaskManagement.Services;
using TaskManagement.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TaskManagement.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        [TestMethod]
        public async Task CreateAndGetTask()
        {
            var repoMock = new Mock<ITaskRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).ReturnsAsync((TaskItem t) => { t.Id = 1; return t; });
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new TaskItem { Id = 1, Name = "Test" });
            var service = new TaskService(repoMock.Object);

            var created = await service.CreateAsync(new TaskItem { Name = "Test" });
            Assert.AreEqual(1, created.Id);

            var got = await service.GetByIdAsync(1);
            Assert.IsNotNull(got);
            Assert.AreEqual("Test", got!.Name);
        }

        [TestMethod]
        public async Task MoveTask_SetsColumn()
        {
            var task = new TaskItem { Id = 2, Name = "T", Status = StatusEnum.TODO };
            var repoMock = new Mock<ITaskRepository>();
            repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(true);

            var service = new TaskService(repoMock.Object);
            var ok = await service.MoveTaskAsync(2, 3);
            Assert.IsTrue(ok);
            Assert.AreEqual(3, (int)task.Status);
        }
    }
}
