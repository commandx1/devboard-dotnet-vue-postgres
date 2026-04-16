namespace DevBoard.Api.Domain.Entities;

public class Tag
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#2563eb";

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
