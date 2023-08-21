namespace HouseCommitteeAppLibrary.DataAccess;

public interface IUserData
{
    Task CreateUser(UserModel user);
    Task<UserModel> GetUserAsync(string id);
    Task<UserModel> GetUserFromAutenticationAsync(string objectId);
    Task<List<UserModel>> GetUsersAsync();
    Task UpdateUser(UserModel user);
}