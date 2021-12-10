public class UserStatus
{
    public int UserNumber { get; set; }
    public int MovieNumber { get; set; }
    public string Status { get; set; }
    public string PosterPath { get; set; }
    public string OriginalTitle { get; set; }
    public string Overview { get; set; }
    public UserStatus(int userNumber, int movieNumber, string status, string posterPath, string originalTitle, string overview)
    {
        UserNumber = userNumber;
        MovieNumber = movieNumber;
        Status = status;
        PosterPath = posterPath;
        OriginalTitle = originalTitle;
        Overview = overview;
    }
}