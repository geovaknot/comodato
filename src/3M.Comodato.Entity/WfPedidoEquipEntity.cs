using System;

namespace _3M.Comodato.Entity
{
    public class WfPedidoEquipEntity
    {
        public String DS_ARQUIVO { get; set; }
        public long ID_WF_PEDIDO_EQUIP { get; set; }
        public string CD_WF_PEDIDO_EQUIP { get; set; }
        public DateTime DT_PEDIDO { get; set; }
        public string TP_PEDIDO { get; set; }

        public int ST_STATUS_PEDIDO { get; set; }
        public string DS_STATUS_PEDIDO { get; set; }

        public long ID_USU_SOLICITANTE { get; set; }
        public string NM_USU_SOLICITANTE { get; set; }

        public long CD_CLIENTE { get; set; }
        public string NM_CLIENTE { get; set; }

        public string CD_MODELO { get; set; }
        public string DS_MODELO { get; set; }

        public string CD_TROCA { get; set; }
        public string CD_ATIVO_FIXO_TROCA { get; set; }
        public DateTime DT_ULT_ATUALIZACAO { get; set; }
        public long ID_USU_ULT_ATU { get; set; }
        public long ID_USU_EMITENTE { get; set; }
        public string DS_CONTATO_NOME { get; set; }
        public string DS_CONTATO_EMAIL { get; set; }
        public string DS_CONTATO_TEL_NUM { get; set; }
        public long ID_CATEGORIA { get; set; }
        public string DS_CATEGORIA { get; set; }
        public string DS_LINHA_PRODUTO { get; set; }
        public int ID_TIPO_SOLICITACAO { get; set; }
        public string CD_LINHA { get; set; }
        public int CD_TENSAO { get; set; }
        public decimal VL_VOLUME_ANO { get; set; }
        public int CD_UNIDADE_MEDIDA { get; set; }
        public int QT_EQUIPAMENTO { get; set; }
        public int CD_COND_LIMPEZA { get; set; }
        public int CD_COND_TEMPERATURA { get; set; }
        public int CD_COND_UMIDADE { get; set; }
        public decimal VL_ALTURA_MIN { get; set; }
        public decimal VL_ALTURA_MAX { get; set; }
        public decimal VL_LARGURA_MIN { get; set; }
        public decimal VL_LARGURA_MAX { get; set; }
        public decimal VL_COMPRIM_MIN { get; set; }
        public decimal VL_COMPRIM_MAX { get; set; }
        public int CD_TIPO_FITA { get; set; }
        public int CD_MODELO_FITA { get; set; }
        public int CD_LARGURA_FITA { get; set; }
        public int CD_TIPO_PRODUTO { get; set; }
        public int CD_LOCAL_INSTAL { get; set; }
        public int CD_VELOCIDADE_LINHA { get; set; }
        public int CD_GUIA_POSICIONAMENTO { get; set; }
        public long ID_ETIQUETA { get; set; }
        public decimal VL_ALTURA_ETIQUETA { get; set; }
        public decimal VL_LARGURA_ETIQUETA { get; set; }
        public string FL_APLIC_DIREITO { get; set; }
        public string FL_APLIC_ESQUERDO { get; set; }
        public string FL_APLIC_SUPERIOR { get; set; }
        public string DS_KITPVA_GRAMACAIXA { get; set; }
        public decimal VL_STRETCH_PESOPALLET { get; set; }
        public decimal VL_STRETCH_ALTURAPALLET { get; set; }
        public int VL_INKJET_NUMCARACTER { get; set; }
        public string DS_ARQ_ANEXO { get; set; }
        public decimal? VL_PESO_MAXIMO { get; set; }
        public decimal? VL_PESO_MINIMO { get; set; }
        public string DS_TITULO { get; set; }
        public string TP_LOCACAO { get; set; }

        public long ID_USUARIO_RESPONS { get; set; }
        public string CD_GRUPO_RESPONS { get; set; }

        public int CD_MOTIVO_DEVOLUCAO { get; set; }
        public string FL_COPIA_NF3M { get; set; }
        public decimal VL_NOTA_FISCAL_3M { get; set; }

        public Int64 CD_EMPRESA { get; set; }
        public string NM_EMPRESA { get; set; }
        public DateTime? DT_RETIRADA_AGENDADA { get; set; }
        public DateTime? DT_RETIRADA_REALIZADA { get; set; }
        public DateTime? DT_PROGRAMADA_TMS { get; set; }
        public DateTime? DT_DEVOLUCAO_3M { get; set; }
        public DateTime? DT_DEVOLUCAO_PLANEJAMENTO { get; set; }

        public string DT_RETIRADA_AGENDADA_Formatada { get; set; }
        public string DT_RETIRADA_REALIZADA_Formatada { get; set; }
        public string DT_PROGRAMADA_TMS_Formatada { get; set; }
        public string DT_DEVOLUCAO_3M_Formatada { get; set; }
        public string DT_DEVOLUCAO_PLANEJAMENTO_Formatada { get; set; }

        public string DS_OBSERVACAO { get; set; }
    }

    public class WfPedidoEquipDevolucaoEntity : WfPedidoEquipEntity
    {
        public Decimal VL_COMPRIM_MAX { get; set; }
        public Decimal VL_PESO_MAXIMO { get; set; }
        public String Transportadora { get; set; }
        public DateTime? DT_RETIRADA_AGENDADA { get; set; }
        public DateTime? DT_RETIRADA_REALIZADA { get; set; }
        public DateTime? DT_PROGRAMADA_TMS { get; set; }
        public DateTime? DT_DEVOLUCAO_3M { get; set; }
        public DateTime? DT_DEVOLUCAO_PLANEJAMENTO { get; set; }
    }
}
