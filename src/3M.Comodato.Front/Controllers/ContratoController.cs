using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Extensions.ExtContrato;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class ContratoController : BaseController
    {
        private Func<string, string> RemoverSeparadorFinal = new Func<string, string>((s) =>
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            else
            {
                return s.EndsWith(",") ? s.Remove(s.Length - 1, 1) : s;
            }
        });

        [_3MAuthentication]
        public ActionResult Index(int? id)
        {
            Contrato contrato = new Contrato();
            if (id.HasValue)
            {
                contrato.nidCliente = id.Value;
            }

            return View(contrato);
        }

        private IEnumerable<Contrato> Pesquisar(ContratoEntity filtro)
        {
            DataTable dtContrato = new ContratoData().ObterLista(filtro);
            var listaContratos = from c in dtContrato.Rows.Cast<DataRow>()
                                 select c.ToModel();
            return listaContratos;
        }

        [_3MAuthentication]
        public JsonResult ObterContratosPorCliente(int codigoCliente)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();
            IEnumerable<Contrato> listaContratos = null;
            try
            {
                ContratoEntity filtro = new ContratoEntity();
                filtro.Cliente.CD_CLIENTE = codigoCliente;

                listaContratos = Pesquisar(filtro);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/Contrato/_gridContratos.cshtml", listaContratos));
                jsonResult.Add("Status", "Success");
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

        [_3MAuthentication]
        public ActionResult Incluir(int? id)
        {
            Contrato contrato = new Contrato();
            if (id.HasValue)
            {
                contrato.nidCliente = id.Value;
            }

            contrato.ddtEmissao = DateTime.Now.ToString("dd/MM/yyyy");
            if (contrato.nidCliente != 0)
            {
                contrato.nnrContrato = contrato.nidCliente.ToString();
            }

            return View(contrato);
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (contrato.ddtEmissao == "")
                    {
                        contrato.ddtEmissao = DateTime.Now.ToString("dd/MM/yyyy");
                    }

                    if (contrato.nidCliente != 0)
                    {
                        contrato.nnrContrato = contrato.nidCliente.ToString();
                    }

                    ContratoEntity contratoInfo = contrato.ToEntity();
                    contratoInfo.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    contratoInfo.TX_CONTRATO_TIPO = this.RemoverSeparadorFinal(contratoInfo.TX_CONTRATO_TIPO);

                    ContratoData data = new ContratoData();
                    if (data.Inserir(ref contratoInfo))
                    {
                        contrato.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        return View(contrato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(contrato);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Contrato contratoInfo = null;
            try
            {
                ContratoEntity filtro = new ContratoEntity();
                filtro.ID_CONTRATO = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));
                contratoInfo = Pesquisar(filtro).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(contratoInfo);
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Contrato contrato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContratoData data = new ContratoData();
                    var contratoInfo = contrato.ToEntity();
                    contratoInfo.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    contratoInfo.TX_CONTRATO_TIPO = this.RemoverSeparadorFinal(contratoInfo.TX_CONTRATO_TIPO);

                    if (data.Alterar(contratoInfo))
                    {
                        contrato.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        return View(contrato);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(contrato);
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Contrato contratoInfo = null;
            try
            {
                ContratoEntity filtro = new ContratoEntity();
                filtro.ID_CONTRATO = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));
                contratoInfo = Pesquisar(filtro).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(contratoInfo);
        }

        [_3MAuthentication]
        [HttpPost, ActionName("Excluir")]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Contrato contratoModel = new Contrato();
            try
            {
                if (ModelState.IsValid)
                {
                    ContratoEntity contratoEntity = new ContratoEntity();
                    contratoEntity.ID_CONTRATO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));

                    ContratoData contratoData = new ContratoData();
                    contratoModel = (from c in contratoData.ObterLista(contratoEntity).Rows.Cast<DataRow>()
                                     select c.ToModel()).FirstOrDefault();
                    contratoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    contratoData.Excluir(contratoEntity);

                    contratoModel.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(contratoModel);
        }

        [_3MAuthentication]
        public ActionResult Imprimir(string idKey)
        {
            string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
            return Redirect(URLSite + $"/RelatorioComodato.aspx?IdKey={idKey}");
        }
    }
}

namespace _3M.Comodato.Front.Extensions.ExtContrato
{
    internal static class ContratoExtensions
    {
        public static Contrato ToModel(this DataRow r)
        {
            Contrato model = new Contrato();

            model.idKey = ControlesUtility.Criptografia.Criptografar(r["ID_CONTRATO"].ToString());
            model.nnrContrato = r["NR_CONTRATO"].ToString();
            model.ddtEmissao = r["DT_EMISSAO"].DataString();
            model.ddtRecebimento = r["DT_RECEBIMENTO"].DataString();
            model.ddtOk = r["DT_OK"].DataString();
            model.nidCliente = Convert.ToInt32(r["CD_CLIENTE"]);
            model.cdsStatus = r["CD_STATUS_CONTRATO"].ToString();
            model.dsStatus = r["DS_STATUS_CONTRATO"].ToString();
            model.cdsClausulas = r["DS_CLAUSULAS"].ToString();
            model.dsCliente = r["NM_CLIENTE"].ToString();

            if (r["TX_CONTRATO_TIPO"] != DBNull.Value)
            {
                model.cdsContratoTipo = r["TX_CONTRATO_TIPO"].ToString();
            }

            if (r["TX_OBS"] != DBNull.Value)
            {
                model.cdsObservacao = r["TX_OBS"].ToString();
            }

            return model;
        }

        public static ContratoEntity ToEntity(this Contrato contrato)
        {
            ContratoEntity contratoInfo = new ContratoEntity();
            if (!string.IsNullOrEmpty(contrato.idKey))
            {
                contratoInfo.ID_CONTRATO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(contrato.idKey));
            }

            contratoInfo.NR_CONTRATO = Convert.ToDecimal(contrato.nnrContrato);
            contratoInfo.DT_EMISSAO = Convert.ToDateTime(contrato.ddtEmissao);

            if (!string.IsNullOrEmpty(contrato.ddtRecebimento))
            {
                contratoInfo.DT_RECEBIMENTO = Convert.ToDateTime(contrato.ddtRecebimento);
            }

            if (!string.IsNullOrEmpty(contrato.ddtOk))
            {
                contratoInfo.DT_OK = Convert.ToDateTime(contrato.ddtOk);
            }

            contratoInfo.Cliente.CD_CLIENTE = Convert.ToInt32(contrato.nidCliente);
            contratoInfo.Status = new StatusContratoEntity() { CD_STATUS_CONTRATO = contrato.cdsStatus };

            contratoInfo.TX_CONTRATO_TIPO = contrato.cdsContratoTipo;
            contratoInfo.DS_CLAUSULAS = contrato.cdsClausulas;
            contratoInfo.TX_OBS = contrato.cdsObservacao;

            return contratoInfo;
        }
    }
}