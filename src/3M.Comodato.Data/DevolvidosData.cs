using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class DevolvidosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public DevolvidosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataSet ObterLista(DevolvidosEntity devolvido)
        {
            if (devolvido.DT_DEV_INICIAL is null)
                devolvido.DT_DEV_INICIAL = DateTime.Now.AddMonths(-1);
            if (devolvido.DT_DEV_FINAL is null)
                devolvido.DT_DEV_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            //DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDevolvidosSelect");

                if (devolvido.DT_DEV_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEV_INICIAL", DbType.Date, devolvido.DT_DEV_INICIAL);

                if (devolvido.DT_DEV_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DEV_FINAL", DbType.Date, devolvido.DT_DEV_FINAL);

                if (devolvido.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, devolvido.CD_CLIENTE);

                if (devolvido.CD_VENDEDOR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, devolvido.CD_VENDEDOR);

                if (!String.IsNullOrEmpty(devolvido.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, devolvido.CD_GRUPO);

                if (!String.IsNullOrEmpty(devolvido.CD_MOTIVO_DEVOLUCAO))
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.String, devolvido.CD_MOTIVO_DEVOLUCAO);

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