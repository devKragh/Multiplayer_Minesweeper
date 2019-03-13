using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MineSweeper.WebHost.Models
{
    /// <summary>
    /// Class used in Minesweeper
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter account username.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

		private bool wrongLoginInfo = false;
		public bool WrongLoginInfo { get { return wrongLoginInfo; } set { wrongLoginInfo = value; } }
	}

    /// <summary>
    /// Class used in Minesweeper
    /// </summary>
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

		public bool AlreadyExist { get; set; }
	}

    public class AccountDetailsViewModel
    {
        public string Username { get; set; }
        public int Rankpoints { get; set; }
    }

    public class AccountEditDetailsViewModel
    {
        [Required]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        //[Compare("NewPassord", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
