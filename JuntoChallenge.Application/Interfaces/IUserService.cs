using JuntoChallenge.Application.DTOs;

namespace JuntoChallenge.Application.Interfaces
{
    public interface IUserService
    {
        List<UserDTO> GetUsers(int pageNumber, int pageSize);
        UserDTO GetUser(int id);
        Task<UserDTO> UpdateUser(int id,UpdateUserDTO userDTO);
        Task<UserDTO?> PostUser(PostUserDTO user);
        LoginResponseDTO Login(LoginDTO login);
        Task<bool> DeleteUser(int id);
        Task<bool> UpdatePassword(UpdatePasswordUserDTO userDTO);


    }
}
