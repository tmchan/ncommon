#region license
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

namespace NCommon.Data
{
    /// <summary>
    /// A unit of work contract that that commits / flushes changes to the store
    /// within a single transaction. The commit / flush should be called when the
    /// implementation is disposed.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region properties
        /// <summary>
        /// Gets a boolean value indicating whether the current unit of work is running under
        /// a transaction.
        /// </summary>
        bool IsInTransaction { get; }
        #endregion

        #region methods

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction.
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();

        /// <summary>
        /// Instructs the <see cref="IUnitOfWork"/> instance to begin a new transaction
        /// with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">One of the values of <see cref="IsolationLevel"/>
        /// that specifies the isolation level of the transaction.</param>
        /// <returns></returns>
        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store.
        /// </summary>
        void Flush();

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction.
        /// </summary>
        void TransactionalFlush();

        /// <summary>
        /// Flushes the changes made in the unit of work to the data store
        /// within a transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel"></param>
        void TransactionalFlush(IsolationLevel isolationLevel);

        #endregion
    }
}