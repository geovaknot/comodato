using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class EstoqueIntermediario : EstoquePeca
    {

        private ClienteEntity _ClienteEntity = null;
        private TecnicoEntity _TecnicoEntity = null;

        public List<Cliente> clientes { get; set; }
        public List<Tecnico> tecnicos { get; set; }

        public decimal QTD_RECEBIDA_NAO_APROVADA { get; set; }

        public ClienteEntity cliente
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

        public TecnicoEntity tecnico
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

        public decimal VL_TOTAL
        {
            get
            {
                decimal qtdPeca, vlPeca;
                //if (decimal.TryParse(QT_PECA_ATUAL, out qtdPeca) && decimal.TryParse(Peca.VL_PECA, out vlPeca))
                if (decimal.TryParse(QT_PECA_ATUAL.ToString(), out qtdPeca) && decimal.TryParse(Peca.VL_PECA, out vlPeca))
                    return qtdPeca * vlPeca;
                else
                    return 0;
            }

        }

        //public class EstoqueIntermediario : BaseModel
        //{
        //    private ClienteEntity _Cliente = null;
        //    private TecnicoEntity _Tecnico = null;


        //    public ClienteEntity Cliente
        //    {
        //        get
        //        {
        //            if (_Cliente == null) _Cliente = new ClienteEntity();
        //            return _Cliente;
        //        }
        //        set
        //        {
        //            if (_Cliente == null) _Cliente = new ClienteEntity();
        //            _Cliente = value;
        //        }
        //    }

        //    public TecnicoEntity Tecnico
        //    {
        //        get
        //        {
        //            if (_Tecnico == null) _Tecnico = new TecnicoEntity();
        //            return _Tecnico;
        //        }
        //        set
        //        {
        //            if (_Tecnico == null) _Tecnico = new TecnicoEntity();
        //            _Tecnico = value;
        //        }
        //    }
        //}

        //public class ListaEstoqueIntermediario : BaseModel
        //{
        //    public Int64 CD_PECA { get; set; }
        //    public string DS_PECA { get; set; }
        //    public Int64 QTD_ESTOQUE { get; set; }
        //}
        //public class ConsultaEstoqueIntermediarioModel: BaseModel
        //{
        //    public string DS_PECA { get; set; }
        //    public string TX_UNIDADE { get; set; }
        //    public decimal VL_PECA { get; set; }
        //    public DateTime DT_ULT_UTILIZACAO { get; set; }
        //    public int QTD_ESTOQUE { get; set; }
        //    public decimal VL_TOTAL => QTD_ESTOQUE * VL_PECA;
        //    public string DS_STATUS { get; set; }
        //    public int QTD_EQUIP { get; set; }
        //    public string DS_ESTOQUE { get; set; }
        //}
    }
}