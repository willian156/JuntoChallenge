using Xunit;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using JuntoChallenge.API.Controllers;
using JuntoChallenge.Application.DTOs;
using JuntoChallenge.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JuntoChallenge.UnitTests.Controller
{
    public class UsersControllerTests
    {
        private readonly IUserService _userService;
        private readonly ILogService _logService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _logService = A.Fake<ILogService>();
            _userService = A.Fake<IUserService>();
            _controller = new UsersController(_userService, _logService);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnListOfUsers()
        {
            //arrange
            var pageNumber = 1;
            var pageSize = 10;
            var fakeUsers = new List<UserDTO>
            {
                new UserDTO { Id = 1, Username = "joao", Email = "joao@email.com" },
                new UserDTO { Id = 2, Username = "ana", Email = "ana@email.com" }
            };

            A.CallTo(() => _userService.GetUsers(pageNumber, pageSize)).Returns(fakeUsers);

            //act
            var result = await _controller.GetUsers(1, 10);

            //assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(fakeUsers);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 20;

            A.CallTo(() => _userService.GetUsers(pageNumber, pageSize))
                .Throws(new Exception("Erro simulado"));

            // Act
            var result = await _controller.GetUsers(pageNumber, pageSize);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result.Result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Erro simulado" });
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserDTO()
        {
            //arrange
            int id = 5;
            var fakeUser = new UserDTO { Id = 5, Username = "joao543", Email = "joao543@email.com", isDeleted = false };


            A.CallTo(() => _userService.GetUser(id)).Returns(fakeUser);

            //act
            var result = await _controller.GetUser(5);

            //assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;

            okResult!.Value.Should().BeEquivalentTo(fakeUser);
        }

        [Fact]
        public async Task GetUser_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var id = 1;

            A.CallTo(() => _userService.GetUser(id))
                .Throws(new Exception("Erro simulado"));

            // Act
            var result = await _controller.GetUser(id);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result.Result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Erro simulado" });
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenGetUserReturnsNull()
        {
            // Arrange
            int id = 1;

            A.CallTo(() => _userService.GetUser(id)).Returns(null as UserDTO);

            // Act
            var result = await _controller.GetUser(id);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PutUser_ShouldReturnOk_WhenUserIsUpdated()
        {
            // Arrange
            int userId = 1;
            var updateDto = new UpdateUserDTO { Username = "NovoNome", Email = "novo@email.com" };
            var oldUser = new UserDTO { Id = userId, Username = "Antigo", Email = "antigo@email.com" };
            var updatedUser = new UserDTO { Id = userId, Username = "NovoNome", Email = "novo@email.com" };

            A.CallTo(() => _userService.GetUser(userId)).Returns(oldUser);
            A.CallTo(() => _userService.UpdateUser(userId, updateDto)).Returns(updatedUser);

            // Act
            var result = await _controller.PutUser(userId, updateDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new
            {
                message = $"User with Id:{updatedUser.Id} updated!"
            });
        }

        [Fact]
        public async Task PutUser_ShouldReturnNotFound_WhenOldUserIsNull()
        {
            // Arrange
            int userId = 1;
            var updateDto = new UpdateUserDTO { Username = "x", Email = "x" };

            A.CallTo(() => _userService.GetUser(userId)).Returns(null as UserDTO);

            // Act
            var result = await _controller.PutUser(userId, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().BeEquivalentTo(new { message = "User not found!" });
        }

        [Fact]
        public async Task PutUser_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            int userId = 1;
            var updateDto = new UpdateUserDTO { Username = "x", Email = "x" };
            var oldUser = new UserDTO { Id = userId, Username = "old", Email = "old" };

            A.CallTo(() => _userService.GetUser(userId)).Returns(oldUser);
            A.CallTo(() => _userService.UpdateUser(userId, updateDto)).Returns(null as UserDTO);

            // Act
            var result = await _controller.PutUser(userId, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();

            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().BeEquivalentTo(new { message = "User not found!" });
        }

        [Fact]
        public async Task PutUser_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int userId = 1;
            var updateDto = new UpdateUserDTO { Username = "error", Email = "fail@email.com" };
            var oldUser = new UserDTO { Id = userId };

            A.CallTo(() => _userService.GetUser(userId)).Returns(oldUser);
            A.CallTo(() => _userService.UpdateUser(userId, updateDto)).Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.PutUser(userId, updateDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Erro inesperado" });
        }

        [Fact]
        public async Task PostUser_ShouldReturnCreated_WhenUserIsSaved()
        {
            // Arrange
            var input = new PostUserDTO { Username = "joao", Email = "joao@email.com", Password = "123" };
            var createdUser = new UserDTO { Id = 10, Username = "joao", Email = "joao@email.com" };

            A.CallTo(() => _userService.PostUser(input)).Returns(createdUser);

            // Act
            var result = await _controller.PostUser(input);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();

            var createdResult = result as CreatedAtActionResult;
            createdResult.ActionName.Should().Be(nameof(_controller.GetUser));
            createdResult.RouteValues["id"].Should().Be(createdUser.Id);
            createdResult.Value.Should().BeEquivalentTo(createdUser);
        }

        [Fact]
        public async Task PostUser_ShouldReturnBadRequest_WhenUserIsReturnedWithZeroId()
        {
            // Arrange
            var input = new PostUserDTO { Username = "teste", Email = "email@email.com", Password = "123" };
            var userWithZeroId = new UserDTO { Id = 0 };

            A.CallTo(() => _userService.PostUser(input)).Returns(userWithZeroId);

            // Act
            var result = await _controller.PostUser(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "User doesn't saved!" });
        }

        [Fact]
        public async Task PostUser_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Arrange
            var input = new PostUserDTO { Username = "falha", Email = "falha@email.com", Password = "123" };

            A.CallTo(() => _userService.PostUser(input)).Returns(null as UserDTO);

            // Act
            var result = await _controller.PostUser(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "User doesn't saved!" });
        }

        [Fact]
        public async Task PostUser_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var input = new PostUserDTO { Username = "erro", Email = "erro@email.com", Password = "123" };

            A.CallTo(() => _userService.PostUser(input)).Throws(new Exception("Erro interno"));

            // Act
            var result = await _controller.PostUser(input);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new { message = "Erro interno" });
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenUserDeleted()
        {
            // Arrange
            int userId = 1;
            A.CallTo(() => _userService.DeleteUser(userId)).Returns(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new
            {
                message = $"User with Id:{userId} was deleted!"
            });
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnBadRequest_WhenDeletionFails()
        {
            // Arrange
            int userId = 2;
            A.CallTo(() => _userService.DeleteUser(userId)).Returns(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new
            {
                message = $"User not deleted!"
            });
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            int userId = 3;
            A.CallTo(() => _userService.DeleteUser(userId))
                .Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new
            {
                message = "Erro inesperado"
            });
        }

        [Fact]
        public async Task Login_ShouldReturnOkResult()
        {
            // Arrange
            var fakeLogin = new LoginDTO() { Username = "joao", Password = "123" };
            var fakeResponse = new LoginResponseDTO() { Id = 1, Username = "joao", JWT = "A-long-time-ago-in-a-galaxy-far-far-away" };
            A.CallTo(() => _userService.Login(fakeLogin))
                .Returns(fakeResponse);

            // Act
            var result = await _controller.Login(fakeLogin);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(fakeResponse);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var fakeLogin = new LoginDTO() { Username = "joao", Password = "123" };
            A.CallTo(() => _userService.Login(fakeLogin))
                .Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.Login(fakeLogin);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new
            {
                message = "Erro inesperado"
            });
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnOk_WhenPasswordIsChanged()
        {
            // Arrange
            var dto = new UpdatePasswordUserDTO
            {
                Username = "joao",
                OldPassword = "123",
                NewPassword = "321"
            };

            A.CallTo(() => _userService.UpdatePassword(dto)).Returns(true);

            // Act
            var result = await _controller.UpdatePassword(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new
            {
                message = $"Password from username {dto.Username} was successfully changed!"
            });
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var dto = new UpdatePasswordUserDTO
            {
                Username = "joao",
                OldPassword = "123",
                NewPassword = "321"
            };

            A.CallTo(() => _userService.UpdatePassword(dto)).Returns(false);

            // Act
            var result = await _controller.UpdatePassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new
            {
                message = $"Password from username {dto.Username} was not changed!"
            });
        }

        [Fact]
        public async Task UpdatePassword_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            var dto = new UpdatePasswordUserDTO
            {
                Username = "joao",
                OldPassword = "123",
                NewPassword = "321"
            };

            A.CallTo(() => _userService.UpdatePassword(dto))
                .Throws(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.UpdatePassword(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().BeEquivalentTo(new
            {
                message = "Erro inesperado"
            });
        }
    }
}