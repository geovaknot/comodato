using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class PedidosData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public PedidosData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(PedidosEntity pedido)
        {
            if (pedido.DT_INICIAL is null)
                pedido.DT_INICIAL = DateTime.Now.AddMonths(-1);
            if (pedido.DT_FINAL is null)
                pedido.DT_FINAL = DateTime.Now;

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRelatorioPedidosSelect");

                if (pedido.DT_INICIAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_INICIAL", DbType.Date, pedido.DT_INICIAL);

                if (pedido.DT_FINAL != null)
                    _db.AddInParameter(dbCommand, "@p_DT_FINAL", DbType.Date, pedido.DT_FINAL);

                if (pedido.cliente.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Decimal, pedido.cliente.CD_CLIENTE);

                //if (pedido.CD_VENDEDOR > 0)
                //    _db.AddInParameter(dbCommand, "@p_CD_VENDEDOR", DbType.Decimal, pedido.CD_VENDEDOR);

                if (!string.IsNullOrEmpty(pedido.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pedido.tecnico.CD_TECNICO);

                if (!String.IsNullOrEmpty(pedido.grupo.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, pedido.grupo.CD_GRUPO);
                                
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