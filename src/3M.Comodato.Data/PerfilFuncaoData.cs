using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class PerfilFuncaoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public PerfilFuncaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref PerfilFuncaoEntity perfilFuncao)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPerfilFuncaoInsert");

                _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfilFuncao.perfil.nidPerfil);
                _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, perfilFuncao.funcao.nidFuncao);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfilFuncao.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfilFuncao.bidAtivo);

                _db.AddOutParameter(dbCommand, "@p_nidPerfilFuncao", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                perfilFuncao.nidPerfilFuncao = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidPerfilFuncao"));

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

        public void Excluir(PerfilFuncaoEntity perfilFuncao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPerfilFuncaoDelete");

                _db.AddInParameter(dbCommand, "@p_nidPerfilFuncao", DbType.Int64, perfilFuncao.nidPerfilFuncao);

                if (perfilFuncao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfilFuncao.nidUsuarioAtualizacao);

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

        public bool Alterar(PerfilFuncaoEntity perfilFuncao)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilFuncaoUpdate");

                _db.AddInParameter(dbCommand, "@p_nidPerfilFuncao", DbType.Int64, perfilFuncao.nidPerfilFuncao);

                if (perfilFuncao.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfilFuncao.perfil.nidPerfil);

                if (perfilFuncao.funcao.nidFuncao != 0)
                    _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, perfilFuncao.funcao.nidFuncao);

                if (perfilFuncao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfilFuncao.nidUsuarioAtualizacao);

                if (perfilFuncao.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfilFuncao.bidAtivo);

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

        public DataTable ObterLista(PerfilFuncaoEntity perfilFuncao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilFuncaoSelect");

                if (perfilFuncao.nidPerfilFuncao != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfilFuncao", DbType.Int64, perfilFuncao.nidPerfilFuncao);

                //if (perfilFuncao.perfil.nidPerfil != 0)
                _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfilFuncao.perfil.nidPerfil);

                if (perfilFuncao.funcao.nidFuncao != 0)
                    _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, perfilFuncao.funcao.nidFuncao);

                if (perfilFuncao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfilFuncao.nidUsuarioAtualizacao);

                if (perfilFuncao.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfilFuncao.bidAtivo);

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

        public DataTable ObterListaALLFuncao(PerfilFuncaoEntity perfilFuncao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilFuncaoSelectALLFuncao");

                //if (perfilFuncao.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfilFuncao.perfil.nidPerfil);

                if (perfilFuncao.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfilFuncao.bidAtivo);

                if (!string.IsNullOrEmpty(perfilFuncao.funcao.ccdFuncao))
                    _db.AddInParameter(dbCommand, "@p_ccdFuncao", DbType.String, perfilFuncao.funcao.ccdFuncao);

                if (!string.IsNullOrEmpty(perfilFuncao.funcao.cdsFuncao))
                    _db.AddInParameter(dbCommand, "@p_cdsFuncao", DbType.String, perfilFuncao.funcao.cdsFuncao);

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
