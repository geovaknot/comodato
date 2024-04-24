$().ready(function () {
    LimparScrollWorkflow();

    $('#CD_CLIENTE').select2({
        minimumInputLength: 1,
        placeholder: "Selecione...",
        allowClear: true,
        tags: true,
        createTag: function (params) {
            if (isNaN(params.term)) {
                return null;
            }

            return {
                id: params.term,
                text: params.term
            };
        }
    });



    //Solution for Issue: https://github.com/select2/select2/issues/3905
    $("#CD_CLIENTE").data("select2").on("results:message", function () {
        this.dropdown._positionDropdown();
    });

    //Chamado SL00033995
    if ($('#CD_CLIENTE option:selected').val() != 0) {
        //$('#CD_CLIENTE').prop("disabled", "disabled");
        //$('#ListaSegmento').prop("disabled", "disabled");
    }

    OcultarCampo($('#divTipoSolicitacaoTroca'));
    ExibirCategoriaSelecionada();
    ExibirTipoSolicitacaoTroca();

    //$('#QuantidadeEquipamento').ForceNumericOnly();
    $('#QuantidadeEquipamento').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });

    $('#TX_TELEFONE').ForceNumericOnly();
    $('#TX_TELEFONE').mask('99 99999-99999');
    $('#TX_EMAIL').mask("A", {
        translation: {
            "A": { pattern: /[\w@\-.+]/, recursive: true }
        }
    });

    //$('#VolumeAno').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#VolumeAno').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true, precision: 0 });
    $('#ValorAlturaMinima').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#ValorAlturaMaxima').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#ValorLarguraMinima').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#ValorLarguraMaxima').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#ComprimentoMinimo').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#ComprimentoMaximo').maskMoney({ allowNegative: false, thousands: '', decimal: ',', allowZero: true, precision: 0 });
    $('#PesoMinimo').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });
    $('#PesoMaximo').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', allowZero: true });

    moment.locale('pt-BR');

    CarregarGRIDMensagem();

    CarregarAnexos(idWFEnvio);
});

