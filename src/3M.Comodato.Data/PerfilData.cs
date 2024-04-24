using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class PerfilData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public PerfilData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref PerfilEntity perfil)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPerfilInsert");

                _db.AddInParameter(dbCommand, "@p_cdsPerfil", DbType.String, perfil.cdsPerfil);
                _db.AddInParameter(dbCommand, "@p_ccdPerfil", DbType.Int32, perfil.ccdPerfil);
                _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfil.bidAtivo);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfil.nidUsuarioAtualizacao);

                _db.AddOutParameter(dbCommand, "@p_nidPerfil", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                perfil.nidPerfil = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidPerfil"));

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

        public void Excluir(PerfilEntity perfil)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPerfilDelete");

                _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfil.nidPerfil);

                if (perfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfil.nidUsuarioAtualizacao);

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

        public bool Alterar(PerfilEntity perfil)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilUpdate");

                _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfil.nidPerfil);

                if (!string.IsNullOrEmpty(perfil.cdsPerfil))
                    _db.AddInParameter(dbCommand, "@p_cdsPerfil", DbType.String, perfil.cdsPerfil);

                if (perfil.ccdPerfil > 0)
                    _db.AddInParameter(dbCommand, "@p_ccdPerfil", DbType.Int32, perfil.ccdPerfil);

                if (perfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfil.bidAtivo);

                if (perfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfil.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(PerfilEntity perfil)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilSelect");

                if (perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfil.nidPerfil);

                if (!string.IsNullOrEmpty(perfil.cdsPerfil))
                    _db.AddInParameter(dbCommand, "@p_cdsPerfil", DbType.String, perfil.cdsPerfil);

                if (perfil.ccdPerfil > 0)
                    _db.AddInParameter(dbCommand, "@p_ccdPerfil", DbType.Int32, perfil.ccdPerfil);

                if (perfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfil.bidAtivo);

                if (perfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfil.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(PerfilEntity perfil, string cdsPerfis)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPerfilSelect");

                if (perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, perfil.nidPerfil);

                if (!string.IsNullOrEmpty(perfil.cdsPerfil))
                    _db.AddInParameter(dbCommand, "@p_cdsPerfil", DbType.String, perfil.cdsPerfil);

                if (perfil.ccdPerfil > 0)
                    _db.AddInParameter(dbCommand, "@p_ccdPerfil", DbType.Int32, perfil.ccdPerfil);

                if (perfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, perfil.nidUsuarioAtualizacao);

                if (perfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, perfil.bidAtivo);

                if (!string.IsNullOrEmpty(cdsPerfis))
                    _db.AddInParameter(dbCommand, "@p_cdsPerfis", DbType.String, cdsPerfis);

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
