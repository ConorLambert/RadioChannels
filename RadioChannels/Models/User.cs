
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RadioChannels.Models
{
	public class User : IdentityUser
	{
        [Key]
        public override string Id { get; set; }
        
        // public override string UserName { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public override string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public override string PasswordHash { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }

        /*
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        */
    }
}