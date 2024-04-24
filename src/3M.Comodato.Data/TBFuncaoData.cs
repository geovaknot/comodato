using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class TBFuncaoData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public TBFuncaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref TBFuncaoEntity TBfuncao)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTBFuncaoInsert");

                _db.AddInParameter(dbCommand, "@p_DS_Funcao", DbType.String, TBfuncao.DS_FUNCAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, TBfuncao.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_CD_Funcao", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                TBfuncao.CD_FUNCAO = Convert.ToInt32(_db.GetParameterValue(dbCommand, "@p_CD_Funcao"));

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

        public void Excluir(TBFuncaoEntity funcao)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTBFuncaoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_Funcao", DbType.Int64, funcao.CD_FUNCAO);

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

        public bool Alterar(TBFuncaoEntity funcao)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTBFuncaoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_Funcao", DbType.Int64, funcao.CD_FUNCAO);

                if (!string.IsNullOrEmpty(funcao.DS_FUNCAO))
                    _db.AddInParameter(dbCommand, "@p_DS_Funcao", DbType.String, funcao.DS_FUNCAO);

                if (funcao.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, funcao.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(TBFuncaoEntity funcao)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTBFuncaoSelect");

                if (funcao.CD_FUNCAO != 0)
                    _db.AddInParameter(dbCommand, "@p_CD_Funcao", DbType.Int64, funcao.CD_FUNCAO);

                if (!string.IsNullOrEmpty(funcao.DS_FUNCAO))
                    _db.AddInParameter(dbCommand, "@p_DS_Funcao", DbType.String, funcao.DS_FUNCAO);

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
