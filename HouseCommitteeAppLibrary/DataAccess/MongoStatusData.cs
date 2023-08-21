using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Memory;

namespace HouseCommitteeAppLibrary.DataAccess;
public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;
    private const string CachName = "StatusData";

    public MongoStatusData(IDbConnection db, IMemoryCache cache)
    {
        this._cache = cache;
        this._statuses = db.StatusCollection;
    }

    public async Task<List<StatusModel>> GetAllStatusesAsync()
    {
        var output = _cache.Get<List<StatusModel>>(CachName);
        if (output == null)
        {
            var results = await _statuses.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CachName, output, TimeSpan.FromDays(1));
        }
        return output;
    }


    public Task CreateStatus(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }
}
