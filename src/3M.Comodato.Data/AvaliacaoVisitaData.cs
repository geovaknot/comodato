using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class AvaliacaoVisitaData
    {
        readonly Database _db;
        DbCommand dbCommand;
        public AvaliacaoVisitaData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref AvaliacaoVisitaEntity avaliacaoVisita)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAvaliacaoVisitaInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, avaliacaoVisita.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_DT_AVALIACAO_VISITA", DbType.DateTime, avaliacaoVisita.DT_AVALIACAO_VISITA);
                _db.AddInParameter(dbCommand, "@p_DS_AVALIACAO_VISITA", DbType.String, avaliacaoVisita.DS_AVALIACAO_VISITA);
                _db.AddInParameter(dbCommand, "@p_NR_GRAU_AVALIACAO", DbType.Decimal, avaliacaoVisita.NR_GRAU_AVALIACAO);
                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, avaliacaoVisita.visita.ID_VISITA);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, avaliacaoVisita.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_ID_AVALIACAO_VISITA", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                avaliacaoVisita.ID_AVALIACAO_VISITA = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_AVALIACAO_VISITA"));


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

        public void Excluir(AvaliacaoVisitaEntity avaliacaoVisita)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAvaliacaoVisitaDelete");

                _db.AddInParameter(dbCommand, "@p_ID_AVALIACAO_VISITA", DbType.Int64, avaliacaoVisita.ID_AVALIACAO_VISITA);

                if (avaliacaoVisita.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, avaliacaoVisita.nidUsuarioAtualizacao);

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

        public bool Alterar(AvaliacaoVisitaEntity avaliacaoVisita)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAvaliacaoVisitaUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_AVALIACAO_VISITA", DbType.Int64, avaliacaoVisita.ID_AVALIACAO_VISITA);

                if (avaliacaoVisita.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, avaliacaoVisita.usuario.nidUsuario);

                if (avaliacaoVisita.DT_AVALIACAO_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_AVALIACAO_VISITA", DbType.DateTime, avaliacaoVisita.DT_AVALIACAO_VISITA);

                if (!String.IsNullOrEmpty(avaliacaoVisita.DS_AVALIACAO_VISITA))
                    _db.AddInParameter(dbCommand, "@p_DS_AVALIACAO_VISITA", DbType.String, avaliacaoVisita.DS_AVALIACAO_VISITA);

                if (avaliacaoVisita.NR_GRAU_AVALIACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_NR_GRAU_AVALIACAO", DbType.Decimal, avaliacaoVisita.NR_GRAU_AVALIACAO);

                if (avaliacaoVisita.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, avaliacaoVisita.visita.ID_VISITA);

                if (avaliacaoVisita.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, avaliacaoVisita.nidUsuarioAtualizacao);

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

        public void Desfazer(Int64 ID_VISITA, Int64 nidUsuarioAtualizacao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcAvaliacaoVisitaDesfazer");

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

        public DataTable ObterLista(AvaliacaoVisitaEntity avaliacaoVisita)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcAvaliacaoVisitaSelect");

                if (avaliacaoVisita.ID_AVALIACAO_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_AVALIACAO_VISITA", DbType.Int64, avaliacaoVisita.ID_AVALIACAO_VISITA);

                if (avaliacaoVisita.usuario.nidUsuario > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, avaliacaoVisita.usuario.nidUsuario);

                if (avaliacaoVisita.DT_AVALIACAO_VISITA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_AVALIACAO_VISITA", DbType.DateTime, avaliacaoVisita.DT_AVALIACAO_VISITA);

                if (!String.IsNullOrEmpty(avaliacaoVisita.DS_AVALIACAO_VISITA))
                    _db.AddInParameter(dbCommand, "@p_DS_AVALIACAO_VISITA", DbType.String, avaliacaoVisita.DS_AVALIACAO_VISITA);

                if (avaliacaoVisita.NR_GRAU_AVALIACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_NR_GRAU_AVALIACAO", DbType.Int32, avaliacaoVisita.NR_GRAU_AVALIACAO);

                if (avaliacaoVisita.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, avaliacaoVisita.visita.ID_VISITA);

                if (avaliacaoVisita.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, avaliacaoVisita.nidUsuarioAtualizacao);

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
