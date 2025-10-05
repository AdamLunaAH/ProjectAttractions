using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models.Utilities.SeedGenerator;
using Models;
using Models.DTO;


namespace DbModels;

[Table("CategoriesDb", Schema = "supusr")]
[Index(nameof(CategoryName), IsUnique = true)]
public class CategoriesDbM : Categories, ISeed<CategoriesDbM>, IEquatable<CategoriesDbM>
{
    [Key]
    public override Guid CategoryId { get; set; }
    [Required]
    public override string CategoryName { get; set; }

    #region implementing entity Navigation properties when model is using interfaces in the relationships between models
    [NotMapped]
    public override List<IAttractions> Attractions { get => AttractionsDbM?.ToList<IAttractions>(); set => new NotImplementedException(); }
    [JsonIgnore]
    public List<AttractionsDbM> AttractionsDbM { get; set; } = null;
    #endregion


    #region implementing IEquatable
    public bool Equals(CategoriesDbM other) => (other != null) && ((CategoryName, CategoryId) ==
        (other.CategoryName, other.CategoryId));

    public override bool Equals(object obj) => Equals(obj as CategoriesDbM);
    public override int GetHashCode() => (CategoryName, CategoryId).GetHashCode();
    #endregion


    #region seeder
    public override CategoriesDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }
    #endregion

    #region Update from DTO
    public CategoriesDbM UpdateFromDTO(CategoriesCuDto org)
    {
        if (org == null) return null;

        CategoryName = org.CategoryName;

        return this;
    }
    #endregion

    #region constructor
    public CategoriesDbM() { }

    public CategoriesDbM(CategoriesCuDto org)
    {
        CategoryId = Guid.NewGuid();
        UpdateFromDTO(org);
    }

    #endregion
}