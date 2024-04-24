using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;
using System.Data;
using System.Net;
using System.Web.Http.Cors;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/EstoqueIntermediarioAPI")]
    [Authorize]
    public class EstoqueIntermediarioAPIController : BaseAPIController
    {
        //GET: EstoqueIntermediario
        /*public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("ObterListaEmpresa")]
        public HttpResponseMessage ObterListaEmpresa()
        {
            List<EmpresaEntity> empresas = new List<EmpresaEntity>();

            try
            {
                EmpresaEntity empresaEntity = new EmpresaEntity();
                DataTableReader dataTableReader = new EmpresaData().ObterLista(empresaEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        empresaEntity = new EmpresaEntity();
                        empresaEntity.CD_Empresa = Convert.ToInt64(dataTableReader["CD_Empresa"]);
                        empresaEntity.NM_Empresa = dataTableReader["NM_Empresa"].ToString();
                        empresaEntity.IIComp = dataTableReader["IIComp"].ToString();
                        empresas.Add(empresaEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { empresas = empresas });
        }*/

        [HttpGet]
        [Route("ObterListaCliente")]
        public HttpResponseMessage ObterListaCliente()
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        clienteEntity = new ClienteEntity
                        {
                            CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                            NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                        };
                        clientes.Add(clienteEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes });
        }

        [HttpGet]
        [Route("ObterListaTecnico")]
        public HttpResponseMessage ObterListaTecnico(int CD_CLIENTE)
        {
            List<TecnicoClienteEntity> tecnicos = new List<TecnicoClienteEntity>();

            try
            {
                TecnicoClienteEntity tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new TecnicoClienteData().ObterLista(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        tecnicoClienteEntity = new TecnicoClienteEntity();
                        tecnicoClienteEntity.tecnico.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                        tecnicoClienteEntity.tecnico.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        tecnicos.Add(tecnicoClienteEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { tecnicos });
        }

        [HttpGet]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(string ativo, string ID_Usuario)
        {
            List<EstoqueEntity> estoques = new List<EstoqueEntity>();
            
            try
            {
                EstoqueEntity estoque = new EstoqueEntity();
                estoque.FL_ATIVO = ativo;
                if (ID_Usuario != null && ID_Usuario != "")
                {
                    long id = Convert.ToInt64(ID_Usuario);
                    if (id > 0)
                    {
                        estoque.ID_USU_RESPONSAVEL = id;
                    }
                    
                }
                DataTableReader dataTableReader = new EstoqueData().ObterLista(estoque).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        estoque = new EstoqueEntity();
                        estoque.CD_ESTOQUE = dataTableReader["CD_ESTOQUE"].ToString();
                        estoque.DS_ESTOQUE = dataTableReader["DS_ESTOQUE"].ToString();
                        estoques.Add(estoque);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { estoques });
        }
    }
}
