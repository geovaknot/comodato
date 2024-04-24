using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class SatisfacaoRespostaData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public SatisfacaoRespostaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref SatisfacaoRespostaEntity respostaEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfRespostaInsert");

                _db.AddInParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int64, respostaEntity.SatisfacaoPesquisa.ID_PESQUISA_SATISF);
                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, respostaEntity.Visita.ID_VISITA);
                _db.AddInParameter(dbCommand, "@p_DT_DATA_RESPOSTA", DbType.DateTime, DateTime.Now);
                _db.AddInParameter(dbCommand, "@p_DS_NOME_RESPONDEDOR", DbType.String, respostaEntity.DS_NOME_RESPONDEDOR);
                _db.AddInParameter(dbCommand, "@p_NM_NOTA_PESQ", DbType.Decimal, respostaEntity.NM_NOTA_PESQ);
                _db.AddInParameter(dbCommand, "@p_DS_JUSTIFICATIVA", DbType.String, respostaEntity.DS_JUSTIFICATIVA);

                if (!string.IsNullOrEmpty(respostaEntity.DS_RESPOSTA1))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_RESPOSTA1", DbType.String, respostaEntity.DS_RESPOSTA1);
                }
                if (!string.IsNullOrEmpty(respostaEntity.DS_RESPOSTA2))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_RESPOSTA2", DbType.String, respostaEntity.DS_RESPOSTA2);
                }
                if (!string.IsNullOrEmpty(respostaEntity.DS_RESPOSTA3))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_RESPOSTA3", DbType.String, respostaEntity.DS_RESPOSTA3);
                }
                if (!string.IsNullOrEmpty(respostaEntity.DS_RESPOSTA4))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_RESPOSTA4", DbType.String, respostaEntity.DS_RESPOSTA4);
                }
                if (!string.IsNullOrEmpty(respostaEntity.DS_RESPOSTA5))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_RESPOSTA5", DbType.String, respostaEntity.DS_RESPOSTA5);
                }

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, respostaEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_RESPOSTA_SATISF", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                respostaEntity.ID_RESPOSTA_SATISF = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_RESPOSTA_SATISF"));

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

        public void Desfazer(long ID_VISITA, long nidUsuarioAtualizacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcSatisfRespostaDesfazer");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);

                if (nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, nidUsuarioAtualizacao);

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

        public DataTable ObterLista(SatisfacaoRespostaEntity respostaEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSatisfRespostaSelect");

                if (respostaEntity.ID_RESPOSTA_SATISF > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_RESPOSTA_SATISF", DbType.Int64, respostaEntity.ID_RESPOSTA_SATISF);
                }

                if (respostaEntity.SatisfacaoPesquisa.ID_PESQUISA_SATISF > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_PESQUISA_SATISF", DbType.Int64, respostaEntity.SatisfacaoPesquisa.ID_PESQUISA_SATISF);
                }

                if (respostaEntity.Visita.ID_VISITA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, respostaEntity.Visita.ID_VISITA);
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
