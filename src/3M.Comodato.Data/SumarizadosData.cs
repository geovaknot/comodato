using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class SumarizadosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public SumarizadosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(SumarizadosEntity sumarizado)
        {
            if (sumarizado.DT_INICIAL is null)
                sumarizado.DT_INICIAL = DateTime.Now.AddMonths(-1);
            if (sumarizado.DT_FINAL is null)
                sumarizado.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioPedidosSumarizado");

                if (sumarizado.DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, sumarizado.DT_INICIAL);

                if (sumarizado.DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, sumarizado.DT_FINAL);
                                
                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

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