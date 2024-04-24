using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class PerfilXFuncao : BaseModel
    {
        private PerfilEntity _Perfil = null;
        private FuncaoEntity _Funcao = null;

        public Int64 nidPerfilFuncao { get; set; }

        //public Perfil perfil { get; set; }

        public List<Perfil> perfis { get; set; }

        //public Funcao funcao { get; set; }

        public List<Funcao> funcoes { get; set; }

        public PerfilEntity perfil
        {
            get
            {
                if (_Perfil == null) _Perfil = new PerfilEntity();
                return _Perfil;
            }
            set
            {
                if (_Perfil == null) _Perfil = new PerfilEntity();
                _Perfil = value;
            }
        }

        public FuncaoEntity funcao
        {
            get
            {
                if (_Funcao == null) _Funcao = new FuncaoEntity();
                return _Funcao;
            }
            set
            {
                if (_Funcao == null) _Funcao = new FuncaoEntity();
                _Funcao = value;
            }
        }


        //public List<ListaPerfilXFuncao> listaPerfilXFuncao { get; set; }
    }

    public class ListaPerfilXFuncao : BaseModel
    {
        public Int64 nidPerfilFuncao { get; set; }
        public Int64 nidPerfil { get; set; }
        public Boolean bidAtivoPerfilFuncao { get; set; }
        public Int64 nidFuncao { get; set; }
        public string ccdFuncao { get; set; }
        public string cdsFuncao { get; set; }
    }
}