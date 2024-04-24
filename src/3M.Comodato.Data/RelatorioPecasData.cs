using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class RelatorioPecasData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public RelatorioPecasData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterRelatorio(string StatusRelatorio)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRptRelatorioPecas");
                if (StatusRelatorio != null)
                {
                    _db.AddInParameter(dbCommand, "@p_Status", DbType.String, StatusRelatorio);
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
