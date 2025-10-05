using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models.Utilities.SeedGenerator;
using Models;

namespace DbModels;

public class AttractionCategoriesDbM : AttractionCategories
{
    [Key]
    public override Guid AttractionCategoryId { get; set; }
    [NotMapped]
    public override IAttractions Attractions { get; set; }
    [NotMapped]
    public override ICategories Categories { get; set; }
}
