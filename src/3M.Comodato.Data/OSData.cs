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
    public class OSData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public OSData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref OSEntity osEntity)
        {

            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("prcOSInsert");

                _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA", DbType.DateTime, osEntity.DT_DATA_ABERTURA);
                _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);
                _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.ativoFixo.CD_ATIVO_FIXO);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, osEntity.visita.ID_VISITA);
                _db.AddInParameter(dbCommand, "@p_TP_MANUTENCAO", DbType.String, osEntity.TP_MANUTENCAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);
                _db.AddOutParameter(dbCommand, "@p_ID_OS", DbType.Int64, 18);

                _db.ExecuteNonQuery(dbCommand);

                osEntity.ID_OS = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_OS"));
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

        public void Excluir(OSEntity osEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcOSDelete");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, osEntity.ID_OS);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

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

        public void ExcluirSemAtivo(OSEntity osEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcOSDeleteSemAtivo");

                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, osEntity.visita.ID_VISITA);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

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

        public bool Alterar(OSEntity osEntity)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.String, osEntity.ID_OS);

                if (osEntity.DT_DATA_ABERTURA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA", DbType.DateTime, osEntity.DT_DATA_ABERTURA);

                if (osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (!string.IsNullOrEmpty(osEntity.ativoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.ativoFixo.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(osEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.tecnico.CD_TECNICO);

                if (!string.IsNullOrEmpty(osEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);

                if (osEntity.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, osEntity.visita.ID_VISITA);

                if (!string.IsNullOrEmpty(osEntity.TP_MANUTENCAO))
                    _db.AddInParameter(dbCommand, "@p_TP_MANUTENCAO", DbType.String, osEntity.TP_MANUTENCAO);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

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

        public DataTable ObterLista(OSEntity osEntity)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSSelect");

                if (osEntity.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.String, osEntity.ID_OS);

                if (osEntity.DT_DATA_ABERTURA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA", DbType.DateTime, osEntity.DT_DATA_ABERTURA);

                if (osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (!string.IsNullOrEmpty(osEntity.ativoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.ativoFixo.CD_ATIVO_FIXO);

                if (!String.IsNullOrEmpty(osEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.tecnico.CD_TECNICO);

                if (!String.IsNullOrEmpty(osEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);

                if (osEntity.visita.ID_VISITA != 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, osEntity.visita.ID_VISITA);

                if (!string.IsNullOrEmpty(osEntity.TP_MANUTENCAO))
                    _db.AddInParameter(dbCommand, "@p_TP_MANUTENCAO", DbType.String, osEntity.TP_MANUTENCAO);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

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

        public DataTable ObterListaPecas(Int64 ID_VISITA)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSPecasSelect");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);

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

        public DataTable ObterListaVisita(OSEntity osEntity, string DT_DATA_VISITA, Int64 CD_CLIENTE)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSSelectVisita");

                if (osEntity.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.String, osEntity.ID_OS);

                if (osEntity.DT_DATA_ABERTURA != null)
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_ABERTURA", DbType.DateTime, osEntity.DT_DATA_ABERTURA);

                if (osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ST_TP_STATUS_VISITA_OS", DbType.Int32, osEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS);

                if (!string.IsNullOrEmpty(osEntity.ativoFixo.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, osEntity.ativoFixo.CD_ATIVO_FIXO);

                if (!String.IsNullOrEmpty(osEntity.tecnico.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, osEntity.tecnico.CD_TECNICO);

                if (!String.IsNullOrEmpty(osEntity.DS_OBSERVACAO))
                    _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, osEntity.DS_OBSERVACAO);

                if (osEntity.visita.ID_VISITA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, osEntity.visita.ID_VISITA);

                if (!string.IsNullOrEmpty(osEntity.TP_MANUTENCAO))
                    _db.AddInParameter(dbCommand, "@p_TP_MANUTENCAO", DbType.String, osEntity.TP_MANUTENCAO);

                if (osEntity.nidUsuarioAtualizacao > 0)
                    _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, osEntity.nidUsuarioAtualizacao);

                if (!string.IsNullOrEmpty(DT_DATA_VISITA))
                    _db.AddInParameter(dbCommand, "@p_DT_DATA_VISITA", DbType.DateTime, Convert.ToDateTime(DT_DATA_VISITA));

                if (CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, CD_CLIENTE);

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
        /// Verifica se todas as OS da Visita estão Finalizadas ou Canceladas
        /// </summary>
        /// <param name="ID_VISITA"></param>
        /// <returns>TRUE - Todas as OS finalizadas e/ou canceladas. FALSE - existem OS com outros status</returns>
        public bool VerificarOSFechadaCancelada(Int64 ID_VISITA, bool? VerificarSoCanceladas = false)
        {
            bool retornoValidacao = false;
            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcOSVerificarOSFechadaCancelada");

                _db.AddInParameter(dbCommand, "@p_ID_VISITA", DbType.Int64, ID_VISITA);
                _db.AddInParameter(dbCommand, "@p_VerificarSoCanceladas", DbType.Boolean, VerificarSoCanceladas);

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];

                DataTableReader dataTableReader = dataTable.CreateDataReader();
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (Convert.ToInt64(dataTableReader["REGISTROS"]) <= 0)
                            retornoValidacao = true;
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
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
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return retornoValidacao;
        }



        /// <summary>
        /// Obtem lista de Ordem de serviço de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<OsSinc> ObterListaOsSinc(Int64 idUsuario)
        {
            try
            {
                IList<OsSinc> listaOs = new List<OsSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select o.* from tbOS o Where o.ID_VISITA in ( " +
                                         "   select v.id_visita from tbvisita v " +
                                         "   inner join tb_tecnico t ON t.cd_tecnico = v.cd_tecnico " +
                                         "   WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO) and " +
                                         "   o.id_visita = v.ID_VISITA AND " +
                                         "         v.DT_DATA_VISITA >= DATEADD(hour,-3, getdate()) - " +
                                         "         Convert(int, (select cvlParametro as QTD from tbParametro where ccdParametro = 'qtdDiasRetroativos')) ) ";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", System.Data.SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            OsSinc os = new OsSinc();
                            os.ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString());
                            os.DT_DATA_ABERTURA = Convert.ToDateTime(SDR["DT_DATA_ABERTURA"] is DBNull ? "01/01/2000" : SDR["DT_DATA_ABERTURA"]);
                            os.ST_TP_STATUS_VISITA_OS = Convert.ToInt32(SDR["ST_TP_STATUS_VISITA_OS"] is DBNull ? 0 : SDR["ST_TP_STATUS_VISITA_OS"]);
                            os.CD_ATIVO_FIXO = Convert.ToString(SDR["CD_ATIVO_FIXO"] is DBNull ? "" : SDR["CD_ATIVO_FIXO"].ToString());
                            os.CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString());
                            os.ID_VISITA = Convert.ToInt64(SDR["ID_VISITA"].ToString());
                            os.TP_MANUTENCAO = Convert.ToChar(SDR["TP_MANUTENCAO"] is DBNull ? " " : SDR["TP_MANUTENCAO"].ToString());
                            os.DS_OBSERVACAO = Convert.ToString(SDR["DS_OBSERVACAO"] is DBNull ? "" : SDR["DS_OBSERVACAO"].ToString());
                            os.nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]);
                            os.dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]);

                            listaOs.Add(os);
                        }
                        cnx.Close();
                        return listaOs;
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
