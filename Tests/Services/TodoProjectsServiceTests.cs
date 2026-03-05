using NotesTodo.Models;
using NotesTodo.Services;
using NotesTodo.Services.Interface;

namespace Tests
{
    [TestClass]
    public sealed class TodoProjectsServiceTests
    {
        private TodoProjectService _service = null!;
        private UserSevice _userService = null!;
        [TestInitialize]
        public void setup()
        {
            _service = new TodoProjectService();
            _userService = new UserSevice();

        }

        [TestMethod]
        public async Task CreateProjectAsync_ShouldCreateProject()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "owner"
            };
            var user = await _userService.RegisterUserAsync(createUser);

            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            // Act
            var createdProject = await _service.CreateProjectAsync(createProject, user.Id);
            // Assert
            Assert.IsNotNull(createdProject);
            Assert.AreEqual(createProject.Name, createdProject.Name);
            Assert.AreEqual(createProject.Description, createdProject.Description);
        }

        [TestMethod]
        public async Task CreateProjectAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 1;
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.CreateProjectAsync(createProject, nonExistentUserId);
            });
        }
        [TestMethod]
        public async Task CreateProjectAsync_ShouldReturnProjectResponse_WhenCreated()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Test User",
                Email = "owner@hello.com"
            };
            var user = await _userService.RegisterUserAsync(createUser);
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            // Act
            var createdProject = await _service.CreateProjectAsync(createProject, user.Id);
            // Assert
            Assert.IsNotNull(createdProject);
            Assert.IsInstanceOfType(createdProject, typeof(TodoProjectResponseDTO));
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenUserIsOwner()
        {
            // Arrange
            var owner = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@world.com"
            });
            var createdProject = await _service.CreateProjectAsync(new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            }, owner.Id);

            // Act
            var retrievedProject = await _service.GetProjectByIdAsync(createdProject.Id, owner.Id);

            // Assert
            Assert.IsNotNull(retrievedProject);
            Assert.AreEqual(createdProject.Id, retrievedProject.Id);
            Assert.AreEqual(createdProject.Name, retrievedProject.Name);
            Assert.AreEqual(createdProject.Description, retrievedProject.Description);
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenUserIsAdmin()
        {
            // Arrange
            var owner = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@world.com"
            });
            var admin = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@world.com"
            });
            var createdProject = await _service.CreateProjectAsync(new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            }, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            }, owner.Id);

            // Act
            var retrievedProject = await _service.GetProjectByIdAsync(createdProject.Id, admin.Id);

            // Assert
            Assert.IsNotNull(retrievedProject);
            Assert.AreEqual(createdProject.Id, retrievedProject.Id);
            Assert.AreEqual(createdProject.Name, retrievedProject.Name);
            Assert.AreEqual(createdProject.Description, retrievedProject.Description);
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenUserIsMember()
        {
            // Arrange
            var owner = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@world.com"
            });
            var member = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Member",
                Email = "member@world.com"
            });
            var createdProject = await _service.CreateProjectAsync(new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            }, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            }, owner.Id);

            // Act
            var retrievedProject = await _service.GetProjectByIdAsync(createdProject.Id, member.Id);

            // Assert
            Assert.IsNotNull(retrievedProject);
            Assert.AreEqual(createdProject.Id, retrievedProject.Id);
            Assert.AreEqual(createdProject.Name, retrievedProject.Name);
            Assert.AreEqual(createdProject.Description, retrievedProject.Description);
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ShouldThrowException_WhenUserIsNotMember()
        {
            // Arrange
            var owner = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@world.com"
            });
            var outsider = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "Outsider",
                Email = "outsider@world.com"
            });
            var createdProject = await _service.CreateProjectAsync(new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            }, owner.Id);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetProjectByIdAsync(createdProject.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetProjectByIdAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var nonExistentProjectId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.GetProjectByIdAsync(nonExistentProjectId, userId);
            });
        }
        [TestMethod]
        public async Task GetUserProjectsAsync_ShouldReturnProjectsForUser()
        {
            // Arrange
            var user1 = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "User1",
                Email = "user1@hello.com"
            });
            var user2 = await _userService.RegisterUserAsync(new CreateUserDTO
            {
                Name = "User2",
                Email = "user2@hello.com"
            });
            var createProject1 = new CreateTodoProjectDTO
            {
                Name = "Test Project 1",
                Description = "Test Description 1"
            };
            var createProject2 = new CreateTodoProjectDTO
            {
                Name = "Test Project 2",
                Description = "Test Description 2"
            };
            var createProject3 = new CreateTodoProjectDTO
            {
                Name = "Test Project 3",
                Description = "Test Description 3"
            };
            await _service.CreateProjectAsync(createProject1, user1.Id);
            await _service.CreateProjectAsync(createProject2, user1.Id);
            await _service.CreateProjectAsync(createProject3, user2.Id);
            // Act
            var userProjects = await _service.GetUserProjectsAsync(user1.Id);
            // Assert
            Assert.IsNotNull(userProjects);
            Assert.AreEqual(2, userProjects.Count);

            Assert.IsTrue(userProjects.Any(p => p.Name == createProject1.Name && p.Description == createProject1.Description));
            Assert.IsTrue(userProjects.Any(p => p.Name == createProject2.Name && p.Description == createProject2.Description));

            Assert.IsFalse(userProjects.Any(p => p.Name == createProject3.Name && p.Description == createProject3.Description));
        }
        [TestMethod]
        public async Task GetUserProjectsAsync_ShouldReturnEmptyList_WhenUserHasNoProjects()
        {
            // Arrange
            var userId = 1;
            // Act
            var userProjects = await _service.GetUserProjectsAsync(userId);
            // Assert
            Assert.IsNotNull(userProjects);
            Assert.AreEqual(0, userProjects.Count);
        }
        [TestMethod]
        public async Task GetUserProjectsAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.GetUserProjectsAsync(nonExistentUserId);
            });
        }
        [TestMethod]
        public async Task UpdateProjectAsync_ShouldUpdateProject_WhenUserIsOwner()
        {
            // Arrange

            var createUser1 = new CreateUserDTO
            {
                Name = "Owner",
                Email = "Owner@world.com"
            };

            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };

            var owner = await _userService.RegisterUserAsync(createUser1);

            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var updateProject = new UpdateTodoProjectDTO
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };
            // Act
            var updatedProject = await _service.UpdateProjectAsync(createdProject.Id, updateProject, owner.Id);
            // Assert
            Assert.IsNotNull(updatedProject);
            Assert.AreEqual(createdProject.Id, updatedProject.Id);
            Assert.AreEqual(updateProject.Name, updatedProject.Name);
            Assert.AreEqual(updateProject.Description, updatedProject.Description);
        }
        [TestMethod]
        public async Task UpdateProjectAsync_ShouldUpdateProject_WhenUserIsAdmin()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "Owner@world.com"
            };
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "Admin@world.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var makeUserAdmin = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            };
            var createdProject = await _service.CreateProjectAsync(createProject, admin.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, makeUserAdmin, owner.Id);

            var updateProject = new UpdateTodoProjectDTO
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };
            // Act
            var updatedProject = await _service.UpdateProjectAsync(createdProject.Id, updateProject, admin.Id);
            // Assert
            Assert.IsNotNull(updatedProject);
            Assert.AreEqual(createdProject.Id, updatedProject.Id);
            Assert.AreEqual(updateProject.Name, updatedProject.Name);
            Assert.AreEqual(updateProject.Description, updatedProject.Description);
        }
        [TestMethod]
        public async Task UpdateProjectAsync_ShouldThrowException_WhenUserIsNotOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "Owner@world.com"
            };
            var createRandomUser = new CreateUserDTO
            {
                Name = "RandomUser",
                Email = "RandomUser@world.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var randomUser = await _userService.RegisterUserAsync(createRandomUser);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var updateProject = new UpdateTodoProjectDTO
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.UpdateProjectAsync(createdProject.Id, updateProject, randomUser.Id));
        }

        [TestMethod]
        public async Task UpdateProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var nonExistentProjectId = 1;
            var updateProject = new UpdateTodoProjectDTO
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.UpdateProjectAsync(nonExistentProjectId, updateProject, userId);
            });
        }
        [TestMethod]
        public async Task UpdateProjectAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 1;
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var createdProject = await _service.CreateProjectAsync(createProject, nonExistentUserId);
            var updateProject = new UpdateTodoProjectDTO
            {
                Name = "Updated Project",
                Description = "Updated Description"
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.UpdateProjectAsync(createdProject.Id, updateProject, nonExistentUserId);
            });
        }
        [TestMethod]
        public async Task DeleteProjectAsync_ShouldDeleteProject_WhenUserIsOwner()
        {
            // Arrange
            var createUser = new CreateUserDTO
            {
                Name = "Owner",
                Email = "Owner@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var user = await _userService.RegisterUserAsync(createUser);
            var createdProject = await _service.CreateProjectAsync(createProject, user.Id);
            // Act
            await _service.DeleteProjectAsync(createdProject.Id, user.Id);
            // Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.GetProjectByIdAsync(createdProject.Id, user.Id);
            });
        }

        [TestMethod]
        public async Task DeleteProjectAsync_ShouldThrowException_WhenUserIsNotOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };

            var createRandomUser = new CreateUserDTO
            {
                Name = "RandomUser",
                Email = "RandomUser@hello.com"
            };
            var createproject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var user = await _userService.RegisterUserAsync(createRandomUser);
            var owner = await _userService.RegisterUserAsync(createOwner);
            var createdProject = await _service.CreateProjectAsync(createproject, owner.Id);
            //act & assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.DeleteProjectAsync(createdProject.Id, user.Id));
        }
        [TestMethod]
        public async Task DeleteProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var nonExistentProjectId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.DeleteProjectAsync(nonExistentProjectId, userId);
            });
        }
        [TestMethod]
        public async Task DeleteProjectAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var nonExistentUserId = 1;
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var createdProject = await _service.CreateProjectAsync(createProject, nonExistentUserId);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.DeleteProjectAsync(createdProject.Id, nonExistentUserId);
            });
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldAddMember_WhenUserIsOwner()
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
                Email = "member@email.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            // Act
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            // Assert

            var project = await _service.GetProjectByIdAsync(createdProject.Id, member.Id);
            Assert.IsTrue(project.Members.Any(m => m.User.Id == member.Id && m.Role == Role.Member));
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenUserIsNotOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };

            var createRandomUser = new CreateUserDTO
            {
                Name = "RandomUser",
                Email = "RandomUser@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var randomUser = await _userService.RegisterUserAsync(createRandomUser);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            //act & assert

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.AddMemberToProjectAsync(createdProject.Id, new AddProjectMemberDTO
                {
                    UserId = randomUser.Id,
                    Role = Role.Member
                }, randomUser.Id));
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var nonExistentProjectId = 1;
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = userId,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.AddMemberToProjectAsync(nonExistentProjectId, addMemberDto, userId);
            });
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenUserDoesNotExist()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var nonExistentUserId = 999;
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = nonExistentUserId,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            });
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenUserIsAlreadyMember()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id));
        }
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenUserIsAlreadyOwner()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = owner.Id,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id));
        } 
        [TestMethod]
        public async Task AddMemberToProjectAsync_ShouldThrowException_WhenUserIsAlreadyAdmin()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addAdminDto = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id));
        }
        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldUpdateMemberRole_whenUserIsOwner()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@admin.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addAdminDto = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id);

            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = owner.Id,
                Role = Role.Admin
            };
            // Act
            await _service.UpdateMemberRoleAsync(createdProject.Id, updateRoleDto, owner.Id);
            // Assert
            var project = await _service.GetProjectByIdAsync(createdProject.Id, owner.Id);
            Assert.IsTrue(project.Members.Any(m => m.User.Id == owner.Id && m.Role == Role.Admin));
        }
        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldUpdateMemberRole_WhenUserIsAdmin()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@hello.com"
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
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addAdminDto = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            };
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = member.Id,
                Role = Role.Admin
            };
            // Act
            await _service.UpdateMemberRoleAsync(createdProject.Id, updateRoleDto, admin.Id);
            // Assert
            var project = await _service.GetProjectByIdAsync(createdProject.Id, owner.Id);
            Assert.IsTrue(project.Members.Any(m => m.User.Id == member.Id && m.Role == Role.Admin));
        }

        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldThrowException_WhenUserIsNotOwnerOrAdmin()
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
            var createTarget = new CreateUserDTO
            {
                Name = "Target",
                Email = "target@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var target = await _userService.RegisterUserAsync(createTarget);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            var addTargetDto = new AddProjectMemberDTO
            {
                UserId = target.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, addTargetDto, owner.Id);
            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = target.Id,
                Role = Role.Admin
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.UpdateMemberRoleAsync(createdProject.Id, updateRoleDto, member.Id));
        }

        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var nonExistentProjectId = 999;
            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = 1,
                Role = Role.Admin
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.UpdateMemberRoleAsync(nonExistentProjectId, updateRoleDto, 1));
        }

        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldThrowException_WhenTargetUserIsNotMember()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = outsider.Id,
                Role = Role.Admin
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.UpdateMemberRoleAsync(createdProject.Id, updateRoleDto, owner.Id));
        }

        [TestMethod]
        public async Task UpdateMemberRoleAsync_ShouldThrowException_WhenTargetUserIsOwner()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var updateRoleDto = new UpdateProjectMemberRoleDTO
            {
                UserId = owner.Id,
                Role = Role.Member
            };
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateMemberRoleAsync(createdProject.Id, updateRoleDto, owner.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldRemoveMember_WhenUserIsOwner()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            // Act
            await _service.RemoveMemberFromProjectAsync(createdProject.Id, member.Id, owner.Id);
            // Assert
            var project = await _service.GetProjectByIdAsync(createdProject.Id, owner.Id);
            Assert.IsFalse(project.Members.Any(m => m.User.Id == member.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldRemoveMember_WhenUserIsAdmin()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@hello.com"
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
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var member = await _userService.RegisterUserAsync(createMember);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addAdminDto = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            };
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            // Act
            await _service.RemoveMemberFromProjectAsync(createdProject.Id, member.Id, admin.Id);
            // Assert
            var project = await _service.GetProjectByIdAsync(createdProject.Id, owner.Id);
            Assert.IsFalse(project.Members.Any(m => m.User.Id == member.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldThrowException_WhenUserIsNotOwnerOrAdmin()
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
            var createTarget = new CreateUserDTO
            {
                Name = "Target",
                Email = "target@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var target = await _userService.RegisterUserAsync(createTarget);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            var addTargetDto = new AddProjectMemberDTO
            {
                UserId = target.Id,
                Role = Role.Member
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, addTargetDto, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.RemoveMemberFromProjectAsync(createdProject.Id, target.Id, member.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var createOwner = new CreateUserDTO
            {
                Name = "Owner",
                Email = "owner@hello.com"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var nonExistentProjectId = 999;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.RemoveMemberFromProjectAsync(nonExistentProjectId, owner.Id, owner.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldThrowException_WhenTargetUserIsNotMember()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.RemoveMemberFromProjectAsync(createdProject.Id, outsider.Id, owner.Id));
        }

        [TestMethod]
        public async Task RemoveMemberFromProjectAsync_ShouldThrowException_WhenTargetUserIsOwner()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.RemoveMemberFromProjectAsync(createdProject.Id, owner.Id, owner.Id));
        }

        [TestMethod]
        public async Task GetMembersFromProjectAsync_ShouldReturnMembers_WhenUserIsMember()
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
            var createAdmin = new CreateUserDTO
            {
                Name = "Admin",
                Email = "admin@hello.com"
            };
            var createProject = new CreateTodoProjectDTO
            {
                Name = "Test Project",
                Description = "Test Description"
            };
            var owner = await _userService.RegisterUserAsync(createOwner);
            var member = await _userService.RegisterUserAsync(createMember);
            var admin = await _userService.RegisterUserAsync(createAdmin);
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            var addMemberDto = new AddProjectMemberDTO
            {
                UserId = member.Id,
                Role = Role.Member
            };
            var addAdminDto = new AddProjectMemberDTO
            {
                UserId = admin.Id,
                Role = Role.Admin
            };
            await _service.AddMemberToProjectAsync(createdProject.Id, addMemberDto, owner.Id);
            await _service.AddMemberToProjectAsync(createdProject.Id, addAdminDto, owner.Id);
            // Act
            var members = await _service.GetMembersFromProjectAsync(createdProject.Id, owner.Id);
            // Assert
            Assert.IsNotNull(members);
            Assert.IsTrue(members.Any(m => m.User.Id == member.Id && m.Role == Role.Member));
            Assert.IsTrue(members.Any(m => m.User.Id == admin.Id && m.Role == Role.Admin));
            Assert.IsTrue(members.Any(m => m.User.Id == owner.Id && m.Role == Role.Owner));
        }

        [TestMethod]
        public async Task GetMembersFromProjectAsync_ShouldThrowException_WhenUserIsNotMember()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.GetMembersFromProjectAsync(createdProject.Id, outsider.Id));
        }

        [TestMethod]
        public async Task GetMembersFromProjectAsync_ShouldThrowException_WhenProjectDoesNotExist()
        {
            // Arrange
            var nonExistentProjectId = 999;
            var userId = 1;
            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.GetMembersFromProjectAsync(nonExistentProjectId, userId));
        }

        [TestMethod]
        public async Task GetMembersFromProjectAsync_ShouldReturnEmptyList_WhenProjectHasNoMembers()
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
            var createdProject = await _service.CreateProjectAsync(createProject, owner.Id);
            // Act
            var members = await _service.GetMembersFromProjectAsync(createdProject.Id, owner.Id);
            // Assert
            Assert.IsNotNull(members);
            Assert.AreEqual(1, members.Count);
        }
    }
}

