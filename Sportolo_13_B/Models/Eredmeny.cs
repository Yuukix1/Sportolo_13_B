namespace SportoloEredmenyApi.Models;

/// <summary>
/// Az eredmeny tábla egy sorát reprezentálja.
/// </summary>
public class Eredmeny
{
    public int Id { get; set; }
    public string Competition { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ResultTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public int SportoloId { get; set; }
}
