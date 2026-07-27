namespace VideoGameCharacterApi.Models;

public class VideoGameCharacter
{
    public int Id{get;set;}
    public required string Name {get;set;}
    public required string Game {get;set;}
    public required string Role {get;set;}
}