using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;

namespace _3M.Comodato.Front.Models
{
    public class ReclamacaoOs : BaseModel
    {
        private RRStatusEntity _RRStatusEntity;
        private ClienteEntity _ClienteEntity;
        private OSPadraoEntity _OSPadraoEntity;

        private TecnicoEntity _TecnicoEntity;
        private AtivoFixoEntity _AtivoFixoEntity;
        private PecaEntity _PecaEntity;


        public long ID_RR_STATUS { get; set; }


        public long ST_STATUS_RR { get; set; }
        public long ID_RELATORIO_RECLAMACAO { get; set; }
        public long ID_USUARIO_RESPONS { get; set; }
        public string CD_GRUPO_RESPONS { get; set; }


        public String CD_TECNICO { get; set; }
        public String CD_CLIENTE { get; set; }
        public String CD_ATIVO_FIXO { get; set; }
        public String CD_PECA { get; set; }
        public long CD_TIPO_ATENDIMENTO { get; set; }
        public string DS_TIPO_ATENDIMENTO { get; set; }

        public Dictionary<int, string> tiposAtendimento { get; set; }

        public long CD_TIPO_RECLAMACAO { get; set; }
        public string DS_TIPO_RECLAMACAO { get; set; }

        public Dictionary<int, string> tiposReclamacao { get; set; }

        public string DS_MOTIVO { get; set; }

        public string DS_DESCRICAO { get; set; }

        public Int32 TEMPO_ATENDIMENTO { get; set; }

        public string TEMPO_ATENDIMENTO_FORMATADO { get; set; }

        public string DS_ARQUIVO_FOTO { get; set; }

        public string DS_TIPO_FOTO { get; set; }

        public Int32 TipoReclamacaoRR { get; set; }
        public Int32 TipoAtendimento { get; set; }


        public string TecnicoSolicitante { get; set; }
        public string NM_Fornecedor { get; set; }
        public Decimal Custo_Peca { get; set; }
        public Decimal Custo_Total { get; set; }
        public Decimal ValorMaoDeObra { get; set; }
        public string VL_Hora_Atendimento { get; set; }
        public string VL_Minuto_Atendimento { get; set; }

        public RRStatusEntity rrStatusEntity
        {
            get
            {

                if (_RRStatusEntity == null) _RRStatusEntity = new RRStatusEntity();
                return _RRStatusEntity;
            }
            set
            {
                if (_RRStatusEntity == null) _RRStatusEntity = new RRStatusEntity();
                _RRStatusEntity = value;

            }
        }
        public TecnicoEntity tecnicoEntity
        {
            get
            {

                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                return _TecnicoEntity;
            }
            set
            {
                if (_TecnicoEntity == null) _TecnicoEntity = new TecnicoEntity();
                _TecnicoEntity = value;

            }
        }
        public ClienteEntity clienteEntity
        {
            get
            {

                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                return _ClienteEntity;
            }
            set
            {
                if (_ClienteEntity == null) _ClienteEntity = new ClienteEntity();
                _ClienteEntity = value;

            }
        }
        public AtivoFixoEntity ativoFixoEntity
        {
            get
            {

                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                return _AtivoFixoEntity;
            }
            set
            {
                if (_AtivoFixoEntity == null) _AtivoFixoEntity = new AtivoFixoEntity();
                _AtivoFixoEntity = value;

            }
        }
        public PecaEntity pecaEntity
        {

            get
            {
                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                return _PecaEntity;
            }
            set
            {

                if (_PecaEntity == null) _PecaEntity = new PecaEntity();
                _PecaEntity = value;
            }
        }
        public OSPadraoEntity osPadraoEntity
        {

            get
            {
                if (_OSPadraoEntity == null) _OSPadraoEntity = new OSPadraoEntity();
                return _OSPadraoEntity;
            }
            set
            {

                if (_OSPadraoEntity == null) _OSPadraoEntity = new OSPadraoEntity();
                _OSPadraoEntity = value;
            }
        }
        public DateTime DataCadastro { get; set; }

        public List<PecaEntity> listaPecas { get; set; }
    }
}