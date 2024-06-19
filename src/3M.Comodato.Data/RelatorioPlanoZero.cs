using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;

namespace _3M.Comodato.Data
{
    public class RelatorioPlanoZeroData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public RelatorioPlanoZeroData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterRelatorio(string Periodo)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptRelatorioPlanoZero");
                if (Periodo != null)
                {
                    _db.AddInParameter(dbCommand, "@p_PERIODO", DbType.String, Periodo);
                }
                

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dbCommand.CommandTimeout = 1800;

                var tp1 = _db.ExecuteDataSet(dbCommand);

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
