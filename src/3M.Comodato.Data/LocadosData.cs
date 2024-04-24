using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class LocadosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public LocadosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(LocadosEntity locado)
        {
            if (locado.DT_INICIAL is null)
                locado.DT_INICIAL = DateTime.Now.AddMonths(-1);
            if (locado.DT_FINAL is null)
                locado.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioLocadosSelect");

                if (locado.DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, locado.DT_INICIAL);

                if (locado.DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, locado.DT_FINAL);

                if (locado.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, locado.CD_CLIENTE);

                if (locado.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, locado.CD_VENDEDOR);

                if (!String.IsNullOrEmpty(locado.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, locado.CD_GRUPO);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
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