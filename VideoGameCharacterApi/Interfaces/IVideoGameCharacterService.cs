using VideoGameCharacterApi.Dtos;

namespace VideoGameCharacterApi.Interfaces;

public interface IVideoGameCharacterService{
    Task<List<GetVideoGameCharacterDto>> GetAllVideoGameCharacters();
    Task<GetVideoGameCharacterDto?> GetVideoGameCharacterById(int id);
    Task<GetVideoGameCharacterDto> CreateNewGameCharacter(CreateVideoGameCharacterDto newVideoGameCharacter);
    Task<GetVideoGameCharacterDto?> UpdateVideoGameCharacter(int id,UpdateVideoGameCharacterDto updateVideoGameCharacter);
    Task<GetVideoGameCharacterDto?> DeleteVideoGameCharacter(int id);
}