namespace HouseCommitteeAppLibrary.DataAccess;

public interface ICategoryData
{
    Task CreateCategory(CategoryModel category);
    Task<List<CategoryModel>> GetAllCatrgoriesAsync();
}