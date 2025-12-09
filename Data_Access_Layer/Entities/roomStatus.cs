using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    public enum roomStatus
    {
        Available = 1,
        Occupied = 2,
        UnderMaintenance = 3,
        Cleaning = 4,
        Reserved = 5
    }
}
