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
    public class WfPedidoEquipItemData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public WfPedidoEquipItemData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref WfPedidoEquipItemEntity entity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipItemInsert");
                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, entity.CD_ATIVO_FIXO);
                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP);

                if (!string.IsNullOrEmpty(entity.DS_ANEXO))
                    _db.AddInParameter(dbCommand, "@p_DS_ANEXO", DbType.String, entity.DS_ANEXO);

                if (!string.IsNullOrEmpty(entity.ST_STATUS))
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS", DbType.String, entity.ST_STATUS);

                if (entity.nidUsuarioAtualizacao != long.MinValue)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.nidUsuarioAtualizacao);

                //if (entity.DT_RETIRADA != DateTime.MinValue)
                //    _db.AddInParameter(dbCommand, "@p_DT_RETIRADA", DbType.DateTime, entity.DT_RETIRADA);

                //if (entity.DT_ENTREGA_3M!=DateTime.MinValue)
                //    _db.AddInParameter(dbCommand, "@p_DT_ENTREGA_3M", DbType.DateTime, entity.DT_ENTREGA_3M);

                _db.AddOutParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_ITEM", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                entity.ID_WF_PEDIDO_EQUIP_ITEM = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_ITEM"));

                retorno = true;

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;

        }

        public void Excluir(WfPedidoEquipItemEntity entity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipItemDelete");

                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_ITEM", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP_ITEM);

                if (entity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, entity.nidUsuarioAtualizacao);

                //if (transacao != null)
                //    _db.ExecuteNonQuery(dbCommand, transacao);
                //else
                    _db.ExecuteNonQuery(dbCommand);

            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ObterLista()
        {
            return ObterLista(null);
        }

        public DataTable ObterLista(WfPedidoEquipItemEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipItemSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada
                    if ( entity.ID_WF_PEDIDO_EQUIP_ITEM>0)
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_ITEM", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP_ITEM);

                    if (!string.IsNullOrEmpty(entity.CD_ATIVO_FIXO))
                        _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, entity.CD_ATIVO_FIXO);

                    if (entity.ID_WF_PEDIDO_EQUIP>0)
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP);

                    if (!string.IsNullOrEmpty(entity.ST_STATUS))
                        _db.AddInParameter(dbCommand, "@p_ST_STATUS", DbType.String, entity.ST_STATUS);

                    if (entity.nidUsuarioAtualizacao>0)
                        _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.nidUsuarioAtualizacao);


                    //if (entity.DT_RETIRADA != DateTime.MinValue)
                    //    _db.AddInParameter(dbCommand, "@p_DT_RETIRADA", DbType.DateTime, entity.DT_RETIRADA);

                    //if (entity.DT_ENTREGA_3M != DateTime.MinValue)
                    //    _db.AddInParameter(dbCommand, "@p_DT_ENTREGA_3M", DbType.DateTime, entity.DT_ENTREGA_3M);

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

        public IEnumerable<WfPedidoEquipItemEntity> ObterListaEntity()
        {
            return ObterListaEntity(null);
        }

        public IEnumerable<WfPedidoEquipItemEntity> ObterListaEntity(WfPedidoEquipItemEntity entity)
        {
            Func<DataRow, WfPedidoEquipItemEntity> dataRowToEntity = new Func<DataRow, WfPedidoEquipItemEntity>((r) => {
                WfPedidoEquipItemEntity equipamentoItem = new WfPedidoEquipItemEntity();

                equipamentoItem.ID_WF_PEDIDO_EQUIP_ITEM = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP_ITEM"]);
                equipamentoItem.CD_ATIVO_FIXO = r["CD_ATIVO_FIXO"].ToString();
                equipamentoItem.ID_WF_PEDIDO_EQUIP = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP"]);

                if (r["DS_ANEXO"] != DBNull.Value)
                    equipamentoItem.DS_ANEXO = r["DS_ANEXO"].ToString();

                if (r["ST_STATUS"] != DBNull.Value)
                    equipamentoItem.ST_STATUS = r["ST_STATUS"].ToString();


                equipamentoItem.nidUsuarioAtualizacao = Convert.ToInt64(r["ID_USUARIO"]);


                equipamentoItem.Modelo = new ModeloEntity()
                {
                    DS_MODELO = r["DS_MODELO"].ToString(),
                    VL_LARG_CAIXA = r["VL_LARG_CAIXA"] != null ? Convert.ToDecimal(r["VL_LARG_CAIXA"].ToString()) : (decimal?)null,
                    VL_ALTUR_CAIXA = r["VL_ALTUR_CAIXA"] != null ? Convert.ToDecimal(r["VL_ALTUR_CAIXA"].ToString()) : (decimal?)null,
                    VL_COMP_CAIXA = r["VL_COMP_CAIXA"] != null ? Convert.ToDecimal(r["VL_COMP_CAIXA"].ToString()) : (decimal?)null,
                    VL_PESO_CUBADO = r["VL_PESO_CUBADO"] != null ? Convert.ToDecimal(r["VL_PESO_CUBADO"].ToString()) : (decimal?)null,
                };

                equipamentoItem.Ativo = new AtivoClienteEntity()
                {
                    NR_NOTAFISCAL = Convert.ToInt64(r["NR_NOTAFISCAL"].ToString()),
                    DS_ARQUIVO_FOTO = r["DS_ARQUIVO_FOTO"].ToString(),
                };

                equipamentoItem.PedidoEquip = new WfPedidoEquipEntity()
                {
                    ST_STATUS_PEDIDO = Convert.ToInt32(r["ST_STATUS_PEDIDO"].ToString())
                };


                return equipamentoItem;
            });


            DataTable dtCategoria = ObterLista(entity);
            return (from r in dtCategoria.Rows.Cast<DataRow>()
                    select dataRowToEntity(r)).ToList();
        }

        public DataTable ObterListaAtivos(long idWFEquip)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoAtivosSelect");

                if (idWFEquip > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, idWFEquip);
                
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
    }
}
