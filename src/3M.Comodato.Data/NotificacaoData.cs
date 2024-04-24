using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace _3M.Comodato.Data
{
    public class NotificacaoData
    {
        #region Propriedades

        private readonly Database _dataBase;
        public DbCommand dbCommand;

        #endregion

        #region Construtores

        public NotificacaoData()
        {
            _dataBase = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        #endregion

        #region Métodos

        public bool Inserir(ref NotificacaoEntity notificacaoEntity)
        {
            try
            {
                dbCommand = _dataBase.GetStoredProcCommand("prcNotificacaoInsert");

                _dataBase.AddInParameter(dbCommand, "@p_Titulo", DbType.String, notificacaoEntity.TITULO);
                _dataBase.AddInParameter(dbCommand, "@p_Mensagem", DbType.String, notificacaoEntity.MENSAGEM);
                _dataBase.AddInParameter(dbCommand, "@p_Lida", DbType.Boolean, notificacaoEntity.LIDA);
                _dataBase.AddInParameter(dbCommand, "@p_IdUsuario", DbType.Int64, notificacaoEntity.ID_USUARIO);
                _dataBase.AddOutParameter(dbCommand, "@p_IdNotificacao", DbType.Int64, 18);

                _dataBase.ExecuteNonQuery(dbCommand);

                notificacaoEntity.ID_NOTIFICACAO = Convert.ToInt64(_dataBase.GetParameterValue(dbCommand, "@p_IdNotificacao"));

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Excluir(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                dbCommand = _dataBase.GetStoredProcCommand("prcNotificacaoDelete");

                _dataBase.AddInParameter(dbCommand, "@p_IdNotificacao", DbType.Int64, notificacaoEntity.ID_NOTIFICACAO);

                if (notificacaoEntity.ID_USUARIO > 0)
                    _dataBase.AddInParameter(dbCommand, "@p_IdUsuario", DbType.Int64, notificacaoEntity.ID_USUARIO);

                _dataBase.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Alterar(NotificacaoEntity notificacaoEntity)
        {
            try
            {
                dbCommand = _dataBase.GetStoredProcCommand("prcNotificaoUpdate");

                _dataBase.AddInParameter(dbCommand, "@p_IdNotificacao", DbType.String, notificacaoEntity.ID_NOTIFICACAO);

                if (!string.IsNullOrEmpty(notificacaoEntity.TITULO))
                    _dataBase.AddInParameter(dbCommand, "@p_Titulo", DbType.String, notificacaoEntity.TITULO);

                if (!string.IsNullOrEmpty(notificacaoEntity.MENSAGEM))
                    _dataBase.AddInParameter(dbCommand, "@p_Mensagem", DbType.String, notificacaoEntity.MENSAGEM);

                if (notificacaoEntity.LIDA != null)
                    _dataBase.AddInParameter(dbCommand, "@p_Lida", DbType.Boolean, notificacaoEntity.LIDA);

                if (notificacaoEntity.ID_USUARIO > 0)
                    _dataBase.AddInParameter(dbCommand, "@p_IdUsuario", DbType.Int64, notificacaoEntity.ID_USUARIO);

                _dataBase.ExecuteNonQuery(dbCommand);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<NotificacaoEntity> ObterLista(NotificacaoEntity notificacaoEntity)
            => MapearCampoNotificacao(ObterNotificacoes(notificacaoEntity));

        public DataTable ObterNotificacoes(NotificacaoEntity notificacaoEntity)
        {
            DbConnection connection = null;

            try
            {
                dbCommand = _dataBase.GetStoredProcCommand("prcNotificacaoSelect");

                if (notificacaoEntity.ID_NOTIFICACAO > 0)
                    _dataBase.AddInParameter(dbCommand, "@p_IdNotificacao", DbType.Int64, notificacaoEntity.ID_NOTIFICACAO);

                if (notificacaoEntity.DATA_INCLUSAO != null && notificacaoEntity.DATA_INCLUSAO != DateTime.MinValue)
                    _dataBase.AddInParameter(dbCommand, "@p_DataInclusao", DbType.DateTime, notificacaoEntity.DATA_INCLUSAO);

                if (notificacaoEntity.ID_USUARIO > 0)
                    _dataBase.AddInParameter(dbCommand, "@p_IdUsuario", DbType.Int64, notificacaoEntity.ID_USUARIO);

                if (notificacaoEntity.LIDA != null)
                    _dataBase.AddInParameter(dbCommand, "@p_Lida", DbType.Boolean, notificacaoEntity.LIDA);

                connection = _dataBase.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _dataBase.ExecuteDataSet(dbCommand);

                DataTable dataTable = new DataTable();
                dataTable = dataSet.Tables[0];

                return dataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private IList<NotificacaoEntity> MapearCampoNotificacao(DataTable dataTable)
        {
            IList<NotificacaoEntity> notificacoes = new List<NotificacaoEntity>();

            foreach (DataRow notificacao in dataTable.Rows)
            {
                notificacoes.Add(new NotificacaoEntity()
                {
                    ID_NOTIFICACAO = Convert.ToInt64(notificacao["ID_NOTIFICACAO"]),
                    TITULO = notificacao["TITULO"].ToString(),
                    MENSAGEM = notificacao["MENSAGEM"].ToString(),
                    LIDA = Convert.ToBoolean(notificacao["LIDA"] is DBNull ? 0 : notificacao["LIDA"]),
                    DATA_INCLUSAO = Convert.ToDateTime(notificacao["DATA_INCLUSAO"].ToString()),
                    ID_USUARIO = Convert.ToInt64(notificacao["ID_USUARIO"] is DBNull ? 0 : notificacao["ID_USUARIO"])
                });
            }

            return notificacoes;
        }

        #endregion
    }
}
