using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
namespace _3M.Comodato.Front.Controllers
{
    public class EstoqueController : BaseController
    {
        [_3MAuthentication]
        public ActionResult Index()
        {
            Func<DataRow, Estoque> modelConverter = new Func<DataRow, Estoque>(dr =>
            {
                var estoque = new Estoque();

                estoque.idKey = ControlesUtility.Criptografia.Criptografar(dr["ID_ESTOQUE"].ToString());
                estoque.ccdTecnico = dr["CD_TECNICO"].ToString();
                estoque.Tecnico.NM_TECNICO = dr["NM_TEC_RESPONSAVEL"].ToString();
                estoque.ccdEstoque = dr["CD_ESTOQUE"].ToString().Trim();
                estoque.cdsEstoque = dr["DS_ESTOQUE"].ToString().Trim();

                estoque.cdsTipoEstoque = ControlesUtility.Dicionarios.TipoEstoque()[dr["TP_ESTOQUE_TEC_3M"].ToString().Trim()];
                if (estoque.cdsTipoEstoque.Contains("3M"))
                {
                    estoque.cdsTipoEstoque = estoque.cdsTipoEstoque.Substring(0, 10);
                }

                estoque.cdsAtivo = dr["FL_ATIVO"].ToString();

                return estoque;
            });

            EstoqueData data = new EstoqueData();
            DataTable dtEstoque = data.ObterLista(new EstoqueEntity());
            List<Estoque> listaEstoque = (from dr in dtEstoque.Rows.Cast<DataRow>()
                                          select modelConverter(dr)).ToList();
            return View(listaEstoque);
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            PopularControles();

            var model = new Estoque();
            model.nidUsuarioResponsavel = string.Empty;

            return View(model);
        }

