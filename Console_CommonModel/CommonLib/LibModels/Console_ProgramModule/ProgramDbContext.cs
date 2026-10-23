using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ProgramModule
{
    public class ProgramDbContext : FileDbContext
    {

        public DbSet<ProgramFile> ProgramFiles { get; set; }

    }
}
