using Microsoft.Extensions.Caching.Memory;

namespace HouseCommitteeAppLibrary.DataAccess;
public class MongoCategoryData : ICategoryData
{
    private readonly IMongoCollection<CategoryModel> _categories;
    private readonly IMemoryCache _cache;
    private const string CachName = "CategoryData";

    public MongoCategoryData(IDbConnection db, IMemoryCache cache)
    {
        this._cache = cache;
        this._categories = db.CategoryCollection;
    }

    public async Task<List<CategoryModel>> GetAllCatrgoriesAsync()
    {
        var output = _cache.Get<List<CategoryModel>>(CachName);
        if (output == null)
        {
            var results = await _categories.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CachName, output, TimeSpan.FromDays(1));
        }
        return output;
    }


    public Task CreateCategory(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
