using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BorderEast.ASPNetCore.Identity.ArangoDB;
using Newtonsoft.Json;

namespace DotNetCoreArango.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    [JsonObject(Id = "IdentityUser")]
    public class ApplicationUser : IdentityUser
    {
    }
}
