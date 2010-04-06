using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace vlko.web.ViewModel.Account
{
    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Email address")]
        public string Email { get; set; }
    }
}
