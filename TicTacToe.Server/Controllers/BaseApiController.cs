using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Data;

namespace TicTacToe.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController<TEntity, TDto> : ControllerBase
    where TEntity : class
    where TDto : class
{
    protected readonly TicTacToeDbContext _context;
    protected readonly IMapper _mapper;
    protected readonly DbSet<TEntity> _dbSet;

    protected BaseApiController(TicTacToeDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _dbSet = _context.Set<TEntity>();
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        try
        {
            var entities = await _dbSet.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get entity by id
    /// </summary>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TDto>> GetById(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return NotFound($"Entity with id {id} not found");
            }

            var dto = _mapper.Map<TDto>(entity);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new entity
    /// </summary>
    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = _mapper.Map<TEntity>(dto);
            
            // Set CreatedAt for new entities
            SetCreatedAt(entity);
            
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<TDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = GetEntityId(entity) }, resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing entity
    /// </summary>
    [HttpPut("{id}")]
    public virtual async Task<ActionResult<TDto>> Update(int id, [FromBody] TDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return NotFound($"Entity with id {id} not found");
            }

            _mapper.Map(dto, entity);
            
            // Set UpdatedAt for updated entities
            SetUpdatedAt(entity);
            
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<TDto>(entity);
            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete an entity
    /// </summary>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return NotFound($"Entity with id {id} not found");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Get entity count
    /// </summary>
    [HttpGet("count")]
    public virtual async Task<ActionResult<int>> GetCount()
    {
        try
        {
            var count = await _dbSet.CountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Override this method to return the ID of the entity
    /// </summary>
    protected abstract object GetEntityId(TEntity entity);

    /// <summary>
    /// Override this method to set CreatedAt timestamp for new entities
    /// </summary>
    protected virtual void SetCreatedAt(TEntity entity)
    {
        // Override in derived classes if needed
    }

    /// <summary>
    /// Override this method to set UpdatedAt timestamp for updated entities
    /// </summary>
    protected virtual void SetUpdatedAt(TEntity entity)
    {
        // Override in derived classes if needed
    }

    /// <summary>
    /// Override this method to customize entity queries (e.g., include related data)
    /// </summary>
    protected virtual IQueryable<TEntity> GetQueryableWithIncludes()
    {
        return _dbSet;
    }
} 