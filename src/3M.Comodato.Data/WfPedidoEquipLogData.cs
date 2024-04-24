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
    public class WfPedidoEquipLogData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public WfPedidoEquipLogData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref WfPedidoEquipLogEntity entity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipLogInsert");

                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.pedidoEquip.ID_WF_PEDIDO_EQUIP);

                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int16, entity.statusPedidoEquip.ST_STATUS_PEDIDO);

                _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, entity.nidUsuarioAtualizacao);

                if (!String.IsNullOrEmpty(entity.CD_GRUPO_RESPONS))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO_RESPONS", DbType.String, entity.CD_GRUPO_RESPONS);

                _db.AddOutParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_LOG", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                entity.ID_WF_PEDIDO_EQUIP_LOG = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_LOG"));

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

        public DataTable ObterLista(WfPedidoEquipLogEntity entity)
        {
            DbConnection connection = null;
            DataTable dataTable = null;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipLogSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_WF_PEDIDO_EQUIP_LOG != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP_LOG", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP_LOG);
                    }

                    if (entity.pedidoEquip.ID_WF_PEDIDO_EQUIP != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.pedidoEquip.ID_WF_PEDIDO_EQUIP);
                    }

                    if (entity.statusPedidoEquip.ST_STATUS_PEDIDO != null)
                    {
                        _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int32, entity.statusPedidoEquip.ST_STATUS_PEDIDO);
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

    }
}
