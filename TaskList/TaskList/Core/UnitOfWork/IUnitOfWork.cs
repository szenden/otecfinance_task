using System;
using System.Threading.Tasks;
using TaskList.Core.Repositories;

namespace TaskList.Core.UnitOfWork
{
    /// <summary>
    /// Interface for managing unit of work operations in the TaskList application
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the TaskList repository instance
        /// </summary>
        ITaskListRepository TaskListRepository { get; }

        /// <summary>
        /// Saves all pending changes to the data store
        /// </summary>
        /// <returns>The number of objects written to the data store</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current database transaction
        /// </summary>
        Task RollbackTransactionAsync();
    }
}