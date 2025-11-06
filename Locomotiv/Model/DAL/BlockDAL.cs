using Locomotiv.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Model.DAL
{
    public class BlockDAL : IBlockDAL
    {
        private readonly ApplicationDbContext _context;
        // Implementation for Train Data Access Layer

        public BlockDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Block> GetAllBlocks()
        {
            return _context.Blocks.ToList();
        }




    }
}
