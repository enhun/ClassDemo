using System.ComponentModel.DataAnnotations;

namespace Model_CoreFirst_Home.Models
{
    public class Login
    {
        [Key]
        [Display(Name = "帳號")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "帳號為5-10碼")]
        [RegularExpression("[A-Za-z][A-Za-z0-9]{4,9}", ErrorMessage = "帳號格式有誤")]
        [Required(ErrorMessage = "必填")]
        public string Account { get; set; }

    [Display(Name = "密碼")]
    [Required(ErrorMessage = "必填")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "密碼為8-16碼")]
    [MinLength(8)]
    [MaxLength(16)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    }
}
