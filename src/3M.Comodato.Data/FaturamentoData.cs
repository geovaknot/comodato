using System;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using _3M.Comodato.Entity;
using System.Collections.Generic;

namespace _3M.Comodato.Data
{
    public class FaturamentoData
    {
        readonly Database _db;
        DbCommand dbCommand;

        public FaturamentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public bool Inserir(ref DadosFaturamentoEntity faturamentoEntity)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcDadosFaturamentoInsert");

                _db.AddInParameter(dbCommand, "@p_CD_Material", DbType.String, faturamentoEntity.CD_Material);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, faturamentoEntity.CD_Cliente);
                _db.AddInParameter(dbCommand, "@p_NRAtivo", DbType.String, faturamentoEntity.CD_Material);
                _db.AddInParameter(dbCommand, "@p_DepartamentoVenda", DbType.String, faturamentoEntity.DepartamentoVenda);
                _db.AddInParameter(dbCommand, "@p_AluguelApos3Anos", DbType.Double, faturamentoEntity.AluguelApos3anos);
                _db.AddInParameter(dbCommand, "@p_DT_UltimoFaturamento", DbType.DateTime, faturamentoEntity.DT_UltimoFaturamento);
                _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.Int64, faturamentoEntity.ID_ATIVO_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_AtivoFixo", DbType.String, faturamentoEntity.AtivoFixo);
                _db.ExecuteNonQuery(dbCommand);

                //faturamentoEntity.ID_PECA_OS = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_PECA_OS"));
                //Mensagem = _db.GetParameterValue(dbCommand, "@p_Mensagem").ToString();
                //faturamentoEntity.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

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

        /*public void Excluir(FaturamentoDetalhamentoEntity faturamentoEntity)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("prcFaturamentoDelete");

                _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, faturamentoEntity.OS.ID_OS);
                _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, faturamentoEntity.peca.CD_PECA);
                _db.AddInParameter(dbCommand, "@p_QT_PECA", DbType.Decimal, faturamentoEntity.QT_PECA);
                _db.AddInParameter(dbCommand, "@p_CD_TP_ESTOQUE_CLI_TEC", DbType.String, faturamentoEntity.CD_TP_ESTOQUE_CLI_TEC);
                _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, faturamentoEntity.tecnico.CD_TECNICO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, faturamentoEntity.cliente.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, faturamentoEntity.nidUsuarioAtualizacao);
                _db.AddInParameter(dbCommand, "@p_ID_PECA_OS", DbType.Int64, faturamentoEntity.ID_PECA_OS);

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

        }*/

        public DataTable ObterLista(DadosFaturamentoEntity faturamento)
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcFaturamentoSelect");

                if (faturamento.ID_ATIVO_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_ATIVO_CLIENTE", DbType.Int64, faturamento.ID_ATIVO_CLIENTE);

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

        public DataTable ObterDeptoVenda()
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            //String deptoVenda;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcParametroDeptoSelect");                

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

        public DataTable ObterCodigoMaterial()
        {

            DbConnection connection = null;
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            //String deptoVenda;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcCodigoMaterialSelect");

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
        /// Obtem lista de Pecas trocadas em Ordem de serviço de um tecnico/usuario para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        /*public IList<FaturamentoSinc> ObterListaPecaOsSinc(Int64 idUsuario)
        {
            try
            {
                IList<FaturamentoSinc> listaPecaOs = new List<FaturamentoSinc>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         " select po.*, o.DT_DATA_OS from tbFaturamento po "+

                                            "inner join tbOSPadrao o(nolock) on "+

                                               " o.ID_OS = po.ID_OS " +

                                            "inner join tb_tecnico t(nolock) ON " +

                                                "t.cd_tecnico = o.cd_tecnico " +

                                            " WHERE(t.ID_USUARIO = @ID_USUARIO OR t.ID_USUARIO_COORDENADOR = @ID_USUARIO)";

                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.BigInt).Value = idUsuario;

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();
                        while (SDR.Read())
                        {
                            FaturamentoSinc pecaOS = new FaturamentoSinc
                            {
                                ID_PECA_OS = Convert.ToInt64(SDR["ID_PECA_OS"].ToString()),
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                QT_PECA = Convert.ToInt32(SDR["QT_PECA"] is DBNull ? 0 : SDR["QT_PECA"]),
                                CD_TP_ESTOQUE_CLI_TEC = Convert.ToChar(SDR["CD_TP_ESTOQUE_CLI_TEC"] is DBNull ? " " : SDR["CD_TP_ESTOQUE_CLI_TEC"].ToString()),
                                nidUsuarioAtualizacao = Convert.ToInt64(SDR["nidUsuarioAtualizacao"] is DBNull ? 0 : SDR["nidUsuarioAtualizacao"]),
                                dtmDataHoraAtualizacao = Convert.ToDateTime(SDR["dtmDataHoraAtualizacao"] is DBNull ? "01/01/2000" : SDR["dtmDataHoraAtualizacao"]),
                                DS_OBSERVACAO = SDR["DS_OBSERVACAO"].ToString() ?? "",
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString()),
                                DT_DATA_OS = Convert.ToDateTime(SDR["DT_DATA_OS"] is DBNull ? "01/01/2000" : SDR["DT_DATA_OS"])
                            };

                            listaPecaOs.Add(pecaOS);
                        }
                        cnx.Close();
                        return listaPecaOs;
                    }
                }
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


    }*/
    }
}