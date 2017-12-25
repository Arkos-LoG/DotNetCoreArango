using BorderEast.ArangoDB.Client;
using BorderEast.ArangoDB.Client.Models;
using BorderEast.ASPNetCore.Identity.ArangoDB.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreArango.Data
{
    public class ArangoContactStore : StoreBase, IContactStore
    {
        public ArangoContactStore(IArangoClient arangoClient)
          : base(arangoClient)
        {
            client = arangoClient;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            var result = await client.DB().GetAllAsync<T>();
            return result;
        }

        public async Task<UpdatedDocument<T>> InsertAsync<T>(T entity)
        {
            var result = await client.DB().InsertAsync<T>(entity);
            return result;
        }
    }
}
