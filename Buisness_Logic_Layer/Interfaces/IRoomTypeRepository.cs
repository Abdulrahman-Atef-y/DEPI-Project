using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Interfaces
{
    public interface IRoomTypeRepository: IGenericRepository<RoomType>
    {
        Task<List<Review>> GetRoomTypeReviewsAsync(int roomTypeId);
    }
}
