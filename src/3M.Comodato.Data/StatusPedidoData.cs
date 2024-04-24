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
    public class StatusPedidoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public StatusPedidoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public DataTable ObterLista(StatusPedidoEntity statusPedido)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusPedidoSelect");

                if (statusPedido.ID_STATUS_PEDIDO != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_STATUS_PEDIDO", DbType.Int64, statusPedido.ID_STATUS_PEDIDO);

                if (!string.IsNullOrEmpty(statusPedido.DS_STATUS_PEDIDO))
                    _db.AddInParameter(dbCommand, "@p_DS_STATUS_PEDIDO", DbType.String, statusPedido.DS_STATUS_PEDIDO);

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

        public DataTable ObterListaStatus(string statusCarregar)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcStatusPedidoSelectStatus");

                _db.AddInParameter(dbCommand, "@p_StatusCarregar", DbType.String, statusCarregar);

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


        public IList<StatusPedidoSincEntity> ObterListaStatusPedidoSinc()
        {
            try
            {
                IList<StatusPedidoSincEntity> listaStatusPedido = new List<StatusPedidoSincEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select * from tbStatusPedido ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            StatusPedidoSincEntity sp = new StatusPedidoSincEntity();
                            sp.ID_STATUS_PEDIDO = Convert.ToInt64(SDR["id_status_pedido"]);
                            sp.DS_STATUS_PEDIDO = Convert.ToString(SDR["ds_status_pedido"] is DBNull ? "" : SDR["ds_status_pedido"].ToString());
                            sp.DS_STATUS_PEDIDO_ACAO = Convert.ToString(SDR["ds_status_pedido_acao"] is DBNull ? "" : SDR["ds_status_pedido_acao"].ToString());

                            listaStatusPedido.Add(sp);
                        }
                        cnx.Close();
                        return listaStatusPedido;
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
