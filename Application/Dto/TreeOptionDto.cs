namespace Application.Dto;

public class TreeOptionDto: DropDownOptionDto
{
    public ICollection<TreeOptionDto> Children { get; set; } 
}