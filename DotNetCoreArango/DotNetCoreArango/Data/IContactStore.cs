using BorderEast.ArangoDB.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreArango.Data
{
    public interface IContactStore
    {
        Task<UpdatedDocument<T>> InsertAsync<T>(T entity);
        Task<IEnumerable<T>> GetAllAsync<T>();
        Task<IEnumerable<T>> GetAllFilteredByAsync<T>(string filter, string value);
    }
}
