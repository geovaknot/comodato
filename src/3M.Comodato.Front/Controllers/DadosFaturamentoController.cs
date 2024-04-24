using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Data;
using System.Net;

namespace _3M.Comodato.Front.Controllers
{
    public class DadosFaturamentoController : BaseController
    {

        public AtivoClienteService _ativoClienteService;

        public DadosFaturamentoController()
        {
            _ativoClienteService = new AtivoClienteService();
        }

        // GET: DadosFaturamento
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(Models.DadosFaturamento dadosFaturamento)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                DadosFaturamentoEntity faturamentoEntity = new DadosFaturamentoEntity();

                //_ativoClienteService.InformarStatus(osPadrao);

                dadosFaturamento.AluguelApos3anos = double.Parse(dadosFaturamento.VlaluguelApos3anos);

                _ativoClienteService.MapearCamposDadosFaturamentoParaDadosFaturamentoEntity(dadosFaturamento, faturamentoEntity);

                new FaturamentoData().Inserir(ref faturamentoEntity);

                dadosFaturamento.JavaScriptToRun = "MensagemSucesso()";

                jsonResult.Add("Status", "Success");

                //osPadrao.idKey = ControlesUtility.Criptografia.Criptografar(Convert.ToString(osPadraoEntity.ID_OS));

                //dadosFaturamento.JavaScriptToRun = dadosFaturamento.TpStatusOS.ST_STATUS_OS == Convert.ToInt64(ControlesUtility.Enumeradores.TpStatusOSPadrao.Aberta) ? "MensagemSucessoOsAberta()" : "MensagemSucessoAguardandoInicio()";
                
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

        public JsonResult ObterListaFaturamentoJson(Int64 ID_ATIVO_CLIENTE)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                List<Models.DadosFaturamento> listaFaturamento = _ativoClienteService.ObterListaDadosFaturamento(ID_ATIVO_CLIENTE);

                string IDs = "";
                if(listaFaturamento?.Count > 0)
                {
                    foreach(var fat in listaFaturamento)
                    {
                        IDs += fat.ID + ";";
                    }
                }
                if (listaFaturamento?.Count > 0)
                {
                    foreach (var fat in listaFaturamento)
                    {
                        fat.FaturamentosID = IDs;
                    }
                }
                ViewBag.FaturamentosID = IDs;
                if (listaFaturamento.Count > 0)
                {
                    var listaFaturamentoAtivo = listaFaturamento.Where(x => x.Ativo == true).ToList();
                    jsonResult.Add("Html", RenderRazorViewToString("~/Views/AtivoCliente/_gridMVCDadosFaturamento.cshtml", listaFaturamento));
                    jsonResult.Add("Status", "Success");
                    jsonResult.Add("QTD", listaFaturamentoAtivo?.Count);
                }

                else
                    jsonResult.Add("Status", "Error");


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