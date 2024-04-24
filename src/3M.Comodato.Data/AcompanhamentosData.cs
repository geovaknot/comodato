using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class AcompanhamentosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public AcompanhamentosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(List<string> lstCliente)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioAcompanhamentosSelect");

                if (lstCliente.Count > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, string.Join(",", lstCliente));

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 300;

                dataSet = _db.ExecuteDataSet(dbCommand);
                //dataTable = dataSet.Tables[0];
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
            return dataSet;
        }
    }
}