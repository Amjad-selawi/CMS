using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

public class CustomUserValidator : UserValidator<IdentityUser>
{
    public CustomUserValidator(IdentityErrorDescriber errors) : base(errors) { }

    public override async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
    {
        var result = await base.ValidateAsync(manager, user);

        // Remove the check for unique username
        var otherUser = await manager.FindByNameAsync(user.UserName);
        if (otherUser != null && !string.Equals(await manager.GetUserIdAsync(otherUser), await manager.GetUserIdAsync(user)))
        {
            var duplicateUserNameError = result.Errors.FirstOrDefault(e => e.Code == "DuplicateUserName");
            if (duplicateUserNameError != null)
            {
                result = IdentityResult.Success;
            }
        }

        return result;
    }
}
