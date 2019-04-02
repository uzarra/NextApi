using System;
using System.Threading.Tasks;
using Abitech.NextApi.Server.EfCore.DAL;
using Abitech.NextApi.Server.EfCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Abitech.NextApi.Server.EfCore.Service
{
    public class ColumnChangesLogger<TContext> : IColumnChangesLogger
        where TContext : class, IColumnChangesEnabledNextApiDbContext
    {
        private readonly TContext _context;

        public ColumnChangesLogger(TContext context)
        {
            _context = context;
        }

        public bool LoggingEnabled
        {
            get => _context.ColumnChangesLogEnabled;
            set => _context.ColumnChangesLogEnabled = value;
        }

        public async Task<DateTimeOffset?> GetLastChange(string tableName, string columnName, Guid rowGuid)
        {
            return (await _context.ColumnChangesLogs.FirstOrDefaultAsync(e =>
                    e.RowGuid == rowGuid &&
                    e.TableName == tableName &&
                    e.ColumnName == columnName)
                )?.LastChangedOn;
        }

        public async Task SetLastChange(string tableName, string columnName, Guid rowGuid, DateTimeOffset time)
        {
            await _context.ColumnChangesLogs.SetLastColumnChange(tableName, columnName, rowGuid, time);
        }
    }

    /// <summary>
    /// Implements service for column changes logging
    /// </summary>
    public interface IColumnChangesLogger
    {
        /// <summary>
        /// Indicates that logger enabled for current scope
        /// </summary>
        bool LoggingEnabled { get; set; }

        /// <summary>
        /// Returns time when a column of specific entity last changed
        /// </summary>
        /// <param name="tableName">Name of entity table</param>
        /// <param name="columnName">Name of entity property</param>
        /// <param name="rowGuid">Guid of row</param>
        /// <returns></returns>
        Task<DateTimeOffset?> GetLastChange(string tableName, string columnName, Guid rowGuid);

        /// <summary>
        /// Sets time when a column of specific entity last changed
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="rowGuid"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Task SetLastChange(string tableName, string columnName, Guid rowGuid, DateTimeOffset time);
    }
}
