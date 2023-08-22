namespace HouseCommitteeAppLibrary.DataAccess;

public interface ISuggestionData
{
    Task CreateSuggestion(SuggestionModel suggestion);
    Task<List<SuggestionModel>> GetAllApprovedSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsAsync();
    Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApprovalAsync();
    Task<SuggestionModel> GetSuggestion(string id);
    Task<List<SuggestionModel>> GetUsersSuggestionsAsync(string userId);
    Task UpdateSuggestion(SuggestionModel suggestion);
    Task UpvoteSeggestion(string suggestionId, string userId);
}