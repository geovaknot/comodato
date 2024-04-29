using _3M.Comodato.Data;
using _3M.Comodato.Entity;
//using _3M.Comodato.Front.Models;
using _3M.Comodato.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace _3M.Comodato.API.Controllers
{
    [RoutePrefix("api/WorkflowAPI")]
    [Authorize]
    public class WorkflowAPIController : BaseAPIController
    {

        [HttpPost]
        [Route("EnviarPedido")]
        public IHttpActionResult EnviarPedido(WfPedidoEquipEntity pedidoEntity)
        {
            try
            {
                //bool semAnexo = string.IsNullOrEmpty(pedidoEntity.DS_ARQ_ANEXO);

                WfPedidoEquipData pedidoData = new WfPedidoEquipData();
                WfPedidoEquipEntity pedidoTroca = new WfPedidoEquipEntity();

                WfPedidoEquipItemData data = new WfPedidoEquipItemData();
                WfPedidoEquipItemEntity ativoTroca = new WfPedidoEquipItemEntity();

                if (pedidoEntity.TP_PEDIDO == "E" &&
                    pedidoEntity.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseMarketing &&
                    pedidoEntity.CD_TROCA == "S" &&
                    !string.IsNullOrEmpty(pedidoEntity.CD_ATIVO_FIXO_TROCA))
                {

                    pedidoTroca.CD_WF_PEDIDO_EQUIP = new WfPedidoEquipData().ObterNovoCodigoPedido();
                    pedidoTroca.CD_CLIENTE = pedidoEntity.CD_CLIENTE;
                    pedidoTroca.ST_STATUS_PEDIDO = 20;
                    pedidoTroca.TP_PEDIDO = "D";
                    pedidoTroca.DT_PEDIDO = DateTime.Now;
                    pedidoTroca.ID_USU_EMITENTE = pedidoEntity.ID_USU_ULT_ATU;
                    pedidoTroca.ID_USU_SOLICITANTE = pedidoEntity.ID_USU_SOLICITANTE;
                    pedidoTroca.ID_USU_ULT_ATU = pedidoEntity.ID_USU_ULT_ATU;
                    pedidoTroca.DS_TITULO = pedidoEntity.DS_TITULO;
                    pedidoTroca.DS_CONTATO_NOME = pedidoEntity.DS_CONTATO_NOME;
                    pedidoTroca.DS_CONTATO_EMAIL = pedidoEntity.DS_CONTATO_EMAIL;
                    pedidoTroca.DS_CONTATO_TEL_NUM = pedidoEntity.DS_CONTATO_TEL_NUM;
                    pedidoData.Inserir(ref pedidoTroca);
                    InserirLog(pedidoEntity.ID_WF_PEDIDO_EQUIP, pedidoEntity.ST_STATUS_PEDIDO, pedidoEntity.ID_USU_ULT_ATU, pedidoEntity.CD_GRUPO_RESPONS);

                    ativoTroca.CD_ATIVO_FIXO = pedidoEntity.CD_ATIVO_FIXO_TROCA;
                    ativoTroca.ID_WF_PEDIDO_EQUIP = pedidoTroca.ID_WF_PEDIDO_EQUIP;
                    data.Inserir(ref ativoTroca);
                }

                if(pedidoEntity.TP_PEDIDO == "D" && string.IsNullOrEmpty(pedidoEntity.CD_GRUPO_RESPONS) && (pedidoEntity.ST_STATUS_PEDIDO == 20 || pedidoEntity.ST_STATUS_PEDIDO == 21))
                {
                    pedidoEntity.ST_STATUS_PEDIDO = 22;
                    pedidoEntity.CD_GRUPO_RESPONS = "LOJ1";
                }

                if(pedidoEntity.TP_PEDIDO == "E"  && !string.IsNullOrEmpty(pedidoEntity.CD_MODELO) && !string.IsNullOrEmpty(pedidoEntity.CD_LINHA) && !string.IsNullOrEmpty(pedidoEntity.TP_LOCACAO))
                {

                    //Verifica quel grupo deste pedido
                    List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();
                    //string itemGrupo;
                    Utility.WorkflowUtility wfUtil = new WorkflowUtility();
                    // CONSULTA QUAL SERA O NOVO GRUPO DE ATUAÇÃO .........................
                    listaGrupos = wfUtil.IdentificaGrupo(pedidoEntity.ST_STATUS_PEDIDO, Convert.ToInt32(pedidoEntity.ID_CATEGORIA), pedidoEntity.TP_LOCACAO, pedidoEntity.CD_LINHA, pedidoEntity.CD_MODELO);

                    if (listaGrupos?.Count > 0)
                        pedidoEntity.CD_GRUPO_RESPONS = listaGrupos.First().CD_GRUPOWF;
                    else
                        pedidoEntity.CD_GRUPO_RESPONS = null;
                }

                pedidoData.Alterar(pedidoEntity);
                InserirLog(pedidoEntity.ID_WF_PEDIDO_EQUIP, pedidoEntity.ST_STATUS_PEDIDO, pedidoEntity.ID_USU_ULT_ATU, pedidoEntity.CD_GRUPO_RESPONS);

                if (pedidoEntity.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseMarketing || pedidoEntity.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.AnaliseLogistica)
                    EnviarEmail(pedidoEntity);

                //ATUALIZAR STATUS DEVOLUCAO E REFAZER KAT DO CLIENTE
                AtivoClienteData ativoClienteData = new AtivoClienteData();

                //if (pedidoEntity.TP_PEDIDO == "E" && pedidoEntity.CD_TROCA == "S"
                //    && !string.IsNullOrEmpty(pedidoEntity.CD_ATIVO_FIXO_TROCA)
                //    && pedidoEntity.ST_STATUS_PEDIDO > 0)
                //{
                //    //Status de devolucao de ativo da troca em andamento:
                //    ativoClienteData.AtualizarStatusDevolucao(pedidoEntity.CD_CLIENTE, pedidoEntity.CD_ATIVO_FIXO_TROCA+',', '2', pedidoEntity.ID_USU_ULT_ATU);
                //} else 

                if (pedidoEntity.TP_PEDIDO == "D" && pedidoEntity.ST_STATUS_PEDIDO >
                    Convert.ToInt32(ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho))
                {
                    string listaAtivos = "";

                    DataTable dtAtivos = data.ObterListaAtivos(pedidoEntity.ID_WF_PEDIDO_EQUIP);

                    foreach (DataRow row in dtAtivos.Rows)
                    {
                        if (row["CD_ATIVO_FIXO"].ToString() != "")
                        {
                            listaAtivos = listaAtivos + row["CD_ATIVO_FIXO"].ToString() + ",";
                        }
                    }

                    //Status de devolucao de ativo em andamento:
                    ativoClienteData.AtualizarStatusDevolucao(pedidoEntity.CD_CLIENTE, listaAtivos, '2', pedidoEntity.ID_USU_ULT_ATU);

                    //Refazendo KAT:
                    ClienteData clienteData = new ClienteData();
                    clienteData.CalcularKAT(Convert.ToInt32(pedidoEntity.CD_CLIENTE));
                }

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));
                JO.Add("Data", JsonConvert.SerializeObject(pedidoEntity, Formatting.None));
                JO.Add("DataTroca", JsonConvert.SerializeObject(pedidoTroca, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("EfetuarUpload")]
        public IHttpActionResult EfetuarUpload(long ID_WF_PEDIDO_EQUIP, string GUID, string DS_TITULO_ANEXO, string DS_DESCRICAO_ANEXO, string NomeResponsavel, int nidUsuarioAtualizacao)
        {
            try
            {
                WfPedidoEquipData data = new WfPedidoEquipData();
                data.EfetuarUpload(ID_WF_PEDIDO_EQUIP, GUID, DS_TITULO_ANEXO, DS_DESCRICAO_ANEXO, NomeResponsavel, nidUsuarioAtualizacao);

                JObject JO = new JObject();
                JO.Add("Arquivo", JsonConvert.SerializeObject(GUID, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ExcluirPedido")]
        public IHttpActionResult ExcluirPedido(long id_wf_pedido_equip, long nidUsuario)
        {
            try
            {
                WfPedidoEquipEntity pedidoEntity = new WfPedidoEquipEntity();
                pedidoEntity.ID_WF_PEDIDO_EQUIP = id_wf_pedido_equip;
                pedidoEntity.ID_USU_ULT_ATU = nidUsuario;

                WfPedidoEquipData data = new WfPedidoEquipData();
                data.Excluir(pedidoEntity, null);

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemExclusaoSucesso, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AlterarStatus")]
        public IHttpActionResult AlterarStatus(WfPedidoEquipEntity pedidoEntity)
        {

            try
            {
                //... Obter informações complementares do pedido
                WfPedidoEquipData pedidoData = new WfPedidoEquipData();
                WfPedidoEquipEntity pedidoEntity2 = new WfPedidoEquipEntity();
                pedidoEntity2 = pedidoData.ObterPorCodigo(pedidoEntity.ID_WF_PEDIDO_EQUIP);
                pedidoEntity.CD_MODELO = pedidoEntity2.CD_MODELO;
                pedidoEntity.CD_LINHA = pedidoEntity2.CD_LINHA;
                pedidoEntity.TP_LOCACAO = pedidoEntity2.TP_LOCACAO;
                
                //Verifica quel grupo deste pedido
                List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();
                //string itemGrupo;
                Utility.WorkflowUtility wfUtil = new WorkflowUtility();


                // CONSULTA QUAL SERA O NOVO GRUPO DE ATUAÇÃO .........................
                listaGrupos = wfUtil.IdentificaGrupo(pedidoEntity.ST_STATUS_PEDIDO, Convert.ToInt32(pedidoEntity.ID_CATEGORIA), pedidoEntity.TP_LOCACAO, pedidoEntity.CD_LINHA, pedidoEntity.CD_MODELO);
                if (listaGrupos.Count>0)
                {
                    pedidoEntity.CD_GRUPO_RESPONS = listaGrupos.First().CD_GRUPOWF;

                }

                bool acessoLiberado = false;
                UsuarioPerfilEntity userAtual = new UsuarioPerfilEntity();
                userAtual.usuario.nidUsuario = pedidoEntity.ID_USU_ULT_ATU;
                DataTable dtrUser = new UsuarioPerfilData().ObterLista(userAtual);

                foreach (DataRow dr in dtrUser.Rows)
                {
                    int perfil = Convert.ToInt32(dr["ccdPerfil"].ToString());
                    if (perfil == Convert.ToInt64(ControlesUtility.Enumeradores.Perfil.Administrador3M))
                        acessoLiberado = true;
                    break;
                }

                WfGrupoUsuData wfguData = new WfGrupoUsuData();
                WfGrupoUsuEntity wfguEntity = new WfGrupoUsuEntity();
                wfguEntity.usuario.nidUsuario = pedidoEntity.ID_USU_ULT_ATU;
                DataTable dtr = wfguData.ObterLista(wfguEntity);

                foreach (DataRow dr in dtr.Rows)
                {
                    if (pedidoEntity.CD_GRUPO_RESPONS.Trim().ToUpper() == dr["CD_GRUPOWF"].ToString().Trim().ToUpper())
                    {
                        acessoLiberado = true;
                        break;
                    }
                }

                if (pedidoEntity.ID_USUARIO_RESPONS == pedidoEntity.ID_USU_ULT_ATU)
                    acessoLiberado = true;

                if (acessoLiberado)
                {



                    if (!string.IsNullOrEmpty(pedidoEntity.DT_RETIRADA_AGENDADA_Formatada))
                        pedidoEntity.DT_RETIRADA_AGENDADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_AGENDADA_Formatada);

                    if (!string.IsNullOrEmpty(pedidoEntity.DT_RETIRADA_REALIZADA_Formatada))
                        pedidoEntity.DT_RETIRADA_REALIZADA = Convert.ToDateTime(pedidoEntity.DT_RETIRADA_REALIZADA_Formatada);

                    if (!string.IsNullOrEmpty(pedidoEntity.DT_PROGRAMADA_TMS_Formatada))
                        pedidoEntity.DT_PROGRAMADA_TMS = Convert.ToDateTime(pedidoEntity.DT_PROGRAMADA_TMS_Formatada);

                    if (!string.IsNullOrEmpty(pedidoEntity.DT_DEVOLUCAO_3M_Formatada))
                        pedidoEntity.DT_DEVOLUCAO_3M = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_3M_Formatada);

                    if (!string.IsNullOrEmpty(pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO_Formatada))
                        pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO = Convert.ToDateTime(pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO_Formatada);

                    if (pedidoEntity.DT_DEVOLUCAO_3M != null && pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO != null)
                    {
                        // Begin - Erasmo - Suporte - GSW 29012021 - Chamado IM7995350 - Item retirado conforme chamdo.

                        //DevolverAtivos(pedidoEntity.ID_WF_PEDIDO_EQUIP, pedidoEntity.DT_DEVOLUCAO_PLANEJAMENTO);  //Amtes era DT_DEVOLUCAO_3M

                        // End - Erasmo - Suporte - GSW 29012021 - Chamado IM7995350


                        //Alterar status de devolução para devolvido (3):
                        WfPedidoEquipItemData dataPedEquioItem = new WfPedidoEquipItemData();
                        AtivoClienteData ativoClienteData = new AtivoClienteData();
                        if (pedidoEntity.ST_STATUS_PEDIDO > Convert.ToInt32(ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Rascunho))
                        {
                            string listaAtivos = "";

                            DataTable dtAtivos = dataPedEquioItem.ObterListaAtivos(pedidoEntity.ID_WF_PEDIDO_EQUIP);

                            foreach (DataRow row in dtAtivos.Rows)
                            {
                                if (row["CD_ATIVO_FIXO"].ToString() != "")
                                {
                                    listaAtivos = listaAtivos + row["CD_ATIVO_FIXO"].ToString() + ",";
                                }
                            }

                            ativoClienteData.AtualizarStatusDevolucao(pedidoEntity.CD_CLIENTE, listaAtivos, '3', pedidoEntity.ID_USU_ULT_ATU);
                        }
                    }



                    new WfPedidoEquipData().AlterarStatus(pedidoEntity);
                    InserirLog(pedidoEntity.ID_WF_PEDIDO_EQUIP, pedidoEntity.ST_STATUS_PEDIDO, pedidoEntity.ID_USUARIO_RESPONS, pedidoEntity.CD_GRUPO_RESPONS);
                    EnviarEmail(pedidoEntity);

                    JObject JO = new JObject();
                    JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));
                    JO.Add("Data", JsonConvert.SerializeObject(pedidoEntity, Formatting.None));
                    return Ok(JO);
                }
                else
                {
                    JObject JO = new JObject();
                    JO.Add("Mensagem", JsonConvert.SerializeObject("Você não possui permissão para tal ação!", Formatting.None));
                    return Ok(JO);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        private void DevolverAtivos(long iD_WF_PEDIDO_EQUIP, DateTime? DT_DEVOLUCAO_PLANEJAMENTO)
        {
            try
            {
                new WfPedidoEquipData().DevolverAtivos(iD_WF_PEDIDO_EQUIP, DT_DEVOLUCAO_PLANEJAMENTO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
            }
        }

        [HttpPost]
        [Route("AlterarGrupoResponsavel")]
        public IHttpActionResult AlterarGrupoResponsavel(WfPedidoEquipEntity pedidoEntity)
        {
            try
            {
                new WfPedidoEquipData().AlterarGrupoResponsavel(pedidoEntity);

                JObject JO = new JObject();
                JO.Add("Mensagem", JsonConvert.SerializeObject(ControlesUtility.Constantes.MensagemGravacaoSucesso, Formatting.None));
                JO.Add("Data", JsonConvert.SerializeObject(pedidoEntity, Formatting.None));
                return Ok(JO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }


        protected void InserirLog(Int64 ID_WF_PEDIDO_EQUIP, int ST_STATUS_PEDIDO, long nidUsuario, string CD_GRUPO_RESPONS)
        {
            string cnmNome = string.Empty;

            // Cria LOG
            WfPedidoEquipLogEntity wfPedidoEquipLogEntity = new WfPedidoEquipLogEntity();

            wfPedidoEquipLogEntity.pedidoEquip.ID_WF_PEDIDO_EQUIP = ID_WF_PEDIDO_EQUIP;
            wfPedidoEquipLogEntity.statusPedidoEquip.ST_STATUS_PEDIDO = ST_STATUS_PEDIDO;
            wfPedidoEquipLogEntity.nidUsuarioAtualizacao = nidUsuario;
            wfPedidoEquipLogEntity.CD_GRUPO_RESPONS = CD_GRUPO_RESPONS;

            new WfPedidoEquipLogData().Inserir(ref wfPedidoEquipLogEntity);

            // Busca informações
            WfStatusPedidoEquipEntity wfStatusPedidoEquipEntity = new WfStatusPedidoEquipEntity();
            wfStatusPedidoEquipEntity.ST_STATUS_PEDIDO = ST_STATUS_PEDIDO;
            DataTableReader dataTableReader = new WfStatusPedidoEquipData().ObterLista(wfStatusPedidoEquipEntity).CreateDataReader();

            if (dataTableReader.HasRows)
            {
                if (dataTableReader.Read())
                    wfStatusPedidoEquipEntity.DS_COMENTARIO = dataTableReader["DS_COMENTARIO"].ToString();
            }

            if (dataTableReader != null)
            {
                dataTableReader.Dispose();
                dataTableReader = null;
            }

            if (nidUsuario > 0)
            {
                UsuarioEntity usuarioEntity = new UsuarioEntity();
                usuarioEntity.nidUsuario = nidUsuario;
                dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                        cnmNome = dataTableReader["cnmNome"].ToString();
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }
            }
            // Cria registro em Histórico
            WfPedidoComentEntity wfPedidoComentEntity = new WfPedidoComentEntity();

            wfPedidoComentEntity.pedidoEquip.ID_WF_PEDIDO_EQUIP = ID_WF_PEDIDO_EQUIP;
            wfPedidoComentEntity.DS_COMENT = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy hh:mm") + " - " + cnmNome + ": " + wfStatusPedidoEquipEntity.DS_COMENTARIO;
            wfPedidoComentEntity.nidUsuarioAtualizacao = nidUsuario;

            new WfPedidoComentData().Inserir(ref wfPedidoComentEntity);
        }

        [HttpGet]
        [Route("TesteEmail")]
        public IHttpActionResult TesteEmail(string email = "", long codigoPedido = 0, int statusTeste = 0)
        {
            try
            {

                var pedido = new WfPedidoEquipData().ObterPorCodigo(codigoPedido);
                if (statusTeste > 0)
                {
                    pedido.ST_STATUS_PEDIDO = statusTeste;
                }

                EnviarEmail(pedido, email);
                return Ok();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Task EnviarEmail(WfPedidoEquipEntity pedidoEntity, string emailAdicional = "")
        {
            try
            {
                MailSender mailSender = new MailSender();

                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;
                string mailTo = string.Empty;
                string mailSubject = string.Empty;

                if (!string.IsNullOrEmpty(emailAdicional))
                    mailTo = $"{emailAdicional};";

                string tipoPedido = string.Empty;

                var pedidoEquip = new WfPedidoEquipData().ObterPorCodigo(pedidoEntity.ID_WF_PEDIDO_EQUIP);

                if (pedidoEquip != null && pedidoEquip.ID_WF_PEDIDO_EQUIP > 0)
                {
                    if (pedidoEquip.TP_PEDIDO == "E")
                        tipoPedido = "Pedido de Envio de Equipamento";
                    else
                        tipoPedido = "Pedido de Devolução de Equipamento";

                    List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

                    listaGrupos = new WfGrupoData().ObterGruposResponsaveisPorPedido(pedidoEquip);
                    mailSubject = "3M.Comodato - " + tipoPedido;

                    // Envia e-mail para todos os usuários do grupo
                    if (pedidoEquip.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.AnaliseLogistica || pedidoEquip.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoEnvio.AnaliseMarketing)
                        mailMessage = new EmailController().PedidoCriadoEmail(pedidoEquip);
                    else
                        mailMessage = new EmailController().PedidoAlteradoEmail(pedidoEquip);

                    foreach (var grupo in listaGrupos)
                    {
                        if (grupo.ID_GRUPOWF > 0)
                        {
                            WfGrupoUsuEntity wfGrupoUsuEntity = new WfGrupoUsuEntity();
                            wfGrupoUsuEntity.grupoWf.ID_GRUPOWF = grupo.ID_GRUPOWF;
                            List<WfGrupoUsuEntity> wfGrupoUsu = new WfGrupoUsuData().ObterListaEntity(wfGrupoUsuEntity);

                            foreach (var item in wfGrupoUsu)
                            {
                                mailTo += item.usuario.cdsEmail + ";";
                            }
                        }
                    }

                    // Envia e-mail também para o usuários solicitante
                    UsuarioEntity usuarioEntity = new UsuarioEntity();
                    usuarioEntity.nidUsuario = pedidoEquip.ID_USU_SOLICITANTE;

                    DataTableReader dataTableReader = new UsuarioData().ObterLista(usuarioEntity).CreateDataReader();

                    if (dataTableReader.Read())
                        mailTo += dataTableReader["cdsEmail"].ToString();

                    dataTableReader.Dispose();

                    if (mailTo != null && mailSubject != null && mailMessage != null)
                        mailSender.Send(mailTo, mailSubject, mailMessage, null, null);

                    if (pedidoEquip.ST_STATUS_PEDIDO == (int)ControlesUtility.Enumeradores.WorkflowPedidoDevolucao.Solicitado)
                    {
                        EmpresaEntity empresaEntity = new EmpresaData().ObterPorId(Convert.ToInt64(pedidoEquip.CD_EMPRESA));
                        mailSubject = $"Coleta de Comodato ({pedidoEquip.CD_WF_PEDIDO_EQUIP}/{pedidoEquip.DT_PEDIDO.Year.ToString()}) {empresaEntity.NM_Empresa}";

                        mailTo = pedidoEquip.DS_CONTATO_EMAIL;

                        if (pedidoEquip.DS_CONTATO_EMAIL != null)
                        {
                            mailMessage = new EmailController().PedidoSolicitadoEmail(pedidoEquip);
                            mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                        }
                    }
                }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        [HttpPost]
        [Route("PopularSegmentos")]
        public IHttpActionResult PopularSegmentos()
        {
            try
            {

                IList<SegmentoEntity> listaSegmentos = new List<SegmentoEntity>();
                //listaSegmentos.Add(new Segmento());

                SegmentoData data = new SegmentoData();
                SegmentoEntity segmento = new SegmentoEntity();
                listaSegmentos = (IList<SegmentoEntity>)data.ObterLista(segmento);

                JObject jObject = new JObject();
                //jObject.Add("retorno", JsonConvert.SerializeObject(listaSegmentos, Formatting.None));
                jObject.Add("retorno", JArray.FromObject(listaSegmentos));

                return Ok(jObject);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AtualizarClienteSegmento")]
        public void AtualizarClienteSegmento(Int64 cd_cliente, Int64 id_segmento)
        {
            try
            {
                ClienteData clienteData = new ClienteData();
                SegmentoData segmentoData = new SegmentoData();

                ClienteEntity cliente = new ClienteEntity();
                cliente.CD_CLIENTE = cd_cliente;

                DataTableReader dataTableReader = clienteData.ObterLista(cliente).CreateDataReader();
                if (dataTableReader.HasRows)
                {
                    segmentoData.AtualizarSegmento(cd_cliente, id_segmento);
                }
                else
                {
                    dataTableReader = clienteData.ObterListaBPCS(cliente).CreateDataReader();
                    if (dataTableReader.HasRows)
                    {
                        if (dataTableReader.Read())
                        {
                            cliente.CD_CLIENTE = Convert.ToInt32(dataTableReader["CCUST"]);
                            cliente.EN_ENDERECO = dataTableReader["CAD1"].ToString();
                            cliente.EN_BAIRRO = dataTableReader["CAD2"].ToString();
                            cliente.EN_CIDADE = dataTableReader["CAD3"].ToString();
                            cliente.TX_FAX = dataTableReader["CMFAXN"].ToString();
                            cliente.TX_TELEFONE = dataTableReader["CPHON"].ToString();
                            cliente.NM_CLIENTE = dataTableReader["CNME"].ToString();
                            cliente.EN_ESTADO = dataTableReader["CSTE"].ToString();
                            cliente.EN_CEP = dataTableReader["CZIP"].ToString();
                            cliente.CD_ABC = dataTableReader["RVABC"].ToString();

                            if (!string.IsNullOrEmpty(dataTableReader["RVCREG"].ToString().Trim()))
                            {
                                bool encontrado = false;
                                DataTableReader dtRegiao = new RegiaoData().ObterLista(new RegiaoEntity() { CD_REGIAO = dataTableReader["RVCREG"].ToString() }).CreateDataReader();

                                if (dtRegiao.HasRows)
                                {
                                    if (dtRegiao.Read())
                                    {
                                        cliente.regiao.CD_REGIAO = dtRegiao["CD_REGIAO"].ToString();
                                        cliente.regiao.DS_REGIAO = dtRegiao["DS_REGIAO"].ToString();
                                        encontrado = true;
                                    }
                                }

                                if (dtRegiao != null)
                                {
                                    dtRegiao.Dispose();
                                    dtRegiao = null;
                                }

                                // Se não encontrar a região, utiliza o valor padrão para NÃO ENCONTRADO
                                if (encontrado == false)
                                {
                                    dtRegiao = new RegiaoData().ObterLista(new RegiaoEntity() { CD_REGIAO = "??" }).CreateDataReader();

                                    if (dtRegiao.HasRows)
                                    {
                                        if (dtRegiao.Read())
                                        {
                                            cliente.regiao.CD_REGIAO = dtRegiao["CD_REGIAO"].ToString();
                                            cliente.regiao.DS_REGIAO = dtRegiao["DS_REGIAO"].ToString();
                                        }
                                    }

                                    if (dtRegiao != null)
                                    {
                                        dtRegiao.Dispose();
                                        dtRegiao = null;
                                    }
                                }
                            }

                            cliente.CD_FILIAL = dataTableReader["RVREF5"].ToString();
                            cliente.CL_CLIENTE = dataTableReader["RVTYPE"].ToString();
                            cliente.CD_RAC = dataTableReader["RVREF3"].ToString();

                            if (dataTableReader["BCCCGC"] != DBNull.Value)
                                cliente.NR_CNPJ = Convert.ToInt32("0" + dataTableReader["BCCCGC"]).ToString("00000000") + "/" + Convert.ToInt32("0" + dataTableReader["BCFCGC"]).ToString("0000") + "-" + Convert.ToInt32("0" + dataTableReader["BCDCGC"]).ToString("00");
                        }

                    }

                    cliente.Segmento.ID_SEGMENTO = id_segmento;

                    clienteData.Inserir(ref cliente);
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
            }
        }

        /// <summary>
        /// Consulta anexos
        /// </summary>
        /// <param name="idWF"></param>
        /// <returns>DadosSQL</returns>
        [HttpPost]
        [Route("CarregarAnexos")]
        public IHttpActionResult CarregarAnexos(int ID_WF_PEDIDO_EQUIP)
        {
            DataSet dataSet = new WfPedidoEquipData().CarregarAnexos(ID_WF_PEDIDO_EQUIP);
            DataTable dataTable = dataSet.Tables[0];
            DataTable dtTipo = dataSet.Tables[1];

            if (dataTable.Rows.Count < 1)
            {
                JObject JOo = new JObject();
                JOo.Add("Dados", JsonConvert.SerializeObject("<br /><font color='lightgray' size='3'>*Anexar um documento por vez.</font>", Formatting.None));
                return Ok(JOo);
            }

            string html = "<table bgcolor='lightblue' style='width:100%; border: 2px solid white;'>";
            try
            {
                ////Para apresentar cabeçalho:
                //html = html + "<tr>";
                //DataRow rowTitulo = dataTable.Rows[0];
                //for (int j = 0; j < rowTitulo.Table.Columns.Count; j++)
                //{
                //    html = html + "<td>";

                //    html = html + rowTitulo.Table.Columns[j].ToString();

                //    html = html + "</td>";
                //}

                //html = html + "</tr>";

                html = html + "<tr bgcolor='white' style='width:100%; border: 2px solid white;'><th>Título</th><th>Descrição</th><th>Responsável</th><th>Data</th><th></th><th></th></tr>";

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    html = html + "<tr style='width:100%; border: 2px solid white;'>";

                    //html = html + "<td>" + i + "</td>";

                    DataRow row = dataTable.Rows[i];
                    for (int j = 0; j < row.Table.Columns.Count - 2; j++)
                    {
                        html = html + "<td style='padding:5px;'>";

                        html = html + dataTable.Rows[i][j].ToString();

                        html = html + "</td>";
                    }

                    string[] ext = dataTable.Rows[i][5].ToString().Split('.');
                    string pastaConst = "PastaWorkflowUploadEnvio";
                    if (dtTipo.Rows[0][0].ToString() == "D")
                    {
                        pastaConst = "PastaWorkflowUploadDevolucao";
                    }

                    html = html + "<td style='padding:5px;'>" +
                        "<a class='fas fa-file-alt fa-lg' href='"
                        + Utility.ControlesUtility.Parametro.ObterValorParametro(Utility.ControlesUtility.Constantes.URLSite)
                        + "/Workflow/DownloadFile?pastaConstante=" + pastaConst + "&fileName="
                        + dataTable.Rows[i][5].ToString() + "' id='lnkDownload' title='Baixar Anexo'>(." + ext[1] + ")</a></td>";

                    //"<a class='fas fa-file-alt fa-lg' href='#' onclick='BaixarAnexo(" + dataTable.Rows[i][4].ToString() + ")'></a></td>";

                    html = html + "<td style='padding:5px;'><a class='fas fa-trash-alt fa-lg' href='#' onclick='ExcluirAnexo(" + dataTable.Rows[i][4].ToString() + ")'></a></td>";

                    html = html + "</tr>";
                }

                html = html + "</table>";
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Dados", JsonConvert.SerializeObject(html, Formatting.None));
            return Ok(JO);
        }

        /// <summary>
        /// Consulta anexos
        /// </summary>
        /// <param name="nidAnexo"></param>
        /// <returns>DadosSQL</returns>
        [HttpPost]
        [Route("ExcluirAnexo")]
        public IHttpActionResult ExcluirAnexo(int nidAnexo)
        {
            try
            {
                new WfPedidoEquipData().ExcluirAnexo(nidAnexo);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Retorno", JsonConvert.SerializeObject("OK", Formatting.None));
            return Ok(JO);
        }

        [HttpPost]
        [Route("JobEmailDiario")]
        public IHttpActionResult JobEmailDiario()
        {
            try
            {
                // Diário (0)
                int nrDias = 0;

                MailSender mailSender = new MailSender();

                //string mailTo = string.Empty;
                string mailSubject = "Workflow de Envio/Devolução de Equipamentos";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;

                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = "<p>Segue abaixo as atividades pendentes de sua ação:</p>";

                Conteudo += "#LISTA";
                Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                List<Models.Workflow> listaWorkflow = new List<Models.Workflow>();

                DataTableReader dataTableReader = new WfPedidoEquipData().ObterListaEmailWorkflow(nrDias).CreateDataReader();

                while (dataTableReader.Read())
                {
                    Models.Workflow workflow = new Models.Workflow();

                    workflow.wfPedidoEquipEntity.CD_WF_PEDIDO_EQUIP = dataTableReader["CD_WF_PEDIDO_EQUIP"].ToString();
                    workflow.wfPedidoEquipEntity.DT_ULT_ATUALIZACAO = Convert.ToDateTime(dataTableReader["DT_ULT_ATUALIZACAO"]);
                    workflow.DIAS = Convert.ToInt32(dataTableReader["DIAS"]);
                    workflow.wfPedidoEquipEntity.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_NOME"].ToString();
                    workflow.wfPedidoEquipEntity.TP_PEDIDO = dataTableReader["TP_PEDIDO"].ToString();
                    workflow.wfPedidoEquipEntity.ID_CATEGORIA = Convert.ToInt64("0" + dataTableReader["ID_CATEGORIA"]);
                    workflow.wfPedidoEquipEntity.TP_LOCACAO = dataTableReader["TP_LOCACAO"].ToString();
                    workflow.wfPedidoEquipEntity.CD_LINHA = dataTableReader["CD_LINHA"].ToString();
                    workflow.wfPedidoEquipEntity.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                    workflow.wfPedidoEquipEntity.ST_STATUS_PEDIDO = Convert.ToInt32("0" + dataTableReader["ST_STATUS_PEDIDO"]);

                    List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

                    listaGrupos = new WfGrupoData().ObterGruposResponsaveisPorPedido(workflow.wfPedidoEquipEntity);

                    foreach (var grupo in listaGrupos)
                    {
                        workflow.GRUPOS += (!string.IsNullOrEmpty(workflow.GRUPOS) ? ";" : string.Empty) + grupo.CD_GRUPOWF.Trim();
                    }

                    listaWorkflow.Add(workflow);
                }

                if (dataTableReader != null)
                    dataTableReader.Dispose();

                // Busca todos os e-mails por Grupo de Workflow
                WfGrupoUsuEntity wfGrupoUsuEntity = new WfGrupoUsuEntity();
                List<WfGrupoUsuEntity> wfGrupoUsu = new WfGrupoUsuData().ObterListaEntity(wfGrupoUsuEntity);

                foreach (var item in wfGrupoUsu)
                {
                    for (int a = 0; a < listaWorkflow.Count; a++)
                    {
                        if (listaWorkflow[a].GRUPOS.Contains(item.grupoWf.CD_GRUPOWF.Trim()) == true)
                            listaWorkflow[a].EMAILS += item.usuario.cdsEmail + ";";
                    }
                }

                // Separa a lista de Grupos por e-mails (distinct)
                var wfGrupoUsuEmail = wfGrupoUsu.Select(x => x.usuario.cdsEmail).Distinct().ToList();

                // Com cada e-mail e mãos, busca na listaWorkflow os pedidos destes e-mails para envio de forma individual
                foreach (var email in wfGrupoUsuEmail)
                {
                    string ListaPedidos = string.Empty;
                    var listaWorkflowEmail = listaWorkflow.FindAll(x => x.EMAILS.Contains(email)).ToList();   // (x => x.EMAILS == email).ToList();

                    if (listaWorkflowEmail.Count > 0)
                    {
                        ListaPedidos += "<table border=1 style='border-collapse: collapse;'>";
                        ListaPedidos += "   <thead style='background-color: #c2c2c2'>";
                        ListaPedidos += "       <tr>";
                        ListaPedidos += "           <td style='width: 70px'>Pedido #</td>";
                        ListaPedidos += "           <td style='width: 90px'>Desde</td>";
                        ListaPedidos += "           <td style='width: 50px'>Dias</td>";
                        ListaPedidos += "           <td style='width: 250px'>Status</td>";
                        ListaPedidos += "           <td style='width: 90px'>Tipo</td>";
                        ListaPedidos += "           <td style='width: 250px'>Grupo de Atuação</td>";
                        ListaPedidos += "       </tr>";
                        ListaPedidos += "   </thead>";
                        ListaPedidos += "   <tbody>";

                        // Monta a lista por e-mail
                        foreach (var itemLista in listaWorkflowEmail)
                        {
                            ListaPedidos += "       <tr>";
                            ListaPedidos += "           <td><strong>" + itemLista.wfPedidoEquipEntity.CD_WF_PEDIDO_EQUIP + "</strong></td>";
                            ListaPedidos += "           <td>" + Convert.ToDateTime(itemLista.wfPedidoEquipEntity.DT_ULT_ATUALIZACAO).ToString("dd/MM/yyyy") + "</td>";
                            ListaPedidos += "           <td>" + itemLista.DIAS.ToString() + "</td>";
                            ListaPedidos += "           <td>" + itemLista.wfPedidoEquipEntity.DS_STATUS_PEDIDO + "</td>";
                            ListaPedidos += "           <td>" + (itemLista.wfPedidoEquipEntity.TP_PEDIDO.ToUpper() == "E" ? "Envio" : "Devolução") + "</td>";
                            ListaPedidos += "           <td>" + itemLista.GRUPOS + "</td>";
                            ListaPedidos += "       </tr>";
                        }

                        ListaPedidos += "   </tbody>";
                        ListaPedidos += "</table>";

                        Conteudo = Conteudo.Replace("#LISTA", ListaPedidos);

                        MensagemEmail.Replace("#Conteudo", Conteudo);
                        mailMessage = MensagemEmail.ToString();
                        mailSender.Send(email, mailSubject, mailMessage, Attachments, mailCopy);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Retorno", JsonConvert.SerializeObject("OK", Formatting.None));
            return Ok(JO);
        }

        [HttpPost]
        [Route("JobEmailAdministrador")]
        public IHttpActionResult JobEmailAdministrador()
        {
            try
            {
                // Acima de X dias (parametrizado)
                int nrDias = Convert.ToInt32("0" + ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.DiasPendenciaAprovacao));

                // Se não encontrar ou vier zero, será assumido 20 dias como padrão
                if (nrDias == 0)
                    nrDias = 20;

                MailSender mailSender = new MailSender();

                string mailTo = string.Empty;
                string mailSubject = "Workflow de Envio/Devolução de Equipamentos";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;

                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = "<p>Segue abaixo as atividades com pendência <strong>acima de " + nrDias.ToString() + " dias</strong>:</p>";

                Conteudo += "#LISTA";
                Conteudo += "<p>Por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB:</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";

                List<Models.Workflow> listaWorkflow = new List<Models.Workflow>();

                DataTableReader dataTableReader = new WfPedidoEquipData().ObterListaEmailWorkflow(nrDias).CreateDataReader();

                while (dataTableReader.Read())
                {
                    Models.Workflow workflow = new Models.Workflow();

                    workflow.wfPedidoEquipEntity.CD_WF_PEDIDO_EQUIP = dataTableReader["CD_WF_PEDIDO_EQUIP"].ToString();
                    workflow.wfPedidoEquipEntity.DT_ULT_ATUALIZACAO = Convert.ToDateTime(dataTableReader["DT_ULT_ATUALIZACAO"]);
                    workflow.DIAS = Convert.ToInt32(dataTableReader["DIAS"]);
                    workflow.wfPedidoEquipEntity.DS_STATUS_PEDIDO = dataTableReader["DS_STATUS_NOME"].ToString();
                    workflow.wfPedidoEquipEntity.TP_PEDIDO = dataTableReader["TP_PEDIDO"].ToString();
                    workflow.wfPedidoEquipEntity.ID_CATEGORIA = Convert.ToInt64("0" + dataTableReader["ID_CATEGORIA"]);
                    workflow.wfPedidoEquipEntity.TP_LOCACAO = dataTableReader["TP_LOCACAO"].ToString();
                    workflow.wfPedidoEquipEntity.CD_LINHA = dataTableReader["CD_LINHA"].ToString();
                    workflow.wfPedidoEquipEntity.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                    workflow.wfPedidoEquipEntity.ST_STATUS_PEDIDO = Convert.ToInt32("0" + dataTableReader["ST_STATUS_PEDIDO"]);

                    List<WfGrupoEntity> listaGrupos = new List<WfGrupoEntity>();

                    listaGrupos = new WfGrupoData().ObterGruposResponsaveisPorPedido(workflow.wfPedidoEquipEntity);

                    foreach (var grupo in listaGrupos)
                    {
                        workflow.GRUPOS += (!string.IsNullOrEmpty(workflow.GRUPOS) ? ";" : string.Empty) + grupo.CD_GRUPOWF.Trim();
                    }

                    listaWorkflow.Add(workflow);
                }

                if (dataTableReader != null)
                    dataTableReader.Dispose();

                // Busca todos os usuários com perfil Administrador 3M
                UsuarioPerfilEntity usuarioPerfilEntity = new UsuarioPerfilEntity();
                usuarioPerfilEntity.usuario.bidAtivo = true;
                dataTableReader = new UsuarioPerfilData().ObterListaPerfil(usuarioPerfilEntity, "Administrador 3M").CreateDataReader();

                while (dataTableReader.Read())
                {
                    if (!string.IsNullOrEmpty(dataTableReader["cdsEmail"].ToString()))
                        mailTo += (!string.IsNullOrEmpty(mailTo) ? ";" : string.Empty) + dataTableReader["cdsEmail"].ToString();
                }

                if (dataTableReader != null)
                    dataTableReader.Dispose();

                string ListaPedidos = string.Empty;

                if (listaWorkflow.Count > 0)
                {
                    ListaPedidos += "<table border=1 style='border-collapse: collapse;'>";
                    ListaPedidos += "   <thead style='background-color: #c2c2c2'>";
                    ListaPedidos += "       <tr>";
                    ListaPedidos += "           <td style='width: 70px'>Pedido #</td>";
                    ListaPedidos += "           <td style='width: 90px'>Desde</td>";
                    ListaPedidos += "           <td style='width: 50px'>Dias</td>";
                    ListaPedidos += "           <td style='width: 250px'>Status</td>";
                    ListaPedidos += "           <td style='width: 90px'>Tipo</td>";
                    ListaPedidos += "           <td style='width: 250px'>Grupo de Atuação</td>";
                    ListaPedidos += "       </tr>";
                    ListaPedidos += "   </thead>";
                    ListaPedidos += "   <tbody>";

                    // Monta a lista por e-mail
                    foreach (var itemLista in listaWorkflow)
                    {
                        ListaPedidos += "       <tr>";
                        ListaPedidos += "           <td><strong>" + itemLista.wfPedidoEquipEntity.CD_WF_PEDIDO_EQUIP + "</strong></td>";
                        ListaPedidos += "           <td>" + Convert.ToDateTime(itemLista.wfPedidoEquipEntity.DT_ULT_ATUALIZACAO).ToString("dd/MM/yyyy") + "</td>";
                        ListaPedidos += "           <td>" + itemLista.DIAS.ToString() + "</td>";
                        ListaPedidos += "           <td>" + itemLista.wfPedidoEquipEntity.DS_STATUS_PEDIDO + "</td>";
                        ListaPedidos += "           <td>" + (itemLista.wfPedidoEquipEntity.TP_PEDIDO.ToUpper() == "E" ? "Envio" : "Devolução") + "</td>";
                        ListaPedidos += "           <td>" + itemLista.GRUPOS + "</td>";
                        ListaPedidos += "       </tr>";
                    }

                    ListaPedidos += "   </tbody>";
                    ListaPedidos += "</table>";

                    Conteudo = Conteudo.Replace("#LISTA", ListaPedidos);

                    MensagemEmail.Replace("#Conteudo", Conteudo);
                    mailMessage = MensagemEmail.ToString();
                    mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);
                }

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                return BadRequest(ex.Message);
            }

            JObject JO = new JObject();
            JO.Add("Retorno", JsonConvert.SerializeObject("OK", Formatting.None));
            return Ok(JO);
        }
    }
}
