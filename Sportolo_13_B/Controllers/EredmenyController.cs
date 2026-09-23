using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using SportoloEredmenyApi.Data;
using SportoloEredmenyApi.Models;

namespace SportoloEredmenyApi.Controllers;

[ApiController]
[Route("eredmeny")]
public class EredmenyController : ControllerBase
{
    private readonly DbConnectionFactory _dbFactory;

    public EredmenyController(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<ActionResult<List<Eredmeny>>> GetAll()
    {
        var list = new List<Eredmeny>();

        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand(
            "SELECT Id, Competition, Description, ResultTime, UpdateTime, SportoloId FROM eredmeny", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(MapEredmeny(reader));
        }

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Eredmeny>> GetById(int id)
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand(
            "SELECT Id, Competition, Description, ResultTime, UpdateTime, SportoloId FROM eredmeny WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return Ok(MapEredmeny(reader));
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Eredmeny>> Create(CreateEredmenyDto dto)
    {
        var now = DateTime.Now;

        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand(@"
            INSERT INTO eredmeny (Competition, Description, ResultTime, UpdateTime, SportoloId)
            VALUES (@competition, @description, @resultTime, @updateTime, @sportoloId);
            SELECT LAST_INSERT_ID();", conn);

        cmd.Parameters.AddWithValue("@competition", dto.Competition);
        cmd.Parameters.AddWithValue("@description", (object?)dto.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@resultTime", now);
        cmd.Parameters.AddWithValue("@updateTime", now);
        cmd.Parameters.AddWithValue("@sportoloId", dto.SportoloId);

        var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        var created = new Eredmeny
        {
            Id = newId,
            Competition = dto.Competition,
            Description = dto.Description,
            ResultTime = now,
            UpdateTime = now,
            SportoloId = dto.SportoloId
        };

        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEredmenyDto dto)
    {
        var now = DateTime.Now;

        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand(@"
            UPDATE eredmeny
            SET Competition = @competition,
                Description = @description,
                UpdateTime  = @updateTime,
                SportoloId  = @sportoloId
            WHERE Id = @id", conn);

        cmd.Parameters.AddWithValue("@competition", dto.Competition);
        cmd.Parameters.AddWithValue("@description", (object?)dto.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@updateTime", now);
        cmd.Parameters.AddWithValue("@sportoloId", dto.SportoloId);
        cmd.Parameters.AddWithValue("@id", id);

        var affected = await cmd.ExecuteNonQueryAsync();
        if (affected == 0) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("DELETE FROM eredmeny WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        var affected = await cmd.ExecuteNonQueryAsync();
        if (affected == 0) return NotFound();

        return NoContent();
    }

    [HttpGet("sportolo/{sportoloId:int}/nev-email")]
    public async Task<ActionResult<SportoloNameEmailDto>> GetSportoloNameEmail(int sportoloId)
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("SELECT name, email FROM sportolo WHERE Id = @id", conn);
        cmd.Parameters.AddWithValue("@id", sportoloId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return Ok(new SportoloNameEmailDto
            {
                Name = reader.GetString("name"),
                Email = reader.GetString("email")
            });
        }

        return NotFound();
    }

    [HttpGet("sportolo/{sportoloId:int}/eredmenyek")]
    public async Task<ActionResult<SportoloEredmenyeiDto>> GetSportoloEredmenyei(int sportoloId)
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var nameCmd = new MySqlCommand("SELECT name FROM sportolo WHERE Id = @id", conn);
        nameCmd.Parameters.AddWithValue("@id", sportoloId);
        var name = (string?)await nameCmd.ExecuteScalarAsync();
        if (name is null) return NotFound();

        var dto = new SportoloEredmenyeiDto { Name = name };

        using var cmd = new MySqlCommand(
            "SELECT Competition, Description FROM eredmeny WHERE SportoloId = @id", conn);
        cmd.Parameters.AddWithValue("@id", sportoloId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            dto.Eredmenyek.Add(new EredmenySzukitettDto
            {
                Competition = reader.GetString("Competition"),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString("Description")
            });
        }

        return Ok(dto);
    }

    [HttpGet("darabszam")]
    public async Task<ActionResult<int>> GetOsszesEredmenySzama()
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM eredmeny", conn);
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return Ok(count);
    }

    [HttpGet("sportolo/{sportoloId:int}/darabszam")]
    public async Task<ActionResult<int>> GetSportoloEredmenySzama(int sportoloId)
    {
        using var conn = _dbFactory.CreateConnection();
        await conn.OpenAsync();

        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM eredmeny WHERE SportoloId = @id", conn);
        cmd.Parameters.AddWithValue("@id", sportoloId);
        var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return Ok(count);
    }

    private static Eredmeny MapEredmeny(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("Id"),
        Competition = reader.GetString("Competition"),
        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
        ResultTime = reader.GetDateTime("ResultTime"),
        UpdateTime = reader.GetDateTime("UpdateTime"),
        SportoloId = reader.GetInt32("SportoloId")
    };
}
