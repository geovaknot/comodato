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
    public class WfPedidoComentData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public WfPedidoComentData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref WfPedidoComentEntity entity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoComentInsert");
                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.pedidoEquip.ID_WF_PEDIDO_EQUIP);
                _db.AddInParameter(dbCommand, "@p_DS_COMENT", DbType.String, entity.DS_COMENT);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.usuario.nidUsuario);

                _db.AddOutParameter(dbCommand, "@p_ID_WF_PEDIDO_Coment", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                entity.ID_WF_PEDIDO_COMENT = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_WF_PEDIDO_Coment"));

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

        public DataTable ObterLista()
        {
            return ObterLista(null);
        }
        public DataTable ObterLista(WfPedidoComentEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoComentSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_WF_PEDIDO_COMENT != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_Coment", DbType.Int64, entity.ID_WF_PEDIDO_COMENT);
                    }

                    if (entity.pedidoEquip.ID_WF_PEDIDO_EQUIP != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.pedidoEquip.ID_WF_PEDIDO_EQUIP);
                    }

                    if (!string.IsNullOrEmpty(entity.DS_COMENT))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_COMENT", DbType.String, entity.DS_COMENT);
                    }

                    if (entity.nidUsuarioAtualizacao != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.nidUsuarioAtualizacao);
                    }

                    if (entity.dtmDataHoraAtualizacao != DateTime.MinValue)
                    {
                        _db.AddInParameter(dbCommand, "@p_DT_REGISTRO", DbType.DateTime, entity.dtmDataHoraAtualizacao);
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

        public IEnumerable<WfPedidoComentEntity> ObterListaEntity()
        {
            return ObterListaEntity(null);
        }
        public IEnumerable<WfPedidoComentEntity> ObterListaEntity(WfPedidoComentEntity entity)
        {
            DataTable dtCategoria = ObterLista(entity);
            return (from r in dtCategoria.Rows.Cast<DataRow>()
                    select new WfPedidoComentEntity()
                    {
                        ID_WF_PEDIDO_COMENT = Convert.ToInt64(r["ID_WF_PEDIDO_Coment"]),
                        pedidoEquip = new WfPedidoEquipEntity
                        {
                            ID_WF_PEDIDO_EQUIP = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP"]),

                        },
                        DS_COMENT = r["DS_COMENT"].ToString(),
                        nidUsuarioAtualizacao = Convert.ToInt64(r["ID_USUARIO"]),
                        dtmDataHoraAtualizacao = Convert.ToDateTime(r["DT_REGISTRO"]),
                        usuario = new UsuarioEntity
                        {
                            nidUsuario = Convert.ToInt64(r["ID_USUARIO"]),
                            cnmNome = r["cnmNome"].ToString(),
                        },
                    }).ToList();
        }
    }
}
