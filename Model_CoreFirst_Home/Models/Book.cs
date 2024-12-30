namespace Model_CoreFirst_Home.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

public class Book
{
    [Display(Name = "編號")]
    [StringLength(36, MinimumLength = 36)]
    [Key]
    public string BookID { get; set; } = null!;  // 使用 GUID 作為唯一識別碼

    [Display(Name = "主題")]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "主題1~30個字")]
    [Required(ErrorMessage = "必填")]
    public string Title { get; set; } = null!;

    [Display(Name = "內容")]
    [Required(ErrorMessage = "必填")]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; } = null!;

    [Display(Name = "發表人")]
    [StringLength(20, ErrorMessage = "發表人最多20字")]
    [Required(ErrorMessage = "必填")]
    public string Author { get; set; } = null!;

    [Display(Name = "照片")]
    [StringLength(44)]
    public string? Photo { get; set; }

    [Display(Name = "上傳圖片")]
    [NotMapped] // 這個屬性不會映射到資料庫
    public IFormFile? ImageFile { get; set; }

    [HiddenInput]
    public string? ImageType { get; set; }

    [Display(Name = "發布時間")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd  hh:mm:ss}")]
    [HiddenInput]
    public DateTime TimeStamp { get; set; }

    // 關聯屬性：一個貼文可以有多個回覆
    public virtual List<ReBook>? ReBooks { get; set; }
}