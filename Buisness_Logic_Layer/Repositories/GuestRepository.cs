using Buisness_Logic_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class GuestRepository : GenericRepository<Guest>, IGuestRepository
    {
        private readonly DbContext _context;
        public GuestRepository(DbContext context) : base(context)
        {
            _context = context;
        }
    }
}
