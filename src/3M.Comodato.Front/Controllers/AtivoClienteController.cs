using _3M.Comodato.Data;
using _3M.Comodato.Entity;
using _3M.Comodato.Front.Services;
using _3M.Comodato.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace _3M.Comodato.Front.Controllers
{
    public class AtivoClienteController : BaseController
    {
        public AtivoClienteService _ativoClienteService;
        // GET: AtivoCliente

        public AtivoClienteController()
        {
            _ativoClienteService = new AtivoClienteService();
        }

        [_3MAuthentication]
        public ActionResult Index()
        {
            return View();
        }

        [_3MAuthentication]
        public ActionResult Incluir()
        {
            Int32 dias = System.Convert.ToInt32(ControlesUtility.Parametro.ObterValorParametro("QtdDiasManutencao"));
            Models.AtivoCliente ativoCliente = new Models.AtivoCliente
            {
                DT_NOTAFISCAL = DateTime.Now.ToString("dd/MM/yyyy"),
                DT_SUGESTAO = "Sugestão: " +
                DateTime.Now.AddDays(dias).ToString("dd/MM/yyyy")
                + " (" + dias.ToString() + " dias a partir de hoje)",
                clientes = new List<Models.Cliente>(), //ObterListaCliente(),
                ativosFixos = new List<Models.Ativo>(), //ObterListaAtivoFixo(),
                motivosDevolucoes = ObterListaMotivoDevolucao(),
                razoesComodatos = ObterListaRazaoComodato(),
                //tipos = ObterListaTipo()
            };

            PopularTipoServico(ativoCliente);

            return View(ativoCliente);
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Incluir(Models.AtivoCliente ativoCliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();

                    ativoClienteEntity.cliente.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;
                    ativoClienteEntity.ativoFixo.CD_ATIVO_FIXO = ativoCliente.ativoFixo.CD_ATIVO_FIXO;
                    ativoClienteEntity.tipo.CD_TIPO = ativoCliente.tipo.CD_TIPO;
                    if (!string.IsNullOrEmpty(ativoCliente.DT_NOTAFISCAL))
                    {
                        ativoClienteEntity.DT_NOTAFISCAL = Convert.ToDateTime(ativoCliente.DT_NOTAFISCAL);
                    }

                    ativoClienteEntity.NR_NOTAFISCAL = Convert.ToInt64(ativoCliente.NR_NOTAFISCAL);
                    ativoClienteEntity.razaoComodato.CD_RAZAO_COMODATO = ativoCliente.razaoComodato.CD_RAZAO_COMODATO;
                    if (!string.IsNullOrEmpty(ativoCliente.DT_DEVOLUCAO))
                    {
                        ativoClienteEntity.DT_DEVOLUCAO = Convert.ToDateTime(ativoCliente.DT_DEVOLUCAO);
                    }
                    if (!string.IsNullOrEmpty(ativoCliente.DT_FIM_GARANTIA_REFORMA))
                    {
                        ativoClienteEntity.DT_FIM_GARANTIA_REFORMA = Convert.ToDateTime(ativoCliente.DT_FIM_GARANTIA_REFORMA);
                    }
                    ativoClienteEntity.motivoDevolucao.CD_MOTIVO_DEVOLUCAO = ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO;
                    ativoClienteEntity.VL_ALUGUEL = Convert.ToDecimal(ativoCliente.VL_ALUGUEL);
                    ativoClienteEntity.TX_TERMOPGTO = ativoCliente.TX_TERMOPGTO;
                    ativoClienteEntity.QTD_MESES_LOCACAO = ativoCliente.QTD_MESES_LOCACAO;
                    ativoClienteEntity.TX_OBS = ativoCliente.TX_OBS;
                    ativoClienteEntity.DS_ARQUIVO_FOTO = ativoCliente.DS_ARQUIVO_FOTO;
                    ativoClienteEntity.DS_ARQUIVO_FOTO2 = ativoCliente.DS_ARQUIVO_FOTO2;
                    ativoClienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    string Mensagem = string.Empty;
                    new AtivoClienteData().Inserir(ref ativoClienteEntity, ref Mensagem);

                    if (ativoCliente.DT_DEVOLUCAO != null && ativoCliente.DT_DEVOLUCAO != "")
                    {
                        ClienteEntity cli = new ClienteEntity();
                        cli.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;

                        DataTableReader cliente = new ClienteData().ObterLista(cli).CreateDataReader();

                        var email = "";
                        var nome = "";
                        var modelo = "";
                        if (cliente.HasRows)
                        {
                            if (cliente.Read())
                            {
                                email = cliente["EMAIL_VENDEDOR"].ToString();
                                nome = cliente["NM_CLIENTE"].ToString();
                            }
                        }
                        if (cliente != null)
                        {
                            cliente.Dispose();
                            cliente = null;
                        }

                        AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                        ativoFixoEntity.CD_ATIVO_FIXO = ativoCliente.ativoFixo.CD_ATIVO_FIXO;
                        DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                modelo = dataTableReader["DS_MODELO"].ToString();
                            }
                        }
                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        if (email != "" && email != null)
                        {
                            var destino = "";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "D")
                                destino = "Destruição";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "R")
                                destino = "Reforma";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "T")
                                destino = "Transferência";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "V")
                                destino = "Venda";

                            var nomeCli = ativoCliente.cliente.CD_CLIENTE.ToString() + "-" + nome;
                            EnviarEmailATV_CLIENTE(email, destino, ativoCliente.DT_DEVOLUCAO,
                                                                         nomeCli, ativoCliente.ativoFixo.CD_ATIVO_FIXO,
                                                                         modelo);
                        }
                    }


                    if (string.IsNullOrEmpty(Mensagem))
                    {
                        ////ativoCliente.cliente.CD_CLIENTE = 0;
                        //ativoCliente.clientes = ObterListaCliente();
                        //ativoCliente.ativosFixos = ObterListaAtivoFixo();
                        //ativoCliente.motivosDevolucoes = ObterListaMotivoDevolucao();
                        //ativoCliente.razoesComodatos = ObterListaRazaoComodato();

                        ////ativoCliente.tipos = ObterListaTipo();
                        //PopularTipoServico(ativoCliente);

                        if (ativoCliente.TP_ACAO == "GravarSair")
                        {
                            ativoCliente.JavaScriptToRun = "MensagemSucesso()";
                        }
                        else
                        {
                            ativoCliente.JavaScriptToRun = "MensagemSucessoContinuar()";
                        }

                        EmailIncluirAtivoCliente(ativoClienteEntity);
                    }
                    else if(Mensagem == "Ativo com data de devolução em aberto já cadastrado!")
                    {
                        ativoCliente.JavaScriptToRun = "MensagemBloqueio()";
                        ativoCliente.Mensagem = Mensagem;
                    }
                    else
                    {
                        ativoCliente.JavaScriptToRun = "MensagemBloqueio()";
                        ativoCliente.Mensagem = Mensagem;
                    }
                }

                ativoCliente.clientes = ObterListaCliente();
                ativoCliente.ativosFixos = ObterListaAtivoFixo();
                ativoCliente.motivosDevolucoes = ObterListaMotivoDevolucao();
                ativoCliente.razoesComodatos = ObterListaRazaoComodato();

                //ativoCliente.tipos = ObterListaTipo();
                PopularTipoServico(ativoCliente);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(ativoCliente); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        [_3MAuthentication]
        public ActionResult Editar(string idKey)
        {
            Models.AtivoCliente ativoCliente = null;
            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();

                ativoClienteEntity.ID_ATIVO_CLIENTE = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new AtivoClienteData().ObterLista(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        ativoCliente = new Models.AtivoCliente
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_ATIVO_FIXO"].ToString()),
                            ID_ATIVO_CLIENTE = Convert.ToInt64(dataTableReader["ID_ATIVO_CLIENTE"]),
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                            },
                            ativoFixo = new AtivoFixoEntity
                            {
                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                modelo = new ModeloEntity
                                {
                                    CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                    DS_MODELO = dataTableReader["CD_ATIVO_FIXO"].ToString() + " - " + dataTableReader["DS_MODELO"].ToString()
                                }
                            },
                            NR_NOTAFISCAL = Convert.ToInt64("0" + dataTableReader["NR_NOTAFISCAL"]),
                            motivoDevolucao = new MotivoDevolucaoEntity
                            {
                                CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                                DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                            },
                            TX_OBS = dataTableReader["TX_OBS"].ToString(),
                            razaoComodato = new RazaoComodatoEntity
                            {
                                CD_RAZAO_COMODATO = Convert.ToInt64("0" + dataTableReader["CD_RAZAO"]),
                                DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
                            },
                            TX_TERMOPGTO = dataTableReader["TX_TERMOPGTO"].ToString()
                        };

                        //SL00035884
                        ativoCliente.tipo = new TipoEntity();
                        if (dataTableReader["CD_TIPO"] != DBNull.Value && dataTableReader["DS_TIPO"] != DBNull.Value)
                        {
                            ativoCliente.tipo.CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"]);
                            ativoCliente.tipo.DS_TIPO = dataTableReader["DS_TIPO"].ToString();
                        }


                        if (dataTableReader["QTD_MESES_LOCACAO"] != DBNull.Value)
                        {
                            ativoCliente.QTD_MESES_LOCACAO = Convert.ToInt32(dataTableReader["QTD_MESES_LOCACAO"]);
                        }

                        ativoCliente.cliente.Segmento.ID_SEGMENTO = Convert.ToInt64(dataTableReader["ID_SEGMENTO"]);
                        ativoCliente.cliente.Segmento.DS_SEGMENTO_MIN = dataTableReader["DS_SEGMENTOMIN"].ToString();

                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                        {
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_FIM_GARANTIA_REFORMA = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["VL_ALUGUEL"] != DBNull.Value)
                        {
                            ativoCliente.VL_ALUGUEL = Convert.ToDecimal(dataTableReader["VL_ALUGUEL"]).ToString("N2");
                        }
                        if (dataTableReader["DS_ARQUIVO_FOTO"] != DBNull.Value)
                        {
                            ativoCliente.DS_ARQUIVO_FOTO = dataTableReader["DS_ARQUIVO_FOTO"].ToString();
                        }
                        if (dataTableReader["DS_ARQUIVO_FOTO2"] != DBNull.Value)
                        {
                            ativoCliente.DS_ARQUIVO_FOTO2 = dataTableReader["DS_ARQUIVO_FOTO2"].ToString();
                        }
                    }
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                //Lista apenas com o cliente em selecionado
                ativoCliente.clientes = ObterListaClientePorCod(ativoCliente.cliente.CD_CLIENTE);

                ativoCliente.ativosFixos = ObterListaAtivoFixoPorCod(ativoCliente.ativoFixo.CD_ATIVO_FIXO);
                ativoCliente.motivosDevolucoes = ObterListaMotivoDevolucao();
                ativoCliente.razoesComodatos = ObterListaRazaoComodato();
                PopularTipoServico(ativoCliente);

                string[] listaDeptoVenda = _ativoClienteService.ObterDeptoVenda();
                ViewBag.Depto = listaDeptoVenda;

                string[] listaCodigoMaterial = _ativoClienteService.ObterCodigoMaterial();
                ViewBag.CodigoMat = listaCodigoMaterial;
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            if (ativoCliente == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(ativoCliente);
            }
        }

        [HttpPost]
        [_3MAuthentication]
        public ActionResult Editar(Models.AtivoCliente ativoCliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();

                    ativoClienteEntity.ID_ATIVO_CLIENTE = Convert.ToInt64(ativoCliente.ID_ATIVO_CLIENTE);
                    ativoClienteEntity.cliente.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;
                    ativoClienteEntity.ativoFixo.CD_ATIVO_FIXO = ativoCliente.ativoFixo.CD_ATIVO_FIXO;
                    ativoClienteEntity.tipo.CD_TIPO = ativoCliente.tipo.CD_TIPO;
                    if (!string.IsNullOrEmpty(ativoCliente.DT_NOTAFISCAL))
                    {
                        ativoClienteEntity.DT_NOTAFISCAL = Convert.ToDateTime(ativoCliente.DT_NOTAFISCAL);
                    }

                    ativoClienteEntity.NR_NOTAFISCAL = Convert.ToInt64(ativoCliente.NR_NOTAFISCAL);
                    ativoClienteEntity.razaoComodato.CD_RAZAO_COMODATO = ativoCliente.razaoComodato.CD_RAZAO_COMODATO;
                    if (!string.IsNullOrEmpty(ativoCliente.DT_DEVOLUCAO))
                    {
                        ativoClienteEntity.DT_DEVOLUCAO = Convert.ToDateTime(ativoCliente.DT_DEVOLUCAO);
                    }
                    if (!string.IsNullOrEmpty(ativoCliente.DT_FIM_GARANTIA_REFORMA))
                    {
                        ativoClienteEntity.DT_FIM_GARANTIA_REFORMA = Convert.ToDateTime(ativoCliente.DT_FIM_GARANTIA_REFORMA);
                    }

                    ativoClienteEntity.motivoDevolucao.CD_MOTIVO_DEVOLUCAO = ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO;
                    ativoClienteEntity.VL_ALUGUEL = Convert.ToDecimal(ativoCliente.VL_ALUGUEL);
                    ativoClienteEntity.TX_TERMOPGTO = ativoCliente.TX_TERMOPGTO;
                    ativoClienteEntity.QTD_MESES_LOCACAO = ativoCliente.QTD_MESES_LOCACAO;
                    ativoClienteEntity.TX_OBS = ativoCliente.TX_OBS;
                    ativoClienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    ativoClienteEntity.DS_ARQUIVO_FOTO = ativoCliente.DS_ARQUIVO_FOTO;
                    ativoClienteEntity.DS_ARQUIVO_FOTO2 = ativoCliente.DS_ARQUIVO_FOTO2;

                    new AtivoClienteData().Alterar(ativoClienteEntity);
                    
                    if (ativoCliente.DT_DEVOLUCAO != null && ativoCliente.DT_DEVOLUCAO != "")
                    {
                        ClienteEntity cli = new ClienteEntity();
                        cli.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;

                        DataTableReader cliente = new ClienteData().ObterLista(cli).CreateDataReader();

                        var email = "";
                        var nome = "";
                        var modelo = "";
                        if (cliente.HasRows)
                        {
                            if (cliente.Read())
                            {
                                email = cliente["EMAIL_VENDEDOR"].ToString();
                                nome = cliente["NM_CLIENTE"].ToString();
                            }
                        }
                        if (cliente != null)
                        {
                            cliente.Dispose();
                            cliente = null;
                        }

                        AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                        ativoFixoEntity.CD_ATIVO_FIXO = ativoCliente.ativoFixo.CD_ATIVO_FIXO;
                        DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                        if (dataTableReader.HasRows)
                        {
                            if (dataTableReader.Read())
                            {
                                modelo = dataTableReader["DS_MODELO"].ToString();
                            }
                        }
                        if (dataTableReader != null)
                        {
                            dataTableReader.Dispose();
                            dataTableReader = null;
                        }

                        if (email != "" && email != null)
                        {
                            var destino = "";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "D")
                                destino = "Destruição";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "R")
                                destino = "Reforma";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "T")
                                destino = "Transferência";
                            if (ativoCliente.motivoDevolucao.CD_MOTIVO_DEVOLUCAO.ToUpper() == "V")
                                destino = "Venda";

                            var nomeCli = ativoCliente.cliente.CD_CLIENTE.ToString() + "-" + nome;
                            EnviarEmailATV_CLIENTE(email, destino, ativoCliente.DT_DEVOLUCAO, 
                                                                         nomeCli, ativoCliente.ativoFixo.CD_ATIVO_FIXO,
                                                                         modelo);
                        }
                    }

                    ativoCliente.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }

                ativoCliente.clientes = ObterListaClientePorCod(ativoCliente.cliente.CD_CLIENTE); //ObterListaCliente();
                ativoCliente.ativosFixos = ObterListaAtivoFixoPorCod(ativoCliente.ativoFixo.CD_ATIVO_FIXO); //ObterListaAtivoFixo();
                ativoCliente.motivosDevolucoes = ObterListaMotivoDevolucao();
                ativoCliente.razoesComodatos = ObterListaRazaoComodato();
                
                PopularTipoServico(ativoCliente);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            return View(ativoCliente); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        public void EnviarEmailATV_CLIENTE(string email, string motivo, string DT_Devolucao, string cliente, string CD_Ativo, string modelo)
        {
            //Método Modelo envio de email
            try
            {
                //string emailsSolicitacaoPeca = ControlesUtility.Parametro.ObterValorParametro("emailsOSFinalizada");

                // Envia a requisição de troca de senha por e-mail

                MailSender mailSender = new MailSender();

                string mailTo = email;
                string mailSubject = "3M.Comodato - Devolução de Equipamento";
                string mailMessage = string.Empty;
                System.Net.Mail.Attachment Attachments = null;
                string mailCopy = null;

                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = string.Empty;
                //string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);

                //URLSite += "/SolicitacaoPecas/Editar?idKey=" + ControlesUtility.Criptografia.Criptografar(tecnicoEntity.CD_TECNICO + "|" + pedidoEntity.ID_PEDIDO.ToString() + "|" + pedidoEntity.TP_TIPO_PEDIDO + "|Solicitação");
                //URLSite += "/OsPadrao/Pesquisa?ID_OS=" + Id_Os;
                Conteudo += "<p>Um Equipamento foi<strong> Devolvido </strong>!</p>";
                Conteudo += "<p>Segue dados:</p>";
                Conteudo += "<p>Cliente: " + cliente + "<br/>";
                Conteudo += "Data de Devolução: " + DT_Devolucao + "<br/>";
                Conteudo += "Destino do equipamento: " + motivo + "<br/>";
                Conteudo += "Ativo Fixo: " + CD_Ativo + "<br/>";
                Conteudo += "Modelo do Equipamento: " + modelo + "<br/>";

                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();

                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

            }
            catch (Exception ex)
            {


            }
        }

        private void PopularTipoServico(Models.AtivoCliente ativoCliente)
        {
            ativoCliente.tipos = ObterListaTipo();

            //Obtem Segmento do Cliente
            ClienteData clienteData = new ClienteData();
            DataTable dtCliente = clienteData.ObterLista(new ClienteEntity() { CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE });
            ativoCliente.cliente.Segmento.ID_SEGMENTO = dtCliente.Rows[0].FieldOrDefault<Int64>("ID_SEGMENTO");

            //Obtém Código do Segmento DI
            SegmentoEntity segmentoDI = new SegmentoData()
                .ObterLista(new SegmentoEntity { DS_SEGMENTO_MIN = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CodigoSegmentoDistribuidor) })
                .FirstOrDefault();

            bool segmentoValido = segmentoDI.ID_SEGMENTO == ativoCliente.cliente.Segmento.ID_SEGMENTO;

            if (segmentoDI == null || (ativoCliente.clientes.Count > 1 && ativoCliente.cliente.CD_CLIENTE == 0) || !segmentoValido)
            {
                var tipoServicoDI = ativoCliente.tipos.Where(c => c.FlagSegmentoDI).FirstOrDefault();
                ativoCliente.tipos.Remove(tipoServicoDI);
            }
        }

        [_3MAuthentication]
        public ActionResult Excluir(string idKey)
        {
            Models.AtivoCliente ativoCliente = null;

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();

                ativoClienteEntity.ID_ATIVO_CLIENTE = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                DataTableReader dataTableReader = new AtivoClienteData().ObterLista(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        ativoCliente = new Models.AtivoCliente
                        {
                            //idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["CD_ATIVO_FIXO"].ToString()),
                            ID_ATIVO_CLIENTE = Convert.ToInt64(dataTableReader["ID_ATIVO_CLIENTE"]),
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString()
                            },
                            ativoFixo = new AtivoFixoEntity
                            {
                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                modelo = new ModeloEntity
                                {
                                    CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                    DS_MODELO = dataTableReader["CD_ATIVO_FIXO"].ToString() + " - " + dataTableReader["DS_MODELO"].ToString()
                                }
                            },
                            NR_NOTAFISCAL = Convert.ToInt64(dataTableReader["NR_NOTAFISCAL"]),
                            motivoDevolucao = new MotivoDevolucaoEntity
                            {
                                CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                                DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                            },
                            TX_OBS = dataTableReader["TX_OBS"].ToString(),
                            razaoComodato = new RazaoComodatoEntity
                            {
                                CD_RAZAO_COMODATO = Convert.ToInt64(dataTableReader["CD_RAZAO"]),
                                DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
                            },
                            tipo = new TipoEntity
                            {
                                CD_TIPO = Convert.ToInt64(dataTableReader["CD_TIPO"]),
                                DS_TIPO = dataTableReader["DS_TIPO"].ToString()
                            },
                            TX_TERMOPGTO = dataTableReader["TX_TERMOPGTO"].ToString()
                        };

                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                        {
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_FIM_MANUTENCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_FIM_GARANTIA_REFORMA = Convert.ToDateTime(dataTableReader["DT_FIM_MANUTENCAO"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["VL_ALUGUEL"] != DBNull.Value)
                        {
                            ativoCliente.VL_ALUGUEL = Convert.ToDecimal(dataTableReader["VL_ALUGUEL"]).ToString("N2");
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
                throw ex;
            }

            if (ativoCliente == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(ativoCliente);
            }
        }

        [HttpPost, ActionName("Excluir")]
        [_3MAuthentication]
        public ActionResult ExcluirConfirmado(string idKey)
        {
            Models.AtivoCliente ativoCliente = new Models.AtivoCliente();
            try
            {
                if (ModelState.IsValid)
                {
                    AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();

                    ativoClienteEntity.ID_ATIVO_CLIENTE = Convert.ToInt64(ControlesUtility.Criptografia.Descriptografar(idKey));
                    ativoClienteEntity.nidUsuarioAtualizacao = CurrentUser.usuario.nidUsuario;

                    new AtivoClienteData().Excluir(ativoClienteEntity);

                    ativoCliente.JavaScriptToRun = "MensagemSucesso()";
                    //return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }
            return View(ativoCliente);
            //return View(idKey); //se não gravar, devolve a própria view para fazer as correções dos campos apresentados com erro
        }

        internal bool EmailIncluirAtivoCliente(AtivoClienteEntity ativoCliente)
        {
            try
            {
                string emails3MIncluirAtivoCliente = ControlesUtility.Parametro.ObterValorParametro("emails3MIncluirAtivoCliente");

                if (emails3MIncluirAtivoCliente == "false")
                {
                    return false;
                }

                TecnicoEntity tecnicoEntity = new TecnicoEntity();

                //Obter o código do Técnico Principal do cliente
                var tecnicoClienteEntity = new TecnicoClienteEntity();
                tecnicoClienteEntity.CD_ORDEM = 1;
                tecnicoClienteEntity.cliente.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;
                DataTableReader dataTableReader = new TecnicoClienteData().ObterLista(tecnicoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.CD_TECNICO = dataTableReader["CD_TECNICO"].ToString();
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }



                //Obter  os dados do Supervisor Técnico do Técnico Principal do cliente
                dataTableReader = new TecnicoData().ObterLista(tecnicoEntity).CreateDataReader();
                tecnicoEntity.NM_TECNICO = "";
                tecnicoEntity.usuarioSupervisorTecnico.cdsEmail = "";
                tecnicoEntity.usuarioSupervisorTecnico.cnmNome = "";
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        tecnicoEntity.NM_TECNICO = dataTableReader["NM_TECNICO"].ToString();
                        if (dataTableReader["cdsEmail"] != DBNull.Value)
                        {
                            tecnicoEntity.usuario.cdsEmail = dataTableReader["cdsEmail"].ToString();
                        }
                        if (dataTableReader["cdsEmailTecRegional"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioSupervisorTecnico.cdsEmail = dataTableReader["cdsEmailTecRegional"].ToString();
                        }
                        if (dataTableReader["cnmNomeTecRegional"] != DBNull.Value)
                        {
                            tecnicoEntity.usuarioSupervisorTecnico.cnmNome = dataTableReader["cnmNomeTecRegional"].ToString();
                        }
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }



                //Obter o Nome Cliente e email vendedor
                ClienteEntity clienteEntity = new ClienteEntity();
                clienteEntity.CD_CLIENTE = ativoCliente.cliente.CD_CLIENTE;
                dataTableReader = new ClienteData().ObterLista(clienteEntity).CreateDataReader();
                clienteEntity.vendedor.TX_EMAIL = "";
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        clienteEntity.NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString();
                        if (dataTableReader["EMAIL_VENDEDOR"] != DBNull.Value)
                        {
                            clienteEntity.vendedor.TX_EMAIL = dataTableReader["EMAIL_VENDEDOR"].ToString();
                        }
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }


                //Obter o Modelo do AtivoFixo
                AtivoFixoEntity ativoFixoEntity = new AtivoFixoEntity();
                ativoFixoEntity.CD_ATIVO_FIXO = ativoCliente.ativoFixo.CD_ATIVO_FIXO;
                dataTableReader = new AtivoFixoData().ObterLista(ativoFixoEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        if (dataTableReader["DS_MODELO"] != DBNull.Value)
                        {
                            ativoFixoEntity.modelo.DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                        }
                    }
                }
                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }





                MailSender mailSender = new MailSender();

                string mailTo = "";
                if (tecnicoEntity.usuarioSupervisorTecnico.cdsEmail != "")
                {
                    mailTo = tecnicoEntity.usuarioSupervisorTecnico.cdsEmail + ";";
                }
                if (clienteEntity.vendedor.TX_EMAIL != "")
                {
                    mailTo += clienteEntity.vendedor.TX_EMAIL;
                }
                string mailSubject = "3M.Comodato - Inclusão de Equipamento no Cliente";
                string mailMessage = string.Empty;


                System.Net.Mail.Attachment Attachments = null;
                string pastaConstante = "PastaAtivoClienteNF";
                string fileName = ativoCliente.DS_ARQUIVO_FOTO;
                string diretorio = Server.MapPath("~") + string.Concat(ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.CaminhoUpload),
                                    typeof(ControlesUtility.Constantes).GetField(pastaConstante).GetValue(null).ToString());

                var filepath = Path.Combine(diretorio, fileName);

                if (System.IO.File.Exists(filepath))
                {
                    Attachments = new System.Net.Mail.Attachment(filepath);
                }




                string mailCopy = null;
                if (!string.IsNullOrEmpty(emails3MIncluirAtivoCliente))
                {
                    mailCopy = emails3MIncluirAtivoCliente;
                }

                string URLSite = ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.URLSite);
                var MensagemEmail = mailSender.GetConteudoHTML("EmailCorpo.html");
                string Conteudo = "";//ControlesUtility.Parametro.ObterValorParametro(ControlesUtility.Constantes.MensagemEnvioAvaliacao);

                Conteudo += "<p>Um Equipamento acaba de ser incluído no Cliente.</p></br>";
                Conteudo += "<p>Seguem os dados:</p>";
                Conteudo += "<p><strong>Cliente:</strong> " + clienteEntity.NM_CLIENTE + " (" + clienteEntity.CD_CLIENTE + ")</p>";
                Conteudo += "<p><strong>Modelo do Equipamento:</strong> " + ativoFixoEntity.modelo.DS_MODELO + "</p>";
                Conteudo += "<p><strong>Ativo:</strong> " + ativoFixoEntity.CD_ATIVO_FIXO + "</p>";
                Conteudo += "<p><strong>Número da Nota Fiscal:</strong> " + ativoCliente.NR_NOTAFISCAL + "</p>";
                Conteudo += "<p>O pdf da Nota Fiscal estará em anexo se a mesma foi cadastrada.</p></br>";

                Conteudo += "<p>Para acessar o sistema por favor, clique no link abaixo ou copie seu conteúdo para o Navegador WEB</p>";
                Conteudo += "<p><a href='" + URLSite + "'></a>" + URLSite + "</p>";


                MensagemEmail.Replace("#Conteudo", Conteudo);
                mailMessage = MensagemEmail.ToString();
                mailSender.Send(mailTo, mailSubject, mailMessage, Attachments, mailCopy);

                return true;

            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                LogUtility.LogarErro("Erro envio EmailIncluirAtivoCliente para ID_ATIVO_CLIENTE=" + ativoCliente.ID_ATIVO_CLIENTE);
                return false;
            }

        }

        [HttpGet]
        [Route("limpaFiltro")]
        public void limpaFiltro()
        {
            if ((AtivoClienteFilter)Session["filtro_Ativo_Ciente"] != null)
            {

                Session["filtro_Ativo_Ciente"] = null;
            }

        }

        public JsonResult ObterListaAtivoClienteJson(string CD_ATIVO_FIXO, long CD_CLIENTE, long NR_NOTAFISCAL)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            // BEGIN - SUPORTE - Erasmo - GSW - 03/02/2021

            if (CD_ATIVO_FIXO != "" || CD_CLIENTE != 0 || NR_NOTAFISCAL != 0)
            {
                var filtroPreenchido = new AtivoClienteFilter()
                {
                    CD_ATIVO_FIXO = CD_ATIVO_FIXO,
                    CD_CLIENTE = CD_CLIENTE,
                    NR_NOTA_FISCAL = NR_NOTAFISCAL
                };
                Session["filtro_Ativo_Ciente"] = filtroPreenchido;
            }
            // END - SUPORTE - Erasmo - GSW - 03/02/2021

            List<Models.AtivoCliente> ativosClientes = new List<Models.AtivoCliente>();

            try
            {
                AtivoClienteEntity ativoClienteEntity = new AtivoClienteEntity();
                ativoClienteEntity.ativoFixo.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                ativoClienteEntity.cliente.CD_CLIENTE = CD_CLIENTE;
                ativoClienteEntity.NR_NOTAFISCAL = NR_NOTAFISCAL;

                DataTableReader dataTableReader = new AtivoClienteData().ObterLista(ativoClienteEntity).CreateDataReader();

                if (dataTableReader.HasRows)
                {
                    while (dataTableReader.Read())
                    {
                        Models.AtivoCliente ativoCliente = new Models.AtivoCliente
                        {
                            idKey = ControlesUtility.Criptografia.Criptografar(dataTableReader["ID_ATIVO_CLIENTE"].ToString()),
                            ID_ATIVO_CLIENTE = Convert.ToInt64(dataTableReader["ID_ATIVO_CLIENTE"]),
                            cliente = new ClienteEntity
                            {
                                CD_CLIENTE = Convert.ToInt32(dataTableReader["CD_CLIENTE"]),
                                NM_CLIENTE = dataTableReader["NM_CLIENTE"].ToString() + " (" + Convert.ToInt32(dataTableReader["CD_CLIENTE"]).ToString() + ") " + dataTableReader["EN_CIDADE"].ToString() + " - " + dataTableReader["EN_ESTADO"].ToString(),
                                NR_CNPJ = dataTableReader["NR_CNPJ"].ToString(),
                                EN_CIDADE = dataTableReader["EN_CIDADE"].ToString(),
                                EN_ESTADO = dataTableReader["EN_ESTADO"].ToString()
                            },
                            ativoFixo = new AtivoFixoEntity
                            {
                                CD_ATIVO_FIXO = dataTableReader["CD_ATIVO_FIXO"].ToString(),
                                TX_ANO_MAQUINA = dataTableReader["TX_ANO_MAQUINA"].ToString(),
                                linhaProduto = new LinhaProdutoEntity
                                {
                                    CD_LINHA_PRODUTO = Convert.ToInt32("0" + dataTableReader["CD_LINHA_PRODUTO"]),
                                    DS_LINHA_PRODUTO = dataTableReader["DS_LINHA_PRODUTO"].ToString()
                                },
                                modelo = new ModeloEntity
                                {
                                    CD_MODELO = dataTableReader["CD_MODELO"].ToString(),
                                    DS_MODELO = dataTableReader["DS_MODELO"].ToString()
                                },
                                situacaoAtivo = new SituacaoAtivoEntity
                                {
                                    CD_SITUACAO_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_SITUACAO_ATIVO"]),
                                    DS_SITUACAO_ATIVO = dataTableReader["DS_SITUACAO_ATIVO"].ToString()
                                },
                                statusAtivo = new StatusAtivoEntity
                                {
                                    CD_STATUS_ATIVO = Convert.ToInt64("0" + dataTableReader["CD_STATUS_ATIVO"]),
                                    DS_STATUS_ATIVO = dataTableReader["DS_STATUS_ATIVO"].ToString()
                                }
                            },
                            NR_NOTAFISCAL = Convert.ToInt64("0" + dataTableReader["NR_NOTAFISCAL"]),
                            motivoDevolucao = new MotivoDevolucaoEntity
                            {
                                CD_MOTIVO_DEVOLUCAO = dataTableReader["CD_MOTIVO_DEVOLUCAO"].ToString(),
                                DS_MOTIVO_DEVOLUCAO = dataTableReader["DS_MOTIVO_DEVOLUCAO"].ToString()
                            },
                            TX_OBS = dataTableReader["TX_OBS"].ToString(),
                            razaoComodato = new RazaoComodatoEntity
                            {
                                CD_RAZAO_COMODATO = Convert.ToInt64("0" + dataTableReader["CD_RAZAO"]),
                                DS_RAZAO_COMODATO = dataTableReader["DS_RAZAO"].ToString()
                            },
                            tipo = new TipoEntity
                            {
                                CD_TIPO = Convert.ToInt64("0" + dataTableReader["CD_TIPO"]),
                                DS_TIPO = dataTableReader["DS_TIPO"].ToString()
                            },
                            TX_TERMOPGTO = dataTableReader["TX_TERMOPGTO"].ToString()

                        };

                        if (dataTableReader["DT_NOTAFISCAL"] != DBNull.Value)
                        {
                            ativoCliente.DT_NOTAFISCAL = Convert.ToDateTime(dataTableReader["DT_NOTAFISCAL"]).ToString("dd/MM/yyyy");
                        }

                        if (dataTableReader["DT_DEVOLUCAO"] != DBNull.Value)
                        {
                            ativoCliente.DT_DEVOLUCAO = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]).ToString("dd/MM/yyyy");
                            ativoCliente.DT_DEVOLUCAO_GRID = Convert.ToDateTime(dataTableReader["DT_DEVOLUCAO"]);
                        }

                        if (dataTableReader["VL_ALUGUEL"] != DBNull.Value)
                        {
                            ativoCliente.VL_ALUGUEL = Convert.ToDecimal(dataTableReader["VL_ALUGUEL"]).ToString("N2");
                        }

                        //if (dataTableReader["DT_FIM_GARANTIA_REFORMA"] != DBNull.Value)
                        //{
                        //    ativoCliente.DT_FIM_GARANTIA_REFORMA = Convert.ToDateTime(dataTableReader["DT_FIM_GARANTIA_REFORMA"]).ToString("dd/MM/yyyy");
                        //}

                        if (dataTableReader["QTD_MESES_LOCACAO"] != DBNull.Value)
                        {
                            ativoCliente.QTD_MESES_LOCACAO = Convert.ToInt32(dataTableReader["QTD_MESES_LOCACAO"]);
                        }

                        ativoCliente.DS_ARQUIVO_FOTO = dataTableReader["DS_ARQUIVO_FOTO"].ToString();
                        ativoCliente.DS_ARQUIVO_FOTO2 = dataTableReader["DS_ARQUIVO_FOTO2"].ToString();


                        ativosClientes.Add(ativoCliente);
                    }
                }
                //List<Models.ListaHistoricoVisitas> listaHistoricoVisitas = ObterListaHistoricoVisitas(CD_CLIENTE, CD_TECNICO, DT_INICIO, DT_FIM);

                jsonResult.Add("Html", RenderRazorViewToString("~/Views/AtivoCliente/_gridMVCAtivoCliente.cshtml", ativosClientes));
                jsonResult.Add("Status", "Success");
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;
        }


        public JsonResult ObterModeloJson(string CD_ATIVO_FIXO)
        {
            Dictionary<string, object> jsonResult = new Dictionary<string, object>();

            try
            {
                string DS_MODELO = "Não Encontrado";
                AtivoFixoEntity ativoEntity = new AtivoFixoEntity();

                ativoEntity.CD_ATIVO_FIXO = CD_ATIVO_FIXO;
                DataTableReader dataTableReader = new AtivoFixoData().ObterLista(ativoEntity).CreateDataReader();
                if (dataTableReader.HasRows)
                {
                    if (dataTableReader.Read())
                    {
                        DS_MODELO = dataTableReader["DS_MODELO"].ToString();
                    }
                    jsonResult.Add("Status", "true");

                }
                else
                {
                    jsonResult.Add("Status", "false");
                }

                if (dataTableReader != null)
                {
                    dataTableReader.Dispose();
                    dataTableReader = null;
                }

                jsonResult.Add("DS_MODELO", DS_MODELO);
            }
            catch (Exception ex)
            {
                LogUtility.LogarErro(ex);
                throw ex;
            }

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            var jsonList = Json(jsonResult, JsonRequestBehavior.AllowGet);
            jsonList.MaxJsonLength = int.MaxValue;
            return jsonList;

        }


    }
}