using Microsoft.AspNetCore.Mvc;

namespace TestDatabase.Functionality{
  public class VereficationFunctionality:ControllerBase {
      public async Task<IActionResult> UserVerefication(User user){
        if (user == null)
        {
          return Unauthorized(new { status = "error", error = "user not found" });
        }
        if(user.IsAccountDeleted){
          return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
        }
        return null;
      }
  }
}