using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3M.Comodato.Data
{
    public class MotivoDevolucaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public MotivoDevolucaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(MotivoDevolucaoEntity motivoDevolucao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcMotivoDevolucaoSelect");

                if (!string.IsNullOrEmpty(motivoDevolucao.CD_MOTIVO_DEVOLUCAO))
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, motivoDevolucao.CD_MOTIVO_DEVOLUCAO);

                if (!string.IsNullOrEmpty(motivoDevolucao.DS_MOTIVO_DEVOLUCAO))
                    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO_DEVOLUCAO", DbType.String, motivoDevolucao.DS_MOTIVO_DEVOLUCAO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
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
