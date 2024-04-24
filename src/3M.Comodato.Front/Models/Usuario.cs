using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using _3M.Comodato.Entity;

namespace _3M.Comodato.Front.Models
{
    public class Usuario : BaseModel
    {
        private PerfilEntity _Perfil = null;
        private UsuarioPerfilEntity _UsuarioPerfil = null;

        public Int64 nidUsuario { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cnmNome { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsLogin { get; set; }

        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string cdsEmail { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        public string cdsSenha { get; set; }

        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public decimal cd_empresa { get; set; }


        public List<Empresa> empresas { get; set; }

        public List<Perfil> perfis { get; set; }

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

        public UsuarioPerfilEntity usuarioPerfil
        {
            get
            {
                if (_UsuarioPerfil == null) _UsuarioPerfil = new UsuarioPerfilEntity();
                return _UsuarioPerfil;
            }
            set
            {
                if (_UsuarioPerfil == null) _UsuarioPerfil = new UsuarioPerfilEntity();
                _UsuarioPerfil = value;
            }
        }

        public Int64 nidPerfilExternoPadrao { get; set; }
    }

    public class UsuarioLogin
    {
        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsLogin { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsSenha { get; set; }
        public string token { get; set; }
    }

    public class UsuarioLoginRecuperar
    {
        [StringLength(100, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Formato inválido!")]
        public string cdsEmail { get; set; }
    }

    public class UsuarioLoginTrocarSenha
    {
        public string cnmNome { get; set; }

        public string cdsPerfil { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        public string cdsSenhaAtual { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"[a-zA-Z0-9]{12,50}", ErrorMessage = "Somente letras e/ou números entre 12 e 50 caracteres!")]
        public string cdsSenha { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Compare("cdsSenha", ErrorMessage = "A nova senha e a confirmação devem ser iguais.")]
        //[RegularExpression(@"[a-zA-Z0-9]{12,50}", ErrorMessage = "Somente letras e/ou números entre 12 e 50 caracteres!")]
        public string cdsConfirmarSenha { get; set; }
    }

    public class UsuarioLoginTrocarSenhaViaChave 
    {
        public Int64 nidUsuario { get; set; }

        public string cnmNome { get; set; }

        public string cdsPerfil { get; set; }

        public string ccdChaveAcessoTrocarSenha { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        //[RegularExpression(@"[a-zA-Z0-9]{12,50}", ErrorMessage = "Somente letras e/ou números entre 12 e 50 caracteres!")]
        public string cdsSenha { get; set; }

        [StringLength(50, ErrorMessage = "Limite de caracteres ultrapassado!")]
        [Required(ErrorMessage = "Conteúdo obrigatório!")]
        [Compare("cdsSenha", ErrorMessage = "A nova senha e a confirmação devem ser iguais.")]
        //[RegularExpression(@"[a-zA-Z0-9]{12,50}", ErrorMessage = "Somente letras e/ou números entre 12 e 50 caracteres!")]
        public string cdsConfirmarSenha { get; set; }
    }
}