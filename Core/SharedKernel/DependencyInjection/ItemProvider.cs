using System;
using System.Threading.Tasks;

namespace Chest.Core.DependencyInjection
{
    public interface IItemProvider <T>
    {
        Task<T[]> GetAllItems() ;
        Task<T> GetItemById (Guid itemId) ; 
        Task DeleteItem (T item) ;
        Task UpdateItem (T oldItem, T newItem) ;
        Task AddItem (T item) ;
        Task DeleteAllItems () ;
    }
}