using Microsoft.EntityFrameworkCore;
using VideoGameCharacterApi.Data;
using VideoGameCharacterApi.Dtos;
using VideoGameCharacterApi.Interfaces;
using VideoGameCharacterApi.Models;

namespace VideoGameCharacterApi.Services;

public class VideoGameCharacterService(VideoGameCharacterContext dbContext)
    : IVideoGameCharacterService
{
    public async Task<List<GetVideoGameCharacterDto>> GetAllVideoGameCharacters()
    {
        return await dbContext.VideoGameCharacters
            .Select(x => new GetVideoGameCharacterDto(
                x.Id,
                x.Name,
                x.Game,
                x.Role))
            .ToListAsync();
    }

    public async Task<GetVideoGameCharacterDto?> GetVideoGameCharacterById(int id)
    {
        return await dbContext.VideoGameCharacters
            .Where(x => x.Id == id)
            .Select(x => new GetVideoGameCharacterDto(
                x.Id,
                x.Name,
                x.Game,
                x.Role))
            .FirstOrDefaultAsync();
    }

    public async Task<GetVideoGameCharacterDto> CreateNewGameCharacter(
        CreateVideoGameCharacterDto newVideoGameCharacter)
    {
        var newCharacter = new VideoGameCharacter
        {
            Name = newVideoGameCharacter.Name,
            Game = newVideoGameCharacter.Game,
            Role = newVideoGameCharacter.Role
        };

        dbContext.VideoGameCharacters.Add(newCharacter);
        await dbContext.SaveChangesAsync();

        return new GetVideoGameCharacterDto(
            newCharacter.Id,
            newCharacter.Name,
            newCharacter.Game,
            newCharacter.Role);
    }

    public async Task<GetVideoGameCharacterDto?> UpdateVideoGameCharacter(
        int id,
        UpdateVideoGameCharacterDto updateVideoGameCharacter)
    {
        var character = await dbContext.VideoGameCharacters.FindAsync(id);

        if (character is null)
            return null;

        character.Name = updateVideoGameCharacter.Name;
        character.Game = updateVideoGameCharacter.Game;
        character.Role = updateVideoGameCharacter.Role;

        await dbContext.SaveChangesAsync();

        return new GetVideoGameCharacterDto(
            character.Id,
            character.Name,
            character.Game,
            character.Role);
    }

    public async Task<GetVideoGameCharacterDto?> DeleteVideoGameCharacter(int id)
    {
        var character = await dbContext.VideoGameCharacters.FindAsync(id);

        if (character is null)
            return null;

        dbContext.VideoGameCharacters.Remove(character);
        await dbContext.SaveChangesAsync();

        return new GetVideoGameCharacterDto(
            character.Id,
            character.Name,
            character.Game,
            character.Role);
    }
}