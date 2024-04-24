using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class ParametroData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public ParametroData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref ParametroEntity parametro)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcParametroInsert");

                _db.AddInParameter(dbCommand, "@p_ccdParametro", DbType.String, parametro.ccdParametro);
                _db.AddInParameter(dbCommand, "@p_cdsParametro", DbType.String, parametro.cdsParametro);
                _db.AddInParameter(dbCommand, "@p_cvlParametro", DbType.String, parametro.cvlParametro);
                _db.AddInParameter(dbCommand, "@p_flgTipoParametro", DbType.String, parametro.flgTipoParametro);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, parametro.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_nidParametro", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                parametro.nidParametro = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidParametro"));

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

        public void Excluir(ParametroEntity parametro)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcParametroDelete");

                _db.AddInParameter(dbCommand, "@p_nidParametro", DbType.Int64, parametro.nidParametro);

                if (parametro.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, parametro.nidUsuarioAtualizacao);

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

        public bool Alterar(ParametroEntity parametro)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcParametroUpdate");

                _db.AddInParameter(dbCommand, "@p_nidParametro", DbType.Int64, parametro.nidParametro);

                if (!string.IsNullOrEmpty(parametro.ccdParametro))
                    _db.AddInParameter(dbCommand, "@p_ccdParametro", DbType.String, parametro.ccdParametro);

                if (!string.IsNullOrEmpty(parametro.cdsParametro))
                    _db.AddInParameter(dbCommand, "@p_cdsParametro", DbType.String, parametro.cdsParametro);

                if (!string.IsNullOrEmpty(parametro.cvlParametro))
                    _db.AddInParameter(dbCommand, "@p_cvlParametro", DbType.String, parametro.cvlParametro);

                if (!string.IsNullOrEmpty(parametro.flgTipoParametro))
                    _db.AddInParameter(dbCommand, "@p_flgTipoParametro", DbType.String, parametro.flgTipoParametro);

                if (parametro.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, parametro.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(ParametroEntity parametro)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcParametroSelect");

                if (parametro.nidParametro != 0)
                    _db.AddInParameter(dbCommand, "@p_nidParametro", DbType.Int64, parametro.nidParametro);

                if (!string.IsNullOrEmpty(parametro.ccdParametro))
                    _db.AddInParameter(dbCommand, "@p_ccdParametro", DbType.String, parametro.ccdParametro);

                if (!string.IsNullOrEmpty(parametro.cdsParametro))
                    _db.AddInParameter(dbCommand, "@p_cdsParametro", DbType.String, parametro.cdsParametro);

                if (!string.IsNullOrEmpty(parametro.cvlParametro))
                    _db.AddInParameter(dbCommand, "@p_cvlParametro", DbType.String, parametro.cvlParametro);

                if (!string.IsNullOrEmpty(parametro.flgTipoParametro))
                    _db.AddInParameter(dbCommand, "@p_flgTipoParametro", DbType.String, parametro.flgTipoParametro);

                if (parametro.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, parametro.nidUsuarioAtualizacao);

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
