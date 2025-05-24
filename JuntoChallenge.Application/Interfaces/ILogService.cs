using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuntoChallenge.Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(string level, string message, Exception? ex = null);
    }
}
