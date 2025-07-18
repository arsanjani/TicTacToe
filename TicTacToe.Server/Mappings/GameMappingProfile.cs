using AutoMapper;
using TicTacToe.Server.DTOs;
using TicTacToe.Server.Entities;

namespace TicTacToe.Server.Mappings;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<GameEntity, GameDto>();

        // DTO to Entity mappings
        CreateMap<GameDto, GameEntity>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id > 0)); // Only map Id if it's greater than 0 (for updates)
    }
} 