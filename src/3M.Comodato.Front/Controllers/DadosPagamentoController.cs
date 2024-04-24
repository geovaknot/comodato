using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class DadosPagamentoController : BaseController
    {
        public AtivoClienteService _ativoClienteService;

        public DadosPagamentoController()
        {
            _ativoClienteService = new AtivoClienteService();
        }

        // GET: DadosFaturamento
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Incluir(DadosPagamento dadosPagamento)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            try
            {
                if (ModelState.IsValid)
                {
                    DadosPagamentoEntity pagamentoEntity = new DadosPagamentoEntity();

                    //_ativoClienteService.InformarStatus(osPadrao);

                    _ativoClienteService.MapearCamposDadosPagamentoParaDadosPagamentoEntity(dadosPagamento, pagamentoEntity);

                    new PagamentoData().Inserir(ref pagamentoEntity);

                    jsonResult.Add("Status", "Success");
                    //osPadrao.idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToString(osPadraoEntity.ID_OS));

                    //dadosFaturamento.JavaScriptToRun = dadosFaturamento.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta) ? "MensagemSucessoOsAberta()" : "MensagemSucessoAguardandoInicio()";
                }

                else
                {
                    jsonResult.Add("Status", "Error");
                }
            }
            catch (Exception ex)
            {
                jsonResult.Add("Status", "Error");
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //PopularListas(osPadrao);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;

            return jsonList;
        }

        public JsonResult ObterListaPagamentoJson(string ID_FAT)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<DadosPagamento> listaPagamento = _ativoClienteService.ObterListaDadosPagamento(ID_FAT);

                if (listaPagamento?.Count > 0)
                {
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/AtivoCliente/_gridMVCDadosPagamento.cshtml", listaPagamento));
                    jsonResult.Add("Status", "Success");
                }

                else
                {                  
                    jsonResult.Add("Status", "Error");
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }
    }
}