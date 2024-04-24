using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;

namespace _3M.Comodato.Data
{
    public class TransactionData : IDisposable
    {
        private bool _completed = false;
        internal Database _db;
        internal DbConnection _connection;
        internal DbTransaction _transaction;

        public TransactionData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
            _connection = _db.CreateConnection();
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                if (!_completed)
                {
                    this._transaction.Commit();
                    _completed = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            try
            {
                this.Rollback();
            }
            finally
            {
                if (this._connection != null)
                {
                    if (this._connection.State == ConnectionState.Open)
                    {
                        this._connection.Close();
                    }
                    this._connection.Dispose();

                }

                this._transaction = null;
                this._connection = null;
                this._db = null;
            }
        }

        public void Rollback()
        {
            try
            {
                if (!_completed)
                {
                    this._transaction.Rollback();
                    _completed = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
