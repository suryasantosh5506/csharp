using Microsoft.EntityFrameworkCore;
using VideoGameCharacterApi.Models;

namespace VideoGameCharacterApi.Data;

public class VideoGameCharacterContext(DbContextOptions<VideoGameCharacterContext>options):DbContext(options)
{
    public DbSet<VideoGameCharacter>VideoGameCharacters=>Set<VideoGameCharacter>();
}