function CarregarGRIDMensagem() {
    var ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();

    if (ID_WF_PEDIDO_EQUIP == "" || ID_WF_PEDIDO_EQUIP == "0" || ID_WF_PEDIDO_EQUIP == 0) {
        return;
    }

    var URL = URLObterListaMensagem + "?ID_WF_PEDIDO_EQUIP=" + ID_WF_PEDIDO_EQUIP;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                $('#gridmvcMensagem').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function OcultarCategorias() {
    OcultarCampo($('#Categoria' + CategoriaFechador));
    OcultarCampo($('#Categoria' + CategoriaIdentificador));
    OcultarCampo($('#Categoria' + CategoriaAcessorios));
}

function ExibirCategoriaSelecionada() {
    OcultarCategorias();
    var cd_categoria = $('#Categoria option:selected').val();
    if (null != cd_categoria) {
        //if (cd_categoria == CategoriaIdentificador || cd_categoria == CategoriaFechador || cd_categoria == CategoriaAcessorios) {
        //    var obj = $('#Categoria' + cd_categoria);
        //    if (obj != null) {
        //        ExibirCampo(obj);
        //    }                       
        //}

        if (cd_categoria == CategoriaIdentificador) {
            ExibirCampo($('#Categoria' + CategoriaIdentificador));
            ExibirCampo($('#Categoria' + CategoriaAcessorios));
        } else if (cd_categoria == CategoriaAcessorios) {
            ExibirCampo($('#Categoria' + CategoriaAcessorios));
        } else if (cd_categoria == CategoriaFechador) {
            ExibirCampo($('#Categoria' + CategoriaFechador));
        }

        if (cd_categoria == CategoriaAcessorios || cd_categoria == CategoriaIdentificador) {
            PopularGridAcessorios();
        }
    }
}

function ExibirTipoSolicitacaoTroca() {
    if ($('#TipoSolicitacao').val() == TipoSolicitacaoTroca) {
        ExibirCampo($('#divTipoSolicitacaoTroca'));

        if (CodigoAtivoTroca != null || CodigoAtivoTroca != '' || CodigoAtivoTroca != 0 || CodigoAtivoTroca != '0') {
            PopularAtivoCliente($('#CD_CLIENTE option:selected').val(), CodigoAtivoTroca);
        }
    }
}

//$('#CD_CLIENTE').change(function () {
//    var CD_CLIENTE = $('#CD_CLIENTE option:selected').val();
//    if (CD_CLIENTE != null && CD_CLIENTE != '') {
//        PreencherCliente(CD_CLIENTE);
//        PopularAtivoCliente(CD_CLIENTE, '');

//        var pedidoEntity = ObterObjetoPedido();

//        pedidoEntity.ST_STATUS_PEDIDO = 0;

//        EnviarPedido(pedidoEntity, false);

//    }
//});

//$("#CD_VENDEDOR").change(function () {
//    if ($("#CD_VENDEDOR").val() != null && $("#CD_VENDEDOR").val() != "" && $("#CD_VENDEDOR").val() != "Selecione...") {
//        document.getElementById('#CD_CLIENTE').removeAttribute("readonly");
//        $('#CD_CLIENTE').removeAttribute("readonly");
//    }
//}

$("#CD_CLIENTE").change(function () {
    
    var vendedor = $("#ID_USU_SOLICITANTE option:selected").val();
    if (vendedor == null || vendedor == "" || vendedor == "Selecione..." || vendedor == undefined) {
        Alerta("Atenção", "Favor informar o solicitante/vendedor");
        $("#CD_CLIENTE").val(0);
        return false;
    } else {

        var CD_CLIENTE = $("#CD_CLIENTE").val();

        if (CD_CLIENTE != "") {
            var URL = URLVerificarCodigo + "?CD_CLIENTE=" + CD_CLIENTE;

            $.ajax({
                url: URL,
                processData: true,
                dataType: "json",
                cache: false,
                contentType: "application/json",
                beforeSend: function () {
                    $("#loader").css("display", "block");
                },
                complete: function () {
                    $("#loader").css("display", "none");
                },
                success: function (res) {
                    $("#loader").css("display", "none");

                    if (res.Status == "Redirecionar") {
                        //if (res.idkey != null && res.idkey != '') {
                        PreencherCliente(CD_CLIENTE);
                        PopularAtivoCliente(CD_CLIENTE, '');

                        var pedidoEntity = ObterObjetoPedido();

                        pedidoEntity.ST_STATUS_PEDIDO = 0;

                        //if (res.cliente.DS_SEGMENTO != null && res.cliente.DS_SEGMENTO != ""
                        //    && res.cliente.DS_SEGMENTO != undefined ||
                        //    ($('#DS_SEGMENTO').val() != null && $('#DS_SEGMENTO').val() != "")) {
                        //    $('#DS_SEGMENTO').prop("readonly", "readonly");
                        //}

                        EnviarPedido(pedidoEntity, false);
                        //}
                    }
                    else {
                        $('#CD_CLIENTE').val(res.cliente.CD_CLIENTE);
                        $('#EN_ENDERECO').val(res.cliente.EN_ENDERECO);
                        $('#EN_BAIRRO').val(res.cliente.EN_BAIRRO);
                        $('#EN_CIDADE').val(res.cliente.EN_CIDADE);
                        $("#TX_FAX").val(res.cliente.TX_FAX);
                        $("#TX_TELEFONE").val(res.cliente.TX_TELEFONE);
                        $("#NM_CLIENTE").val(res.cliente.NM_CLIENTE);
                        $("#EN_ESTADO").val(res.cliente.EN_ESTADO);
                        $("#EN_CEP").val(res.cliente.EN_CEP);

                        $("#CD_ABC").val(res.cliente.CD_ABC);
                        $("#regiao_CD_REGIAO").val(res.cliente.regiao.CD_REGIAO);
                        $("#CD_FILIAL").val(res.cliente.CD_FILIAL);
                        $("#CL_CLIENTE").val(res.cliente.CL_CLIENTE);
                        $("#CD_RAC").val(res.cliente.CD_RAC);

                        $("#NR_CNPJ").val(res.cliente.NR_CNPJ);

                        PopularAtivoCliente(CD_CLIENTE, '');

                        pedidoEntity = ObterObjetoPedido();

                        pedidoEntity.ST_STATUS_PEDIDO = 0;

                        //EnviarPedido(pedidoEntity, false);

                        //if (res.cliente.Segmento.DS_SEGMENTO == null || res.cliente.Segmento.DS_SEGMENTO == ""
                        //    || res.cliente.Segmento.DS_SEGMENTO == undefined) {
                        PopularSegmento();
                        //}
                        var vendedor = $("#ID_USU_SOLICITANTE").val();
                        if (vendedor != null && vendedor != "" && vendedor != "Selecione..." && vendedor != undefined) {
                            res.cliente.vendedor.CD_VENDEDOR = vendedor;

                            IncluirNovoCliente(res.cliente);

                            EnviarPedido(pedidoEntity, false);
                        }
                    }

                    //PopularAtivoCliente(CD_CLIENTE, '');
                    //pedidoEntity = ObterObjetoPedido();
                    //pedidoEntity.ST_STATUS_PEDIDO = 0;
                    //AtualizarClienteSegmento();
                    //EnviarPedido(pedidoEntity, false);
                }
            });
        }
    }
});

function IncluirNovoCliente(cliente) {

    cliente.BPCS = 1;

    var URL = URLAPI + "ClienteAPI/Adicionar";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        data: JSON.stringify(cliente),
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.retorno != null) {
                Alerta("AVISO", "Novo cliente registrado!");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

$('#ListaSegmento').on('change', function (e) {
    AtualizarClienteSegmento();
});

function AtualizarClienteSegmento() {
    //Se cliente existe atualiza segmento, caso não, inserir novo cliente
    var CD_CLIENTE = $('#CD_CLIENTE').val();
    var ID_SEGMENTO = $('#ListaSegmento option:selected').val();

    if (ID_SEGMENTO == "" || ID_SEGMENTO == null || ID_SEGMENTO == 0 || ID_SEGMENTO == undefined) {
        ID_SEGMENTO = 1;
    }
    //var token = sessionStorage.getItem("token");
    if (CD_CLIENTE != "" && CD_CLIENTE != null) {

        $.ajax({
            type: 'POST',
            url: URLAPI + "WorkflowAPI/AtualizarClienteSegmento?CD_CLIENTE=" + CD_CLIENTE + "&ID_SEGMENTO=" + ID_SEGMENTO,
            processData: false,
            //data: JSON.stringify(clienteEntity),
            dataType: "json",
            cache: false,
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                //$("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                //Alerta("Alerta", "O segmento do cliente foi alterado.");
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", JSON.parse(res.responseText).Message);
            }
        });

    }
}

function PopularSegmento() {
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "WorkflowAPI/PopularSegmentos",
        processData: false,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            var data = JSON.parse(res.retorno);
            if (data != null) {
                for (i = 0; i < data.length; i++) {
                    MontarCombo($('#ListaSegmento'), data[i].id_segmento, data[i].ds_segmento);
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}


//Validação para Categoria - Fechador ou Identificador
$('#Categoria').on('change', function (e) {
    ExibirCategoriaSelecionada();
    PopularModelo($('#Modelo'), $('#Categoria option:selected').val());

    $('#Linha').val('');
    $('#DescricaoCategoria').val('');
    $('#LinhaProduto').val('');
});

//Validação para Troca
$('#TipoSolicitacao').on('change', function (e) {
    OcultarCampo($('#divTipoSolicitacaoTroca'));
    ExibirTipoSolicitacaoTroca();
});

$("#Modelo").on('change', function (e) {

    var cdModelo = $("#Modelo option:selected").val();
    PreencherModelo(cdModelo);
});

//INFORMAÇÕES DE N.F. do ATIVO
$('#NumeroAtivoTroca').on('change', function (e) {
    var item = $('#NumeroAtivoTroca option:selected');

    $('#NumeroNFTroca').val(item.data('NR_NOTAFISCAL'));
    //$('#DataNFTroca').val(moment(new Date(item.data('DT_NOTAFISCAL'))).format('L'));
    $('#DataNFTroca').val(FormatarData(item.data('DT_NOTAFISCAL')));
    $('#ModeloEquipamentoTroca').val(item.data('DS_MODELO'));
});

$('#btnAdicionarAcessorio').click(function () {
    AdicionarAcessorio();
});

$('#btnSalvarRascunho').click(function () {

    var cliente = $('#CD_CLIENTE').val();
    if (cliente == "" || cliente == null || cliente == undefined || cliente == 0) {
        Alerta("Atenção", "Favor informar um cliente");
        return false;
    }

    var segmento = $('#ListaSegmento').val();
    if (segmento == "" || segmento == null || segmento == undefined) {
        Alerta("Atenção", "Favor informar o segmento do cliente");
        return false;
    }

    //SL00034831
    var contato = $('#DS_CONTATO_NOME').val();
    if (contato == "" || contato == null || contato == undefined) {
        //Alerta("Atenção", "Favor informar um contato");
        //return false;
    } else
        if (contato.length > 100) {
            Alerta("Atenção", "O Nome do Contato deve ter no máximo 100 caracteres");
            return false;
        }

    //SL00034831
    
    $('#TX_TELEFONE').mask('99 99999-99999');

    var tel = $('#TX_TELEFONE').val();
    if (tel == "" || tel == null || tel == undefined) {
        //Alerta("Atenção", "Favor informar um telefone de contato");
        //return false;
    } else
        if (tel.length > 14) {
            Alerta("Atenção", "O telefone de contato principal deve ter no máximo 14 caracteres");
            return false;
        }



    var pedidoEntity = ObterObjetoPedido();
    pedidoEntity.ST_STATUS_PEDIDO = 0;

    //UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadEnvio", function (data) {
    //    $('#DS_ARQUIVO').val(data.file[0]);
    //    var guid = $('#DS_ARQUIVO').val();
    //    if (guid == "") {
    //        Alerta("Aviso", "Falha na operação! Favor tentar novamente.");
    //        return false;
    //    }

    //    var URL = URLAPI + "WorkflowAPI/AtualizaUpload?ID_WF_PEDIDO_EQUIP=" + pedidoEntity.ID_WF_PEDIDO_EQUIP + "&GUID=" + guid;
    //    $.ajax({
    //        type: 'POST',
    //        url: URL,
    //        dataType: "json",
    //        cache: false,
    //        async: false,
    //        contentType: "application/json",
    //        //headers: { "Authorization": "Basic " + localStorage.token },
    //        //data: null,
    //        beforeSend: function () {
    //            $("#loader").css("display", "block");
    //        },
    //        complete: function () {
    //            $("#loader").css("display", "none");
    //        },
    //        success: function (res) {
    //            $("#loader").css("display", "none");
    //            $('#DS_ARQUIVO').val(res.Arquivo);
    //        },
    //        error: function (res) {
    //            $("#loader").css("display", "none");
    //            Alerta("ERRO", JSON.parse(res.responseText).Message);
    //        }
    //    });
    //});

    //var arqAnexo = $('#DS_ARQUIVO').val();
    //arqAnexo.replace('"', '');
    //if (arqAnexo.charAt(0) == '"') {
    //    arqAnexo = arqAnexo.substring(1);
    //}
    //if (arqAnexo.charAt(arqAnexo.length - 1) == '"') {
    //    arqAnexo = arqAnexo.substring(0, arqAnexo.length - 1);
    //}
    //pedidoEntity.DS_ARQ_ANEXO = arqAnexo;

    EnviarPedido(pedidoEntity, true);

});

$('#btnSalvarSubmeter').click(function () {
    var pedidoEntity = ObterObjetoPedido();
    var recarregarAcompanhamento = false;

    var checar = checarCamposObrigatorios();

    if (checar == false)
        return false;
        
    //if (pedidoEntity.ST_STATUS_PEDIDO == "" || pedidoEntity.ST_STATUS_PEDIDO == undefined)
    //    pedidoEntity.ST_STATUS_PEDIDO = codigoStatusRascunho;

    pedidoEntity.ST_STATUS_PEDIDO = parseInt(pedidoEntity.ST_STATUS_PEDIDO);

    if (pedidoEntity.ST_STATUS_PEDIDO == codigoStatusRascunho || pedidoEntity.ST_STATUS_PEDIDO == codigoStatusPendenteAnexar)
        pedidoEntity.ST_STATUS_PEDIDO = codigoStatusEnvio;
    else
        recarregarAcompanhamento = true;

    //var guid = UploadFilesWF(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadEnvio");

    //if (guid == "") {
    //    Alerta("Aviso", "Falha na operação! Favor tentar novamente.");
    //    return false;
    //}

    //pedidoEntity.DS_ARQ_ANEXO = guid;
    //if (pedidoEntity.DS_ARQ_ANEXO != null && pedidoEntity.ST_STATUS_PEDIDO <= 2) {
    //    pedidoEntity.ST_STATUS_PEDIDO = 2;
    //}


    //var data = new FormData();

    ////Add the Multiple selected files into the data object
    //var files = $("input[type='file']").get(0).files;
    //for (i = 0; i < files.length; i++) {
    //    data.append("files" + i, files[i]);
    //}

    //var guid = "";

    //var URL = URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadEnvio";

    //if (files.length > 0) {
    //    $.ajax({
    //        type: 'POST',
    //        url: URL,
    //        data: data,
    //        dataType: "json",
    //        contentType: false,
    //        processData: false,
    //        beforeSend: function () {
    //            $("#loader").css("display", "block");
    //        },
    //        success: function (data) {
    //            $("#loader").css("display", "none");
    //            guid = data.file[0];
    //            pedidoEntity.DS_ARQ_ANEXO = guid;
    //            if (pedidoEntity.DS_ARQ_ANEXO != null && pedidoEntity.ST_STATUS_PEDIDO <= 2) {
    //                pedidoEntity.ST_STATUS_PEDIDO = 2;
    //            }
    //        },
    //        error: function () {
    //            $("#loader").css("display", "none");
    //            Alerta("Falha", "Não foi possível enviar o arquivo.");
    //        }
    //    }).done(function (data) {

    //        //var URL = URLAPI + "WorkflowAPI/AtualizaUpload?ID_WF_PEDIDO_EQUIP=" + pedidoEntity.ID_WF_PEDIDO_EQUIP + "&GUID=" + guid;
    //        //$.ajax({
    //        //    type: 'POST',
    //        //    url: URL,
    //        //    dataType: "json",
    //        //    cache: false,
    //        //    async: false,
    //        //    contentType: "application/json",
    //        //    //headers: { "Authorization": "Basic " + localStorage.token },
    //        //    //data: null,

    //        //    complete: function () {
    //        //        $("#loader").css("display", "none");
    //        //    },
    //        //    success: function (res) {
    //        //        $("#loader").css("display", "none");
    //        //    },
    //        //    error: function (res) {
    //        //        $("#loader").css("display", "none");
    //        //        Alerta("ERRO", JSON.parse(res.responseText).Message);
    //        //    }
    //        //});


    //    }).done(function (data) {

    //        EnviarPedido(pedidoEntity, true);

    //        if (recarregarAcompanhamento == true) {
    //            //DefinirExibicaoAtual();
    //            CarregarSituacaoStatus();
    //            DefinirFluxoStatusPedido();
    //            CarregarGRIDMensagem();
    //            CarregarGrupoResponsavel();
    //        }
    //    });
    //} else {
        EnviarPedido(pedidoEntity, true);

        if (recarregarAcompanhamento == true) {
            //DefinirExibicaoAtual();
            CarregarSituacaoStatus();
            DefinirFluxoStatusPedido();
            CarregarGRIDMensagem();
            CarregarGrupoResponsavel();
        }
    //}

});

function uploadAnexo() {

    var titulo = $('#DS_TITULO_ANEXO').val();
    var desc = $('#DS_DESCRICAO_ANEXO').val();
    var resp = $('#NomeResponsavel').text();
    var arquivo = $('#labelFile').text();

    if (titulo == '') {
        Alerta('Alerta', 'Informe o título do anexo');
        return false;
    } else if (desc == '') {
        Alerta('Alerta', 'Informe a descrição do anexo');
        return false;
    } else if (resp == '') {
        Alerta('Alerta', 'Informe o responsável do anexo');
        return false;
    } else if (arquivo == '') {
        Alerta('Alerta', 'Informe o arquivo a anexar');
        return false;
    }

    var pedidoEntity = ObterObjetoPedido();

    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadEnvio", function (data) {
        $('#DS_ARQUIVO').val(data.file[0]);
        var guid = $('#DS_ARQUIVO').val();
        if (guid == "") {
            Alerta("Aviso", "Falha na operação! Favor tentar novamente.");
            return false;
        }
        //var token = sessionStorage.getItem("token");
        var URL = URLAPI + "WorkflowAPI/EfetuarUpload?ID_WF_PEDIDO_EQUIP=" + pedidoEntity.ID_WF_PEDIDO_EQUIP + "&GUID=" + guid + "&DS_TITULO_ANEXO=" + titulo + "&DS_DESCRICAO_ANEXO=" + desc + "&NomeResponsavel=" + resp + "&nidUsuarioAtualizacao=" + nidUsuario;
        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            //data: null,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                //$('#DS_ARQUIVO').val(res.Arquivo);
                Alerta("Sucesso", "Arquivo anexado com sucesso! <hr><p><font size='3' color='lightgray'>GUID: " + res.Arquivo + "</font></p>");
                
                $('#DS_TITULO_ANEXO').val('');
                $('#DS_DESCRICAO_ANEXO').val('');
                //$('#labelFile').val('');
                $('#DS_ARQUIVO').val('');
                $('#labelFile').text('Selecionar Arquivo');
                //possuianexo = true;
                //Carregar novo anexo na tela
                CarregarAnexos(pedidoEntity.ID_WF_PEDIDO_EQUIP);
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", JSON.parse(res.responseText).Message);
            }
        });
    });
}

