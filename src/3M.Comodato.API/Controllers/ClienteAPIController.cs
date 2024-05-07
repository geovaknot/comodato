using _3M.Comodato.Data;
//using System.Web.Mvc;
using _3M.Comodato.Entity;
//using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/ClienteAPI")]
    [Authorize]
    public class ClienteAPIController : BaseAPIController
    {
        /// <summary>
        /// Consulta de Clientes
        /// </summary>
        /// <param name="clienteEntity"></param>
        /// <returns>List<ClienteEntity> clientes</returns>
        [HttpPost]
        [Route("ObterLista")]
        public HttpResponseMessage ObterLista(ClienteEntity clienteEntity)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                if (clienteEntity == null)
                {
                    clienteEntity = new ClienteEntity();
                }

                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        clienteEntity = new ClienteEntity();
                        CarregarClienteEntity(dataTableReader, clienteEntity);
                        clientes.Add(clienteEntity);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }

        [HttpPost]
        [AcceptVerbs("POST")]
        [Route("Adicionar")]
        public IHttpActionResult Adicionar(ClienteEntity clienteEntity)
        {
            try
            {
                ClienteData data = new ClienteData();
                if (data.Inserir(ref clienteEntity))
                    return Ok(clienteEntity);
                else
                    return InternalServerError();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Consulta de Clientes ATIVOS (DT_DESATIVACAO = NULL) independente de perfil e usuário
        /// </summary>
        /// <param name="clienteEntity"></param>
        /// <returns>List<ClienteEntity> clientes</returns>
        [HttpPost]
        [Route("ObterListaAtivos")]
        public HttpResponseMessage ObterListaAtivos(ClienteEntity clienteEntity)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                if (clienteEntity == null)
                {
                    clienteEntity = new ClienteEntity();
                }

                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                        {
                            clienteEntity = new ClienteEntity();

                            CarregarClienteEntity(dataTableReader, clienteEntity);

                            clientes.Add(clienteEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }

        /// <summary>
        /// Consulta de Clientes vinculados ao usuário
        /// </summary>
        /// <param name="nidUsuario"></param>
        /// <param name="SomenteAtivos"></param>
        /// <returns>List<ClienteEntity> clientes</returns>
        /// 

        

        [HttpGet]
        [Route("ObterListaComboPorUsuarioPerfilLocados")]
        public HttpResponseMessage ObterListaComboPorUsuarioPerfilLocados(Int64 nidUsuario, bool? SomenteAtivos = false, string camposNecessarios = null)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterListaCampoCliente(clienteEntity, nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                clienteEntity = new ClienteEntity();
                                CarregarClienteEntity(dataTableReader, clienteEntity);
                                clientes.Add(clienteEntity);
                            }
                        }
                        else
                        {
                            clienteEntity = new ClienteEntity();
                            CarregarClienteEntity(dataTableReader, clienteEntity);
                            clientes.Add(clienteEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            if (!String.IsNullOrEmpty(camposNecessarios))
            {
                var retorno = SerializarJsonRetornoView(clientes, camposNecessarios);
                return Request.CreateResponse(HttpStatusCode.OK, new { clientes = retorno });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }

        [HttpGet]
        [Route("ObterListaPorUsuarioPerfil")]
        public HttpResponseMessage ObterListaPorUsuarioPerfil(Int64 nidUsuario, bool? SomenteAtivos = false, string camposNecessarios = null)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterListaCampoCliente(clienteEntity, nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                clienteEntity = new ClienteEntity();
                                CarregarClienteEntity(dataTableReader, clienteEntity);
                                clientes.Add(clienteEntity);
                            }
                        }
                        else
                        {
                            clienteEntity = new ClienteEntity();
                            CarregarClienteEntity(dataTableReader, clienteEntity);
                            clientes.Add(clienteEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            if(!String.IsNullOrEmpty(camposNecessarios))
            {
                var retorno = SerializarJsonRetornoView(clientes, camposNecessarios);
                return Request.CreateResponse(HttpStatusCode.OK, new { clientes = retorno });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }

        /// <summary>
        /// Consulta de Clientes Excluiso para Combo Cliente vinculados ao usuário
        /// </summary>
        /// <param name="nidUsuario"></param>
        /// <param name="SomenteAtivos"></param>
        /// <returns>List<ClienteEntity> clientes</returns>
        [HttpGet]
        [Route("ObterListaComboPorUsuarioPerfil")]
        public HttpResponseMessage ObterListaComboPorUsuarioPerfil(Int64 nidUsuario, bool? SomenteAtivos = false, string camposNecessarios = null)
        {
            List<ClienteComboEntity> clientes = new List<ClienteComboEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                DataTableReader dataTableReader = new ClienteData().ObterListaCombo(nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                ClienteComboEntity clienteComboEntity = new ClienteComboEntity();
                                CarregarClienteComboEntity(dataTableReader, clienteComboEntity);
                                clientes.Add(clienteComboEntity);
                            }
                        }
                        else
                        {
                            ClienteComboEntity clienteComboEntity = new ClienteComboEntity();
                            CarregarClienteComboEntity(dataTableReader, clienteComboEntity);
                            clientes.Add(clienteComboEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            if (!String.IsNullOrEmpty(camposNecessarios))
            {
                var retorno = SerializarJsonRetornoView(clientes, camposNecessarios);
                return Request.CreateResponse(HttpStatusCode.OK, new { clientes = retorno });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }


        /// <summary>
        /// Consulta de Clientes vinculados ao usuário via CD_TECNICO
        /// </summary>
        /// <param name="CD_TECNICO"></param>
        /// <param name="SomenteAtivos"></param>
        /// <returns>List<ClienteEntity> clientes</returns>
        [HttpGet]
        [Route("ObterListaPorTecnicoPerfil")]
        public HttpResponseMessage ObterListaPorTecnicoPerfil(string CD_TECNICO, bool SomenteAtivos = false)
        {
            bool encontrado = false;
            Int64 nidUsuario = 0;
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                // Ao informar o CD_TECNICO, busca o ID_USUARIO vinculado no CD_TECNICO
                TecnicoEntity tecnicoEntity = new TecnicoEntity();
                tecnicoEntity.CD_TECNICO = CD_TECNICO;
                DataTableReader dtTecnico = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();

                if (dtTecnico.HasRows)
                {
                    if (dtTecnico.Read())
                    {
                        nidUsuario = Convert.ToInt64("0" + dtTecnico["ID_USUARIO"]);
                        encontrado = true;
                    }
                }

                if (dtTecnico != null)
                {
                    dtTecnico.Dispose();
                    dtTecnico = null;
                }

                // Se não encontrar o ID_USUARIO do CD_TECNICO, interrompe a consulta dos clientes pois não existe usuário vinculado
                if (encontrado == false)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
                }

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
                                clienteEntity = new ClienteEntity();

                                CarregarClienteEntity(dataTableReader, clienteEntity);

                                clientes.Add(clienteEntity);
                            }
                        }
                        else
                        {
                            clienteEntity = new ClienteEntity();

                            CarregarClienteEntity(dataTableReader, clienteEntity);

                            clientes.Add(clienteEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }

        [HttpGet]
        [Route("ObterListaPerfilCliente")]
        public HttpResponseMessage ObterListaPerfilCliente(Int64 nidUsuario, bool? SomenteAtivos = false, string camposNecessarios = null)
        {
            List<ClienteEntity> clientes = new List<ClienteEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                clienteEntity.usuario.nidUsuario = nidUsuario;
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                clienteEntity = new ClienteEntity();
                                CarregarClienteEntity(dataTableReader, clienteEntity);
                                clientes.Add(clienteEntity);
                            }
                        }
                        else
                        {
                            clienteEntity = new ClienteEntity();
                            CarregarClienteEntity(dataTableReader, clienteEntity);
                            clientes.Add(clienteEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            if(camposNecessarios != null)
            {
                var ret = SerializarJsonRetornoView(clientes, camposNecessarios);
                return Request.CreateResponse(HttpStatusCode.OK, new { clientes = ret });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }


        [HttpGet]
        [Route("ObterListaComboPerfilCliente")]
        public HttpResponseMessage ObterListaComboPerfilCliente(Int64 nidUsuario, bool? SomenteAtivos = false, string camposNecessarios = null)
        {
            List<ClienteComboEntity> clientes = new List<ClienteComboEntity>();

            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                clienteEntity.usuario.nidUsuario = nidUsuario;
                DataTableReader dataTableReader = new ClienteData().ObterListaCombo(nidUsuario).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        if (SomenteAtivos == true)
                        {
                            if (dataTableReader["DT_DESATIVACAO"] == DBNull.Value)
                            {
                                ClienteComboEntity clienteComboEntity = new ClienteComboEntity();
                                CarregarClienteComboEntity(dataTableReader, clienteComboEntity);
                                clientes.Add(clienteComboEntity);
                            }
                        }
                        else
                        {
                            ClienteComboEntity clienteComboEntity = new ClienteComboEntity();
                            CarregarClienteComboEntity(dataTableReader, clienteComboEntity);
                            clientes.Add(clienteComboEntity);
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            if (camposNecessarios != null)
            {
                var ret = SerializarJsonRetornoView(clientes, camposNecessarios);
                return Request.CreateResponse(HttpStatusCode.OK, new { clientes = ret });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { clientes = clientes });
        }


        /// <summary>
        /// Consulta o cliente informado
        /// </summary>
        /// <param name="CD_CLIENTE"></param>
        /// <returns>ClienteEntity clienteEntity</returns>
        [HttpGet]
        [Route("Obter")]
        public HttpResponseMessage Obter(int CD_CLIENTE)
        {
            ClienteEntity clienteEntity = new ClienteEntity();

            try
            {
                clienteEntity.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        CarregarClienteEntity(dataTableReader, clienteEntity);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { cliente = clienteEntity });
        }

        /// <summary>
        /// Busca a quantidade de equipamentos do cliente
        /// </summary>
        /// <param name="CD_CLIENTE"></param>
        /// <returns>int nvlQtdeEquipamentos</returns>
        [HttpGet]
        [Route("ObterQtdeEquipamentos")]
        public HttpResponseMessage ObterQtdeEquipamentos(int CD_CLIENTE)
        {
            int nvlQtdeEquipamentos = 0;
            try
            {
                ClienteEntity clienteEntity = new ClienteEntity();
                clienteEntity.CD_CLIENTE = CD_CLIENTE;
                DataTableReader dataTableReader = new ClienteData().ObterListaQtdeEquipamentos(clienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        nvlQtdeEquipamentos = Convert.ToInt32(dataTableReader["nvlQtdeEquipamentos"]);
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                //throw ex;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { nvlQtdeEquipamentos = nvlQtdeEquipamentos });
        }

        /// <summary>
        /// Transfere os dados do DataTableReader para ClienteEntity
        /// </summary>
        /// <param name="dataTableReader"></param>
        /// <param name="clienteEntity"></param>
        protected void CarregarClienteEntity(DataTableReader dataTableReader, ClienteEntity clienteEntity)
        {
            clienteEntity.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
            clienteEntity.grupo.CD_GRUPO = dataTableReader["CD_GRUPO"].ToString();
            clienteEntity.grupo.DS_GRUPO = dataTableReader["DS_GRUPO"].ToString();
            clienteEntity.CD_RAC = dataTableReader["CD_RAC"].ToString();
            clienteEntity.vendedor.CD_VENDEDOR = Convert.ToInt64("0" + dataTableReader["CD_VENDEDOR"]);
            clienteEntity.vendedor.NM_VENDEDOR = dataTableReader["NM_VENDEDOR"].ToString();
            clienteEntity.NR_CNPJ = dataTableReader["NR_CNPJ"].ToString();
            //clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString();
            clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
            clienteEntity.EN_ENDERECO = dataTableReader["EN_ENDERECO"].ToString();
            clienteEntity.EN_BAIRRO = dataTableReader["EN_BAIRRO"].ToString();
            clienteEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            clienteEntity.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            clienteEntity.EN_CEP = dataTableReader["EN_CEP"].ToString();
            clienteEntity.regiao.CD_REGIAO = dataTableReader["CD_REGIAO"].ToString();
            clienteEntity.regiao.DS_REGIAO = dataTableReader["DS_REGIAO"].ToString();
            clienteEntity.CD_ABC = dataTableReader["CD_ABC"].ToString();
            clienteEntity.CD_FILIAL = dataTableReader["CD_FILIAL"].ToString();
            clienteEntity.CL_CLIENTE = dataTableReader["CL_CLIENTE"].ToString();
            clienteEntity.TX_TELEFONE = dataTableReader["TX_TELEFONE"].ToString();
            clienteEntity.TX_FAX = dataTableReader["TX_FAX"].ToString();
            clienteEntity.executivo.CD_EXECUTIVO = Convert.ToInt64("0" + dataTableReader["CD_EXECUTIVO"]);
            clienteEntity.executivo.NM_EXECUTIVO = dataTableReader["NM_EXECUTIVO"].ToString();
            clienteEntity.QT_PERIODO = Convert.ToInt32("0" + dataTableReader["QT_PERIODO"]);
            clienteEntity.FL_KAT_FIXO = Convert.ToBoolean(dataTableReader["FL_KAT_FIXO"]);

            if (dataTableReader["TX_EMAIL"] != DBNull.Value)
            {
                clienteEntity.TX_EMAIL = dataTableReader["TX_EMAIL"].ToString();
            }

            if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
            {
                clienteEntity.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"]);
            }

            if (dataTableReader["FL_PESQ_SATISF"] != DBNull.Value)
            {
                clienteEntity.FL_PESQ_SATISF = dataTableReader["FL_PESQ_SATISF"].ToString();
            }
            
            if (dataTableReader["ID_SEGMENTO"] != DBNull.Value)
            {
                clienteEntity.Segmento.ID_SEGMENTO = Convert.ToInt64(dataTableReader["ID_SEGMENTO"]);
            }
            if (dataTableReader["DS_SEGMENTO"] != DBNull.Value)
            {
                clienteEntity.Segmento.DS_SEGMENTO = dataTableReader["DS_SEGMENTO"].ToString();
            }
        }

        protected void CarregarClienteComboEntity(DataTableReader dataTableReader, ClienteComboEntity clienteComboEntity)
        {
            clienteComboEntity.CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]);
            clienteComboEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
            clienteComboEntity.EN_CIDADE = dataTableReader["EN_CIDADE"].ToString();
            clienteComboEntity.EN_ESTADO = dataTableReader["EN_ESTADO"].ToString();
            if (dataTableReader["DT_DESATIVACAO"] != DBNull.Value)
            {
                clienteComboEntity.DT_DESATIVACAO = Convert.ToDateTime(dataTableReader["DT_DESATIVACAO"]);
            }
        }

        [HttpGet]
        [Route("ObterListaClienteSinc")]
        public IHttpActionResult ObterListaClienteSinc(Int64 idUsuario)
        {
            IList<ClienteSinc> listaCliente = new List<ClienteSinc>();
            try
            {
                ClienteData clienteData = new ClienteData();
                listaCliente = clienteData.ObterListaClienteSinc(idUsuario);

                JObject JO = new JObject
                {
                    { "CLIENTE", JArray.FromObject(listaCliente) }
                };

                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("AlterarEnvioPesquisa")]
        public IHttpActionResult AlterarEnvioPesquisa(string fl_pesq_satisf, long nidUsuario)
        {
            try
            {
                ClienteData data = new ClienteData();

                ClienteEntity clienteEntity = new ClienteEntity();
                clienteEntity.FL_PESQ_SATISF = fl_pesq_satisf;
                clienteEntity.nidUsuarioAtualizacao = nidUsuario;

                data.AlterarFlagEnvioPesquisa(clienteEntity);

                return Ok(ControlesUtility.Constantes.MensagemGravacaoSucesso);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CalcularKAT")]
        public IHttpActionResult CalcularKAT(int CD_CLIENTE)
        {
            string retorno = string.Empty;
            string DS_CLASSIFICACAO_KAT = string.Empty;
            int QT_PERIODO = 0;

            try
            {
                DataTableReader dataTableReader = new ClienteData().CalcularKAT(CD_CLIENTE).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        retorno = "Cálculo do KAT efetuado com sucesso!";

                        if (CD_CLIENTE != 0)
                        {
                            ClienteEntity clienteEntity = new ClienteEntity();
                            clienteEntity.CD_CLIENTE = CD_CLIENTE;
                            DataTableReader dtReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();

                            if (dtReader.HasRows)
                            {
                                if (dtReader.Read())
                                {
                                    DS_CLASSIFICACAO_KAT = dtReader["DS_CLASSIFICACAO_KAT"].ToString();
                                    QT_PERIODO = Convert.ToInt16("0" + dtReader["QT_PERIODO"]);
                                }
                            }
                        }

                        retorno += "<hr/>&raquo;<strong>" + dataTableReader["TOTAL_CALCULADOS"].ToString() + "</strong> clientes CALCULADOS." +
                            "<br/>&raquo;<strong>" + dataTableReader["TOTAL_BLOQUEADOS"].ToString() + "</strong> clientes NÃO calculados por BLOQUEIO." +
                            "<br/>&raquo;<strong>" + dataTableReader["TOTAL_DESATIVADOS"].ToString() + "</strong> clientes NÃO calculados por DESATIVAÇÃO.";

                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject
            {
                { "retorno", JsonConvert.SerializeObject(retorno, Formatting.None) }
            };

            if (CD_CLIENTE != 0)
            {
                jObject.Add("DS_CLASSIFICACAO_KAT", JsonConvert.SerializeObject(DS_CLASSIFICACAO_KAT, Formatting.None));
                jObject.Add("QT_PERIODO", JsonConvert.SerializeObject(QT_PERIODO, Formatting.None));
            }
            return Ok(jObject);
        }

        [HttpPost]
        [Route("GerarRelatorioKAT")]
        public IHttpActionResult GerarRelatorioKAT()
        {
            try
            {
                new ClienteData().GerarRelatorioKAT();
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject jObject = new JObject
            {
                { "retorno", JsonConvert.SerializeObject("Sincronização efetuada com sucesso!", Formatting.None) }
            };

            return Ok(jObject);
        }


    }
}