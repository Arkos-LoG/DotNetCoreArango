using BorderEast.ArangoDB.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreArango.Data.Entities
{
    public class Contact : ArangoBaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserEmail { get; set; }
    }
}
