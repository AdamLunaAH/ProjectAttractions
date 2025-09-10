using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Seido.Utilities.SeedGenerator;
using Models;

namespace DbModels;

public class UsersDbM : Users, ISeed<UsersDbM>
{
    // public override Guid UserId { get => base.UserId; set => base.UserId = value; }
    [Key]
    public override Guid UserId { get; set; }
    public override string FirstName { get; set; }
    public override string LastName { get; set; }
    public override DateTime CreatedAt { get; set; }
    public override DateTime UpdatedAt { get; set; }

    #region constructors
    public UsersDbM() { }
    #endregion



    public new UsersDbM Seed(SeedGenerator seeder)
    {
        base.Seed(seeder);
        return this;
    }

}