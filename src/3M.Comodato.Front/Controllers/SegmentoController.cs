using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class SegmentoController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            IEnumerable<Segmento> segmentos = null;
            try
            {

                segmentos = Pesquisar(null);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(segmentos);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            return View(new Segmento());
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(Segmento model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SegmentoEntity segmentoEntity = new SegmentoEntity();
                    segmentoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    segmentoEntity.DS_SEGMENTO_MIN = model.ds_segmentomin;
                    segmentoEntity.DS_SEGMENTO = model.ds_segmento;
                    segmentoEntity.DS_DESCRICAO = model.ds_descricao;
                    segmentoEntity.NM_CRITICIDADE = model.nm_criticidade;

                    SegmentoData data = new SegmentoData();
                    if (data.Inserir(ref segmentoEntity))
                    {
                        model.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(model);
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Segmento model = null;
            try
            {
                SegmentoEntity segmentoEntity = new SegmentoEntity();
                segmentoEntity.ID_SEGMENTO = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));
                model = Pesquisar(segmentoEntity).FirstOrDefault();

                ViewBag.PermitirAlteracao = !SegmentoBloqueado(model);

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(model);
        }

        private IEnumerable<Segmento> Pesquisar(SegmentoEntity filtro)
        {
            SegmentoData data = new SegmentoData();

            if (filtro == null)
            {
                filtro = new SegmentoEntity();
            }

            return (from s in data.ObterLista(filtro)
                    select new Segmento()
                    {
                        idKey = ControlesUtility.Criptografia.Criptografar(s.ID_SEGMENTO.ToString()),
                        id_segmento = s.ID_SEGMENTO,
                        ds_segmentomin = s.DS_SEGMENTO_MIN,
                        ds_segmento = s.DS_SEGMENTO,
                        nm_criticidade = s.NM_CRITICIDADE,
                        ds_descricao = string.IsNullOrEmpty(s.DS_DESCRICAO) ? "" : s.DS_DESCRICAO
                    });
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Segmento model)
        {
            try
            {
                Segmento segmento = Pesquisar(new SegmentoEntity() { ID_SEGMENTO = model.id_segmento }).FirstOrDefault();
                ViewBag.PermitirAlteracao = !SegmentoBloqueado(segmento);

                if (ModelState.IsValid)
                {
                    bool permitirAlteracao = ViewBag.PermitirAlteracao;

                    SegmentoEntity segmentoEntity = new SegmentoEntity();
                    segmentoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    segmentoEntity.ID_SEGMENTO = model.id_segmento;
                    segmentoEntity.DS_SEGMENTO_MIN = !permitirAlteracao ? segmento.ds_segmentomin : model.ds_segmentomin;
                    segmentoEntity.DS_SEGMENTO = model.ds_segmento;
                    segmentoEntity.DS_DESCRICAO = model.ds_descricao;
                    segmentoEntity.NM_CRITICIDADE = model.nm_criticidade;

                    SegmentoData data = new SegmentoData();
                    if (data.Alterar(segmentoEntity))
                    {
                        model.JavaScriptToRun = "MensagemSucesso()";
                    }
                    else
                    {
                        ViewBag.Mensagem = "Operação não concluída.";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(model);
        }


        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Segmento model = null;
            try
            {
                SegmentoEntity segmentoEntity = new SegmentoEntity();
                segmentoEntity.ID_SEGMENTO = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));

                model = Pesquisar(segmentoEntity).FirstOrDefault();

                if (SegmentoBloqueado(model))
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (model == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Segmento model = new Segmento();
            try
            {
                if (ModelState.IsValid)
                {
                    SegmentoEntity segmentoEntity = new SegmentoEntity();
                    segmentoEntity.ID_SEGMENTO = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    model = Pesquisar(segmentoEntity).FirstOrDefault();

                    if (SegmentoBloqueado(model))
                    {
                        return RedirectToAction("Index");
                    }

                    segmentoEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    new SegmentoData().Excluir(segmentoEntity);

                    model.JavaScriptToRun = "MensagemSucesso()";
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(model); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }


        private bool SegmentoBloqueado(Segmento model)
        {
            return ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CodigoSegmentoDistribuidor) == model.ds_segmentomin ||
                    ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CodigoSegmentoRealocarExcluir) == model.ds_segmentomin;
        }

        public ActionResult SegmentoUnicoJaExiste(string ds_segmentomin, Int64? id_segmento)
        {
            try
            {
                bool permitirSegmento = false;

                //Busca por DS_SEGMENTO_MIN
                Segmento model = Pesquisar(new SegmentoEntity() { DS_SEGMENTO_MIN = ds_segmentomin }).FirstOrDefault();
                
                if (!id_segmento.HasValue)
                {
                    permitirSegmento = !SegmentoBloqueado(model) || model == null;
                }
                else
                {
                    //Busca por ID_SEGMENTO
                    Segmento segmento = Pesquisar(new SegmentoEntity() { ID_SEGMENTO = id_segmento.Value }).FirstOrDefault();
                    bool segmentoFixo = SegmentoBloqueado(segmento);
                    if (segmentoFixo)
                    {
                        permitirSegmento = segmento.ds_segmentomin == model.ds_segmentomin;
                    }
                    else
                    {
                        permitirSegmento = !SegmentoBloqueado(model) && segmento != null;
                    }
                }
                return Json(permitirSegmento, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}