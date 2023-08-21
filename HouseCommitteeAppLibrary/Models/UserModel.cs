using System.Reflection.Metadata.Ecma335;

namespace HouseCommitteeAppLibrary.Models;
public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ObjectIdentifier { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AppartmentNum { get; set; }
    public string EmailAddress { get; set; }
    public List<BasicSuggestionModel> AuthoredSuggestions { get; set; } = new();
    public List<BasicSuggestionModel> VotedOnSuggestions { get; set; } = new();
}
