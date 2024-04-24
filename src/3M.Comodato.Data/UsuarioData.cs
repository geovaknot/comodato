using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class UsuarioData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public UsuarioData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");

        }

        public bool Inserir(ref UsuarioEntity usuario)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioInsert");

                _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuario.cnmNome);
                _db.AddInParameter(dbCommand, "@p_cdsLogin", DbType.String, usuario.cdsLogin);
                _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuario.cdsEmail);
                _db.AddInParameter(dbCommand, "@p_cdsSenha", DbType.String, usuario.cdsSenha);
                _db.AddInParameter(dbCommand, "@p_cd_empresa", DbType.String, usuario.cd_empresa);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuario.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_dtmDataHoraTrocaLoginExterno", DbType.DateTime, usuario.dtmDataHoraTrocaLoginExterno);
                //_db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuario.bidAtivo);
                _db.AddInParameter(dbCommand, "@p_ccdChaveAcessoTrocarSenha", DbType.String, usuario.ccdChaveAcessoTrocarSenha);
                _db.AddOutParameter(dbCommand, "@p_nidUsuario", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                usuario.nidUsuario = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidUsuario"));

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

        public void Excluir(UsuarioEntity usuario)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcUsuarioDelete");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuario.nidUsuario);

                if (usuario.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuario.nidUsuarioAtualizacao);

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

        public bool Alterar(UsuarioEntity usuario)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioUpdate");

                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuario.nidUsuario);

                if (usuario.cnmNome != null)
                    _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuario.cnmNome);

                if (usuario.cdsLogin != null)
                    _db.AddInParameter(dbCommand, "@p_cdsLogin", DbType.String, usuario.cdsLogin);

                if (usuario.cdsEmail != null)
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuario.cdsEmail);

                if (usuario.cd_empresa > 0)
                    _db.AddInParameter(dbCommand, "@p_cd_empresa", DbType.String, usuario.cd_empresa);

                if (usuario.cdsSenha != null)
                    _db.AddInParameter(dbCommand, "@p_cdsSenha", DbType.String, usuario.cdsSenha);

                if (usuario.dtmDataHoraTrocaLoginExterno != null)
                    _db.AddInParameter(dbCommand, "@p_dtmDataHoraTrocaLoginExterno", DbType.DateTime, usuario.dtmDataHoraTrocaLoginExterno);

                if (usuario.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuario.nidUsuarioAtualizacao);

                if (usuario.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuario.bidAtivo);

                if (usuario.ccdChaveAcessoTrocarSenha != null)
                    _db.AddInParameter(dbCommand, "@p_ccdChaveAcessoTrocarSenha", DbType.String, usuario.ccdChaveAcessoTrocarSenha);

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

        public DataTable ObterLista(UsuarioEntity usuario)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioSelect");

                if (usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuario.nidUsuario);

                if (!string.IsNullOrEmpty(usuario.cnmNome))
                    _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuario.cnmNome);

                if (!string.IsNullOrEmpty(usuario.cdsLogin))
                    _db.AddInParameter(dbCommand, "@p_cdsLogin", DbType.String, usuario.cdsLogin);

                if (!string.IsNullOrEmpty(usuario.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuario.cdsEmail);

                if (!string.IsNullOrEmpty(usuario.cdsSenha))
                    _db.AddInParameter(dbCommand, "@p_cdsSenha", DbType.String, usuario.cdsSenha);

                if (usuario.dtmDataHoraTrocaLoginExterno != null)
                    _db.AddInParameter(dbCommand, "@p_dtmDataHoraTrocaLoginExterno", DbType.DateTime, usuario.dtmDataHoraTrocaLoginExterno);

                if (usuario.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuario.nidUsuarioAtualizacao);

                if (usuario.bidAtivo != null)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuario.bidAtivo);

                if (usuario.ccdChaveAcessoTrocarSenha != null)
                    _db.AddInParameter(dbCommand, "@p_ccdChaveAcessoTrocarSenha", DbType.String, usuario.ccdChaveAcessoTrocarSenha);

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

        public DataTable ObterLista(UsuarioEntity usuario, string idUsuarios, string cdsPerfis)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcUsuarioSelect");

                if (usuario.nidUsuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuario.nidUsuario);

                if (!string.IsNullOrEmpty(usuario.cnmNome))
                    _db.AddInParameter(dbCommand, "@p_cnmNome", DbType.String, usuario.cnmNome);

                if (!string.IsNullOrEmpty(usuario.cdsLogin))
                    _db.AddInParameter(dbCommand, "@p_cdsLogin", DbType.String, usuario.cdsLogin);

                if (!string.IsNullOrEmpty(usuario.cdsEmail))
                    _db.AddInParameter(dbCommand, "@p_cdsEmail", DbType.String, usuario.cdsEmail);

                if (!string.IsNullOrEmpty(usuario.cdsSenha))
                    _db.AddInParameter(dbCommand, "@p_cdsSenha", DbType.String, usuario.cdsSenha);

                if (usuario.dtmDataHoraTrocaLoginExterno != null)
                    _db.AddInParameter(dbCommand, "@p_dtmDataHoraTrocaLoginExterno", DbType.DateTime, usuario.dtmDataHoraTrocaLoginExterno);

                if (usuario.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, usuario.nidUsuarioAtualizacao);

                if (usuario.bidAtivo == true)
                    _db.AddInParameter(dbCommand, "@p_bidAtivo", DbType.Boolean, usuario.bidAtivo);

                if (usuario.ccdChaveAcessoTrocarSenha != null)
                    _db.AddInParameter(dbCommand, "@p_ccdChaveAcessoTrocarSenha", DbType.String, usuario.ccdChaveAcessoTrocarSenha);

                if (!string.IsNullOrEmpty(idUsuarios))
                    _db.AddInParameter(dbCommand, "@p_nidUsuarios", DbType.String, idUsuarios);

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

        public IList<UsuarioSinc> ObterListaUsuarioSinc(long idUsuario)
        {
            try
            {
                IList<UsuarioSinc> listaUsuario = new List<UsuarioSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select * from tbUsuario where nidUsuario = @ID_USUARIO ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            UsuarioSinc usuario = new UsuarioSinc();
                            usuario.nidUsuario = Convert.ToInt64(SDR["nidUsuario"].ToString());
                            usuario.cnmNome = Convert.ToString(SDR["cnmNome"] is DBNull ? "" : SDR["cnmNome"].ToString());
                            usuario.cdsLogin = Convert.ToString(SDR["cdsLogin"] is DBNull ? "" : SDR["cdsLogin"].ToString());
                            usuario.cdsEmail = Convert.ToString(SDR["cdsEmail"] is DBNull ? "" : SDR["cdsEmail"].ToString());
                            usuario.cdsSenha = Convert.ToString(SDR["cdsSenha"] is DBNull ? "" : SDR["cdsSenha"].ToString());
                            usuario.bidAtivo = Convert.ToBoolean(SDR["bidAtivo"] is DBNull ? false : SDR["bidAtivo"]);
                            usuario.ccdChaveAcessoTrocarSenha= Convert.ToString(SDR["ccdChaveAcessoTrocarSenha"] is DBNull ? "" : SDR["ccdChaveAcessoTrocarSenha"].ToString());
                            usuario.dtmDataHoraTrocaLoginExterno= Convert.ToDateTime(SDR["dtmDataHoraTrocaLoginExterno"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraTrocaLoginExterno"]);
                            usuario.cd_empresa = Convert.ToInt64(SDR["CD_EMPRESA"] is DBNull ? 0 : SDR["CD_EMPRESA"]);
                            usuario.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            usuario.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);

                            listaUsuario.Add(usuario);
                        }
                        cnx.Close();
                        return listaUsuario;
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable ObterListaSubordinados(Int64 usuario)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSubordinadosSelect");

                if (usuario != 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int64, usuario);

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

        public void GravaLogTrocaSenha(string log)
        {
            using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
            {
                cnx.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " INSERT INTO tbLogTrocaSenha " +
                                         " ( DT_DATA_LOG, DS_LOG ) " +
                                         " VALUES ( getdate(), @ds_log ); " +
                                         " select @@IDENTITY ;";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ds_log", System.Data.SqlDbType.NVarChar, -1).Value = log;

                        cmd.Connection = cnx;
                        cmd.ExecuteScalar();
                        cnx.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cnx.Close();
                    cnx.Dispose();
                }

            }
        }
    }
}
