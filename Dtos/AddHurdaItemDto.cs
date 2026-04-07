namespace HurdaApi.Dtos;

public class AddHurdaItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ScrapAmount { get; set; }
}
