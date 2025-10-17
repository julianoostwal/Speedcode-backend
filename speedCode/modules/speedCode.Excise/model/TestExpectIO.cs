using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace speedCode.Excise.model;

[Table("testExpectIO")]
public class TestExpectIO
{
    [Key] public Guid id { get; set; }

    public string? input { get; set; }
    public string output { get; set; }

    [Required] public Guid testCodeId { get; set; }

    [ForeignKey("testCodeId")] public virtual TestCode TestCode { get; set; }
}