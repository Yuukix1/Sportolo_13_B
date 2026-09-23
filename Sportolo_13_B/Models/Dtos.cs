namespace SportoloEredmenyApi.Models;

public class CreateEredmenyDto
{
    public string Competition { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SportoloId { get; set; }
}

public class UpdateEredmenyDto
{
    public string Competition { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SportoloId { get; set; }
}

public class SportoloNameEmailDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class EredmenySzukitettDto
{
    public string Competition { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SportoloEredmenyeiDto
{
    public string Name { get; set; } = string.Empty;
    public List<EredmenySzukitettDto> Eredmenyek { get; set; } = new();
}