function CarregarAnexos(idWF) {
    var URL = URLAPI + "WorkflowAPI/CarregarAnexos?ID_WF_PEDIDO_EQUIP=" + idWF;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            var anexos = res.Dados;
            
            if (anexos.charAt(0) == '"') {
                anexos = anexos.substring(1);
            }
            if (anexos.charAt(anexos.length - 1) == '"') {
                anexos = anexos.substring(0, anexos.length - 1);
            }

            $("#anexos").html(anexos);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function VerificaAnexos(idWF) {
    var URL = URLAPI + "WorkflowAPI/VerificaAnexos?ID_WF_PEDIDO_EQUIP=" + idWF;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            var anexos = res.Quantidade;

            if (anexos > 0)
                possuianexo = true;
            else
                possuianexo = false;
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function ExcluirAnexo(nidAnexo) {
    var URL = URLAPI + "WorkflowAPI/ExcluirAnexo?nidAnexo=" + nidAnexo;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");

            CarregarAnexos(idWFEnvio);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function PreencherCliente(CD_CLIENTE) {
    //var token = sessionStorage.getItem("token");
    if (CD_CLIENTE != "") {
        var clienteEntity = new Object();
        clienteEntity.CD_CLIENTE = CD_CLIENTE;

        $.ajax({
            type: 'POST',
            url: URLAPI + "ClienteAPI/ObterLista",
            processData: false,
            data: JSON.stringify(clienteEntity),
            dataType: "json",
            cache: false,
            contentType: "application/json",
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                var data = res.clientes;
                if (res.clientes != null && res.clientes.length > 0) {
                    data = data[0];

                    $('#EN_ENDERECO').val(data.EN_ENDERECO);
                    $('#EN_BAIRRO').val(data.EN_BAIRRO);
                    $('#EN_CIDADE').val(data.EN_CIDADE);
                    $("#NM_CLIENTE").val(data.NM_CLIENTE);
                    $("#EN_ESTADO").val(data.EN_ESTADO);
                    $("#EN_CEP").val(data.EN_CEP);
                    $("#NR_CNPJ").val(data.NR_CNPJ);
                    $("#TX_EMAIL").val(data.TX_EMAIL);
                    $("#TX_TELEFONE").val(data.TX_TELEFONE);

                    //if (data.Segmento.ID_SEGMENTO > 0) {
                    $("#ListaSegmento").val(data.Segmento.ID_SEGMENTO);
                    //} else {
                    //    PopularSegmento();
                    //    $("#DS_SEGMENTO").val();
                    //}

                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", JSON.parse(res.responseText).Message);
            }
        });
    }
}

function PopularModelo(obj, idCategoria) {
    LimparCombo(obj);

    if (idCategoria == '' || idCategoria == '0' || idCategoria == 0 || idCategoria == null)
        return false;

    var modeloEntity = {};

    modeloEntity = {
        CATEGORIA: {
            ID_CATEGORIA: parseInt(idCategoria),
        },
    };


    //var modeloEntity = new Object();
    //modeloEntity.CATEGORIA = new Object();
    //modeloEntity.CATEGORIA.ID_CATEGORIA = idCategoria;

    //$.ajax({
    //    type: 'POST',
    //    url: URLAPI + "ModeloAPI/ObterLista",
    //    processData: false,
    //    data: JSON.stringify(modeloEntity),
    //    dataType: "json",
    //    cache: false,
    //    contentType: "application/json",
    //    beforeSend: function () {
    //        $("#loader").css("display", "block");
    //    },
    //    complete: function () {
    //        $("#loader").css("display", "none");
    //    },
    //    success: function (res) {
    //        var data = JSON.parse(res.MODELO);
    //        if (data != null) {
    //            for (i = 0; i < data.length; i++) {
    //                MontarCombo(obj, data[i].CD_MODELO, data[i].DS_MODELO);
    //            }
    //        }
    //    },
    //    error: function (res) {
    //        Alerta("ERRO", JSON.parse(res.responseText).Message);
    //    }
    //});
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "ModeloAPI/ObterLista",
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(modeloEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            ExibirCampo($('#loader'));
        },
        complete: function () {
            OcultarCampo($('#loader'));
        },
        success: function (res) {
            $("#loader").css("display", "none");
            var data = JSON.parse(res.MODELO);
            if (data != null) {
                for (i = 0; i < data.length; i++) {
                    MontarCombo(obj, data[i].CD_MODELO, data[i].DS_MODELO);
                }
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });


}

function PreencherModelo(cdModelo) {
    $('#Linha').val('');
    $('#ModeloAlturaMinima').val('');
    $('#ModeloAlturaMaxima').val('');
    $('#ModeloLarguraMinima').val('');
    $('#ModeloLarguraMaxima').val('');
    $('#ModeloComprimentoMinimo').val('');
    $('#ModeloComprimentoMaximo').val('');

    $('#DescricaoCategoria').val('');
    $('#LinhaProduto').val('');

    if (cdModelo == '' || cdModelo == null || cdModelo == 0)
        return;

    var modeloEntity = new Object();
    modeloEntity.CD_MODELO = cdModelo;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "ModeloAPI/ObterLista",
        processData: false,
        data: JSON.stringify(modeloEntity),
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            var data = JSON.parse(res.MODELO);
            if (data != null) {
                if (data.length > 0) {
                    $('#Linha').val(data[0].TP_EMPACOTAMENTO);
                    $('#ModeloAlturaMinima').val(data[0].VL_ALTUR_MIN)
                    $('#ModeloAlturaMaxima').val(data[0].VL_ALTUR_MAX);
                    $('#ModeloLarguraMinima').val(data[0].VL_LARG_MIN);
                    $('#ModeloLarguraMaxima').val(data[0].VL_LARG_MAX);
                    $('#ModeloComprimentoMinimo').val(data[0].VL_COMP_MIN);
                    $('#ModeloComprimentoMaximo').val(data[0].VL_COMP_MAX);

                    $('#DescricaoCategoria').val(data[0].CATEGORIA.DS_CATEGORIA);
                    $('#LinhaProduto').val(data[0].LINHA_PRODUTO.DS_LINHA_PRODUTO);
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function PopularAtivoCliente(codigoCliente, ativoSelecionado) {
    if (codigoCliente == '' || codigoCliente == 0 || codigoCliente == '0')
        return;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "AtivoAPI/ObterListaAtivoCliente?CD_Cliente=" + codigoCliente + "&SomenteATIVOSsemDTDEVOLUCAO=true",
        processData: false,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            var data = JSON.parse(res.listaAtivosClientes);
            if (data != null) {
                for (i = 0; i < data.length; i++) {

                    var option = $('<option>', { value: data[i].CD_ATIVO_FIXO, text: data[i].DS_ATIVO_FIXO });
                    option.data('NR_NOTAFISCAL', data[i].NR_NOTAFISCAL);
                    option.data('DT_NOTAFISCAL', data[i].DT_NOTAFISCAL);
                    option.data('DS_MODELO', data[i].DS_MODELO);

                    if (data[i].CD_ATIVO_FIXO == ativoSelecionado) {
                        option.attr('selected', 'selected');
                    }
                    option.appendTo($('#NumeroAtivoTroca'));
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function AdicionarAcessorio() {
    var url = URLAPI + "WfAcessorioPedidoAPI/Adicionar";

    var cliente = $("#CD_CLIENTE option:selected").val();
    if (cliente == "" || cliente == null) {
        Alerta("Alerta", "Favor selecionar um cliente!");
        return false;
    }

    //var acessorioPedidoWorkflowEntity = new Object();

    //acessorioPedidoWorkflowEntity.ID_WF_PEDIDO_EQUIP = $("#ID_WF_PEDIDO_EQUIP").val();
    //acessorioPedidoWorkflowEntity.CD_MODELO = $("#ddlModelosIdentificador option:selected").val();
    //acessorioPedidoWorkflowEntity.QTD_SOLICITADO = $("#txtModeloIdQuantidade").val();

    var acessorioPedidoWorkflowEntity = {
        ID_WF_PEDIDO_EQUIP: $("#ID_WF_PEDIDO_EQUIP").val(),
        CD_MODELO: $("#ddlModelosIdentificador option:selected").val(),
        QTD_SOLICITADO: $("#txtModeloIdQuantidade").val()
    };

    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: url,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(acessorioPedidoWorkflowEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            PopularGridAcessorios();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function RemoverAcessorio(codigoAcessorio) {
    var url = URLAPI + "WfAcessorioPedidoAPI/Remover?ID_WF_ACESSORIO_EQUIP=" + codigoAcessorio + "&nidUsuario=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: url,
        async: false,
        cache: false,
        contentType: "application/json",
        dataType: "json",

        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");
            PopularGridAcessorios();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function PopularGridAcessorios() {
    $.ajax({
        type: 'POST',
        url: actionListarAcessorios + '?codigoWorkflow=' + $('#ID_WF_PEDIDO_EQUIP').val(),
        //data: JSON.stringify(filtro),
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.Status == 'Success') {
                $('#gridAcessoriosModelo').html(data.Html);
                $('.grid-mvc').gridmvc();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}

function ObterObjetoPedido() {
    var pedidoEntity = new Object();
    pedidoEntity.ID_WF_PEDIDO_EQUIP = $('#ID_WF_PEDIDO_EQUIP').val();
    pedidoEntity.CD_WF_PEDIDO_EQUIP = $('#CD_WF_PEDIDO_EQUIP').val();
    pedidoEntity.ST_STATUS_PEDIDO = $('#ST_STATUS_PEDIDO').val();


    pedidoEntity.DS_TITULO = $('#DS_TITULO').val();
    pedidoEntity.ID_USU_SOLICITANTE = $('#ID_USU_SOLICITANTE option:selected').val();

    pedidoEntity.CD_CLIENTE = $('#CD_CLIENTE').val();
    pedidoEntity.DS_CONTATO_NOME = $('#DS_CONTATO_NOME').val();
    pedidoEntity.DS_CONTATO_EMAIL = $('#TX_EMAIL').val();
    pedidoEntity.DS_CONTATO_TEL_NUM = $('#TX_TELEFONE').val();

    pedidoEntity.ID_CATEGORIA = $('#Categoria option:selected').val();
    pedidoEntity.TP_LOCACAO = $('#TipoLocacao option:selected').val();

    pedidoEntity.ID_TIPO_SOLICITACAO = $('#TipoSolicitacao option:selected').val();
    pedidoEntity.CD_MODELO = $("#Modelo option:selected").val();
    pedidoEntity.CD_LINHA = $('#Linha option:selected').val();
    pedidoEntity.CD_TENSAO = $('#Tensao option:selected').val();
    pedidoEntity.VL_VOLUME_ANO = FormatarValorJson($('#VolumeAno').val());
    pedidoEntity.CD_UNIDADE_MEDIDA = $('#UnidadeMedida option:selected').val();
    pedidoEntity.QT_EQUIPAMENTO = $('#QuantidadeEquipamento').val();
    pedidoEntity.ID_USU_ULT_ATU = nidUsuario;
    //TROCA

    pedidoEntity.CD_TROCA = "N";
    var cd_ativo_fixo_troca = $('#NumeroAtivoTroca option:selected').val();
    if (cd_ativo_fixo_troca != '') {
        pedidoEntity.CD_TROCA = "S";
        pedidoEntity.CD_ATIVO_FIXO_TROCA = cd_ativo_fixo_troca;
    }

    //Condições de Aplicação
    pedidoEntity.CD_COND_LIMPEZA = $('#CondicaoLimpeza option:selected').val();
    pedidoEntity.CD_COND_TEMPERATURA = $('#CondicaoTemperatura option:selected').val();
    pedidoEntity.CD_COND_UMIDADE = $('#CondicaoUmidade option:selected').val();

    if ($('#Categoria option:selected').val() == CategoriaFechador) {
        //FECHADOR - Dados da Caixa
        pedidoEntity.VL_ALTURA_MIN = FormatarValorJson($('#ValorAlturaMinima').val());
        pedidoEntity.VL_ALTURA_MAX = FormatarValorJson($('#ValorAlturaMaxima').val());
        pedidoEntity.VL_LARGURA_MIN = FormatarValorJson($('#ValorLarguraMinima').val());
        pedidoEntity.VL_LARGURA_MAX = FormatarValorJson($('#ValorLarguraMaxima').val());
        pedidoEntity.VL_COMPRIM_MIN = FormatarValorJson($('#ComprimentoMinimo').val());
        pedidoEntity.VL_COMPRIM_MAX = FormatarValorJson($('#ComprimentoMaximo').val());
        pedidoEntity.VL_PESO_MINIMO = FormatarValorJson($('#PesoMinimo').val());
        pedidoEntity.VL_PESO_MAXIMO = FormatarValorJson($('#PesoMaximo').val());


        //FECHADOR - Dados da Fita
        pedidoEntity.CD_TIPO_FITA = $('#Fita option:selected').val();
        pedidoEntity.CD_MODELO_FITA = $('#ModeloFita option:selected').val();
        pedidoEntity.CD_LARGURA_FITA = $('#LarguraFita option:selected').val();
    }

    if ($('#Categoria option:selected').val() == CategoriaIdentificador) {
        //IDENTIFICADOR - Dados para Aplicadores e Impressoras
        pedidoEntity.CD_TIPO_PRODUTO = $('#TipoProduto option:selected').val();
        pedidoEntity.CD_LOCAL_INSTAL = $('#LocalInstalacao option:selected').val();
        pedidoEntity.CD_VELOCIDADE_LINHA = $('#VelocidadeLinha option:selected').val();
        pedidoEntity.CD_GUIA_POSICIONAMENTO = $('#GuiaPosicionamento option:selected').val();

        pedidoEntity.FL_APLIC_DIREITO = "N";
        pedidoEntity.FL_APLIC_ESQUERDO = "N";
        pedidoEntity.FL_APLIC_SUPERIOR = "N";

        if ($("#AplicadorDireito").is(':checked')) {
            pedidoEntity.FL_APLIC_DIREITO = "S";
        }
        if ($("#AplicadorEsquerdo").is(':checked')) {
            pedidoEntity.FL_APLIC_ESQUERDO = "S";
        }
        if ($("#AplicadorSuperior").is(':checked')) {
            pedidoEntity.FL_APLIC_SUPERIOR = "S";
        }

        //IDENTIFICADOR - Dados da Etiqueta
        pedidoEntity.ID_ETIQUETA = $('#Etiqueta option:selected').val();
        pedidoEntity.VL_ALTURA_ETIQUETA = $('#AlturaEtiqueta').val();
        pedidoEntity.VL_LARGURA_ETIQUETA = $('#LarguraEtiqueta').val();

        pedidoEntity.DS_KITPVA_GRAMACAIXA = $('#PvaGramaCaixaPVA').val();
        pedidoEntity.VL_STRETCH_PESOPALLET = $('#StrechPesoPallet').val();
        pedidoEntity.VL_STRETCH_ALTURAPALLET = $('#StrechAlturaPallet').val();
        pedidoEntity.VL_INKJET_NUMCARACTER = $('#InkjetCaracteresCaixa').val();

    }

    pedidoEntity.DS_OBSERVACAO = $('#DS_OBSERVACAO').val();

    pedidoEntity.DS_SEGMENTO = $('#ListaSegmento').val();

    //pedidoEntity.DS_ARQ_ANEXO = $('#DS_ARQUIVO').val();
    var arqAnexo = $('#DS_ARQUIVO').val();
    arqAnexo.replace('"', '');
    if (arqAnexo.charAt(0) == '"') {
        arqAnexo = arqAnexo.substring(1);
    }
    if (arqAnexo.charAt(arqAnexo.length - 1) == '"') {
        arqAnexo = arqAnexo.substring(0, arqAnexo.length - 1);
    }
    pedidoEntity.DS_ARQ_ANEXO = arqAnexo;
    pedidoEntity.TP_PEDIDO = 'E';

    return pedidoEntity;
}

function ObterObjetoPedidoDevolucao() {
    var pedidoEntity = new Object();
    //pedidoEntity.ID_WF_PEDIDO_EQUIP = $('#ID_WF_PEDIDO_EQUIP').val();
    //pedidoEntity.CD_WF_PEDIDO_EQUIP = $('#CD_WF_PEDIDO_EQUIP').val();
    pedidoEntity.ST_STATUS_PEDIDO = 20;

    pedidoEntity.DS_TITULO = $('#DS_TITULO').val();
    pedidoEntity.ID_USU_SOLICITANTE = $('#ID_USU_SOLICITANTE option:selected').val();

    pedidoEntity.CD_CLIENTE = $('#CD_CLIENTE option:selected').val();
    pedidoEntity.DS_CONTATO_NOME = $('#DS_CONTATO_NOME').val();
    pedidoEntity.DS_CONTATO_EMAIL = $('#TX_EMAIL').val();
    pedidoEntity.DS_CONTATO_TEL_NUM = $('#TX_TELEFONE').val();
    pedidoEntity.ID_USU_ULT_ATU = nidUsuario;

    pedidoEntity.CD_MOTIVO_DEVOLUCAO = $('#MotivoDevolucao option:selected').val();
    pedidoEntity.FL_COPIA_NF3M = $("#PossuiNotaFiscal3M").val();
    pedidoEntity.VL_NOTA_FISCAL_3M = FormatarValorJson($('#ValorNotaFiscal3M').val());
    //pedidoEntity.DS_ARQ_ANEXO = $('#DS_ARQUIVO').val();
    pedidoEntity.TP_PEDIDO = 'D';

    return pedidoEntity;
}

function EnviarPedido(pedidoEntity, exibirMsg) {
    //WorkflowAPI/EnviarPedido
    //var token = sessionStorage.getItem("token");
    var url = URLAPI + "WorkflowAPI/EnviarPedido";
    $.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(pedidoEntity),
        //processData: true,
        async: false,
        cache: false,
        contentType: "application/json",
        dataType: "json",

        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (response) {
            $("#loader").css("display", "none");

            var data = JSON.parse(response.DataTroca);

            if ((data.ID_WF_PEDIDO_EQUIP > 0 && data.ID_WF_PEDIDO_EQUIP != null
                && data.ID_WF_PEDIDO_EQUIP != "") && data.ID_WF_PEDIDO_EQUIP != undefined) {
                MensagemTroca(data.CD_WF_PEDIDO_EQUIP);
            } else if (exibirMsg == true) {
                MensagemSucesso();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function AdicionarEquipamento(ID_WF_PEDIDO_EQUIP, CD_ATIVO_FIXO_TROCA) {
    var url = URLAPI + "WfPedidoEquipItemAPI/Adicionar";
    //var token = sessionStorage.getItem("token");
    var equipItemEntity = {
        ID_WF_PEDIDO_EQUIP: ID_WF_PEDIDO_EQUIP,
        CD_ATIVO_FIXO: CD_ATIVO_FIXO_TROCA
    };

    $.ajax({
        type: 'POST',
        url: url,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(equipItemEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}


$('#fileUpload').click(function () {
    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadEnvio", function (data) {
        $('#DS_ARQUIVO').val(data.file[0]);

        Alerta("Sucesso", "Upload efetuado, favor salvar rascunho ou submeter solicitação!");
    });
    return false;
});

$('#ValorAlturaMinima').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloAlturaMinima = $('#ModeloAlturaMinima').val();
    var ModeloAlturaMaxima = $('#ModeloAlturaMaxima').val();
    var ValorAlturaMinima = $('#ValorAlturaMinima').val();

    if (ModeloAlturaMinima == "")
        ModeloAlturaMinima = 0;
    if (ModeloAlturaMaxima == "")
        ModeloAlturaMaxima = 0;
    if (ValorAlturaMinima == "")
        ValorAlturaMinima = 0;

    ModeloAlturaMinima = parseInt(ModeloAlturaMinima);
    ModeloAlturaMaxima = parseInt(ModeloAlturaMaxima);
    ValorAlturaMinima = parseInt(ValorAlturaMinima);

    if (ValorAlturaMinima < ModeloAlturaMinima)
        novoCSS = novoCSS + ' text-danger';
    else if (ValorAlturaMinima > ModeloAlturaMaxima)
        novoCSS = novoCSS + ' text-danger';

    $('#ValorAlturaMinima').removeClass();
    $('#ValorAlturaMinima').addClass(novoCSS);
});

$('#ValorAlturaMaxima').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloAlturaMinima = $('#ModeloAlturaMinima').val();
    var ModeloAlturaMaxima = $('#ModeloAlturaMaxima').val();
    var ValorAlturaMaxima = $('#ValorAlturaMaxima').val();

    if (ModeloAlturaMinima == "")
        ModeloAlturaMinima = 0;
    if (ModeloAlturaMaxima == "")
        ModeloAlturaMaxima = 0;
    if (ValorAlturaMaxima == "")
        ValorAlturaMaxima = 0;

    ModeloAlturaMinima = parseInt(ModeloAlturaMinima);
    ModeloAlturaMaxima = parseInt(ModeloAlturaMaxima);
    ValorAlturaMaxima = parseInt(ValorAlturaMaxima);

    if (ValorAlturaMaxima < ModeloAlturaMinima)
        novoCSS = novoCSS + ' text-danger';
    else if (ValorAlturaMaxima > ModeloAlturaMaxima)
        novoCSS = novoCSS + ' text-danger';

    $('#ValorAlturaMaxima').removeClass();
    $('#ValorAlturaMaxima').addClass(novoCSS);
});

$('#ValorLarguraMinima').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloLarguraMinima = $('#ModeloLarguraMinima').val();
    var ModeloLarguraMaxima = $('#ModeloLarguraMaxima').val();
    var ValorLarguraMinima = $('#ValorLarguraMinima').val();

    if (ModeloLarguraMinima == "")
        ModeloLarguraMinima = 0;
    if (ModeloLarguraMaxima == "")
        ModeloLarguraMaxima = 0;
    if (ValorLarguraMinima == "")
        ValorLarguraMinima = 0;

    ModeloLarguraMinima = parseInt(ModeloLarguraMinima);
    ModeloLarguraMaxima = parseInt(ModeloLarguraMaxima);
    ValorLarguraMinima = parseInt(ValorLarguraMinima);

    if (ValorLarguraMinima < ModeloLarguraMinima)
        novoCSS = novoCSS + ' text-danger';
    else if (ValorLarguraMinima > ModeloLarguraMaxima)
        novoCSS = novoCSS + ' text-danger';

    $('#ValorLarguraMinima').removeClass();
    $('#ValorLarguraMinima').addClass(novoCSS);
});

$('#ValorLarguraMaxima').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloLarguraMinima = $('#ModeloLarguraMinima').val();
    var ModeloLarguraMaxima = $('#ModeloLarguraMaxima').val();
    var ValorLarguraMaxima = $('#ValorLarguraMaxima').val();

    if (ModeloLarguraMinima == "")
        ModeloLarguraMinima = 0;
    if (ModeloLarguraMaxima == "")
        ModeloLarguraMaxima = 0;
    if (ValorLarguraMaxima == "")
        ValorLarguraMaxima = 0;

    ModeloLarguraMinima = parseInt(ModeloLarguraMinima);
    ModeloLarguraMaxima = parseInt(ModeloLarguraMaxima);
    ValorLarguraMaxima = parseInt(ValorLarguraMaxima);

    if (ValorLarguraMaxima < ModeloLarguraMinima)
        novoCSS = novoCSS + ' text-danger';
    else if (ValorLarguraMaxima > ModeloLarguraMaxima)
        novoCSS = novoCSS + ' text-danger';

    $('#ValorLarguraMaxima').removeClass();
    $('#ValorLarguraMaxima').addClass(novoCSS);
});

$('#ComprimentoMinimo').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloComprimentoMinimo = $('#ModeloComprimentoMinimo').val();
    var ModeloComprimentoMaximo = $('#ModeloComprimentoMaximo').val();
    var ComprimentoMinimo = $('#ComprimentoMinimo').val();

    if (ModeloComprimentoMinimo == "")
        ModeloComprimentoMinimo = 0;
    if (ModeloComprimentoMaximo == "")
        ModeloComprimentoMaximo = 0;
    if (ComprimentoMinimo == "")
        ComprimentoMinimo = 0;

    ModeloComprimentoMinimo = parseInt(ModeloComprimentoMinimo);
    ModeloComprimentoMaximo = parseInt(ModeloComprimentoMaximo);
    ComprimentoMinimo = parseInt(ComprimentoMinimo);

    if (ComprimentoMinimo < ModeloComprimentoMinimo)
        novoCSS = novoCSS + ' text-danger';
    else if (ComprimentoMinimo > ModeloComprimentoMaximo)
        novoCSS = novoCSS + ' text-danger';

    $('#ComprimentoMinimo').removeClass();
    $('#ComprimentoMinimo').addClass(novoCSS);
});

$('#ComprimentoMaximo').blur(function () {
    var novoCSS = 'form-control col-md-12';
    var ModeloComprimentoMinimo = $('#ModeloComprimentoMinimo').val();
    var ModeloComprimentoMaximo = $('#ModeloComprimentoMaximo').val();
    var ComprimentoMaximo = $('#ComprimentoMaximo').val();

    if (ModeloComprimentoMinimo == "")
        ModeloComprimentoMinimo = 0;
    if (ModeloComprimentoMaximo == "")
        ModeloComprimentoMaximo = 0;
    if (ComprimentoMaximo == "")
        ComprimentoMaximo = 0;

    ModeloComprimentoMinimo = parseInt(ModeloComprimentoMinimo);
    ModeloComprimentoMaximo = parseInt(ModeloComprimentoMaximo);
    ComprimentoMaximo = parseInt(ComprimentoMaximo);

    if (ComprimentoMaximo < ModeloComprimentoMinimo)
        novoCSS = novoCSS + ' text-danger';
    else if (ComprimentoMaximo > ModeloComprimentoMaximo)
        novoCSS = novoCSS + ' text-danger';

    $('#ComprimentoMaximo').removeClass();
    $('#ComprimentoMaximo').addClass(novoCSS);
});


function checarCamposObrigatorios() {
    var titulo = $('#DS_TITULO').val();
    if (titulo == "" || titulo == null || titulo == undefined) {
        Alerta("Atenção", "Favor informar um título");
        return false;
    }

    var solicitante = $('#ID_USU_SOLICITANTE').val();
    if (solicitante == "" || solicitante == null || solicitante == undefined) {
        Alerta("Atenção", "Favor informar um Solicitante");
        return false;
    }

    var cliente = $('#CD_CLIENTE option:selected').val();
    if (cliente == "" || cliente == null || cliente == undefined || cliente == 0) {
        Alerta("Atenção", "Favor informar um cliente");
        return false;
    }

    var segmento = $('#ListaSegmento').val();
    if (segmento == "" || segmento == null || segmento == undefined) {
        Alerta("Atenção", "Favor informar o segmento do cliente");
        return false;
    }

  //SL00034831
    var contato = $('#DS_CONTATO_NOME').val();
    if (contato == "" || contato == null || contato == undefined) {
        Alerta("Atenção", "Favor informar um contato");
        return false;
    } else
        if (contato.length > 100) {
        Alerta("Atenção", "O Nome do Contato deve ter no máximo 100 caracteres");
        return false;
    }

    var email = $('#TX_EMAIL').val();
    if (email == "" || email == null || email == undefined) {
        Alerta("Atenção", "Favor informar um e-mail de contato");
        return false;
    }

   //SL00034831
   var tel = $('#TX_TELEFONE').val();
    if (tel == "" || tel == null || tel == undefined) {
        Alerta("Atenção", "Favor informar um telefone de contato");
        return false;
    } else
    if (tel.length > 14) {
        Alerta("Atenção", "O telefone de contato deve ter no máximo 14 caracteres");
        return false;
    }

    var categoria = $("#Categoria option:selected").val();
    if (categoria == "" || categoria == null || categoria == undefined || categoria == 0) {
        Alerta("Atenção", "Favor informar uma categoria");
        return false;
    }

    var locacao = $("#TipoLocacao option:selected").val();
    if (locacao == "" || locacao == null || locacao == undefined || locacao == 0) {
        Alerta("Atenção", "Favor informar um tipo de locação");
        return false;
    }

    var solicitacao = $("#TipoSolicitacao option:selected").val();
    if (solicitacao == "" || solicitacao == null || solicitacao == undefined || solicitacao == 0) {
        Alerta("Atenção", "Favor informar um tipo de solicitação");
        return false;
    }

    var modelo = $("#Modelo option:selected").val();
    if ((categoria > 0 && categoria < 6) && (modelo == "" || modelo == null || modelo == undefined || modelo == 0)) {
        Alerta("Atenção", "Favor informar um modelo");
        return false;
    }

    if (modelo != "" && modelo != null && modelo != undefined) {
        var volume = $('#VolumeAno').val();
        if (volume == "" || volume == null || volume == undefined) {
            Alerta("Atenção", "Favor informar o volume por ano");
            return false;
        }

        var unidade = $('#UnidadeMedida').val();
        if (unidade == "" || unidade == null || unidade == undefined) {
            Alerta("Atenção", "Favor informar a unidade de medida");
            return false;
        }

        var tensao = $('#Tensao').val();
        if (tensao == "" || tensao == null || tensao == undefined) {
            Alerta("Atenção", "Favor informar a tensão");
            return false;
        }

        var qtdEquip = $('#QuantidadeEquipamento').val();
        if (qtdEquip == "" || qtdEquip == null || qtdEquip == undefined) {
            Alerta("Atenção", "Favor informar a quantidade de equipamentos");
            return false;
        }
    }

    if (categoria == 1) {
        var fita = $('#Fita option:selected').val();
        if (fita == "" || fita == null || fita == undefined || fita == 0) {
            Alerta("Atenção", "Favor informar a fita");
            return false;
        }

        var modeloFita = $('#ModeloFita option:selected').val();
        if (modeloFita == "" || modeloFita == null || modeloFita == undefined || modeloFita == 0) {
            Alerta("Atenção", "Favor informar o modelo de fita");
            return false;
        }

        var larguraFita = $('#LarguraFita option:selected').val();
        if (larguraFita == "" || larguraFita == null || larguraFita == undefined || larguraFita == 0) {
            Alerta("Atenção", "Favor informar a largura da fita");
            return false;
        }
    }

    if (categoria == 2) {
        var tipoProduto = $('#TipoProduto option:selected').val();
        if (tipoProduto == "" || tipoProduto == null || tipoProduto == undefined || tipoProduto == 0) {
            Alerta("Atenção", "Favor informar o tipo de produto");
            return false;
        }

        var local = $('#LocalInstalacao option:selected').val();
        if (local == "" || local == null || local == undefined || local == 0) {
            Alerta("Atenção", "Favor informar o local de instalação");
            return false;
        }

        var velocidade = $('#VelocidadeLinha option:selected').val();
        if (velocidade == "" || velocidade == null || velocidade == undefined || velocidade == 0) {
            Alerta("Atenção", "Favor informar a velocidade da linha");
            return false;
        }

        var guia = $('#GuiaPosicionamento option:selected').val();
        if (guia == "" || guia == null || guia == undefined || guia == 0) {
            Alerta("Atenção", "Favor informar a guia de posicionamento");
            return false;
        }

        //var etiqueta = $('#Etiqueta option:selected').val();
        //if (etiqueta == "" || etiqueta == null || etiqueta == undefined || etiqueta == 0) {
        //    Alerta("Atenção", "Favor informar a etiqueta");
        //    return false;
        //}

        //var alturaEtiqueta = $('#AlturaEtiqueta').val();
        //if (alturaEtiqueta == "" || alturaEtiqueta == null || alturaEtiqueta == undefined || alturaEtiqueta == 0) {
        //    Alerta("Atenção", "Favor informar a altura da etiqueta");
        //    return false;
        //}

        //var larguraEtiqueta = $('#LarguraEtiqueta').val();
        //if (larguraEtiqueta == "" || larguraEtiqueta == null || larguraEtiqueta == undefined || larguraEtiqueta == 0) {
        //    Alerta("Atenção", "Favor informar a largura da etiqueta");
        //    return false;
        //}

        //var grama = $('#PvaGramaCaixaPVA').val();
        //if (grama == "" || grama == null || grama == undefined || grama == 0) {
        //    Alerta("Atenção", "Favor informar a gramatura da caixa");
        //    return false;
        //}

        //var strech = $('#StrechPesoPallet').val();
        //if (strech == "" || strech == null || strech == undefined || strech == 0) {
        //    Alerta("Atenção", "Favor informar o peso do pallet");
        //    return false;
        //}

        //var strechAltura = $('#StrechAlturaPallet').val();
        //if (strechAltura == "" || strechAltura == null || strechAltura == undefined || strechAltura == 0) {
        //    Alerta("Atenção", "Favor informar a altura do pallet");
        //    return false;
        //}

        //var ink = $('#InkjetCaracteresCaixa').val();
        //if (ink == "" || ink == null || ink == undefined || ink == 0) {
        //    Alerta("Atenção", "Favor informar o número de caracteres por caixa");
        //    return false;
        //}
    }

    var limpeza = $("#CondicaoLimpeza option:selected").val();
    if (limpeza == "" || limpeza == null || limpeza == undefined || limpeza == 0) {
        Alerta("Atenção", "Favor informar a condição de limpeza");
        return false;
    }

    var temp = $("#CondicaoTemperatura option:selected").val();
    if (temp == "" || temp == null || temp == undefined || temp == 0) {
        Alerta("Atenção", "Favor informar a condição da temperatura");
        return false;
    }

    var umidade = $("#CondicaoUmidade option:selected").val();
    if (umidade == "" || umidade == null || umidade == undefined || umidade == 0) {
        Alerta("Atenção", "Favor informar a condição de umidade");
        return false;
    }

    var idWF = $('#ID_WF_PEDIDO_EQUIP').val();
    VerificaAnexos(idWF);

    var arq = possuianexo;
    if (arq == "" || arq == null || arq == undefined || arq == 0 || arq == false) {
        Alerta("Atenção", "Favor adicionar pelo menos 1 anexo");
        return false;
    }

    return true;
}