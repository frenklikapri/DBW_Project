using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleSharing.Core.Interfaces
{
    public interface IInactiveFileRemoverService
    {
        Task<bool> DeleteInactiveFilesAsync();
    }
}
