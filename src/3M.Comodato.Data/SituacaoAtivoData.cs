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
    public class SituacaoAtivoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public SituacaoAtivoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref SituacaoAtivoEntity situacaoAtivo)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcSituacaoAtivoInsert");

               // _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.String, situacaoAtivo.CD_SITUACAO_ATIVO);
                _db.AddInParameter(dbCommand, "@p_DS_SITUACAO_ATIVO", DbType.String, situacaoAtivo.DS_SITUACAO_ATIVO);

                if (situacaoAtivo.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, situacaoAtivo.nidUsuarioAtualizacao);
                }
                _db.AddOutParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.Int64, 18);
                _db.ExecuteNonQuery(dbCommand);

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

        public void Excluir(SituacaoAtivoEntity situacaoAtivo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcSituacaoAtivoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.String, situacaoAtivo.CD_SITUACAO_ATIVO);

                if (situacaoAtivo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, situacaoAtivo.nidUsuarioAtualizacao);

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

        public bool Alterar(SituacaoAtivoEntity situacaoAtivo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSituacaoAtivoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.String, situacaoAtivo.CD_SITUACAO_ATIVO);

                _db.AddInParameter(dbCommand, "@p_DS_SITUACAO_ATIVO", DbType.String, situacaoAtivo.DS_SITUACAO_ATIVO);


                if (situacaoAtivo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, situacaoAtivo.nidUsuarioAtualizacao);


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
   
        public DataTable ObterLista(SituacaoAtivoEntity situacaoAtivo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSituacaoAtivoSelect");

                if (situacaoAtivo.CD_SITUACAO_ATIVO>0)
                    _db.AddInParameter(dbCommand, "@p_CD_SITUACAO_ATIVO", DbType.Int64, situacaoAtivo.CD_SITUACAO_ATIVO);

                if (!string.IsNullOrEmpty(situacaoAtivo.DS_SITUACAO_ATIVO))
                    _db.AddInParameter(dbCommand, "@p_DS_SITUACAO_ATIVO", DbType.String, situacaoAtivo.DS_SITUACAO_ATIVO);

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
