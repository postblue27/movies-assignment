public class User
{
    public int Number { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public User(int number, string name, string username, string password)
    {
        Number = number;
        Name = name;
        Username = username;
        Password = password;
    }

}