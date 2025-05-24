using JuntoChallenge.Application.Interfaces;
using JuntoChallenge.Domain.Entities;
using JuntoChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuntoChallenge.Application.Services
{
    public class LogService : ILogService
    {
        private readonly DataContext _context;
        public LogService(DataContext context) 
        {
            _context = context;
        }

        public async Task LogAsync(string level, string message, Exception? ex = null)
        {
            var entry = new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message,
                Exception = ex != null ? ex?.ToString() : ""
            };

            _context.Logs.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}
