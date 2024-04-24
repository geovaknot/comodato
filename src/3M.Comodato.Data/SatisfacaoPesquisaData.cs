using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class SatisfacaoPesquisaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public SatisfacaoPesquisaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref SatisfacaoPesquisaEntity pesquisaEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfPesquisaInsert");

                _db.AddInParameter(dbCommand, "@p_DS_TITULO", DbType.String, pesquisaEntity.DS_TITULO);
                _db.AddInParameter(dbCommand, "@p_TP_PESQUISA", DbType.Int32, pesquisaEntity.TP_PESQUISA);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PESQUISA", DbType.Int32, pesquisaEntity.ST_STATUS_PESQUISA);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO_RESPONSAVEL", DbType.Int64, pesquisaEntity.USUARIO_RESPONSAVEL.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, pesquisaEntity.DS_DESCRICAO);
                _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA1", DbType.String, pesquisaEntity.DS_PERGUNTA1);
                _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA2", DbType.String, pesquisaEntity.DS_PERGUNTA2);
                _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA3", DbType.String, pesquisaEntity.DS_PERGUNTA3);
                _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA4", DbType.String, pesquisaEntity.DS_PERGUNTA4);
                _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA5", DbType.String, pesquisaEntity.DS_PERGUNTA5);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pesquisaEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                pesquisaEntity.ID_PESQUISA_SATISF = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PESQUISA_SATISF"));

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

        public bool Alterar(SatisfacaoPesquisaEntity pesquisaEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfPesquisaUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int64, pesquisaEntity.ID_PESQUISA_SATISF);
                _db.AddInParameter(dbCommand, "@p_DS_TITULO", DbType.String, pesquisaEntity.DS_TITULO);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PESQUISA", DbType.Int32, pesquisaEntity.ST_STATUS_PESQUISA);
                _db.AddInParameter(dbCommand, "@p_ID_USUARIO_RESPONSAVEL", DbType.Int64, pesquisaEntity.USUARIO_RESPONSAVEL.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_TP_PESQUISA", DbType.Int32, pesquisaEntity.TP_PESQUISA);


                if (!string.IsNullOrEmpty(pesquisaEntity.DS_DESCRICAO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, pesquisaEntity.DS_DESCRICAO);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_PERGUNTA1))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA1", DbType.String, pesquisaEntity.DS_PERGUNTA1);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_PERGUNTA2))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA2", DbType.String, pesquisaEntity.DS_PERGUNTA2);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_PERGUNTA3))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA3", DbType.String, pesquisaEntity.DS_PERGUNTA3);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_PERGUNTA4))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA4", DbType.String, pesquisaEntity.DS_PERGUNTA4);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_PERGUNTA5))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_PERGUNTA5", DbType.String, pesquisaEntity.DS_PERGUNTA5);
                }

                if (pesquisaEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pesquisaEntity.nidUsuarioAtualizacao);
                }

                _db.ExecuteNonQuery(dbCommand);

                blnOK = true;

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

        public long? ObterCodigoPesquisaSatisfacaoAtiva(long codigoCliente, long codigoVisita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfPesquisaAtiva");

                if (codigoCliente > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, codigoCliente);
                }

                if (codigoVisita > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, codigoVisita);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                return (long?)_db.ExecuteScalar(dbCommand);
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
        }

        public long? ObterCodigoPesquisaResposta(long codigoVisita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfRespostaPesquisa");

                if (codigoVisita > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, codigoVisita);
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                return (long?)_db.ExecuteScalar(dbCommand);
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
        }
        public DataTable ObterLista(SatisfacaoPesquisaEntity pesquisaEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfPesquisaSelect");

                if (pesquisaEntity.ID_PESQUISA_SATISF > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int64, pesquisaEntity.ID_PESQUISA_SATISF);
                }

                if (!string.IsNullOrEmpty(pesquisaEntity.DS_TITULO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_TITULO", DbType.String, pesquisaEntity.DS_TITULO);
                }

                if (pesquisaEntity.ST_STATUS_PESQUISA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_PESQUISA", DbType.Int32, pesquisaEntity.ST_STATUS_PESQUISA);
                }

                if (pesquisaEntity.TP_PESQUISA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_TP_PESQUISA", DbType.Int32, pesquisaEntity.TP_PESQUISA);
                }

                if (pesquisaEntity.USUARIO_RESPONSAVEL.nidUsuario > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_USUARIO_RESPONSAVEL", DbType.Int64, pesquisaEntity.USUARIO_RESPONSAVEL.nidUsuario);
                }

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
