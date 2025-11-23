using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Interfaces
{
    public interface IRoomRepository: IGenericRepository<Room> 
    {
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime from, DateTime to, int? roomTypeId = null);
    }
}
