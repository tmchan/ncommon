﻿#region license
//Copyright 2008 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Data;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWork"/> interface to provide an implementation
    /// of a IUnitOfWork that uses NHibernate to query and update the underlying store.
    /// </summary>
    public class NHUnitOfWork : IUnitOfWork
    {
        #region fields
        private bool _disposed;
        private ISession _session; // The session used by the <see cref="NHUnitOfWork"/> instance.
        private NHTransaction _transaction; // The current transaction under which the UnitOfWork instance is operating.
        #endregion

        #region ctor
        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="NHUnitOfWork"/> that uses the provided
        /// NHibernate <see cref="ISession"/> instance.
        /// </summary>
        /// <param name="session">The NHiberante <see cref="ISession"/> instance to use.</param>
        public NHUnitOfWork(ISession session)
        {
            Guard.Against<ArgumentNullException>(session == null, "Cannot create a NHUnitOfWork that uses a null reference ISession instance.");
            _session = session;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the <see cref="ISession"/> that the <see cref="NHUnitOfWork"/> wraps.
        /// </summary>
        public ISession Session
        {
            get { return _session; }
        } 
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes off managed resources used by the NHUnitOfWork instance.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose (bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;

            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;    
            }
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
            _disposed = true;
        }
        #endregion

        #region Implementation of IUnitOfWork
        /// <summary>
        /// Gets a boolean value indicating whether the current unit of work is running under
        /// a transaction.
        /// </summary>
        public bool IsInTransaction
        {
            get { return _transaction != null; }
        }

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted); //Default isolation is ReadCommitted
        }

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction
        /// with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/>
        /// that specifies the isolation level of the transaction.</param>
        /// <returns></returns>
        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            Guard.Against<InvalidOperationException>(_transaction != null,
                                                     "Cannot begin a new transaction while an existing transaction is still running. " + 
                                                     "Please commit or rollback the existing transaction before starting a new one.");
            _transaction = new NHTransaction(_session.BeginTransaction(isolationLevel));
            _transaction.TransactionCommitted += TransactionCommitted;
            _transaction.TransactionRolledback += TransactionRolledback;
            return _transaction;
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        public void Flush()
        {
            _session.Flush(); //Flush the underlying session.
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction.
        /// </summary>
        public void TransactionalFlush()
        {
            TransactionalFlush(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void TransactionalFlush(IsolationLevel isolationLevel)
        {
            // Start a transaction if one isn't already running.
            if (!IsInTransaction)
                BeginTransaction(isolationLevel);
            try
            {
                _session.Flush();
                _transaction.Commit();
            }
            catch 
            {
                _transaction.Rollback();
                throw;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Handles the <see cref="ITransaction.TransactionRolledback"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRolledback(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                        "Expected the sender of TransactionRolledback event to be the transaction that was created by the NHUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Handles the <see cref="ITransaction.TransactionCommitted"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionCommitted(object sender, EventArgs e)
        {
            Guard.IsEqual<InvalidOperationException>(sender, _transaction,
                "Expected the sender of TransactionComitted event to be the transaction that was created by the NHUnitOfWork instance.");
            ReleaseCurrentTransaction();
        }

        /// <summary>
        /// Releases the current transaction in the <see cref="NHUnitOfWork"/> instance.
        /// </summary>
        private void ReleaseCurrentTransaction()
        {
            if (_transaction != null)
            {
                _transaction.TransactionCommitted -= TransactionCommitted;
                _transaction.TransactionRolledback -= TransactionRolledback;
                _transaction.Dispose();
            }
            _transaction = null;
        }
        #endregion
    }
}
