using _3M.Comodato.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace _3M.Comodato.Data
{
    public class RelatorioReclamacaoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public RelatorioReclamacaoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<RelatorioReclamacaoEntity> ObterListaEntity(RelatorioReclamacaoEntity entity, DateTime? periodoInicial, DateTime? periodoFinal)
        {
            Func<DataRow, RelatorioReclamacaoEntity> Converter = new Func<DataRow, RelatorioReclamacaoEntity>((r) =>
                {
                    RelatorioReclamacaoEntity relatorioReclamacao = new RelatorioReclamacaoEntity();

                    relatorioReclamacao.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(r["ID_RELATORIO_RECLAMACAO"]);
                    relatorioReclamacao.ST_STATUS_RR = Convert.ToInt32(r["ST_STATUS_RR"]);
                    relatorioReclamacao.tecnicoEntity.CD_TECNICO = r["CD_TECNICO"].ToString();
                    relatorioReclamacao.clienteEntity.CD_CLIENTE = Convert.ToInt32(r["CD_CLIENTE"]);
                    relatorioReclamacao.clienteEntity.NM_CLIENTE = r["NM_CLIENTE"].ToString();
                    relatorioReclamacao.ativoFixoEntity.CD_ATIVO_FIXO = r["CD_ATIVO_FIXO"].ToString();
                    relatorioReclamacao.TecnicoSolicitante = r["TECNICO_SOLICITANTE"].ToString();
                    relatorioReclamacao.clienteEntity.NM_CLIENTE = r["NM_CLIENTE"].ToString();
                    relatorioReclamacao.pecaEntity.CD_PECA = r["CD_PECA"].ToString();
                    relatorioReclamacao.pecaEntity.DS_PECA = r["DS_PECA"].ToString();
                    relatorioReclamacao.rrStatusEntity.DS_STATUS_NOME_REDUZ = r["DS_STATUS_NOME_REDUZ"].ToString();
                    relatorioReclamacao.TipoAtendimento = Convert.ToInt32(r["CD_TIPO_ATENDIMENTO"]);
                    relatorioReclamacao.TipoReclamacaoRR = Convert.ToInt32(r["CD_TIPO_RECLAMACAO"]);
                    relatorioReclamacao.TEMPO_ATENDIMENTO = Convert.ToInt32(r["VL_TEMPO_ATENDIMENTO"]);
                    relatorioReclamacao.NM_Fornecedor = r["NM_Fornecedor"].ToString();
                    relatorioReclamacao.Custo_Peca = Convert.ToDecimal("0" + r["VL_CUSTO_PECA"]);
                    //relatorioReclamacao.Custo_Total = Convert.ToDecimal("0" + r["CUSTO_TOTAL"]);
                    relatorioReclamacao.ValorMaoDeObra = Convert.ToDecimal("0" + r["VL_MAO_OBRA"]);
                    relatorioReclamacao.DS_MOTIVO = r["DS_MOTIVO"].ToString();
                    relatorioReclamacao.DS_DESCRICAO = r["DS_DESCRICAO"].ToString();
                    relatorioReclamacao.CD_GRUPO_RESPONS = r["CD_GRUPO_RESPONS"].ToString();
                    relatorioReclamacao.osPadraoEntity.ID_OS = Convert.ToInt64("0" + r["ID_OS"]);
                    // relatorioReclamacao.ID_USUARIO_RESPONS = Convert.ToInt32(r["ID_USUARIO_RESPONS"].ToString());
                    relatorioReclamacao.DS_ARQUIVO_FOTO = r["DS_ARQUIVO_FOTO"].ToString();
                    relatorioReclamacao.DS_TIPO_FOTO = r["DS_TIPO_FOTO"].ToString();

                    if(!string.IsNullOrEmpty(r["Dt_Criacao"].ToString()))
                    relatorioReclamacao.DataCadastro = Convert.ToDateTime(r["Dt_Criacao"]);

                    return relatorioReclamacao;
                });

            DataTable dataTable = this.ObterLista(entity, periodoInicial, periodoFinal);
            return (from r in dataTable.Rows.Cast<DataRow>() select Converter(r)).ToList();
        }


        public IEnumerable<RelatorioReclamacaoEntity> ObterListaEntity(RelatorioReclamacaoEntity entity)
        {
            Func<DataRow, RelatorioReclamacaoEntity> Converter = new Func<DataRow, RelatorioReclamacaoEntity>((r) =>
            {
                RelatorioReclamacaoEntity relatorioReclamacao = new RelatorioReclamacaoEntity();

                relatorioReclamacao.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(r["ID_RELATORIO_RECLAMACAO"]);
                relatorioReclamacao.ST_STATUS_RR = Convert.ToInt32(r["ST_STATUS_RR"]);
                relatorioReclamacao.tecnicoEntity.CD_TECNICO = r["CD_TECNICO"].ToString();
                relatorioReclamacao.clienteEntity.CD_CLIENTE = Convert.ToInt32(r["CD_CLIENTE"]);
                relatorioReclamacao.clienteEntity.NM_CLIENTE = r["NM_CLIENTE"].ToString();
                relatorioReclamacao.ativoFixoEntity.CD_ATIVO_FIXO = r["CD_ATIVO_FIXO"].ToString();
                relatorioReclamacao.TecnicoSolicitante = r["TECNICO_SOLICITANTE"].ToString();
                relatorioReclamacao.clienteEntity.NM_CLIENTE = r["NM_CLIENTE"].ToString();
                relatorioReclamacao.pecaEntity.CD_PECA = r["CD_PECA"].ToString();
                relatorioReclamacao.rrStatusEntity.DS_STATUS_NOME_REDUZ = r["DS_STATUS_NOME_REDUZ"].ToString();
                relatorioReclamacao.TipoAtendimento = Convert.ToInt32(r["CD_TIPO_ATENDIMENTO"]);
                relatorioReclamacao.TipoReclamacaoRR = Convert.ToInt32(r["CD_TIPO_RECLAMACAO"]);
                relatorioReclamacao.TEMPO_ATENDIMENTO = Convert.ToInt32(r["VL_TEMPO_ATENDIMENTO"]);
                relatorioReclamacao.NM_Fornecedor = r["NM_Fornecedor"].ToString();
                relatorioReclamacao.Custo_Peca = Convert.ToDecimal("0" + r["VL_CUSTO_PECA"]);
                //relatorioReclamacao.Custo_Total = Convert.ToDecimal("0" + r["CUSTO_TOTAL"]);
                relatorioReclamacao.ValorMaoDeObra = Convert.ToDecimal("0" + r["VL_MAO_OBRA"]);
                relatorioReclamacao.DS_MOTIVO = r["DS_MOTIVO"].ToString();
                relatorioReclamacao.DS_DESCRICAO = r["DS_DESCRICAO"].ToString();
                relatorioReclamacao.CD_GRUPO_RESPONS = r["CD_GRUPO_RESPONS"].ToString();
                relatorioReclamacao.osPadraoEntity.ID_OS = Convert.ToInt64("0" + r["ID_OS"]);
                // relatorioReclamacao.ID_USUARIO_RESPONS = Convert.ToInt32(r["ID_USUARIO_RESPONS"].ToString());
                relatorioReclamacao.DS_ARQUIVO_FOTO = r["DS_ARQUIVO_FOTO"].ToString();
                relatorioReclamacao.DS_TIPO_FOTO = r["DS_TIPO_FOTO"].ToString();

                if (!string.IsNullOrEmpty(r["Dt_Criacao"].ToString()))
                    relatorioReclamacao.DataCadastro = Convert.ToDateTime(r["Dt_Criacao"]);

                relatorioReclamacao.pecaEntity.DS_PECA = r["DS_PECA"].ToString();
                return relatorioReclamacao;
            });

            DataTable dataTable = this.ObterLista(entity, null, null);
            return (from r in dataTable.Rows.Cast<DataRow>() select Converter(r)).ToList();
        }


        public DataTable ObterLista(RelatorioReclamacaoEntity entity, DateTime? periodoInicial = null, DateTime? periodoFinal = null)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRRelatorioReclamacaoSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_RELATORIO_RECLAMACAO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_RELATORIO_RECLAMACAO", DbType.Int64, entity.ID_RELATORIO_RECLAMACAO);
                    }

                    if (Convert.ToInt32(entity.ST_STATUS_RR) > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_RR_STATUS", DbType.Int64, entity.ST_STATUS_RR);
                    }

                    if (!string.IsNullOrEmpty(entity.tecnicoEntity.CD_TECNICO))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, entity.tecnicoEntity.CD_TECNICO);
                    }

                    //Possui ST_STATUS_PEDIDO = 0 (Rascunho)
                    if (entity.clienteEntity.CD_CLIENTE > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int32, entity.clienteEntity.CD_CLIENTE);
                    }

                    if (!string.IsNullOrEmpty(entity.ativoFixoEntity.CD_ATIVO_FIXO))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.Int64, entity.ativoFixoEntity.CD_ATIVO_FIXO);
                    }

                    if (!string.IsNullOrEmpty(entity.pecaEntity.CD_PECA))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.Int64, entity.pecaEntity.CD_PECA);
                    }

                    if (entity.TipoAtendimento > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_TIPO_ATENDIMENTO", DbType.Int64, entity.TipoAtendimento);
                    }

                    if (entity.TipoReclamacaoRR > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_TIPO_RECLAMACAO", DbType.Int64, entity.TipoReclamacaoRR);
                    }

                    if (entity.osPadraoEntity.ID_OS > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, entity.osPadraoEntity.ID_OS);
                    }
                    #endregion
                }

                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
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

        public string ObterNovoCodigoPedido()
        {
            DbConnection connection = null;
            try
            {
                using (dbCommand = _db.GetSqlStringCommand("SELECT NEXT VALUE FOR SEQ_WF_PEDIDO_EQUIP"))
                {

                    connection = _db.CreateConnection();
                    dbCommand.Connection = connection;
                    connection.Open();

                    return _db.ExecuteScalar(dbCommand).ToString();
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
        }

        public bool Inserir(RelatorioReclamacaoEntity relatorioReclamacao, DbTransaction transacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("dbo.prcRRRelatorioReclamacaoInsert");

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, relatorioReclamacao.ID_USUARIO_RESPONS);

                if (relatorioReclamacao.ST_STATUS_RR != int.MinValue)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_RR_STATUS", DbType.Int64, relatorioReclamacao.ST_STATUS_RR);
                }

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, relatorioReclamacao.CD_CLIENTE);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, relatorioReclamacao.CD_TECNICO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, relatorioReclamacao.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, relatorioReclamacao.CD_PECA);

                if (relatorioReclamacao.TipoAtendimento > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_ATENDIMENTO", DbType.Int64, relatorioReclamacao.TipoAtendimento);

                if (relatorioReclamacao.TipoReclamacaoRR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_RECLAMACAO", DbType.Int64, relatorioReclamacao.TipoReclamacaoRR);

                if (!string.IsNullOrEmpty(relatorioReclamacao.DS_MOTIVO))
                    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO", DbType.String, relatorioReclamacao.DS_MOTIVO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.DS_DESCRICAO))
                    _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, relatorioReclamacao.DS_DESCRICAO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.DS_ARQUIVO_FOTO))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO", DbType.String, relatorioReclamacao.DS_ARQUIVO_FOTO);

                if (relatorioReclamacao.osPadraoEntity.ID_OS > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, relatorioReclamacao.osPadraoEntity.ID_OS);

                _db.AddInParameter(dbCommand, "@p_TEMPO_ATENDIMENTO", DbType.Int32, relatorioReclamacao.TEMPO_ATENDIMENTO);
                _db.AddInParameter(dbCommand, "@p_TOKEN", DbType.Int64, relatorioReclamacao.TOKEN);
                _db.AddOutParameter(dbCommand, "@p_TOKEN_GERADO", DbType.Int64, 18);
                _db.AddOutParameter(dbCommand, "@p_nidRRRelatorioReclamacao", DbType.Int64, 18);

                if (transacao != null)
                {
                    _db.ExecuteNonQuery(dbCommand, transacao);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand);
                }

                relatorioReclamacao.ID_RELATORIO_RECLAMACAO = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_nidRRRelatorioReclamacao"));
                relatorioReclamacao.TOKEN = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_TOKEN_GERADO"));

                blnOK = true;
            }
 
            catch (Exception ex)
            {
                throw ex;
            }
            return blnOK;
        }

        public void Excluir(RelatorioReclamacaoEntity relatorioreclamacao, DbTransaction transacao = null)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("dbo.prcRRRelatorioReclamacaoDelete");

                _db.AddInParameter(dbCommand, "@p_nidRRRelatorioReclamacao", DbType.Int64, relatorioreclamacao.ID_RELATORIO_RECLAMACAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, relatorioreclamacao.ID_USUARIO_RESPONS);

                if (transacao != null)
                {
                    _db.ExecuteNonQuery(dbCommand, transacao);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand);
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

        public bool Alterar(RelatorioReclamacaoEntity relatorioReclamacao, DbTransaction transacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("dbo.prcRRRelatorioReclamacaoUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_RELATORIO_RECLAMACAO", DbType.Int64, relatorioReclamacao.ID_RELATORIO_RECLAMACAO);

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, relatorioReclamacao.ID_USUARIO_RESPONS);

                if (relatorioReclamacao.ST_STATUS_RR != int.MinValue)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_RR", DbType.Int64, relatorioReclamacao.ST_STATUS_RR);
                }

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_CLIENTE))
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, relatorioReclamacao.CD_CLIENTE);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_TECNICO))
                    _db.AddInParameter(dbCommand, "@p_CD_TECNICO", DbType.String, relatorioReclamacao.CD_TECNICO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_ATIVO_FIXO))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, relatorioReclamacao.CD_ATIVO_FIXO);

                if (!string.IsNullOrEmpty(relatorioReclamacao.CD_PECA))
                    _db.AddInParameter(dbCommand, "@p_CD_PECA", DbType.String, relatorioReclamacao.CD_PECA);

                if (relatorioReclamacao.TipoAtendimento > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_ATENDIMENTO", DbType.Int64, relatorioReclamacao.TipoAtendimento);

                if (relatorioReclamacao.TipoReclamacaoRR > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_RECLAMACAO", DbType.Int64, relatorioReclamacao.TipoReclamacaoRR);

                if (!string.IsNullOrEmpty(relatorioReclamacao.DS_MOTIVO))
                    _db.AddInParameter(dbCommand, "@p_DS_MOTIVO", DbType.String, relatorioReclamacao.DS_MOTIVO);


                if (!string.IsNullOrEmpty(relatorioReclamacao.DS_DESCRICAO))
                    _db.AddInParameter(dbCommand, "@p_DS_DESCRICAO", DbType.String, relatorioReclamacao.DS_DESCRICAO);



                //if (!string.IsNullOrEmpty(relatorioReclamacao.DS_ARQUIVO_FOTO))
                //    _db.AddInParameter(dbCommand, "@p_DS_ARQUIVO_FOTO", DbType.String, relatorioReclamacao.DS_ARQUIVO_FOTO);


                if (!string.IsNullOrEmpty(relatorioReclamacao.NM_Fornecedor))
                    _db.AddInParameter(dbCommand, "@p_NM_FORNECEDOR", DbType.String, relatorioReclamacao.NM_Fornecedor);


                if (relatorioReclamacao.Custo_Peca > 0)
                    _db.AddInParameter(dbCommand, "@p_CUSTO_PECA", DbType.Decimal, relatorioReclamacao.Custo_Peca);

                if (relatorioReclamacao.Custo_Total > 0)
                    _db.AddInParameter(dbCommand, "@p_CUSTO_TOTAL", DbType.Decimal, relatorioReclamacao.Custo_Total);

                if (relatorioReclamacao.ValorMaoDeObra > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_MAO_OBRA", DbType.Decimal, relatorioReclamacao.ValorMaoDeObra);


                if (relatorioReclamacao.TEMPO_ATENDIMENTO > 0)
                    _db.AddInParameter(dbCommand, "@p_TEMPO_ATENDIMENTO", DbType.Int32, relatorioReclamacao.TEMPO_ATENDIMENTO);


                if (transacao != null)
                {
                    _db.ExecuteNonQuery(dbCommand, transacao);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand);
                }
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

        public int ObterCodigoGrupo(string CD_GRUPO, Int32 nidUsuario)
        {
            int ID_Grupo = 0;


            DbConnection connection = null;
            DataTable dataTable = null;
            DataTableReader dataTableReader = null;
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcRRGrupoUsuario");

             //   _db.AddInParameter(dbCommand, "@p_ST_RR_STATUS", DbType.Int64, ST_RR_STATUS);
                _db.AddInParameter(dbCommand, "@p_CD_GRUPOWF", DbType.String, CD_GRUPO);
                _db.AddInParameter(dbCommand, "@p_nidUsuario", DbType.Int32, nidUsuario);
                


                connection = _db.CreateConnection();
                dbCommand.Connection = connection;
                connection.Open();

                DataSet dataSet = _db.ExecuteDataSet(dbCommand);
                dataTable = dataSet.Tables[0];
                dataTableReader = dataTable.CreateDataReader();
                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        ID_Grupo = Convert.ToInt32(dataTableReader["ID_GRUPOWF"]);
                    }
                }

                return ID_Grupo;
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
            //return dataTable;
            //
            return ID_Grupo;

        }


        /// <summary>
        /// Obtem lista de RelatorioReclamacao Ativos para o sincronismo Mobile
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>  
        public IList<RelatorioReclamacaoSincEntity> ObterListaRelatorioReclamacaoSinc(Int64? idUsuario)
        {
            try
            {
                IList<RelatorioReclamacaoSincEntity> listaRelatorioReclamacao = new List<RelatorioReclamacaoSincEntity>();
                SqlDataReader SDR = null;
                using (SqlConnection cnx = new SqlConnection(_db.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText =
                                         @" select t.* 
                                              from tbRRrelatorioReclamacao t 
                                             inner join tb_tecnico tt 
                                                ON tt.CD_TECNICO = t.CD_TECNICO 
                                             where dtmDataHoraAtualizacao > (getdate() - 90) 
                                               AND (tt.id_usuario = @id_usuario OR @id_usuario = '') ";

                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_USUARIO", SqlDbType.NVarChar).Value = Convert.ToString(idUsuario);

                        cmd.Connection = cnx;
                        cnx.Open();
                        SDR = cmd.ExecuteReader();

                        while (SDR.Read())
                        {
                            RelatorioReclamacaoSincEntity relatorioReclamacao = new RelatorioReclamacaoSincEntity
                            {
                                ID_RELATORIO_RECLAMACAO = Convert.ToInt64(SDR["ID_RELATORIO_RECLAMACAO"] is DBNull ? 0 : SDR["ID_RELATORIO_RECLAMACAO"]),
                                ST_STATUS_RR = Convert.ToInt64(SDR["ST_STATUS_RR"] is DBNull ? 0 : SDR["ST_STATUS_RR"]),
                                CD_TECNICO = Convert.ToString(SDR["CD_TECNICO"] is DBNull ? "" : SDR["CD_TECNICO"].ToString()),
                                CD_CLIENTE = Convert.ToString(SDR["CD_CLIENTE"] is DBNull ? "" : SDR["CD_CLIENTE"].ToString()),
                                CD_ATIVO_FIXO = Convert.ToString(SDR["CD_ATIVO_FIXO"] is DBNull ? "" : SDR["CD_ATIVO_FIXO"].ToString()),
                                CD_PECA = Convert.ToString(SDR["CD_PECA"] is DBNull ? "" : SDR["CD_PECA"].ToString()),
                                CD_TIPO_ATENDIMENTO = Convert.ToInt64(SDR["CD_TIPO_ATENDIMENTO"] is DBNull ? 0 : SDR["CD_TIPO_ATENDIMENTO"]),
                                CD_TIPO_RECLAMACAO = Convert.ToInt64(SDR["CD_TIPO_RECLAMACAO"] is DBNull ? 0 : SDR["CD_TIPO_RECLAMACAO"]),
                                DS_MOTIVO = Convert.ToString(SDR["DS_MOTIVO"] is DBNull ? "" : SDR["DS_MOTIVO"].ToString()),
                                DS_DESCRICAO = Convert.ToString(SDR["DS_DESCRICAO"] is DBNull ? "" : SDR["DS_DESCRICAO"].ToString()),
                                VL_TEMPO_ATENDIMENTO = Convert.ToInt32(SDR["VL_TEMPO_ATENDIMENTO"] is DBNull ? "" : SDR["VL_TEMPO_ATENDIMENTO"].ToString()),
                                DS_TIPO_FOTO = Convert.ToString(SDR["DS_TIPO_FOTO"] is DBNull ? "" : SDR["DS_TIPO_FOTO"].ToString()),
                                DS_ARQUIVO_FOTO = Convert.ToString(SDR["DS_ARQUIVO_FOTO"] is DBNull ? "" : SDR["DS_ARQUIVO_FOTO"].ToString()),
                                NM_FORNECEDOR = Convert.ToString(SDR["NM_FORNECEDOR"] is DBNull ? "" : SDR["NM_FORNECEDOR"].ToString()),
                                VL_MAO_OBRA = Convert.ToDecimal(SDR["VL_MAO_OBRA"] is DBNull ? "0" : SDR["VL_MAO_OBRA"].ToString()),
                                VL_CUSTO_PECA = Convert.ToDecimal(SDR["VL_CUSTO_PECA"] is DBNull ? "0" : SDR["VL_CUSTO_PECA"].ToString()),
                                CD_GRUPO_RESPONS = Convert.ToString(SDR["CD_GRUPO_RESPONS"] is DBNull ? "" : SDR["CD_GRUPO_RESPONS"].ToString()),
                                ID_OS = Convert.ToInt64(SDR["ID_OS"].ToString()),
                                TOKEN = Convert.ToInt64(SDR["TOKEN"].ToString())
                            };

                            listaRelatorioReclamacao.Add(relatorioReclamacao);
                        }
                        cnx.Close();
                        return listaRelatorioReclamacao;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
