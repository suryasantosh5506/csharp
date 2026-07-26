using Microsoft.AspNetCore.Mvc;
using VideoGameCharacterApi.Dtos;
using VideoGameCharacterApi.Interfaces;

namespace VideoGameCharacterApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VideoGameCharacterController(IVideoGameCharacterService videoGameCharacterService):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GetVideoGameCharacterDto>>> GetAllVideoGameCharacters()=> 
            Ok(await videoGameCharacterService.GetAllVideoGameCharacters());
    [HttpGet("{id:int}",Name ="GetVideoGameCharacterById")]
    public async Task<ActionResult<GetVideoGameCharacterDto>> GetVideoGameCharacterById(int id)
    {
        var character=await videoGameCharacterService.GetVideoGameCharacterById(id);
        if(character is null) return NotFound();
        return Ok(character);
    }

    [HttpPost]
    public async Task<ActionResult<GetVideoGameCharacterDto>> CreateNewGameCharacter(CreateVideoGameCharacterDto newCharacter){
        var character=await videoGameCharacterService.CreateNewGameCharacter(newCharacter);
        return CreatedAtRoute("GetVideoGameCharacterById",new {id=character.Id},character);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GetVideoGameCharacterDto>> UpdateVideoGameCharacter(int id,UpdateVideoGameCharacterDto updateCharacter){
        var character=await videoGameCharacterService.UpdateVideoGameCharacter(id,updateCharacter);
        if(character is null) return NotFound();
        return Ok(character);
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<GetVideoGameCharacterDto>> DeleteVideoGameCharacter(int id){
        var character=await videoGameCharacterService.DeleteVideoGameCharacter(id);
        if(character is null) return NotFound();
        return Ok(character);
    }
}