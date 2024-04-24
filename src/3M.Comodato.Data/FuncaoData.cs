using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class FuncaoData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public FuncaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref FuncaoEntity funcao)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcFuncaoInsert");

                _db.AddInParameter(dbCommand, "@p_ccdFuncao", DbType.String, funcao.ccdFuncao);
                _db.AddInParameter(dbCommand, "@p_cdsFuncao", DbType.String, funcao.cdsFuncao);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, funcao.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, funcao.bidAtivo);

                _db.AddOutParameter(dbCommand, "@p_nidFuncao", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                funcao.nidFuncao = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidFuncao"));

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

        public void Excluir(FuncaoEntity funcao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcFuncaoDelete");

                _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, funcao.nidFuncao);

                if (funcao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, funcao.nidUsuarioAtualizacao);

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

        public bool Alterar(FuncaoEntity funcao)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFuncaoUpdate");

                _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, funcao.nidFuncao);

                if (!string.IsNullOrEmpty(funcao.ccdFuncao))
                    _db.AddInParameter(dbCommand, "@p_ccdFuncao", DbType.String, funcao.ccdFuncao);

                if (!string.IsNullOrEmpty(funcao.cdsFuncao))
                    _db.AddInParameter(dbCommand, "@p_cdsFuncao", DbType.String, funcao.cdsFuncao);

                if (funcao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, funcao.nidUsuarioAtualizacao);

                if (funcao.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, funcao.bidAtivo);

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

        public DataTable ObterLista(FuncaoEntity funcao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFuncaoSelect");

                if (funcao.nidFuncao != 0)
                    _db.AddInParameter(dbCommand, "@p_nidFuncao", DbType.Int64, funcao.nidFuncao);

                if (!string.IsNullOrEmpty(funcao.ccdFuncao))
                    _db.AddInParameter(dbCommand, "@p_ccdFuncao", DbType.String, funcao.ccdFuncao);

                if (!string.IsNullOrEmpty(funcao.cdsFuncao))
                    _db.AddInParameter(dbCommand, "@p_cdsFuncao", DbType.String, funcao.cdsFuncao);

                if (funcao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, funcao.nidUsuarioAtualizacao);

                if (funcao.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, funcao.bidAtivo);

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
