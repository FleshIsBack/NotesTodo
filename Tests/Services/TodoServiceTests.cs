using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotesTodo.DAL;
using NotesTodo.Models;
using NotesTodo.Services;

namespace Tests.Services
{
    [TestClass]
    public sealed class TodoServiceTests
    {
        private TodoService _service = null!;
        private UserSevice _userService = null!;
        private TodoProjectService _projectService = null!;
        private TodoDb _context = null!;
        private IConfiguration _configuration = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TodoDb>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoDb(options);
            _configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
                    { "Jwt:Secret", "this-is-a-test-secret-that-is-long-enough" },
                    { "Jwt:Issuer", "NotesTodo" },
                    { "Jwt:Audience", "NotesTodo" }
    }).Build();

            _userService = new UserSevice(_context, _configuration);
            _projectService = new TodoProjectService(_context);
            _service = new TodoService(_context);
        }

        [TestMethod]
        public async Task CreateTodoAsync_ShouldCreateTodo_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            // Act
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            // Assert
            Assert.IsNotNull(createdTodo);
            Assert.AreEqual(createTodo.Title, createdTodo.Title);
            Assert.AreEqual(createTodo.Description, createdTodo.Description);
            Assert.AreEqual(createTodo.Criticality, createdTodo.Criticality);
            Assert.AreEqual(owner.Id, createdTodo.CreatedByUserId);
        }

        [TestMethod]
        public async Task CreateTodoAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.CreateTodoAsync(createTodo, outsider.Id));
        }

        [TestMethod]
        public async Task CreateTodoAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = 999,
                Criticality = Criticality.Medium
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.CreateTodoAsync(createTodo, owner.Id));
        }

        [TestMethod]
        public async Task CreateTodoAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var nonExistentUserId = 999;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.CreateTodoAsync(createTodo, nonExistentUserId));
        }

        [TestMethod]
        public async Task GetTodoByIdAsync_ShouldReturnTodo_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            // Act
            var retrievedTodo = await _service.GetTodoByIdAsync(createdTodo.Id, owner.Id);
            // Assert
            Assert.IsNotNull(retrievedTodo);
            Assert.AreEqual(createdTodo.Id, retrievedTodo.Id);
            Assert.AreEqual(createdTodo.Title, retrievedTodo.Title);
            Assert.AreEqual(createdTodo.Description, retrievedTodo.Description);
        }

        [TestMethod]
        public async Task GetTodoByIdAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetTodoByIdAsync(createdTodo.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetTodoByIdAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var nonExistentTodoId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetTodoByIdAsync(nonExistentTodoId, userId));
        }

        [TestMethod]
        public async Task GetTodosByProjectIdAsync_ShouldReturnTodos_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo1 = new CreateTodoDTO
            {
                Title = "Test Todo 1",
                Description = "Test Description 1",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createTodo2 = new CreateTodoDTO
            {
                Title = "Test Todo 2",
                Description = "Test Description 2",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Low
            };
            await _service.CreateTodoAsync(createTodo1, owner.Id);
            await _service.CreateTodoAsync(createTodo2, owner.Id);
            // Act
            var todos = await _service.GetTodosByProjectIdAsync(createdProject.Id, owner.Id);
            // Assert
            Assert.IsNotNull(todos);
            Assert.AreEqual(2, todos.Count);
            Assert.IsTrue(todos.Any(t => t.Title == createTodo1.Title && t.Description == createTodo1.Description));
            Assert.IsTrue(todos.Any(t => t.Title == createTodo2.Title && t.Description == createTodo2.Description));
        }

        [TestMethod]
        public async Task GetTodosByProjectIdAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetTodosByProjectIdAsync(createdProject.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetTodosByProjectIdAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var nonExistentProjectId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetTodosByProjectIdAsync(nonExistentProjectId, userId));
        }

        [TestMethod]
        public async Task GetTodosByProjectIdAsync_ShouldReturnEmptyList_WhenProjectHasNoTodos()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            // Act
            var todos = await _service.GetTodosByProjectIdAsync(createdProject.Id, owner.Id);
            // Assert
            Assert.IsNotNull(todos);
            Assert.AreEqual(0, todos.Count);
        }

        [TestMethod]
        public async Task UpdateTodoAsync_ShouldUpdateTodo_WhenUserIsOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            var updateTodo = new UpdateTodoDTO
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                Status = TodoStatus.InProgress,
                DateDue = DateTime.UtcNow.AddDays(14),
                Criticality = Criticality.High
            };
            // Act
            var updatedTodo = await _service.UpdateTodoAsync(createdTodo.Id, updateTodo, owner.Id);
            // Assert
            Assert.IsNotNull(updatedTodo);
            Assert.AreEqual(createdTodo.Id, updatedTodo.Id);
            Assert.AreEqual(updateTodo.Title, updatedTodo.Title);
            Assert.AreEqual(updateTodo.Description, updatedTodo.Description);
            Assert.AreEqual(updateTodo.Status, updatedTodo.Status);
            Assert.AreEqual(updateTodo.Criticality, updatedTodo.Criticality);
        }

        [TestMethod]
        public async Task UpdateTodoAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            var updateTodo = new UpdateTodoDTO
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                Status = TodoStatus.InProgress,
                Criticality = Criticality.High
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.UpdateTodoAsync(createdTodo.Id, updateTodo, outsider.Id));
        }

        [TestMethod]
        public async Task UpdateTodoAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var nonExistentTodoId = 999;
            var userId = 1;
            var updateTodo = new UpdateTodoDTO
            {
                Title = "Updated Todo",
                Description = "Updated Description",
                Status = TodoStatus.InProgress,
                Criticality = Criticality.High
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.UpdateTodoAsync(nonExistentTodoId, updateTodo, userId));
        }

        [TestMethod]
        public async Task DeleteTodoAsync_ShouldDeleteTodo_WhenUserIsOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            // Act
            await _service.DeleteTodoAsync(createdTodo.Id, owner.Id);
            // Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetTodoByIdAsync(createdTodo.Id, owner.Id));
        }

        [TestMethod]
        public async Task DeleteTodoAsync_ShouldThrowException_WhenUserIsNotOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createMember = new CreateUserDTO
            {
                Name = "Member",
                Email = "member@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            await _projectService.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            var createTodo = new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            };
            var createdTodo = await _service.CreateTodoAsync(createTodo, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.DeleteTodoAsync(createdTodo.Id, member.Id));
        }

        [TestMethod]
        public async Task DeleteTodoAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var nonExistentTodoId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.DeleteTodoAsync(nonExistentTodoId, userId));
        }

        [TestMethod]
        public async Task CreateSubTaskAsync_ShouldCreateSubTask_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createSubTask = new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            };
            // Act
            var createdSubTask = await _service.CreateSubTaskAsync(createSubTask, owner.Id);
            // Assert
            Assert.IsNotNull(createdSubTask);
            Assert.AreEqual(createSubTask.Title, createdSubTask.Title);
            Assert.AreEqual(createSubTask.Description, createdSubTask.Description);
            Assert.AreEqual(createSubTask.Criticality, createdSubTask.Criticality);
        }

        [TestMethod]
        public async Task CreateSubTaskAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createSubTask = new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.CreateSubTaskAsync(createSubTask, outsider.Id));
        }

        [TestMethod]
        public async Task CreateSubTaskAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createSubTask = new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = 999,
                Criticality = Criticality.Low
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.CreateSubTaskAsync(createSubTask, owner.Id));
        }

        [TestMethod]
        public async Task GetSubTaskByIdAsync_ShouldReturnSubTask_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            // Act
            var retrievedSubTask = await _service.GetSubTaskByIdAsync(createdSubTask.Id, owner.Id);
            // Assert
            Assert.IsNotNull(retrievedSubTask);
            Assert.AreEqual(createdSubTask.Id, retrievedSubTask.Id);
            Assert.AreEqual(createdSubTask.Title, retrievedSubTask.Title);
            Assert.AreEqual(createdSubTask.Description, retrievedSubTask.Description);
        }

        [TestMethod]
        public async Task GetSubTaskByIdAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetSubTaskByIdAsync(createdSubTask.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetSubTaskByIdAsync_ShouldThrowException_WhenSubTaskDoesNotExist()
        {
            // Arrange
            var nonExistentSubTaskId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetSubTaskByIdAsync(nonExistentSubTaskId, userId));
        }

        [TestMethod]
        public async Task GetSubTasksByTodoIdAsync_ShouldReturnSubTasks_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createSubTask1 = new CreateSubTaskDTO
            {
                Title = "SubTask 1",
                Description = "SubTask Description 1",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            };
            var createSubTask2 = new CreateSubTaskDTO
            {
                Title = "SubTask 2",
                Description = "SubTask Description 2",
                TodoId = createdTodo.Id,
                Criticality = Criticality.High
            };
            await _service.CreateSubTaskAsync(createSubTask1, owner.Id);
            await _service.CreateSubTaskAsync(createSubTask2, owner.Id);
            // Act
            var subTasks = await _service.GetSubTasksByTodoIdAsync(createdTodo.Id, owner.Id);
            // Assert
            Assert.IsNotNull(subTasks);
            Assert.AreEqual(2, subTasks.Count);
            Assert.IsTrue(subTasks.Any(s => s.Title == createSubTask1.Title && s.Description == createSubTask1.Description));
            Assert.IsTrue(subTasks.Any(s => s.Title == createSubTask2.Title && s.Description == createSubTask2.Description));
        }

        [TestMethod]
        public async Task GetSubTasksByTodoIdAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetSubTasksByTodoIdAsync(createdTodo.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetSubTasksByTodoIdAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var nonExistentTodoId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetSubTasksByTodoIdAsync(nonExistentTodoId, userId));
        }

        [TestMethod]
        public async Task GetSubTasksByTodoIdAsync_ShouldReturnEmptyList_WhenTodoHasNoSubTasks()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act
            var subTasks = await _service.GetSubTasksByTodoIdAsync(createdTodo.Id, owner.Id);
            // Assert
            Assert.IsNotNull(subTasks);
            Assert.AreEqual(0, subTasks.Count);
        }

        [TestMethod]
        public async Task UpdateSubTaskAsync_ShouldUpdateSubTask_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            var updateSubTask = new UpdateSubTaskDTO
            {
                Title = "Updated SubTask",
                Description = "Updated SubTask Description",
                Status = TodoStatus.InProgress,
                Criticality = Criticality.High
            };
            // Act
            var updatedSubTask = await _service.UpdateSubTaskAsync(createdSubTask.Id, updateSubTask, owner.Id);
            // Assert
            Assert.IsNotNull(updatedSubTask);
            Assert.AreEqual(createdSubTask.Id, updatedSubTask.Id);
            Assert.AreEqual(updateSubTask.Title, updatedSubTask.Title);
            Assert.AreEqual(updateSubTask.Description, updatedSubTask.Description);
            Assert.AreEqual(updateSubTask.Status, updatedSubTask.Status);
            Assert.AreEqual(updateSubTask.Criticality, updatedSubTask.Criticality);
        }

        [TestMethod]
        public async Task UpdateSubTaskAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            var updateSubTask = new UpdateSubTaskDTO
            {
                Title = "Updated SubTask",
                Description = "Updated SubTask Description",
                Status = TodoStatus.InProgress,
                Criticality = Criticality.High
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.UpdateSubTaskAsync(createdSubTask.Id, updateSubTask, outsider.Id));
        }

        [TestMethod]
        public async Task UpdateSubTaskAsync_ShouldThrowException_WhenSubTaskDoesNotExist()
        {
            // Arrange
            var nonExistentSubTaskId = 999;
            var userId = 1;
            var updateSubTask = new UpdateSubTaskDTO
            {
                Title = "Updated SubTask",
                Description = "Updated SubTask Description",
                Status = TodoStatus.InProgress,
                Criticality = Criticality.High
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.UpdateSubTaskAsync(nonExistentSubTaskId, updateSubTask, userId));
        }
        [TestMethod]
        public async Task DeleteSubTaskAsync_ShouldDeleteSubTask_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            // Act
            await _service.DeleteSubTaskAsync(createdSubTask.Id, owner.Id);
            // Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetSubTaskByIdAsync(createdSubTask.Id, owner.Id));
        }

        [TestMethod]
        public async Task DeleteSubTaskAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var createdSubTask = await _service.CreateSubTaskAsync(new CreateSubTaskDTO
            {
                Title = "Test SubTask",
                Description = "Test SubTask Description",
                TodoId = createdTodo.Id,
                Criticality = Criticality.Low
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.DeleteSubTaskAsync(createdSubTask.Id, outsider.Id));
        }

        [TestMethod]
        public async Task DeleteSubTaskAsync_ShouldThrowException_WhenSubTaskDoesNotExist()
        {
            // Arrange
            var nonExistentSubTaskId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.DeleteSubTaskAsync(nonExistentSubTaskId, userId));
        }
        [TestMethod]
        public async Task AddMemberToTodoAsync_ShouldAddMember_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createMember = new CreateUserDTO
            {
                Name = "Member",
                Email = "member@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            await _projectService.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            // Act
            await _service.AddMemberToTodoAsync(createdTodo.Id, addMemberDto, owner.Id);
            // Assert
            var todo = await _service.GetTodoByIdAsync(createdTodo.Id, owner.Id);
            Assert.IsTrue(todo.AssignedUsers.Any(u => u.Id == member.Id));
        }

        [TestMethod]
        public async Task AddMemberToTodoAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = outsider.Id,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.AddMemberToTodoAsync(createdTodo.Id, addMemberDto, outsider.Id));
        }

        [TestMethod]
        public async Task AddMemberToTodoAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var nonExistentTodoId = 999;
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = owner.Id,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.AddMemberToTodoAsync(nonExistentTodoId, addMemberDto, owner.Id));
        }

        [TestMethod]
        public async Task AddMemberToTodoAsync_ShouldThrowException_WhenUserIsAlreadyAssigned()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createMember = new CreateUserDTO
            {
                Name = "Member",
                Email = "member@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            await _projectService.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            await _service.AddMemberToTodoAsync(createdTodo.Id, addMemberDto, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddMemberToTodoAsync(createdTodo.Id, addMemberDto, owner.Id));
        }
        [TestMethod]
        public async Task RemoveMemberFromTodoAsync_ShouldRemoveMember_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createMember = new CreateUserDTO
            {
                Name = "Member",
                Email = "member@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            await _projectService.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            await _service.AddMemberToTodoAsync(createdTodo.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            // Act
            await _service.RemoveMemberFromTodoAsync(createdTodo.Id, member.Id, owner.Id);
            // Assert
            var todo = await _service.GetTodoByIdAsync(createdTodo.Id, owner.Id);
            Assert.IsFalse(todo.AssignedUsers.Any(u => u.Id == member.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromTodoAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.RemoveMemberFromTodoAsync(createdTodo.Id, outsider.Id, outsider.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromTodoAsync_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var nonExistentTodoId = 999;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.RemoveMemberFromTodoAsync(nonExistentTodoId, owner.Id, owner.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromTodoAsync_ShouldThrowException_WhenTargetUserIsNotAssigned()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.RemoveMemberFromTodoAsync(createdTodo.Id, outsider.Id, owner.Id));
        }
        [TestMethod]
        public async Task GetAssignedMembers_ShouldReturnMembers_WhenUserIsMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createMember = new CreateUserDTO
            {
                Name = "Member",
                Email = "member@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            await _projectService.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            await _service.AddMemberToTodoAsync(createdTodo.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);
            // Act
            var assignedMembers = await _service.GetAssingedMembers(createdTodo.Id, owner.Id);
            // Assert
            Assert.IsNotNull(assignedMembers);
            Assert.IsTrue(assignedMembers.Any(m => m.User.Id == member.Id));
        }

        [TestMethod]
        public async Task GetAssignedMembers_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createOutsider = new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var outsider = await _userService.RegisterUserAsync(createOutsider);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetAssingedMembers(createdTodo.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetAssignedMembers_ShouldThrowException_WhenTodoDoesNotExist()
        {
            // Arrange
            var nonExistentTodoId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetAssingedMembers(nonExistentTodoId, userId));
        }

        [TestMethod]
        public async Task GetAssignedMembers_ShouldReturnEmptyList_WhenTodoHasNoAssignedMembers()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _projectService.CreateProjectAsync(createProject, owner.Id);
            var createdTodo = await _service.CreateTodoAsync(new CreateTodoDTO
            {
                Title = "Test Todo",
                Description = "Test Description",
                DateDue = DateTime.UtcNow.AddDays(7),
                TodoProjectId = createdProject.Id,
                Criticality = Criticality.Medium
            }, owner.Id);
            // Act
            var assignedMembers = await _service.GetAssingedMembers(createdTodo.Id, owner.Id);
            // Assert
            Assert.IsNotNull(assignedMembers);
            Assert.AreEqual(0, assignedMembers.Count);
        }
    }
}