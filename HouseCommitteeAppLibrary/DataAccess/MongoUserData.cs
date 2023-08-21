using MongoDB.Driver;

namespace HouseCommitteeAppLibrary.DataAccess;
public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;
    public MongoUserData(IDbConnection db)
    {
        this._users = db.UserCollection;
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<UserModel> GetUserAsync(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    /*this is the object identifier that the azure active directory b2c has given our user
     and we will look the user uo based upon that identifier.
    when the user logs in were gonna have an identifier , and we will look them up by that 
    identifier to get all their information from our system*/
    public async Task<UserModel> GetUserFromAutenticationAsync(string objectId)
    {
        var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return results.FirstOrDefault();
    }

    public Task CreateUser(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    public Task UpdateUser(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
        /*replace the doc that matches filter, with user, and if there is no one, create one*/
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }



}
