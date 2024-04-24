using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class WfAcessorioPedidoData
    {
        Database _db;
        DbCommand dbCommand;

        public WfAcessorioPedidoData()
        { 
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref WfAcessorioPedidoEntity wfAcessorioPedido, DbTransaction transacao = null)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("dbo.prcWfAcessorioPedidoInsert");

                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_PEDIDO_EQUIP);
                _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, wfAcessorioPedido.CD_MODELO);
                _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADO", DbType.Int64, wfAcessorioPedido.QTD_SOLICITADO);


                _db.AddOutParameter(dbCommand, "@p_ID_WF_ACESSORIO_EQUIP", DbType.Int64, 18);


                if (transacao != null)
                    _db.ExecuteNonQuery(dbCommand, transacao);
                else
                    _db.ExecuteNonQuery(dbCommand);


                wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_WF_ACESSORIO_EQUIP"));

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

        public void Excluir(WfAcessorioPedidoEntity wfAcessorioPedido, DbTransaction transacao = null)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("dbo.prcWfAcessorioPedidoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_WF_ACESSORIO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP);

                if (wfAcessorioPedido.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, wfAcessorioPedido.nidUsuarioAtualizacao);

                if (transacao != null)
                    _db.ExecuteNonQuery(dbCommand, transacao);
                else
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

        public bool Alterar(WfAcessorioPedidoEntity wfAcessorioPedido, DbTransaction transacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("dbo.prcWfAcessorioPedidoUpdate");


                if (wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_WF_ACESSORIO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP);
                if (wfAcessorioPedido.ID_WF_PEDIDO_EQUIP > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_PEDIDO_EQUIP);
                if (!string.IsNullOrEmpty(wfAcessorioPedido.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, wfAcessorioPedido.CD_MODELO);
                if (wfAcessorioPedido.QTD_SOLICITADO > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADO", DbType.Int64, wfAcessorioPedido.QTD_SOLICITADO);


                if (transacao != null)
                    _db.ExecuteNonQuery(dbCommand, transacao);
                else
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
            return blnOK;
        }

        public DataTable ObterLista(WfAcessorioPedidoEntity wfAcessorioPedido)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("dbo.prcWfAcessorioPedidoSelect");

                if (wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_WF_ACESSORIO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_ACESSORIO_EQUIP);
                if (wfAcessorioPedido.ID_WF_PEDIDO_EQUIP > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, wfAcessorioPedido.ID_WF_PEDIDO_EQUIP);
                if (!string.IsNullOrEmpty(wfAcessorioPedido.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, wfAcessorioPedido.CD_MODELO);
                if (wfAcessorioPedido.QTD_SOLICITADO > 0)
                    _db.AddInParameter(dbCommand, "@p_QTD_SOLICITADO", DbType.Int64, wfAcessorioPedido.QTD_SOLICITADO);


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                //if (transacao != null)
                //    dataSet = _db.ExecuteDataSet(dbCommand, transacao);
                //else
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
