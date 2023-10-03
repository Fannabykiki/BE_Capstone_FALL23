using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();
        void RollBack();
    }
}
