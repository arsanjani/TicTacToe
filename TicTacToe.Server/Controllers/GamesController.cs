using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Data;
using TicTacToe.Server.DTOs;
using TicTacToe.Server.Entities;

namespace TicTacToe.Server.Controllers;

[Route("api/[controller]")]
public class GamesController(TicTacToeDbContext context, IMapper mapper) : BaseApiController<GameEntity, GameDto>(context, mapper)
{
    
    /// <summary>
    /// Get game by GameId (string identifier)
    /// </summary>
    [HttpGet("by-game-id/{gameId}")]
    public async Task<ActionResult<GameDto>> GetByGameId(string gameId)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(g => g.GameId == gameId);
            if (entity == null)
            {
                return NotFound($"Game with GameId {gameId} not found");
            }

            var dto = _mapper.Map<GameDto>(entity);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get games by player ID
    /// </summary>
    [HttpGet("by-player/{playerId}")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetByPlayerId(string playerId)
    {
        try
        {
            var entities = await _dbSet
                .Where(g => g.Player1Id == playerId || g.Player2Id == playerId)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<GameDto>>(entities);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get games by state
    /// </summary>
    [HttpGet("by-state/{state}")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetByState(Models.GameState state)
    {
        try
        {
            var entities = await _dbSet
                .Where(g => g.State == state)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<GameDto>>(entities);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get active games (waiting for players or in progress)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetActiveGames()
    {
        try
        {
            var entities = await _dbSet
                .Where(g => g.State == Models.GameState.WaitingForPlayers || g.State == Models.GameState.InProgress)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<GameDto>>(entities);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get finished games
    /// </summary>
    [HttpGet("finished")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetFinishedGames()
    {
        try
        {
            var entities = await _dbSet
                .Where(g => g.State == Models.GameState.Finished)
                .OrderByDescending(g => g.EndedAt)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<GameDto>>(entities);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update game by GameId
    /// </summary>
    [HttpPut("by-game-id/{gameId}")]
    public async Task<ActionResult<GameDto>> UpdateByGameId(string gameId, [FromBody] GameDto gameDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbSet.FirstOrDefaultAsync(g => g.GameId == gameId);
            if (entity == null)
            {
                return NotFound($"Game with GameId {gameId} not found");
            }

            _mapper.Map(gameDto, entity);
            SetUpdatedAt(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<GameDto>(entity);
            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
} 