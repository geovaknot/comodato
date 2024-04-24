using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class RegiaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public RegiaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(RegiaoEntity regiao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcV_REGIAOSelect");

                if (!string.IsNullOrEmpty(regiao.CD_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_CD_REGIAO", DbType.String, regiao.CD_REGIAO);

                if (!string.IsNullOrEmpty(regiao.DS_REGIAO))
                    _db.AddInParameter(dbCommand, "@p_DS_REGIAO", DbType.String, regiao.DS_REGIAO);

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
