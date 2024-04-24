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
    public class SolicitacaoAtendimentoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public SolicitacaoAtendimentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }



        public string ObterNovoCodigo()
        {
            DbConnection connection = null;
            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                using (dbCommand = _db.GetSqlStringCommand("SELECT NEXT VALUE FOR SEQ_TB_SOLICITA_ATEND"))
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

        public bool Inserir(SolicitacaoAtendimentoEntity solicitacao)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSolicitaAtendimentoInsert");

                _db.AddInParameter(dbCommand, "@p_ID_SOLICITA_ATENDIMENTO", DbType.Int64, solicitacao.ID_SOLICITA_ATENDIMENTO);
                _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.String, solicitacao.CLIENTE.CD_CLIENTE);
                _db.AddInParameter(dbCommand, "@p_ID_USU_SOLICITANTE", DbType.Int64, solicitacao.ID_USU_SOLICITANTE);
                _db.AddInParameter(dbCommand, "@p_DT_DATA_SOLICITACAO", DbType.DateTime, solicitacao.DT_DATA_SOLICITACAO);
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, solicitacao.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_ID_TIPO_ATENDIMENTO", DbType.Int32, solicitacao.TipoAtendimento.CD_TIPO_ATENDIMENTO);

                if (!String.IsNullOrEmpty(solicitacao.AtivoFixo.CD_ATIVO_FIXO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, solicitacao.AtivoFixo.CD_ATIVO_FIXO);
                }

                _db.AddInParameter(dbCommand, "@p_DS_CONTATO", DbType.String, solicitacao.DS_CONTATO);
                _db.AddInParameter(dbCommand, "@p_ID_STATUS_ATENDIMENTO", DbType.Int32, solicitacao.StatusAtendimento.ID_STATUS_ATENDIMENTO);

                if (solicitacao.OS.ID_OS > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, solicitacao.OS.ID_OS);
                }

                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.String, solicitacao.nidUsuarioAtualizacao);

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
        public bool Alterar(SolicitacaoAtendimentoEntity solicitacao)
        {
            bool retorno = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSolicitaAtendimentoUpdate");
                _db.AddInParameter(dbCommand, "@p_ID_SOLICITA_ATENDIMENTO", DbType.Int64, solicitacao.ID_SOLICITA_ATENDIMENTO);
                _db.AddInParameter(dbCommand, "@p_ID_STATUS_ATENDIMENTO", DbType.Int32, solicitacao.StatusAtendimento.ID_STATUS_ATENDIMENTO);
                if (solicitacao.OS.ID_OS > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, solicitacao.OS.ID_OS);
                }
                _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, solicitacao.DS_OBSERVACAO);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.String, solicitacao.nidUsuarioAtualizacao);
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

        public IEnumerable<SolicitacaoAtendimentoEntity> ObterListaEntity(SolicitacaoAtendimentoEntity solicitacaoAtendimento)
        {
            return this.ObterListaEntity(solicitacaoAtendimento, null);
        }
        public IEnumerable<SolicitacaoAtendimentoEntity> ObterListaEntity(SolicitacaoAtendimentoEntity solicitacaoAtendimento, DateTime? periodoInicio)
        {
            Func<DataRow, SolicitacaoAtendimentoEntity> solicitacaoAtendimentoConverter = new Func<DataRow, SolicitacaoAtendimentoEntity>((row) =>
            {
                SolicitacaoAtendimentoEntity entity = new SolicitacaoAtendimentoEntity();

                entity.ID_SOLICITA_ATENDIMENTO = Convert.ToInt64(row["ID_SOLICITA_ATENDIMENTO"]);

                entity.CLIENTE.CD_CLIENTE = Convert.ToInt64(row["CD_CLIENTE"]);
                entity.CLIENTE.NM_CLIENTE = row["NM_CLIENTE"].ToString();

                entity.ID_USU_SOLICITANTE = Convert.ToInt64(row["ID_USU_SOLICITANTE"]);
                entity.DT_DATA_SOLICITACAO = Convert.ToDateTime(row["DT_DATA_SOLICITACAO"]);
                entity.nidUsuarioAtualizacao = Convert.ToInt64(row["nidUsuarioAtualizacao"]);
                entity.dtmDataHoraAtualizacao = Convert.ToDateTime(row["dtmDataHoraAtualizacao"]);

                //TipoAtendimento
                entity.TipoAtendimento.ID_TIPO_ATENDIMENTO = Convert.ToInt32(row["ID_TIPO_ATENDIMENTO"]);
                entity.TipoAtendimento.CD_TIPO_ATENDIMENTO = Convert.ToString(row["CD_TIPO_ATENDIMENTO"]);
                entity.TipoAtendimento.DS_TIPO_ATENDIMENTO = Convert.ToString(row["DS_TIPO_ATENDIMENTO"]);

                //StatusAtendimento
                entity.StatusAtendimento.ID_STATUS_ATENDIMENTO = Convert.ToInt32(row["ID_STATUS_ATENDIMENTO"]);
                entity.StatusAtendimento.DS_STATUS_ATENDIMENTO = Convert.ToString(row["DS_STATUS_ATENDIMENTO"]);

                if (row["DS_OBSERVACAO"] != DBNull.Value)
                {
                    entity.DS_OBSERVACAO = row["DS_OBSERVACAO"].ToString();
                }

                if (row["CD_ATIVO_FIXO"] != DBNull.Value)
                {
                    entity.AtivoFixo.CD_ATIVO_FIXO = Convert.ToString(row["CD_ATIVO_FIXO"]);
                }

                if (row["DS_CONTATO"] != DBNull.Value)
                {
                    entity.DS_CONTATO = Convert.ToString(row["DS_CONTATO"]);
                }

                if (row["ID_OS"] != DBNull.Value)
                {
                    entity.OS.ID_OS = Convert.ToInt64(row["ID_OS"]);
                }

                entity.QT_EQUIPAMENTO = Convert.ToInt32(row["QTD_EQUIPAMENTO"]);

                return entity;
            });

            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                DataTable dataTable = this.ObterLista(solicitacaoAtendimento, periodoInicio);
                listaSolicitacaoAtendimento = (from r in dataTable.Rows.Cast<DataRow>()
                                               select solicitacaoAtendimentoConverter(r)).ToList();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listaSolicitacaoAtendimento;
        }

        public DataTable ObterLista(SolicitacaoAtendimentoEntity solicitacaoAtendimento)
        {
            return this.ObterLista(solicitacaoAtendimento, null);
        }
        public DataTable ObterLista(SolicitacaoAtendimentoEntity solicitacaoAtendimento, DateTime? periodoInicio)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcSolicitaAtendimentoSelect");
                if (null != solicitacaoAtendimento)
                {
                    #region Parâmetros de Entrada

                    if (solicitacaoAtendimento.ID_SOLICITA_ATENDIMENTO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_SOLICITA_ATENDIMENTO", DbType.Int64, solicitacaoAtendimento.ID_SOLICITA_ATENDIMENTO);
                    }

                    if (solicitacaoAtendimento.CLIENTE.CD_CLIENTE != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, solicitacaoAtendimento.CLIENTE.CD_CLIENTE);
                    }

                    if (solicitacaoAtendimento.ID_USU_SOLICITANTE != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_USU_SOLICITANTE", DbType.Int64, solicitacaoAtendimento.ID_USU_SOLICITANTE);
                    }

                    if (solicitacaoAtendimento.DT_DATA_SOLICITACAO != DateTime.MinValue)
                    {
                        _db.AddInParameter(dbCommand, "@p_DT_DATA_SOLICITACAO", DbType.DateTime, solicitacaoAtendimento.DT_DATA_SOLICITACAO);
                    }

                    if (!string.IsNullOrEmpty(solicitacaoAtendimento.DS_OBSERVACAO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_OBSERVACAO", DbType.String, solicitacaoAtendimento.DS_OBSERVACAO);
                    }

                    if (solicitacaoAtendimento.TipoAtendimento.ID_TIPO_ATENDIMENTO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_TIPO_ATENDIMENTO", DbType.Int32, solicitacaoAtendimento.TipoAtendimento.ID_TIPO_ATENDIMENTO);
                    }

                    if (!string.IsNullOrEmpty(solicitacaoAtendimento.AtivoFixo.CD_ATIVO_FIXO))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO", DbType.String, solicitacaoAtendimento.AtivoFixo.CD_ATIVO_FIXO);
                    }

                    if (!string.IsNullOrEmpty(solicitacaoAtendimento.DS_CONTATO))
                    {
                        _db.AddInParameter(dbCommand, "@p_DS_CONTATO", DbType.String, solicitacaoAtendimento.DS_CONTATO);
                    }

                    if (solicitacaoAtendimento.StatusAtendimento.ID_STATUS_ATENDIMENTO != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_STATUS_ATENDIMENTO", DbType.Int32, solicitacaoAtendimento.StatusAtendimento.ID_STATUS_ATENDIMENTO);
                    }

                    if (solicitacaoAtendimento.OS.ID_OS != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_OS", DbType.Int64, solicitacaoAtendimento.OS.ID_OS);
                    }

                    if (periodoInicio.HasValue)
                    {
                        _db.AddInParameter(dbCommand, "@p_DT_INI_PERIODO_FILTRO", DbType.Date, periodoInicio.Value);
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
    }
}
