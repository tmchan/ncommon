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
using NCommon.Storage;
using NHibernate;

namespace NCommon.Data.NHibernate
{
    /// <summary>
    /// Implements the <see cref="IUnitOfWorkFactory"/> interface to provide an implementation of a factory
    /// that creates <see cref="NHUnitOfWork"/> instances.
    /// </summary>
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region methods
        /// <summary>
        /// Get delegate used to build ISession instances.
        /// </summary>
        private static Func<ISession> _sessionProvider;
        private static readonly object _sessionProviderLock = new object();
        #endregion

        #region methods
        /// <summary>
        /// Sets the delegate to be used for building <see cref="ISession"/> instances.
        /// </summary>
        /// <param name="sessionProvider">The delegate used for building <see cref="ISession"/> instances.</param>
        public static void SetSessionProvider (Func<ISession> sessionProvider)
        {
            lock(_sessionProviderLock)
                _sessionProvider = sessionProvider;
        }
        #endregion

        #region Implementation of IUnitOfWorkFactory
        /// <summary>
        /// Creates a new instance of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork Create()
        {
            Guard.Against<InvalidOperationException>(_sessionProvider == null,
                                                    "A ISession provider has not been specified. Please specify set a " +
                                                     "provider using SetSessuibProvider before creating NHUnitOfWork instances");
            return new NHUnitOfWork(_sessionProvider());
        }
        #endregion
    }
}