

using System.ComponentModel.DataAnnotations;

public class BlogRequest
{
    [Required, MaxLength(250)]
    public string Title { get; set; }

    public string Description { get; set; }
}
