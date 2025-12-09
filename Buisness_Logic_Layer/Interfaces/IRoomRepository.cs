using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
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

        Task<Room> GetFirstFreeRoom(int roomTypeId);

        Task<bool> LockRoom(int roomId, int minutes = 15);

        Task UnlockRoom(int roomId);

        Task CleanExpiredLocks();


        Task<Room> GetRoomWithTypeAsync(int roomId);
        
    }
}
