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
   public class GrupoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public GrupoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref GrupoEntity grupo)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcGrupoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, grupo.CD_GRUPO);
                _db.AddInParameter(dbCommand, "@p_DS_GRUPO", DbType.String, grupo.DS_GRUPO);

                if (grupo.nidUsuarioAtualizacao > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupo.nidUsuarioAtualizacao);
                }
                
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

        public void Excluir(GrupoEntity grupo)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcGrupoDelete");

                _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, grupo.CD_GRUPO);

                if (grupo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupo.nidUsuarioAtualizacao);

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

        public bool Alterar(GrupoEntity grupo)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcGrupoUpdate");

                _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, grupo.CD_GRUPO);

                _db.AddInParameter(dbCommand, "@p_DS_GRUPO", DbType.String, grupo.DS_GRUPO);


                if (grupo.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, grupo.nidUsuarioAtualizacao);


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

        public DataTable ObterLista(GrupoEntity grupo)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcGrupoSelect");

                if (!string.IsNullOrEmpty(grupo.CD_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_CD_GRUPO", DbType.String, grupo.CD_GRUPO);

                if (!string.IsNullOrEmpty(grupo.DS_GRUPO))
                    _db.AddInParameter(dbCommand, "@p_DS_GRUPO", DbType.String, grupo.DS_GRUPO);

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
