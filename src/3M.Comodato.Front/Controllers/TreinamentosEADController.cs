using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class TreinamentosEADController : BaseController
    {

        private bool PermissaoAcesso => ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                ControlesUtility.Enumeradores.Perfil.Tecnico3M.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                ControlesUtility.Enumeradores.Perfil.EquipeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                ControlesUtility.Enumeradores.Perfil.ControleEstoque.ToInt() == (int)CurrentUser.perfil.nidPerfil ||
                  ControlesUtility.Enumeradores.Perfil.Cliente.ToInt() == (int)CurrentUser.perfil.nidPerfil;



        // GET: TreinamentosEAD
        [_3MAuthentication]
        public ActionResult Index()
        {



            Models.TreinamentoEAD treinamentoEAD = new Models.TreinamentoEAD();

            treinamentoEAD.caminho = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.VideoEAD1);

            DefineValorInical(treinamentoEAD);
            VisualizarVideo(treinamentoEAD);

         //   ValidarAcesso(treinamentoEAD);

            return View(treinamentoEAD);
        }

       

        public void ValidarAcesso(Models.TreinamentoEAD treinamentoEAD)
        {


            VisualizarVideo(treinamentoEAD);

            treinamentoEAD.Visualizar = false;
            if (treinamentoEAD.AppMobile || treinamentoEAD.Clientes || treinamentoEAD.UtilizacaoRelatorios || treinamentoEAD.EquipeTecnica ||
                  treinamentoEAD.Equipetecnica3M)
            {
                treinamentoEAD.Visualizar = true;
            }
          //  return treinamentoEAD.Visualizar;

        }


        public void VisualizarVideo(Models.TreinamentoEAD treinamentoEAD)
        {
          //  Models.TreinamentoEAD treinamentoEAD = new Models.TreinamentoEAD();
        
            if (ControlesUtility.Enumeradores.Perfil.Administrador3M.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.Administracao = true;
                treinamentoEAD.Administracao2 = true;
                treinamentoEAD.Administracao3 = true;
                treinamentoEAD.AppMobile = true;
                treinamentoEAD.Clientes = true;
                treinamentoEAD.EquipeTecnica = true;
                treinamentoEAD.Equipetecnica3M = true;
                treinamentoEAD.UtilizacaoRelatorios = true;
                treinamentoEAD.Dashboard = true;
                treinamentoEAD.WorkflowEnvio = true;
                treinamentoEAD.WorkflowDevolucao = true;
                treinamentoEAD.ControleEstoque = true;
            }

            if (ControlesUtility.Enumeradores.Perfil.Tecnico3M.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.AppMobile = true;
                treinamentoEAD.Clientes = true;
                treinamentoEAD.EquipeTecnica = true;
                treinamentoEAD.Equipetecnica3M = true;
            }

                if (ControlesUtility.Enumeradores.Perfil.TecnicoEmpresaTerceira.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.AppMobile = true;
                treinamentoEAD.EquipeTecnica = true;
            }

            if (ControlesUtility.Enumeradores.Perfil.LiderEmpresaTecnica.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.AppMobile = true;
                treinamentoEAD.EquipeTecnica = true;
                treinamentoEAD.UtilizacaoRelatorios = true;


            }

            if (ControlesUtility.Enumeradores.Perfil.EquipeVendas.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.Dashboard = true;
                treinamentoEAD.WorkflowEnvio = true;
                treinamentoEAD.WorkflowDevolucao = true;

            }

            if (ControlesUtility.Enumeradores.Perfil.ControleEstoque.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.Dashboard = true;
                treinamentoEAD.ControleEstoque = true;

            }
            if (ControlesUtility.Enumeradores.Perfil.Cliente.ToInt() == (int)CurrentUser.perfil.nidPerfil)
            {
                treinamentoEAD.Clientes = true;
            }
        }


        private void DefineValorInical(Models.TreinamentoEAD treinamentoEAD )
        {
            treinamentoEAD.Administracao = false;
            treinamentoEAD.Administracao2 = false;
            treinamentoEAD.Administracao3 = false;
            treinamentoEAD.ControleEstoque = false;
            treinamentoEAD.Dashboard = false;
            treinamentoEAD.WorkflowEnvio = false;
            treinamentoEAD.WorkflowDevolucao = false;
            treinamentoEAD.AppMobile = false;
            treinamentoEAD.Clientes = false;
            treinamentoEAD.UtilizacaoRelatorios = false;
            treinamentoEAD.EquipeTecnica = false;
            treinamentoEAD.Equipetecnica3M = false;
        }
    }
}