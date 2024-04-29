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
    public class PedidoEquipamentoData
    {
        private readonly Database _db;
        private DbCommand dbCommand;

        public PedidoEquipamentoData()
        {
            _db = DatabaseFactory.CreateDatabase("ConnectionString");
        }

        public IEnumerable<WfPedidoEquipEntity> ObterListaEntity(WfPedidoEquipEntity entity, DateTime? periodoInicial, DateTime? periodoFinal)
        {
            Func<DataRow, WfPedidoEquipEntity> Converter = new Func<DataRow, WfPedidoEquipEntity>((r) =>
            {
                WfPedidoEquipEntity pedidoEquipamento = new WfPedidoEquipEntity();

                pedidoEquipamento.ID_WF_PEDIDO_EQUIP = Convert.ToInt64(r["ID_WF_PEDIDO_EQUIP"]);
                pedidoEquipamento.CD_WF_PEDIDO_EQUIP = r["CD_WF_PEDIDO_EQUIP"].ToString();
                pedidoEquipamento.DT_PEDIDO = Convert.ToDateTime(r["DT_PEDIDO"]);

                pedidoEquipamento.TP_PEDIDO = r["TP_PEDIDO"].ToString();
                pedidoEquipamento.ST_STATUS_PEDIDO = Convert.ToInt32(r["ST_STATUS_PEDIDO"]);
                pedidoEquipamento.DS_STATUS_PEDIDO = r["DS_STATUS_NOME"].ToString();

                if (r["ID_USU_SOLICITANTE"] != DBNull.Value)
                    pedidoEquipamento.ID_USU_SOLICITANTE = Convert.ToInt64(r["ID_USU_SOLICITANTE"]);

                if (r["NM_USU_SOLICITANTE"] != DBNull.Value)
                    pedidoEquipamento.NM_USU_SOLICITANTE = r["NM_USU_SOLICITANTE"].ToString();

                if (r["CD_CLIENTE"] != DBNull.Value)
                    pedidoEquipamento.CD_CLIENTE = Convert.ToInt64(r["CD_CLIENTE"]);
                if (r["NM_CLIENTE"] != DBNull.Value)
                    pedidoEquipamento.NM_CLIENTE = r["NM_CLIENTE"].ToString();

                if (r["CD_MODELO"] != DBNull.Value)
                    pedidoEquipamento.CD_MODELO = r["CD_MODELO"].ToString();
                if (r["DS_MODELO"] != DBNull.Value)
                    pedidoEquipamento.DS_MODELO = r["DS_MODELO"].ToString();

                if (r["CD_TROCA"] != DBNull.Value)
                    pedidoEquipamento.CD_TROCA = r["CD_TROCA"].ToString();

                if (r["CD_ATIVO_FIXO_TROCA"] != DBNull.Value)
                    pedidoEquipamento.CD_ATIVO_FIXO_TROCA = r["CD_ATIVO_FIXO_TROCA"].ToString();

                pedidoEquipamento.DT_ULT_ATUALIZACAO = Convert.ToDateTime(r["DT_ULT_ATUALIZACAO"]);
                pedidoEquipamento.ID_USU_ULT_ATU  = Convert.ToInt64(r["ID_USU_ULT_ATU"]);


                if (r["ID_USU_EMITENTE"] != DBNull.Value)
                    pedidoEquipamento.ID_USU_EMITENTE= Convert.ToInt64(r["ID_USU_EMITENTE"]);

                if (r["DS_CONTATO_NOME"] != DBNull.Value)
                    pedidoEquipamento.DS_CONTATO_NOME = r["DS_CONTATO_NOME"].ToString();

                if (r["DS_CONTATO_EMAIL"] != DBNull.Value)
                    pedidoEquipamento.DS_CONTATO_EMAIL = r["DS_CONTATO_EMAIL"].ToString();

                if (r["DS_CONTATO_TEL_NUM"] != DBNull.Value)
                    pedidoEquipamento.DS_CONTATO_TEL_NUM = r["DS_CONTATO_TEL_NUM"].ToString();

                if (r["ID_CATEGORIA"] != DBNull.Value)
                    pedidoEquipamento.ID_CATEGORIA = Convert.ToInt64(r["ID_CATEGORIA"]);

                if (r["ID_TIPO_SOLICITACAO"] != DBNull.Value)
                    pedidoEquipamento.ID_TIPO_SOLICITACAO = Convert.ToInt32(r["ID_TIPO_SOLICITACAO"]);

                if (r["CD_LINHA"] != DBNull.Value)
                    pedidoEquipamento.CD_LINHA = r["CD_LINHA"].ToString();

                if (r["CD_TENSAO"] != DBNull.Value)
                    pedidoEquipamento.CD_TENSAO = Convert.ToInt32(r["CD_TENSAO"]);

                if (r["VL_VOLUME_ANO"] != DBNull.Value)
                    pedidoEquipamento.VL_VOLUME_ANO = Convert.ToDecimal(r["VL_VOLUME_ANO"]);

                if (r["CD_UNIDADE_MEDIDA"] != DBNull.Value)
                    pedidoEquipamento.CD_UNIDADE_MEDIDA = Convert.ToInt32(r["CD_UNIDADE_MEDIDA"]);

                if (r["QT_EQUIPAMENTO"] != DBNull.Value)
                    pedidoEquipamento.QT_EQUIPAMENTO = Convert.ToInt32(r["QT_EQUIPAMENTO"]);

                if (r["CD_COND_LIMPEZA"] != DBNull.Value)
                    pedidoEquipamento.CD_COND_LIMPEZA = Convert.ToInt32(r["CD_COND_LIMPEZA"]);

                if (r["CD_COND_TEMPERATURA"] != DBNull.Value)
                    pedidoEquipamento.CD_COND_TEMPERATURA = Convert.ToInt32(r["CD_COND_TEMPERATURA"]);

                if (r["CD_COND_UMIDADE"] != DBNull.Value)
                    pedidoEquipamento.CD_COND_UMIDADE = Convert.ToInt32(r["CD_COND_UMIDADE"]);

                if (r["VL_ALTURA_MIN"] != DBNull.Value)
                    pedidoEquipamento.VL_ALTURA_MIN = Convert.ToDecimal(r["VL_ALTURA_MIN"]);

                if (r["VL_ALTURA_MAX"] != DBNull.Value)
                    pedidoEquipamento.VL_ALTURA_MAX = Convert.ToDecimal(r["VL_ALTURA_MAX"]);

                if (r["VL_LARGURA_MIN"] != DBNull.Value)
                    pedidoEquipamento.VL_LARGURA_MIN = Convert.ToDecimal(r["VL_LARGURA_MIN"]);

                if (r["VL_LARGURA_MAX"] != DBNull.Value)
                    pedidoEquipamento.VL_LARGURA_MAX = Convert.ToDecimal(r["VL_LARGURA_MAX"]);

                if (r["VL_COMPRIM_MIN"] != DBNull.Value)
                    pedidoEquipamento.VL_COMPRIM_MIN = Convert.ToDecimal(r["VL_COMPRIM_MIN"]);

                if (r["VL_COMPRIM_MAX"] != DBNull.Value)
                    pedidoEquipamento.VL_COMPRIM_MAX = Convert.ToDecimal(r["VL_COMPRIM_MAX"]);

                if (r["CD_TIPO_FITA"] != DBNull.Value)
                    pedidoEquipamento.CD_TIPO_FITA = Convert.ToInt32(r["CD_TIPO_FITA"]);

                if (r["CD_MODELO_FITA"] != DBNull.Value)
                    pedidoEquipamento.CD_MODELO_FITA = Convert.ToInt32(r["CD_MODELO_FITA"]);

                if (r["CD_LARGURA_FITA"] != DBNull.Value)
                    pedidoEquipamento.CD_LARGURA_FITA = Convert.ToInt32(r["CD_LARGURA_FITA"]);

                if (r["CD_TIPO_PRODUTO"] != DBNull.Value)
                    pedidoEquipamento.CD_TIPO_PRODUTO = Convert.ToInt32(r["CD_TIPO_PRODUTO"]);

                if (r["CD_LOCAL_INSTAL"] != DBNull.Value)
                    pedidoEquipamento.CD_LOCAL_INSTAL = Convert.ToInt32(r["CD_LOCAL_INSTAL"]);

                if (r["CD_VELOCIDADE_LINHA"] != DBNull.Value)
                    pedidoEquipamento.CD_VELOCIDADE_LINHA = Convert.ToInt32(r["CD_VELOCIDADE_LINHA"]);

                if (r["CD_GUIA_POSICIONAMENTO"] != DBNull.Value)
                    pedidoEquipamento.CD_GUIA_POSICIONAMENTO = Convert.ToInt32(r["CD_GUIA_POSICIONAMENTO"]);

                if (r["ID_ETIQUETA"] != DBNull.Value)
                    pedidoEquipamento.ID_ETIQUETA = Convert.ToInt64(r["ID_ETIQUETA"]);

                if (r["VL_ALTURA_ETIQUETA"] != DBNull.Value)
                    pedidoEquipamento.VL_ALTURA_ETIQUETA = Convert.ToDecimal(r["VL_ALTURA_ETIQUETA"]);

                if (r["VL_LARGURA_ETIQUETA"] != DBNull.Value)
                    pedidoEquipamento.VL_LARGURA_ETIQUETA = Convert.ToDecimal(r["VL_LARGURA_ETIQUETA"]);

                if (r["FL_APLIC_DIREITO"] != DBNull.Value)
                    pedidoEquipamento.FL_APLIC_DIREITO = r["FL_APLIC_DIREITO"].ToString();

                if (r["FL_APLIC_ESQUERDO"] != DBNull.Value)
                    pedidoEquipamento.FL_APLIC_ESQUERDO = r["FL_APLIC_ESQUERDO"].ToString();

                if (r["FL_APLIC_SUPERIOR"] != DBNull.Value)
                    pedidoEquipamento.FL_APLIC_SUPERIOR = r["FL_APLIC_SUPERIOR"].ToString();

                if (r["DS_KITPVA_GRAMACAIXA"] != DBNull.Value)
                    pedidoEquipamento.DS_KITPVA_GRAMACAIXA = r["DS_KITPVA_GRAMACAIXA"].ToString();

                if (r["VL_STRETCH_PESOPALLET"] != DBNull.Value)
                    pedidoEquipamento.VL_STRETCH_PESOPALLET = Convert.ToDecimal(r["VL_STRETCH_PESOPALLET"]);

                if (r["VL_STRETCH_ALTURAPALLET"] != DBNull.Value)
                    pedidoEquipamento.VL_STRETCH_ALTURAPALLET = Convert.ToDecimal(r["VL_STRETCH_ALTURAPALLET"]);

                if (r["VL_INKJET_NUMCARACTER"] != DBNull.Value)
                    pedidoEquipamento.VL_INKJET_NUMCARACTER = Convert.ToInt32(r["VL_INKJET_NUMCARACTER"]);

                if (r["DS_ARQ_ANEXO"] != DBNull.Value)
                    pedidoEquipamento.DS_ARQ_ANEXO = r["DS_ARQ_ANEXO"].ToString();

                if (r["VL_PESO_MAXIMO"] != DBNull.Value)
                    pedidoEquipamento.VL_PESO_MAXIMO = Convert.ToDecimal(r["VL_PESO_MAXIMO"]);

                if (r["VL_PESO_MINIMO"] != DBNull.Value)
                    pedidoEquipamento.VL_PESO_MINIMO = Convert.ToDecimal(r["VL_PESO_MINIMO"]);

                if (r["DS_TITULO"] != DBNull.Value)
                    pedidoEquipamento.DS_TITULO = r["DS_TITULO"].ToString();

                if (r["TP_LOCACAO"] != DBNull.Value)
                    pedidoEquipamento.TP_LOCACAO = r["TP_LOCACAO"].ToString();

                if (r["CD_MOTIVO_DEVOLUCAO"] != DBNull.Value)
                    pedidoEquipamento.CD_MOTIVO_DEVOLUCAO = Convert.ToInt32(r["CD_MOTIVO_DEVOLUCAO"]);

                if (r["FL_COPIA_NF3M"] != DBNull.Value)
                    pedidoEquipamento.FL_COPIA_NF3M = r["FL_COPIA_NF3M"].ToString();

                if (r["VL_NOTA_FISCAL_3M"] != DBNull.Value)
                    pedidoEquipamento.VL_NOTA_FISCAL_3M = Convert.ToDecimal(r["VL_NOTA_FISCAL_3M"]);

                if (r["ID_USUARIO_RESPONS"] != DBNull.Value)
                    pedidoEquipamento.ID_USUARIO_RESPONS = Convert.ToInt64(r["ID_USUARIO_RESPONS"]);

                if (r["CD_GRUPO_RESPONS"] != DBNull.Value)
                    pedidoEquipamento.CD_GRUPO_RESPONS = r["CD_GRUPO_RESPONS"].ToString();

                return pedidoEquipamento;
            });

            DataTable dataTable = this.ObterLista(entity, periodoInicial, periodoFinal);
            return (from r in dataTable.Rows.Cast<DataRow>() select Converter(r)).ToList();
        }

        public DataTable ObterLista(WfPedidoEquipEntity entity, DateTime? periodoInicial, DateTime? periodoFinal)
        {
            DbConnection connection = null;
            DataTable dataTable = null;
            List<SolicitacaoAtendimentoEntity> listaSolicitacaoAtendimento = new List<SolicitacaoAtendimentoEntity>();
            try
            {
                dbCommand = _db.GetStoredProcCommand("prcWfPedidoEquipSelect");
                if (null != entity)
                {
                    #region Parâmetros de Entrada

                    if (entity.ID_WF_PEDIDO_EQUIP != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, entity.ID_WF_PEDIDO_EQUIP);
                    }

                    if (!string.IsNullOrEmpty(entity.CD_WF_PEDIDO_EQUIP))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_WF_PEDIDO_EQUIP", DbType.String, entity.CD_WF_PEDIDO_EQUIP);
                    }

                    if (!string.IsNullOrEmpty(entity.TP_PEDIDO))
                    {
                        _db.AddInParameter(dbCommand, "@p_TP_PEDIDO", DbType.String, entity.TP_PEDIDO);
                    }

                    //Possui ST_STATUS_PEDIDO = 0 (Rascunho)
                    if (entity.ST_STATUS_PEDIDO > 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int32, entity.ST_STATUS_PEDIDO);
                    }


                    if (entity.ID_USU_SOLICITANTE!= 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_ID_USU_SOLICITANTE", DbType.Int64, entity.ID_USU_SOLICITANTE);
                    }

                    if (entity.CD_CLIENTE != 0)
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, entity.CD_CLIENTE);
                    }

                    if (!string.IsNullOrEmpty(entity.CD_MODELO))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, entity.CD_MODELO);
                    }
                    if (!string.IsNullOrEmpty(entity.CD_TROCA))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_TROCA", DbType.String, entity.CD_TROCA);
                    }
                    if (!string.IsNullOrEmpty(entity.CD_ATIVO_FIXO_TROCA))
                    {
                        _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO_TROCA", DbType.String, entity.CD_ATIVO_FIXO_TROCA);
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


        public bool Inserir(ref WfPedidoEquipEntity wfPedidoEquip, DbTransaction transacao = null)
        {
            bool retorno = false;

            try
            {

                dbCommand = _db.GetStoredProcCommand("dbo.prcWfPedidoEquipInsert");

                _db.AddInParameter(dbCommand, "@p_CD_WF_PEDIDO_EQUIP", DbType.String, wfPedidoEquip.CD_WF_PEDIDO_EQUIP);
                _db.AddInParameter(dbCommand, "@p_DT_PEDIDO", DbType.DateTime, wfPedidoEquip.DT_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_TP_PEDIDO", DbType.String, wfPedidoEquip.TP_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Int64, wfPedidoEquip.ST_STATUS_PEDIDO);
                _db.AddInParameter(dbCommand, "@p_ID_USU_SOLICITANTE", DbType.Int64, wfPedidoEquip.ID_USU_SOLICITANTE);
                if (wfPedidoEquip.CD_CLIENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, wfPedidoEquip.CD_CLIENTE);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_MODELO))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, wfPedidoEquip.CD_MODELO);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_TROCA))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TROCA", DbType.String, wfPedidoEquip.CD_TROCA);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_ATIVO_FIXO_TROCA))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO_TROCA", DbType.String, wfPedidoEquip.CD_ATIVO_FIXO_TROCA);
                }

                _db.AddInParameter(dbCommand, "@p_DT_ULT_ATUALIZACAO", DbType.DateTime, DateTime.Now);

                _db.AddInParameter(dbCommand, "@p_ID_USU_ULT_ATU", DbType.Int64, wfPedidoEquip.ID_USU_ULT_ATU);
                if (wfPedidoEquip.ID_USU_EMITENTE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_USU_EMITENTE", DbType.Int64, wfPedidoEquip.ID_USU_EMITENTE);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_NOME))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_NOME", DbType.String, wfPedidoEquip.DS_CONTATO_NOME);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_EMAIL))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_EMAIL", DbType.String, wfPedidoEquip.DS_CONTATO_EMAIL);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_TEL_NUM))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_TEL_NUM", DbType.String, wfPedidoEquip.DS_CONTATO_TEL_NUM);
                }

                if (wfPedidoEquip.ID_CATEGORIA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_CATEGORIA", DbType.String, wfPedidoEquip.ID_CATEGORIA);
                }

                if (wfPedidoEquip.ID_TIPO_SOLICITACAO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_SOLICITACAO", DbType.Int64, wfPedidoEquip.ID_TIPO_SOLICITACAO);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_LINHA ))
                {
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA", DbType.String, wfPedidoEquip.CD_LINHA);
                }

                if (wfPedidoEquip.CD_TENSAO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TENSAO", DbType.Int64, wfPedidoEquip.CD_TENSAO);
                }

                if (wfPedidoEquip.VL_VOLUME_ANO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_VOLUME_ANO", DbType.Decimal, wfPedidoEquip.VL_VOLUME_ANO);
                }

                if (wfPedidoEquip.CD_UNIDADE_MEDIDA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_UNIDADE_MEDIDA", DbType.Int64, wfPedidoEquip.CD_UNIDADE_MEDIDA);
                }

                if (wfPedidoEquip.QT_EQUIPAMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_QT_EQUIPAMENTO", DbType.Int64, wfPedidoEquip.QT_EQUIPAMENTO);
                }

                if (wfPedidoEquip.CD_COND_LIMPEZA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_COND_LIMPEZA", DbType.Int64, wfPedidoEquip.CD_COND_LIMPEZA);
                }

                if (wfPedidoEquip.CD_COND_TEMPERATURA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_COND_TEMPERATURA", DbType.Int64, wfPedidoEquip.CD_COND_TEMPERATURA);
                }

                if (wfPedidoEquip.CD_COND_UMIDADE > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_COND_UMIDADE", DbType.Int64, wfPedidoEquip.CD_COND_UMIDADE);
                }

                if (wfPedidoEquip.VL_ALTURA_MIN > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_MIN", DbType.Decimal, wfPedidoEquip.VL_ALTURA_MIN);
                }

                if (wfPedidoEquip.VL_ALTURA_MAX > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_MAX", DbType.Decimal, wfPedidoEquip.VL_ALTURA_MAX);
                }

                if (wfPedidoEquip.VL_LARGURA_MIN > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_MIN", DbType.Decimal, wfPedidoEquip.VL_LARGURA_MIN);
                }

                if (wfPedidoEquip.VL_LARGURA_MAX > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_MAX", DbType.Decimal, wfPedidoEquip.VL_LARGURA_MAX);
                }

                if (wfPedidoEquip.VL_COMPRIM_MIN > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_COMPRIM_MIN", DbType.Decimal, wfPedidoEquip.VL_COMPRIM_MIN);
                }

                if (wfPedidoEquip.VL_COMPRIM_MAX > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_COMPRIM_MAX", DbType.Decimal, wfPedidoEquip.VL_COMPRIM_MAX);
                }

                if (wfPedidoEquip.CD_TIPO_FITA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_FITA", DbType.Int64, wfPedidoEquip.CD_TIPO_FITA);
                }

                if (wfPedidoEquip.CD_MODELO_FITA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO_FITA", DbType.Int64, wfPedidoEquip.CD_MODELO_FITA);
                }

                if (wfPedidoEquip.CD_LARGURA_FITA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_LARGURA_FITA", DbType.Int64, wfPedidoEquip.CD_LARGURA_FITA);
                }

                if (wfPedidoEquip.CD_TIPO_PRODUTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_PRODUTO", DbType.Int64, wfPedidoEquip.CD_TIPO_PRODUTO);
                }

                if (wfPedidoEquip.CD_LOCAL_INSTAL > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_LOCAL_INSTAL", DbType.Int64, wfPedidoEquip.CD_LOCAL_INSTAL);
                }

                if (wfPedidoEquip.CD_VELOCIDADE_LINHA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_VELOCIDADE_LINHA", DbType.Int64, wfPedidoEquip.CD_VELOCIDADE_LINHA);
                }

                if (wfPedidoEquip.CD_GUIA_POSICIONAMENTO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_GUIA_POSICIONAMENTO", DbType.Int64, wfPedidoEquip.CD_GUIA_POSICIONAMENTO);
                }

                if (wfPedidoEquip.ID_ETIQUETA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_ID_ETIQUETA", DbType.Int64, wfPedidoEquip.ID_ETIQUETA);
                }

                if (wfPedidoEquip.VL_ALTURA_ETIQUETA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_ETIQUETA", DbType.Decimal, wfPedidoEquip.VL_ALTURA_ETIQUETA);
                }

                if (wfPedidoEquip.VL_LARGURA_ETIQUETA > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_ETIQUETA", DbType.Decimal, wfPedidoEquip.VL_LARGURA_ETIQUETA);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_DIREITO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_DIREITO", DbType.String, wfPedidoEquip.FL_APLIC_DIREITO);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_ESQUERDO))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_ESQUERDO", DbType.String, wfPedidoEquip.FL_APLIC_ESQUERDO);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_SUPERIOR))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_SUPERIOR", DbType.String, wfPedidoEquip.FL_APLIC_SUPERIOR);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_KITPVA_GRAMACAIXA))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_KITPVA_GRAMACAIXA", DbType.String, wfPedidoEquip.DS_KITPVA_GRAMACAIXA);
                }

                if (wfPedidoEquip.VL_STRETCH_PESOPALLET > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_STRETCH_PESOPALLET", DbType.Decimal, wfPedidoEquip.VL_STRETCH_PESOPALLET);
                }

                if (wfPedidoEquip.VL_STRETCH_ALTURAPALLET > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_STRETCH_ALTURAPALLET", DbType.Decimal, wfPedidoEquip.VL_STRETCH_ALTURAPALLET);
                }

                if (wfPedidoEquip.VL_INKJET_NUMCARACTER > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_INKJET_NUMCARACTER", DbType.Int64, wfPedidoEquip.VL_INKJET_NUMCARACTER);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_ARQ_ANEXO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_ARQ_ANEXO", DbType.String, wfPedidoEquip.DS_ARQ_ANEXO);
                }


                if (wfPedidoEquip.VL_PESO_MAXIMO.HasValue)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_MAXIMO", DbType.Decimal, wfPedidoEquip.VL_PESO_MAXIMO.Value);
                }

                if (wfPedidoEquip.VL_PESO_MINIMO.HasValue)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_MINIMO", DbType.Decimal , wfPedidoEquip.VL_PESO_MINIMO.Value);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_TITULO))
                {
                    _db.AddInParameter(dbCommand, "@p_DS_TITULO", DbType.String, wfPedidoEquip.DS_TITULO);
                }

                if (!string.IsNullOrEmpty(wfPedidoEquip.TP_LOCACAO))
                {
                    _db.AddInParameter(dbCommand, "@p_TP_LOCACAO", DbType.String, wfPedidoEquip.TP_LOCACAO);
                }

                if (wfPedidoEquip.CD_MOTIVO_DEVOLUCAO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.Int32, wfPedidoEquip.CD_MOTIVO_DEVOLUCAO);
                }
                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_COPIA_NF3M))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_COPIA_NF3M", DbType.String, wfPedidoEquip.FL_COPIA_NF3M);
                }
                if (wfPedidoEquip.VL_NOTA_FISCAL_3M > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_NOTA_FISCAL_3M", DbType.Decimal, wfPedidoEquip.VL_NOTA_FISCAL_3M);
                }





                _db.AddOutParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, 18);


                if (transacao != null)
                {
                    _db.ExecuteNonQuery(dbCommand, transacao);
                }
                else
                {
                    _db.ExecuteNonQuery(dbCommand);
                }

                wfPedidoEquip.ID_WF_PEDIDO_EQUIP = Convert.ToInt64(_db.GetParameterValue(dbCommand, "@p_ID_WF_PEDIDO_EQUIP"));

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

        public void Excluir(WfPedidoEquipEntity wfPedidoEquip, DbTransaction transacao = null)
        {
            try
            {

                dbCommand = _db.GetStoredProcCommand("dbo.prcWfPedidoEquipDelete");

                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, wfPedidoEquip.ID_WF_PEDIDO_EQUIP);
                _db.AddInParameter(dbCommand, "@p_nidUsuarioAtualizacao", DbType.Int64, wfPedidoEquip.ID_USU_ULT_ATU);

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

        public bool Alterar(WfPedidoEquipEntity wfPedidoEquip, DbTransaction transacao = null)
        {
            bool blnOK = false;

            try
            {
                dbCommand = _db.GetStoredProcCommand("dbo.prcWfPedidoEquipUpdate");

                _db.AddInParameter(dbCommand, "@p_ID_WF_PEDIDO_EQUIP", DbType.Int64, wfPedidoEquip.ID_WF_PEDIDO_EQUIP);

                if (wfPedidoEquip.ID_USU_SOLICITANTE>0)
                    _db.AddInParameter(dbCommand, "@p_ID_USU_SOLICITANTE", DbType.Int64, wfPedidoEquip.ID_USU_SOLICITANTE);

                if (wfPedidoEquip.ST_STATUS_PEDIDO!= int.MinValue)
                {
                    _db.AddInParameter(dbCommand, "@p_ST_STATUS_PEDIDO", DbType.Decimal, wfPedidoEquip.ST_STATUS_PEDIDO);
                }

                if (wfPedidoEquip.CD_CLIENTE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_CLIENTE", DbType.Int64, wfPedidoEquip.CD_CLIENTE);

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_MODELO))
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO", DbType.String, wfPedidoEquip.CD_MODELO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_TROCA))
                    _db.AddInParameter(dbCommand, "@p_CD_TROCA", DbType.String, wfPedidoEquip.CD_TROCA);

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_ATIVO_FIXO_TROCA))
                    _db.AddInParameter(dbCommand, "@p_CD_ATIVO_FIXO_TROCA", DbType.String, wfPedidoEquip.CD_ATIVO_FIXO_TROCA);

                if (wfPedidoEquip.ID_USU_ULT_ATU>0)
                    _db.AddInParameter(dbCommand, "@p_ID_USU_ULT_ATU", DbType.Int64, wfPedidoEquip.ID_USU_ULT_ATU);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_NOME))
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_NOME", DbType.String, wfPedidoEquip.DS_CONTATO_NOME);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_EMAIL))
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_EMAIL", DbType.String, wfPedidoEquip.DS_CONTATO_EMAIL);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_CONTATO_TEL_NUM))
                    _db.AddInParameter(dbCommand, "@p_DS_CONTATO_TEL_NUM", DbType.String, wfPedidoEquip.DS_CONTATO_TEL_NUM);

                if (wfPedidoEquip.ID_CATEGORIA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_CATEGORIA", DbType.Int64, wfPedidoEquip.ID_CATEGORIA);

                if (wfPedidoEquip.ID_TIPO_SOLICITACAO > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_TIPO_SOLICITACAO", DbType.Int32, wfPedidoEquip.ID_TIPO_SOLICITACAO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.CD_LINHA ))
                    _db.AddInParameter(dbCommand, "@p_CD_LINHA", DbType.String, wfPedidoEquip.CD_LINHA);

                if (wfPedidoEquip.CD_TENSAO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TENSAO", DbType.Int32, wfPedidoEquip.CD_TENSAO);

                if (wfPedidoEquip.VL_VOLUME_ANO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_VOLUME_ANO", DbType.Decimal, wfPedidoEquip.VL_VOLUME_ANO);

                if (wfPedidoEquip.CD_UNIDADE_MEDIDA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_UNIDADE_MEDIDA", DbType.Int32, wfPedidoEquip.CD_UNIDADE_MEDIDA);

                if (wfPedidoEquip.QT_EQUIPAMENTO > 0)
                    _db.AddInParameter(dbCommand, "@p_QT_EQUIPAMENTO", DbType.Int32, wfPedidoEquip.QT_EQUIPAMENTO);

                if (wfPedidoEquip.CD_COND_LIMPEZA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_COND_LIMPEZA", DbType.Int32, wfPedidoEquip.CD_COND_LIMPEZA);

                if (wfPedidoEquip.CD_COND_TEMPERATURA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_COND_TEMPERATURA", DbType.Int32, wfPedidoEquip.CD_COND_TEMPERATURA);

                if (wfPedidoEquip.CD_COND_UMIDADE > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_COND_UMIDADE", DbType.Int32, wfPedidoEquip.CD_COND_UMIDADE);

                if (wfPedidoEquip.VL_ALTURA_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_MIN", DbType.Decimal, wfPedidoEquip.VL_ALTURA_MIN);

                if (wfPedidoEquip.VL_ALTURA_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_MAX", DbType.Decimal, wfPedidoEquip.VL_ALTURA_MAX);

                if (wfPedidoEquip.VL_LARGURA_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_MIN", DbType.Decimal, wfPedidoEquip.VL_LARGURA_MIN);

                if (wfPedidoEquip.VL_LARGURA_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_MAX", DbType.Decimal, wfPedidoEquip.VL_LARGURA_MAX);

                if (wfPedidoEquip.VL_COMPRIM_MIN > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMPRIM_MIN", DbType.Decimal, wfPedidoEquip.VL_COMPRIM_MIN);

                if (wfPedidoEquip.VL_COMPRIM_MAX > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_COMPRIM_MAX", DbType.Decimal, wfPedidoEquip.VL_COMPRIM_MAX);

                if (wfPedidoEquip.CD_TIPO_FITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_FITA", DbType.Int32, wfPedidoEquip.CD_TIPO_FITA);

                if (wfPedidoEquip.CD_MODELO_FITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_MODELO_FITA", DbType.Int32, wfPedidoEquip.CD_MODELO_FITA);

                if (wfPedidoEquip.CD_LARGURA_FITA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LARGURA_FITA", DbType.Int32, wfPedidoEquip.CD_LARGURA_FITA);

                if (wfPedidoEquip.CD_TIPO_PRODUTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_TIPO_PRODUTO", DbType.Int32, wfPedidoEquip.CD_TIPO_PRODUTO);

                if (wfPedidoEquip.CD_LOCAL_INSTAL > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_LOCAL_INSTAL", DbType.Int32, wfPedidoEquip.CD_LOCAL_INSTAL);

                if (wfPedidoEquip.CD_VELOCIDADE_LINHA > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_VELOCIDADE_LINHA", DbType.Int32, wfPedidoEquip.CD_VELOCIDADE_LINHA);

                if (wfPedidoEquip.CD_GUIA_POSICIONAMENTO > 0)
                    _db.AddInParameter(dbCommand, "@p_CD_GUIA_POSICIONAMENTO", DbType.Int32, wfPedidoEquip.CD_GUIA_POSICIONAMENTO);

                if (wfPedidoEquip.ID_ETIQUETA > 0)
                    _db.AddInParameter(dbCommand, "@p_ID_ETIQUETA", DbType.Int64, wfPedidoEquip.ID_ETIQUETA);

                if (wfPedidoEquip.VL_ALTURA_ETIQUETA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_ALTURA_ETIQUETA", DbType.Decimal, wfPedidoEquip.VL_ALTURA_ETIQUETA);

                if (wfPedidoEquip.VL_LARGURA_ETIQUETA > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_LARGURA_ETIQUETA", DbType.Decimal, wfPedidoEquip.VL_LARGURA_ETIQUETA);

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_DIREITO))
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_DIREITO", DbType.String, wfPedidoEquip.FL_APLIC_DIREITO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_ESQUERDO))
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_ESQUERDO", DbType.String, wfPedidoEquip.FL_APLIC_ESQUERDO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_APLIC_SUPERIOR))
                    _db.AddInParameter(dbCommand, "@p_FL_APLIC_SUPERIOR", DbType.String, wfPedidoEquip.FL_APLIC_SUPERIOR);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_KITPVA_GRAMACAIXA))
                    _db.AddInParameter(dbCommand, "@p_DS_KITPVA_GRAMACAIXA", DbType.String, wfPedidoEquip.DS_KITPVA_GRAMACAIXA);

                if (wfPedidoEquip.VL_STRETCH_PESOPALLET > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_STRETCH_PESOPALLET", DbType.Decimal, wfPedidoEquip.VL_STRETCH_PESOPALLET);

                if (wfPedidoEquip.VL_STRETCH_ALTURAPALLET > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_STRETCH_ALTURAPALLET", DbType.Decimal, wfPedidoEquip.VL_STRETCH_ALTURAPALLET);

                if (wfPedidoEquip.VL_INKJET_NUMCARACTER > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_INKJET_NUMCARACTER", DbType.Int32, wfPedidoEquip.VL_INKJET_NUMCARACTER);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_ARQ_ANEXO))
                    _db.AddInParameter(dbCommand, "@p_DS_ARQ_ANEXO", DbType.String, wfPedidoEquip.DS_ARQ_ANEXO);

                if (wfPedidoEquip.VL_PESO_MAXIMO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_MAXIMO", DbType.Decimal, wfPedidoEquip.VL_PESO_MAXIMO);

                if (wfPedidoEquip.VL_PESO_MINIMO > 0)
                    _db.AddInParameter(dbCommand, "@p_VL_PESO_MINIMO", DbType.Decimal, wfPedidoEquip.VL_PESO_MINIMO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.DS_TITULO))
                    _db.AddInParameter(dbCommand, "@p_DS_TITULO", DbType.String, wfPedidoEquip.DS_TITULO);

                if (!string.IsNullOrEmpty(wfPedidoEquip.TP_LOCACAO))
                    _db.AddInParameter(dbCommand, "@p_TP_LOCACAO", DbType.String, wfPedidoEquip.TP_LOCACAO);

                if (wfPedidoEquip.CD_MOTIVO_DEVOLUCAO > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_CD_MOTIVO_DEVOLUCAO", DbType.Int32, wfPedidoEquip.CD_MOTIVO_DEVOLUCAO);
                }
                if (!string.IsNullOrEmpty(wfPedidoEquip.FL_COPIA_NF3M))
                {
                    _db.AddInParameter(dbCommand, "@p_FL_COPIA_NF3M", DbType.String, wfPedidoEquip.FL_COPIA_NF3M);
                }
                if (wfPedidoEquip.VL_NOTA_FISCAL_3M > 0)
                {
                    _db.AddInParameter(dbCommand, "@p_VL_NOTA_FISCAL_3M", DbType.Decimal, wfPedidoEquip.VL_NOTA_FISCAL_3M);
                }


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


    }
}
