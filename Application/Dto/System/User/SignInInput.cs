using System.ComponentModel.DataAnnotations;

namespace Application.Dto.System.User;

public class SignInInput
{
    [MinLength(6)]
    public string UserName { get; set; }
    
    [MinLength(6)]
    public string password { get; set; }
}