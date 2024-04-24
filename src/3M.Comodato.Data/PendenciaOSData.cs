using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class PendenciaOSData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public PendenciaOSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref PendenciaOSEntity pendenciaOS)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSInsert");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pendenciaOS.OS.ID_OS);
                _db.AddInParameter(dbCommand, "@p_DT_ABERTURA", DbType.DateTime, pendenciaOS.DT_ABERTURA);
                _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, pendenciaOS.DS_DESCRICAO);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pendenciaOS.peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pendenciaOS.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pendenciaOS.QT_PECA);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PENDENCIA", DbType.String, pendenciaOS.ST_STATUS_PENDENCIA);
                _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pendenciaOS.CD_TP_ESTOQUE_CLI_TEC);
                _db.AddInParameter(dbCommand, "@p_ST_TP_PENDENCIA", DbType.String, pendenciaOS.ST_TP_PENDENCIA);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pendenciaOS.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, pendenciaOS.TOKEN);
                _db.AddOutParameter(dbCommand, "@p_ID_PENDENCIA_OS", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                pendenciaOS.ID_PENDENCIA_OS = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PENDENCIA_OS"));
                pendenciaOS.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

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

        public void Excluir(PendenciaOSEntity pendenciaOS)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSDelete");

                _db.AddInParameter(dbCommand, "@p_ID_PENDENCIA_OS", DbType.Int64, pendenciaOS.ID_PENDENCIA_OS);

                if (pendenciaOS.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pendenciaOS.nidUsuarioAtualizacao);

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

        public bool Alterar(PendenciaOSEntity pendenciaOS)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_PENDENCIA_OS", DbType.Int64, pendenciaOS.ID_PENDENCIA_OS);

                if (pendenciaOS.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pendenciaOS.OS.ID_OS);

                if (pendenciaOS.DT_ABERTURA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_ABERTURA", DbType.DateTime, pendenciaOS.DT_ABERTURA);

                _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, pendenciaOS.DS_DESCRICAO);

                if (!string.IsNullOrEmpty(pendenciaOS.peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pendenciaOS.peca.CD_PECA);

                if (!string.IsNullOrEmpty(pendenciaOS.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pendenciaOS.tecnico.CD_TECNICO);

                if (pendenciaOS.QT_PECA > 0)
                    _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pendenciaOS.QT_PECA);

                if (!string.IsNullOrEmpty(pendenciaOS.ST_STATUS_PENDENCIA))
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_PENDENCIA", DbType.String, pendenciaOS.ST_STATUS_PENDENCIA);

                if (!string.IsNullOrEmpty(pendenciaOS.CD_TP_ESTOQUE_CLI_TEC))
                    _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pendenciaOS.CD_TP_ESTOQUE_CLI_TEC);

                if (!string.IsNullOrEmpty(pendenciaOS.ST_TP_PENDENCIA))
                    _db.AddInParameter(dbCommand, "@p_ST_TP_PENDENCIA", DbType.String, pendenciaOS.ST_TP_PENDENCIA);

                if (pendenciaOS.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pendenciaOS.nidUsuarioAtualizacao);

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

        public void AlterarStatus(PendenciaOSEntity pendenciaOS)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSUpdateStatus");

                _db.AddInParameter(dbCommand, "@p_ID_PENDENCIA_OS", DbType.Int64, pendenciaOS.ID_PENDENCIA_OS);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PENDENCIA", DbType.String, pendenciaOS.ST_STATUS_PENDENCIA);

                if (pendenciaOS.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pendenciaOS.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(PendenciaOSEntity pendenciaOS)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSSelect");

                if (pendenciaOS.ID_PENDENCIA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_PENDENCIA_OS", DbType.Int64, pendenciaOS.ID_PENDENCIA_OS);

                if (pendenciaOS.OS.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, pendenciaOS.OS.ID_OS);

                if (pendenciaOS.DT_ABERTURA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_ABERTURA", DbType.DateTime, pendenciaOS.DT_ABERTURA);

                _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, pendenciaOS.DS_DESCRICAO);

                if (!string.IsNullOrEmpty(pendenciaOS.peca.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, pendenciaOS.peca.CD_PECA);

                if (!string.IsNullOrEmpty(pendenciaOS.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, pendenciaOS.tecnico.CD_TECNICO);

                if (pendenciaOS.QT_PECA > 0)
                    _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, pendenciaOS.QT_PECA);

                if (!string.IsNullOrEmpty(pendenciaOS.ST_STATUS_PENDENCIA))
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_PENDENCIA", DbType.String, pendenciaOS.ST_STATUS_PENDENCIA);

                if (!string.IsNullOrEmpty(pendenciaOS.CD_TP_ESTOQUE_CLI_TEC))
                    _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, pendenciaOS.CD_TP_ESTOQUE_CLI_TEC);

                if (!string.IsNullOrEmpty(pendenciaOS.ST_TP_PENDENCIA))
                    _db.AddInParameter(dbCommand, "@p_ST_TP_PENDENCIA", DbType.String, pendenciaOS.ST_TP_PENDENCIA);

                if (pendenciaOS.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, pendenciaOS.nidUsuarioAtualizacao);

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

        public DataTable ObterListaCliente(Int64 CD_CLIENTE, Int64 ID_OS, string CD_TECNICO)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcPendenciaOSSelectCliente");

                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, ID_OS);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.Int64, Convert.ToInt64(CD_TECNICO));

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


        /// <summary>
        /// Obtem lista de Pendencias em Ordem de serviço de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario">ID do Usuario do tecnico</param>
        /// <returns></returns>  
        public IList<PendenciaOSSinc> ObterListaPendenciaOsSinc(Int64 idUsuario)
        {
            try
            {
                IList<PendenciaOSSinc> listaPendenciaOs = new List<PendenciaOSSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @"select pe.*, cli.CD_CLIENTE, osp.DT_DATA_OS 
                                                from tbPendenciaOS pe 
                                            inner join tbOSPadrao osp
                                                on pe.ID_OS = osp.ID_OS
                                            inner join TB_CLIENTE cli
                                                on osp.CD_CLIENTE = cli.CD_CLIENTE
                                            Where pe.ST_STATUS_PENDENCIA = '1' --PENDENTE
                                                and pe.ID_OS in (select o.ID_OS 
                                                                    from tbOSPadrao o 
                                                                inner join tb_tecnico t 
                                                                    ON t.cd_tecnico = o.cd_tecnico 
                                                                WHERE (t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO))";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;
                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            PendenciaOSSinc pendencia = new PendenciaOSSinc
                            {
                                ID_PENDENCIA_OS = Convert.ToInt64(SDR["ID_PENDENCIA_OS"].ToString()),
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                DT_ABERTURA = Convert.ToDateTime(SDR["DT_ABERTURA"] is DBNull ? "01/01/2000" : SDR["DT_ABERTURA"]),
                                DS_DESCRICAO = Convert.ToString(SDR["DS_DESCRICAO"] is DBNull ? "" : SDR["DS_DESCRICAO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString()),
                                QT_PECA = Convert.ToInt32(SDR["QT_PECA"] is DBNull ? 0 : SDR["QT_PECA"]),
                                ST_STATUS_PENDENCIA = Convert.ToChar(SDR["ST_STATUS_PENDENCIA"] is DBNull ? " " : SDR["ST_STATUS_PENDENCIA"].ToString()),
                                CD_TP_ESTOQUE_CLI_TEC = Convert.ToChar(SDR["CD_TP_ESTOQUE_CLI_TEC"] is DBNull ? " " : SDR["CD_TP_ESTOQUE_CLI_TEC"].ToString()),
                                ST_TP_PENDENCIA = Convert.ToChar(SDR["ST_TP_PENDENCIA"] is DBNull ? " " : SDR["ST_TP_PENDENCIA"].ToString()),
                                nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]),
                                dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                                CD_CLIENTE = SDR["CD_CLIENTE"].ToString(),
                                DT_DATA_OS = Convert.ToDateTime(SDR["DT_DATA_OS"] is DBNull ? "01/01/2000" : SDR["DT_DATA_OS"])
                            };

                            listaPendenciaOs.Add(pendencia);
                        }
                        cnx.Close();
                        return listaPendenciaOs;
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

    }
}
