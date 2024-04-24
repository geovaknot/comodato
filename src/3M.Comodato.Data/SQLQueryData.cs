using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class SQLQueryData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public SQLQueryData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterDados(string comandoSQL)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                //dbCommand = _db.GetStoredProcCommand("prcTesteSelect");
                dbCommand = _db.GetSqlStringCommand(comandoSQL);
                //if (!string.IsNullOrEmpty(nomeProc))
                //{
                //    _db.AddInParameter(dbCommand, "@p_NOMEOBJ", DbType.String, nomeProc);
                //}

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 600;

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }
    }
}
