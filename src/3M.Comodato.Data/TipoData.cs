using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace _3M.Comodato.Data
{
    public class TipoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public TipoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref TipoEntity tipoEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTipoInsert");

                _db.AddInParameter(dbCommand, "@p_DS_TIPO", DbType.String, tipoEntity.DS_TIPO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tipoEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_CD_TIPO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                tipoEntity.CD_TIPO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_CD_TIPO"));

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

        public void Excluir(TipoEntity tipoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcTipoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.String, tipoEntity.CD_TIPO);

                if (tipoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tipoEntity.nidUsuarioAtualizacao);
                }

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

        public bool Alterar(TipoEntity tipoEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.String, tipoEntity.CD_TIPO);

                _db.AddInParameter(dbCommand, "@p_DS_TIPO", DbType.String, tipoEntity.DS_TIPO);


                if (tipoEntity.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, tipoEntity.nidUsuarioAtualizacao);
                }

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

        public DataTable ObterLista(TipoEntity tipoEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcTipoSelect");

                if (tipoEntity.CD_TIPO != 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO", DbType.Int64, tipoEntity.CD_TIPO);
                }

                if (!string.IsNullOrEmpty(tipoEntity.DS_TIPO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_TIPO", DbType.String, tipoEntity.DS_TIPO);
                }

                if (tipoEntity.FL_SEGMENTO_DI.HasValue)
                {
                    _db.AddInParameter(dbCommand, "@p_FL_SEGMENTO_DI", DbType.Boolean, tipoEntity.FL_SEGMENTO_DI);
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
