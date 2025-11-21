namespace backend.Models;

public partial class Achievement
{
    public ushort Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Image { get; set; } = null!;

    public bool Status { get; set; } = false;

    public Achievement(ushort id, string name, string description, string image)
    {
        Id = id;
        Name = name;
        Description = description;
        Image = image;
    }
}