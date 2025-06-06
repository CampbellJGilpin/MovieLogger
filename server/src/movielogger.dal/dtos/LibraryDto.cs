namespace movielogger.dal.dtos;

public class LibraryDto
{
    public int UserId { get; set; }
    public ICollection<LibraryItemDto> LibraryItems{ get; set; } = new List<LibraryItemDto>();
}