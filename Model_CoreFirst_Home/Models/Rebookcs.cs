
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model_CoreFirst_Home.Models
{
    public class ReBook
    {
        [Display(Name = "編號")]
        [StringLength(36, MinimumLength = 36)]
        [Key]
        public string ReBookID { get; set; } = null!;

        [Display(Name = "回覆內容")]
        [Required(ErrorMessage = "必填")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = null!;

        [Display(Name = "回覆者")]
        [StringLength(20, ErrorMessage = "回覆者最多20字")]
        [Required(ErrorMessage = "必填")]
        public string Author { get; set; } = null!;

        [Display(Name = "回覆時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd  hh:mm:ss}")]
        [HiddenInput]
        public DateTime TimeStamp { get; set; }

        // 關聯到主貼文的外鍵
        [ForeignKey("Book")]
        public string BookID { get; set; } = null!;
        public virtual Book Book { get; set; } = null!;
    }
}
    

