using Amazon.Runtime.CredentialManagement.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using System.Net.WebSockets;

namespace HouseCommitteeAppLibrary.DataAccess;
public class MongoSuggestionData : ISuggestionData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<SuggestionModel> _suggestions;
    private const string CacheName = "SuggestionsData";


    public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
    {
        this._db = db;
        this._userData = userData;
        this._cache = cache;
        this._suggestions = db.SuggestionCollection;
    }

    public async Task<List<SuggestionModel>> GetAllSuggestionsAsync()
    {
        var output = _cache.Get<List<SuggestionModel>>(CacheName);
        if (output == null)
        {
            var result = await _suggestions.FindAsync(s => s.Archived == false);
            output = result.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(15));
            /*this meand admins will not see a new suggestion for 15 minutes
             if this bothers me i will change it to less*/
        }

        return output;
    }

    public async Task<List<SuggestionModel>> GetUsersSuggestionsAsync(string userId)
    {
        var output = _cache.Get<List<SuggestionModel>>(userId);
        if (output == null)
        {
            var result = await _suggestions.FindAsync(s => s.Author.Id == userId);
            output = result.ToList();

            _cache.Set(userId, output, TimeSpan.FromMinutes(15));
        }

        return output;
    }
    public async Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync()
    {
        var output = await GetAllSuggestionsAsync();
        return output.Where(x => x.ApprovedForRelease == true).ToList();
    }

    public async Task<SuggestionModel> GetSuggestion(string id)
    {
        var result = await _suggestions.FindAsync(s => s.Id == id);
        return result.FirstOrDefault();
    }
    public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApprovalAsync()
    {
        var output = await GetAllSuggestionsAsync();
        return output.Where(x =>
            x.ApprovedForRelease == false &&
            x.Rejected == false).ToList();
    }

    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        await _suggestions.ReplaceOneAsync(s => s.Id == suggestion.Id, suggestion);
        _cache.Remove(CacheName);
    }

    public async Task UpvoteSuggestion(string suggestionId, string userId)
    {
        /*creating a transaction*/
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();
        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            var suggestion = (await suggestionInTransaction.FindAsync(s => s.Id == suggestionId)).First();

            bool isUpvote = suggestion.UserVotes.Add(userId);
            if (isUpvote == false)
            {
                /*re clicking on a vote will cancle the vote*/
                suggestion.UserVotes.Remove(userId);
            }

            await suggestionInTransaction.ReplaceOneAsync(s => s.Id == suggestionId, suggestion);
            var userInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.Id);

            if (isUpvote)
            {
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                var suggestionToRemove = user.VotedOnSuggestions.Where(s => s.Id == suggestionId).First();
                user.VotedOnSuggestions.Remove(suggestionToRemove);
            }

            await userInTransaction.ReplaceOneAsync(u => u.Id == userId, user);
            await session.CommitTransactionAsync();

            _cache.Remove(CacheName);
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task CreateSuggestion(SuggestionModel suggestion)
    {
        var client = _db.Client;
        using var session = await client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var db = client.GetDatabase(_db.DbName);
            var suggestionInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            await suggestionInTransaction.InsertOneAsync(suggestion);

            var userInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUserAsync(suggestion.Author.Id);
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await userInTransaction.ReplaceOneAsync(u => u.Id == user.Id, user);

            await session.CommitTransactionAsync();

            /*we will not remove the cache here becuse a new suggestion is not immidiatly visible
             it goes for an admin approval first.
             this means admins will not see a new suggestion for an hour
             if this bothers me i will change it to less in GetAllSuggestionsAsync*/

        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }


}
