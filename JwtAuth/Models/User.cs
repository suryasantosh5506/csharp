namespace JwtAuth.Models;

public class User
{
    public int UserId{get;set;}
    public required string UserName{get;set;}
    public required string PasswordHash{get;set;}
    public string Role{get;set;}="User";
}