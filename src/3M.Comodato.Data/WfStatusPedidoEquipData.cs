using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class WfStatusPedidoEquipData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public WfStatusPedidoEquipData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<WfStatusPedidoEquipEntity> ObterListaEntity(WfStatusPedidoEquipEntity entity)
        {
            Func<DataRow, WfStatusPedidoEquipEntity> Converter = new Func<DataRow, WfStatusPedidoEquipEntity>((r) =>
            {
                WfStatusPedidoEquipEntity tpStatusPedido = new WfStatusPedidoEquipEntity();
                tpStatusPedido.ID_TP_STATUS_PEDIDO_EQUIP = Convert.ToInt64(r["ID_TP_STATUS_PEDIDO_EQUIP"]);
                tpStatusPedido.ST_STATUS_PEDIDO = Convert.ToInt32(r["ST_STATUS_PEDIDO"]);
                tpStatusPedido.DS_STATUS_NOME_REDUZ = Convert.ToString(r["DS_STATUS_NOME_REDUZ"]).Trim();

                if (r["DS_STATUS_NOME"] != DBNull.Value)
                {
                    tpStatusPedido.DS_STATUS_NOME = Convert.ToString(r["DS_STATUS_NOME"]).Trim();
                }

                if (r["DS_STATUS_DESCRICAO"] != DBNull.Value)
                {
                    tpStatusPedido.DS_STATUS_DESCRICAO = Convert.ToString(r["DS_STATUS_DESCRICAO"]).Trim();
                }

                if (r["TP_PEDIDO"] != DBNull.Value)
                {
                    tpStatusPedido.TP_PEDIDO = Convert.ToString(r["TP_PEDIDO"]).Trim();
                }

                return tpStatusPedido;
            });

            DataTable dataTable = this.ObterLista(entity);
            return (from r in dataTable.Rows.Cast<DataRow>() select Converter(r)).ToList();
        }

        public DataTable ObterLista(WfStatusPedidoEquipEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfStatusPedidoEquipSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_TP_STATUS_PEDIDO_EQUIP != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_TP_STATUS_PEDIDO_EQUIP", DbType.Int64, entity.ID_TP_STATUS_PEDIDO_EQUIP);
                    }

                    if (entity.ST_STATUS_PEDIDO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int32, entity.ST_STATUS_PEDIDO);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_STATUS_NOME_REDUZ))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_NOME_REDUZ", DbType.String, entity.DS_STATUS_NOME_REDUZ);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_STATUS_NOME))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_NOME", DbType.String, entity.DS_STATUS_NOME);
                    }
                    if (!string.IsNullOrEmpty(entity.DS_STATUS_DESCRICAO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_STATUS_DESCRICAO", DbType.String, entity.DS_STATUS_DESCRICAO);
                    }
                    if (!string.IsNullOrEmpty(entity.TP_PEDIDO))
                    {
                        _db.AddInParameter(dbCommand, "@p_TP_PEDIDO", DbType.String, entity.TP_PEDIDO);
                    }
                    #endregion
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
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

        public DataTable ObterListaStatus(string statusCarregar, string TP_PEDIDO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfStatusPedidoEquipByStatusSelect");

                _db.AddInParameter(dbCommand, "@p_StatusCarregar", DbType.String, statusCarregar);

                if (!string.IsNullOrEmpty(TP_PEDIDO))
                    _db.AddInParameter(dbCommand, "@p_TP_PEDIDO", DbType.String, TP_PEDIDO);

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
