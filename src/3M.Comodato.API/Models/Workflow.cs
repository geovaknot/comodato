using _3M.Comodato.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3M.Comodato.API.Models
{
    public class Workflow : BaseModel
    {
        public WfPedidoEquipEntity wfPedidoEquipEntity { get; set; } = new WfPedidoEquipEntity();
        public int DIAS { get; set; }
        public String GRUPOS { get; set; }
        public String EMAILS { get; set; }
    }
}