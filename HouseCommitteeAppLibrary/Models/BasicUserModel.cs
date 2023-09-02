namespace HouseCommitteeAppLibrary.Models;
public class BasicUserModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string AppartmentNum { get; set; }
    public string StreetAddress { get; set; }

    public BasicUserModel()
    {
        
    }
    public BasicUserModel(UserModel user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        AppartmentNum = user.AppartmentNum;
        StreetAddress = user.StreetAddress;
    }
}