        [_3MAuthentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Incluir(Estoque model)
        {
            try
            {
                PopularControles();

                if (ModelState.IsValid)
                {

                    DataTable dtTecnicos = ObterListaTecnicosAtivos(Convert.ToInt64(model.nidUsuarioResponsavel));
                    if (dtTecnicos.Rows.Count > 0)
                    {
                        if (dtTecnicos.Rows[0]["CD_TECNICO"] != DBNull.Value)
                        {
                            model.ccdTecnico = dtTecnicos.Rows[0]["CD_TECNICO"].ToString();
                        }
                    }

                    EstoqueEntity estoqueInfo = new EstoqueEntity();
                    estoqueInfo.tecnico.CD_TECNICO = model.ccdTecnico;
                    estoqueInfo.ID_USU_RESPONSAVEL = Convert.ToInt64(model.nidUsuarioResponsavel);//Convert.ToInt64(codigoUsuario);
                    estoqueInfo.CD_ESTOQUE = model.ccdEstoque;
                    estoqueInfo.DS_ESTOQUE = model.cdsEstoque;
                    estoqueInfo.TP_ESTOQUE_TEC_3M = model.cdsTipoEstoque;
                    estoqueInfo.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    estoqueInfo.Cliente.CD_CLIENTE = model.CD_CLIENTE.HasValue ? model.CD_CLIENTE.Value : 0;
                    estoqueInfo.FL_ATIVO = model.cdsAtivo;

                    if (JaExisteEstoqueParaResp(estoqueInfo.ID_USU_RESPONSAVEL, estoqueInfo.TP_ESTOQUE_TEC_3M))
                    {
                        model.JavaScriptToRun = "MensagemErroValidacao();";
                        return View(model);
                    }

                    EstoqueData data = new EstoqueData();
                    if (data.Inserir(ref estoqueInfo))
                    {
                        model.JavaScriptToRun = "MensagemSucesso();";
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
            Estoque model = null;
            try
            {
                EstoqueEntity filtro = new EstoqueEntity();
                filtro.ID_ESTOQUE = long.Parse(ControlesUtility.Criptografia.Descriptografar(idKey));

                EstoqueData data = new EstoqueData();
                DataTable dtEstoque = data.ObterLista(filtro);

                Func<DataRow, Estoque> rowToModel = new Func<DataRow, Estoque>((dr) =>
                {
                    Estoque estoqueModel = new Estoque();
                    estoqueModel.idKey = ControlesUtility.Criptografia.Criptografar(dr["ID_ESTOQUE"].ToString());
                    estoqueModel.ccdTecnico = dr["CD_TECNICO"].ToString();
                    estoqueModel.ccdEstoque = dr["CD_ESTOQUE"].ToString().Trim();
                    estoqueModel.nidUsuarioResponsavel = dr["ID_USU_RESPONSAVEL"].ToString();
                    estoqueModel.cdsEstoque = dr["DS_ESTOQUE"].ToString().Trim();
                    estoqueModel.cdsTipoEstoque = dr["TP_ESTOQUE_TEC_3M"].ToString().Trim();
                    estoqueModel.cdsAtivo = dr["FL_ATIVO"].ToString();
                    if (dr["CD_CLIENTE"] != DBNull.Value)
                    {
                        estoqueModel.CD_CLIENTE = Convert.ToInt64(dr["CD_CLIENTE"]);
                    }
                    return estoqueModel;
                });

                model = (from dr in dtEstoque.Rows.Cast<DataRow>() select rowToModel(dr)).FirstOrDefault();

                PopularListas(model);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        [_3MAuthentication]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(Estoque model)
        {
            try
            {
                PopularListas(model);

                if (ModelState.IsValid)
                {
                    DataTable dtTecnicos = ObterListaTecnicosAtivos(Convert.ToInt64(model.nidUsuarioResponsavel));
                    if (dtTecnicos.Rows.Count > 0)
                    {
                        if (dtTecnicos.Rows[0]["CD_TECNICO"] != DBNull.Value)
                        {
                            model.ccdTecnico = dtTecnicos.Rows[0]["CD_TECNICO"].ToString();
                        }
                    }

                    EstoqueEntity estoqueInfo = new EstoqueEntity();
                    estoqueInfo.ID_ESTOQUE = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(model.idKey));
                    estoqueInfo.tecnico.CD_TECNICO = model.ccdTecnico;
                    estoqueInfo.ID_USU_RESPONSAVEL = Convert.ToInt64(model.nidUsuarioResponsavel);
                    estoqueInfo.CD_ESTOQUE = model.ccdEstoque;
                    estoqueInfo.DS_ESTOQUE = model.cdsEstoque;
                    estoqueInfo.TP_ESTOQUE_TEC_3M = model.cdsTipoEstoque;
                    estoqueInfo.Cliente.CD_CLIENTE = model.CD_CLIENTE.HasValue ? model.CD_CLIENTE.Value : 0;
                    estoqueInfo.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;
                    estoqueInfo.FL_ATIVO = model.cdsAtivo;

                    if (IdUsuResponsavelAtual(estoqueInfo.ID_ESTOQUE) != estoqueInfo.ID_USU_RESPONSAVEL)
                    {
                        if (JaExisteEstoqueParaResp(estoqueInfo.ID_USU_RESPONSAVEL, estoqueInfo.TP_ESTOQUE_TEC_3M))
                        {
                            model.JavaScriptToRun = "MensagemErroValidacao();";
                            return View(model);
                        }
                    }


                    EstoqueData data = new EstoqueData();
                    if (data.Alterar(estoqueInfo))
                    {
                        model.JavaScriptToRun = "MensagemSucesso();";
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

        private void PopularControles()
        {
            PopularListas(null);
        }

        private void PopularListas(Estoque model)
        {
            Action<List<SelectListItem>> adicionarPlaceHolder = new Action<List<SelectListItem>>
                (l => l.Insert(0, new SelectListItem() { Text = "Selecione...", Value = "", Selected = true }));

            List<SelectListItem> listaStatus = (from t in ControlesUtility.Dicionarios.SimNao()
                                                select new SelectListItem() { Text = t.Key, Value = t.Value }).ToList();

            List<SelectListItem> listaTipoEstoque = (from t in ControlesUtility.Dicionarios.TipoEstoque()
                                                     select new SelectListItem() { Text = t.Value, Value = t.Key }).ToList();

            List<SelectListItem> listaClientes = new List<SelectListItem>();
            if (null != model)
            {
                if (model.cdsTipoEstoque == "CLI")
                {
                    ClienteEntity filtroCliente = new ClienteEntity();
                    if (model.CD_CLIENTE.HasValue)
                    {
                        filtroCliente.CD_CLIENTE = model.CD_CLIENTE.Value;
                    }

                    listaClientes = (from t in base.ObterListaCliente(filtroCliente)
                                     select new SelectListItem() { Text = t.NM_CLIENTE, Value = t.CD_CLIENTE.ToString() }).ToList();
                }
                else
                {
                    adicionarPlaceHolder(listaClientes);
                }
            }

            List<SelectListItem> listaResponsavel = new List<SelectListItem>();
            if (model != null)
            {
                string perfis = string.Empty;
                switch (model.cdsTipoEstoque)
                {
                    case "3M1":
                    case "3M2":
                        perfis = "1,5";
                        break;
                    case "TEC":
                        perfis = "2,3,6";
                        break;
                    case "CLI":
                        perfis = "8";
                        break;
                }

                listaResponsavel = (from t in ObterListaPorPerfil(perfis)
                                    select new SelectListItem() { Text = t.cnmNome + " (" + t.cdsLogin + ")", Value = t.nidUsuario.ToString() }).ToList();
            }

            adicionarPlaceHolder(listaStatus);
            adicionarPlaceHolder(listaTipoEstoque);

            adicionarPlaceHolder(listaResponsavel);

            if (model != null)
            {
                var status = listaStatus.Where(c => c.Value == model.cdsAtivo).FirstOrDefault();
                if (null != status)
                {
                    listaStatus.ForEach(c => c.Selected = (c == status));
                }

                var tipoEstoque = listaTipoEstoque.Where(c => c.Value == model.cdsTipoEstoque.Trim()).FirstOrDefault();
                if (null != tipoEstoque)
                {
                    listaTipoEstoque.ForEach(c => c.Selected = (c == tipoEstoque));
                }
                var responsavel = listaResponsavel.Where(c => c.Value == model.nidUsuarioResponsavel).FirstOrDefault();
                if (null != tipoEstoque)
                {
                    listaResponsavel.ForEach(c => c.Selected = (c == responsavel));
                }

                if (model.cdsTipoEstoque == "CLI")
                {
                    var cliente = listaClientes.Where(c => c.Value == model.CD_CLIENTE.ToString()).FirstOrDefault();
                    if (null != cliente)
                    {
                        listaClientes.ForEach(c => c.Selected = (c == cliente));
                    }
                }
            }

            ViewBag.ListaStatus = listaStatus;
            ViewBag.ListaTipoEstoque = listaTipoEstoque;
            ViewBag.ListaResponsavel = listaResponsavel;
            ViewBag.ListaClientes = listaClientes;
        }

        private List<UsuarioEntity> ObterListaPorPerfil(string perfis)
        {
            List<UsuarioEntity> listaUsuarios = new List<UsuarioEntity>();

            try
            {
                List<string> listaPerfis = perfis.Split(',').ToList();
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                DataTable dataTable = new UsuarioPerfilData().ObterListaALLUsuario(usuarioPerfilEntity, true, false);
                listaUsuarios = (from t in dataTable.Rows.Cast<DataRow>()
                                 where t.FieldOrDefault<decimal>("nidPerfil") != default(decimal) && listaPerfis.Contains(t.FieldOrDefault<decimal>("nidPerfil").ToString())
                                 select new UsuarioEntity()
                                 {
                                     nidUsuario = Convert.ToInt64(t["nidUsuario"]),
                                     cnmNome = t["cnmNome"].ToString(),
                                     cdsLogin = t["cdsLogin"].ToString()
                                 }).ToList();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
            }

            return listaUsuarios;
        }
        private DataTable ObterListaTecnicosAtivos(long nidUsuario)
        {
            TecnicoEntity filtro = new TecnicoEntity() { FL_ATIVO = "S" };
            filtro.usuario.nidUsuario = nidUsuario;

            TecnicoData data = new TecnicoData();
            DataTable dtTecnicos = data.ObterLista(filtro);
            return dtTecnicos;
        }

        private bool JaExisteEstoqueParaResp(long idUsuResponsavel, string tpEstoqueTec3M)
        {
            EstoqueEntity filtro = new EstoqueEntity() { ID_USU_RESPONSAVEL = idUsuResponsavel, TP_ESTOQUE_TEC_3M = tpEstoqueTec3M };

            EstoqueData data = new EstoqueData();
            DataTable dtEstoque = data.ObterListaEstoqueResponsavel(filtro);

            if (dtEstoque.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private long IdUsuResponsavelAtual(long idEstoque)
        {
            long idUsuResponsavelAtual = 0;
            EstoqueEntity filtro = new EstoqueEntity() { ID_ESTOQUE = idEstoque };

            EstoqueData data = new EstoqueData();
            DataTable dtEstoque = data.ObterLista(filtro);

            if (dtEstoque.Rows.Count > 0)
            {
                if (dtEstoque.Rows[0]["ID_USU_RESPONSAVEL"] != DBNull.Value)
                {
                    idUsuResponsavelAtual =  Convert.ToInt64(dtEstoque.Rows[0]["ID_USU_RESPONSAVEL"]);
                }
            }

            return idUsuResponsavelAtual;
        }
    }
}