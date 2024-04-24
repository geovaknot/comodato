using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class MensagemData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public MensagemData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref MensagemEntity mensagemEntity)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcMensagemInsert");

                _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, mensagemEntity.pedido.ID_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_DT_OCORRENCIA", DbType.DateTime, mensagemEntity.DT_OCORRENCIA);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, mensagemEntity.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_DS_MENSAGEM", DbType.String, mensagemEntity.DS_MENSAGEM);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, mensagemEntity.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_ID_MENSAGEM", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                mensagemEntity.ID_MENSAGEM = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_MENSAGEM"));

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

        public void Excluir(MensagemEntity mensagemEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcMensagemDelete");

                _db.AddInParameter(dbCommand, "@p_ID_MENSAGEM", DbType.Int64, mensagemEntity.ID_MENSAGEM);

                if (mensagemEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, mensagemEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(MensagemEntity mensagemEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcMensagemUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_MENSAGEM", DbType.Int64, mensagemEntity.ID_MENSAGEM);

                if (mensagemEntity.pedido.ID_PEDIDO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, mensagemEntity.pedido.ID_PEDIDO);

                if (mensagemEntity.DT_OCORRENCIA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_OCORRENCIA", DbType.DateTime, mensagemEntity.DT_OCORRENCIA);

                if (mensagemEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, mensagemEntity.usuario.nidUsuario);

                _db.AddInParameter(dbCommand, "@p_DS_MENSAGEM", DbType.String, mensagemEntity.DS_MENSAGEM);

                if (mensagemEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, mensagemEntity.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(MensagemEntity mensagemEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcMensagemSelect");

                if (mensagemEntity.ID_MENSAGEM != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_MENSAGEM", DbType.Int64, mensagemEntity.ID_MENSAGEM);

                if (mensagemEntity.pedido.ID_PEDIDO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, mensagemEntity.pedido.ID_PEDIDO);

                if (mensagemEntity.DT_OCORRENCIA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_OCORRENCIA", DbType.DateTime, mensagemEntity.DT_OCORRENCIA);

                if (mensagemEntity.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO", DbType.Int64, mensagemEntity.usuario.nidUsuario);

                if (!string.IsNullOrEmpty(mensagemEntity.DS_MENSAGEM))
                    _db.AddInParameter(dbCommand, "@p_DS_MENSAGEM", DbType.String, mensagemEntity.DS_MENSAGEM);

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

        public DataTable ObterListaNotas(PEDIDO_BPCS_NFEntity mensagemEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcNotasPedidoSelect");

                if (mensagemEntity.ID_PEDIDO != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PEDIDO", DbType.Int64, mensagemEntity.ID_PEDIDO);
                
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
