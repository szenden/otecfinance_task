using System;
using System.Threading.Tasks;
using TaskList.Core.Repositories;

namespace TaskList.Core.UnitOfWork
{
    /// <summary>
    /// Implementation of the Unit of Work pattern for managing data operations and transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The task list repository instance
        /// </summary>
        private readonly ITaskListRepository _taskListRepository;

        /// <summary>
        /// Flag indicating whether the object has been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class
        /// </summary>
        /// <param name="taskListRepository">The task list repository to use</param>
        public UnitOfWork(ITaskListRepository taskListRepository)
        {
            _taskListRepository = taskListRepository;
        }

        /// <summary>
        /// Gets the TaskList repository instance
        /// </summary>
        public ITaskListRepository TaskListRepository => _taskListRepository;

        /// <summary>
        /// Saves all pending changes to the data store
        /// </summary>
        /// <returns>The number of objects written to the data store</returns>
        public async Task<int> SaveChangesAsync()
        {
            // In this in-memory implementation, we don't need to save changes
            return await Task.FromResult(1);
        }

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            // In this in-memory implementation, we don't need transaction management
            await Task.CompletedTask;
        }

        /// <summary>
        /// Commits the current database transaction
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            // In this in-memory implementation, we don't need transaction management
            await Task.CompletedTask;
        }

        /// <summary>
        /// Rolls back the current database transaction
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            // In this in-memory implementation, we don't need transaction management
            await Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the UnitOfWork instance
        /// </summary>
        /// <param name="disposing">True to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Clean up any resources here
                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes the UnitOfWork instance and suppresses finalization
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}