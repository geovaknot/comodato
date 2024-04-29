using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Data
{
    public class UsuarioPerfilData
    {

        readonly Database _db;
        DbCommand dbCommand;

        public UsuarioPerfilData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref UsuarioPerfilEntity usuarioPerfil)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioPerfil.nidUsuarioAtualizacao);
                //_db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.bidAtivo);
                _db.AddOutParameter(dbCommand, "@p_nidUsuarioPerfil", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                usuarioPerfil.nidUsuarioPerfil = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidUsuarioPerfil"));

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

        public void Excluir(UsuarioPerfilEntity usuarioPerfil)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilDelete");

                if (usuarioPerfil.nidUsuarioPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioPerfil", DbType.Int64, usuarioPerfil.nidUsuarioPerfil);

                if (usuarioPerfil.usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);

                if (usuarioPerfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioPerfil.nidUsuarioAtualizacao);

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

        public bool Alterar(UsuarioPerfilEntity usuarioPerfil)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilUpdate");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioPerfil", DbType.Int64, usuarioPerfil.nidUsuarioPerfil);

                if (usuarioPerfil.usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);

                if (usuarioPerfil.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);

                if (usuarioPerfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioPerfil.nidUsuarioAtualizacao);

                if (usuarioPerfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.bidAtivo);

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

        public DataTable ObterLista(UsuarioPerfilEntity usuarioPerfil)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilSelect");

                if (usuarioPerfil.nidUsuarioPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioPerfil", DbType.Int64, usuarioPerfil.nidUsuarioPerfil);

                if (usuarioPerfil.usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);

                if (usuarioPerfil.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);

                if (usuarioPerfil.perfil.ccdPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_ccdPerfil", DbType.Int64, usuarioPerfil.perfil.ccdPerfil);

                if (usuarioPerfil.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuarioPerfil.nidUsuarioAtualizacao);

                if (usuarioPerfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.bidAtivo);

                if (usuarioPerfil.usuario.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivoUsuario", DbType.Boolean, usuarioPerfil.usuario.bidAtivo);

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

        public DataTable ObterListaPerfil(UsuarioPerfilEntity usuarioPerfil, string cdsPerfis)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilSelectPerfil");

                _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.usuario.bidAtivo);

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

        public DataTable ObterListaALLUsuario(UsuarioPerfilEntity usuarioPerfil, bool? usuarioAtivo, bool semPerfil)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilSelectALLUsuario");

                //if (usuarioPerfil.usuario.empresa.nidEmpresa != 0)
                //    _db.AddInParameter(dbCommand, "@p_nidEmpresa", DbType.Int64, usuarioPerfil.usuario.empresa.nidEmpresa);

                if (usuarioPerfil.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);

                if (usuarioPerfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.bidAtivo);

                if (!string.IsNullOrEmpty(usuarioPerfil.usuario.cnmNome))
                    _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuarioPerfil.usuario.cnmNome);

                if (!string.IsNullOrEmpty(usuarioPerfil.usuario.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuarioPerfil.usuario.cdsEmail);

                if (usuarioAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivoUsuario", DbType.Boolean, usuarioAtivo);

                _db.AddInParameter(dbCommand, "@p_bidSemPerfil", DbType.Boolean, semPerfil);
                

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

        public DataTable ObterListaDistinctUsuario(UsuarioPerfilEntity usuarioPerfil, bool? usuarioAtivo, bool semPerfil)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilSelectDistinctUsuario");

                if (usuarioPerfil.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);

                if (usuarioPerfil.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuarioPerfil.bidAtivo);

                if (!string.IsNullOrEmpty(usuarioPerfil.usuario.cnmNome))
                    _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuarioPerfil.usuario.cnmNome);

                if (!string.IsNullOrEmpty(usuarioPerfil.usuario.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuarioPerfil.usuario.cdsEmail);

                if (usuarioAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivoUsuario", DbType.Boolean, usuarioAtivo);

                _db.AddInParameter(dbCommand, "@p_bidSemPerfil", DbType.Boolean, semPerfil);


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

        public DataTable ObterListaALLUsuarioExcetoPerfil(UsuarioPerfilEntity usuarioPerfil)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilSelectExcetoPerfil");

                if (usuarioPerfil.perfil.nidPerfil != 0)
                    _db.AddInParameter(dbCommand, "@p_nidPerfil", DbType.Int64, usuarioPerfil.perfil.nidPerfil);

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

        public bool VerificarAcesso(UsuarioPerfilEntity usuarioPerfil, string ccdFuncao)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            bool Retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilVerificarAcesso");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);
                _db.AddInParameter(dbCommand, "@p_ccdFuncao", DbType.String, ccdFuncao);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                Retorno = (dataSet.Tables[0].Rows.Count > 0 ? true : false);

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
            return Retorno;

        }


        public DataTable VerificarAcesso(UsuarioPerfilEntity usuarioPerfil)
        {
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioPerfilVerificarAcesso");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuarioPerfil.usuario.nidUsuario);

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
