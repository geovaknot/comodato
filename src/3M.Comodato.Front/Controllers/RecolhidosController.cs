using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using _3M.Comodato.Entity;
using _3M.Comodato.Data;
using _3M.Comodato.Utility;

namespace _3M.Comodato.Front.Controllers
{
    public class RecolhidosController : BaseController
    {
        // GET: Recolhidos
        [_3MAuthentication]
        public ActionResult Index()
        {
            //Código Original
            //string data = DateTime.Now.ToString("dd/MM/yyyy");
            //string dataMenos5Anos = DateTime.Now.AddYears(-5).ToString("dd/MM/yyyy");

            //Método Criado para atender o chamado AMSSL00033409
            DateTime data = DateTime.Now;
            DateTime dataMenos5Anos = DateTime.Now.AddMonths(-1);

            Models.RecolhidosDetalhe recolhidosDetalhe = new Models.RecolhidosDetalhe
            {
                //Código Original
                //DT_DEV_FINAL = data,
                //DT_DEV_INICIAL = dataMenos5Anos,

                //Método Criado para atender o chamado AMSSL00033409
                DT_DEV_FINAL = data.ToString("dd/MM/yyyy"),
                DT_DEV_INICIAL = dataMenos5Anos.ToString("dd/MM/yyyy"),


                clientes = new List<Models.Cliente>(),
                //Código Original
                AllClientes = ObterListaClientePorUsuarioPerfil(Convert.ToInt64(((UsuarioPerfilEntity)Session["_CurrentUser"]).usuario.nidUsuario), true),

                //Original
                //ativos = new List<Models.Ativo>(),
                //AllAtivos = ObterListaAtivoFixo(false),
                //SL00033409
                modelos = new List<Models.Modelo>(),
                AllModelos = ObterListaModelo(false),
            };
            
            return View(recolhidosDetalhe);
        }


        protected List<Models.Cliente> ObterListaClientePorUsuarioPerfil(Int64 nidUsuario, bool? SomenteAtivos = false)
        {
            List<Models.Cliente> clientes = new List<Models.Cliente>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity, nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                Models.Cliente cliente = new Models.Cliente
                                {
                                    CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                    NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                                };
                                clientes.Add(cliente);
                            }
                        }
                        else
                        {
                            Models.Cliente cliente = new Models.Cliente
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return clientes;
        }



    }
}