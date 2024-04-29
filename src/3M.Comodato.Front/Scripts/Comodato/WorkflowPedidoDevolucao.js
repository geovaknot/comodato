$().ready(function () {
    LimparScrollWorkflow();

    $('#CD_CLIENTE').select2({
        minimumInputLength: 3,
        placeholder: "Selecione...",
        allowClear: true
    });
    $('#ValorNotaFiscal3M').maskMoney({ allowNegative: true, thousands: '.', decimal: ',', allowZero: true });
    PopularGridEquipamentos();

    CarregarGRIDMensagem();

    CarregarAnexos(idWFDevolucao);
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

$('#CD_CLIENTE').change(function () {
    var CD_CLIENTE = $('#CD_CLIENTE option:selected').val();
    if (CD_CLIENTE != null && CD_CLIENTE != '') {
        PreencherCliente(CD_CLIENTE);
        PopularAtivoCliente(CD_CLIENTE);

        var pedidoEntity = ObterObjetoPedido();

        pedidoEntity.ST_STATUS_PEDIDO = 20;

        EnviarPedido(pedidoEntity, false);

    }

});

$('#btnAdicionarEquipamento').click(function () {
    var ativo = $("#ddlAtivo option:selected").val();
    if (ativo != null && ativo != "") {
        AdicionarEquipamento();
    } else {
        Alerta("Aviso", "Selecione um equipamento!");
        return false;
    }
    
});

$('#btnSalvarRascunho').click(function () {
    var pedidoEntity = ObterObjetoPedido();

    pedidoEntity.ST_STATUS_PEDIDO = 20;

    //UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadDevolucao", function (data) {
    //    $('#DS_ARQUIVO').val(data.file[0]);
    //    var guid = $('#DS_ARQUIVO').val();
    //    if (guid == "" || guid.length < 10) {
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

    ////pedidoEntity.DS_ARQ_ANEXO = $('#DS_ARQUIVO').val();
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

    //var data = new FormData();

    ////Add the Multiple selected files into the data object
    //var files = $("input[type='file']").get(0).files;
    //for (i = 0; i < files.length; i++) {
    //    data.append("files" + i, files[i]);
    //}

    //var guid = "";

    //var URL = URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadDevolucao";

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
    //            if (pedidoEntity.DS_ARQ_ANEXO != null && pedidoEntity.ST_STATUS_PEDIDO <= 22) {
    //                pedidoEntity.ST_STATUS_PEDIDO = 22;
    //            }
    //        },
    //        error: function () {
    //            $("#loader").css("display", "none");
    //            Alerta("Falha", "Não foi possível enviar o arquivo.");
    //        }
    //    }).done(function (data) {
    //        $("#loader").css("display", "none");

    //        var URL = URLAPI + "WorkflowAPI/AtualizaUpload?ID_WF_PEDIDO_EQUIP=" + pedidoEntity.ID_WF_PEDIDO_EQUIP + "&GUID=" + guid;
    //        $.ajax({
    //            type: 'POST',
    //            url: URL,
    //            dataType: "json",
    //            cache: false,
    //            async: false,
    //            contentType: "application/json",
    //            //headers: { "Authorization": "Basic " + localStorage.token },
    //            //data: null,

    //            complete: function () {
    //                $("#loader").css("display", "none");
    //            },
    //            success: function (res) {
    //                $("#loader").css("display", "none");
    //            },
    //            error: function (res) {
    //                $("#loader").css("display", "none");
    //                Alerta("ERRO", JSON.parse(res.responseText).Message);
    //            }
    //        });


    //        }).done(function (data) {
    //            $("#loader").css("display", "none");

    //            //var arqAnexo = $('#DS_ARQUIVO').val();
    //            //arqAnexo.replace('"', '');
    //            //if (arqAnexo.charAt(0) == '"') {
    //            //    arqAnexo = arqAnexo.substring(1);
    //            //}
    //            //if (arqAnexo.charAt(arqAnexo.length - 1) == '"') {
    //            //    arqAnexo = arqAnexo.substring(0, arqAnexo.length - 1);
    //            //}
    //            //pedidoEntity.DS_ARQ_ANEXO = arqAnexo;

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

    //    //var arqAnexo = $('#DS_ARQUIVO').val();
    //    //arqAnexo.replace('"', '');
    //    //if (arqAnexo.charAt(0) == '"') {
    //    //    arqAnexo = arqAnexo.substring(1);
    //    //}
    //    //if (arqAnexo.charAt(arqAnexo.length - 1) == '"') {
    //    //    arqAnexo = arqAnexo.substring(0, arqAnexo.length - 1);
    //    //}
    //    //pedidoEntity.DS_ARQ_ANEXO = arqAnexo;

        EnviarPedido(pedidoEntity, true);

    //    if (recarregarAcompanhamento == true) {
    //        //DefinirExibicaoAtual();
    //        CarregarSituacaoStatus();
    //        DefinirFluxoStatusPedido();
    //        CarregarGRIDMensagem();
    //        CarregarGrupoResponsavel();
    //    }
    //}
});

function PreencherCliente(CD_CLIENTE) {
    if (CD_CLIENTE != "") {
        var clienteEntity = new Object();
        clienteEntity.CD_CLIENTE = CD_CLIENTE;

        $('#EN_ENDERECO').val('');
        $('#EN_BAIRRO').val('');
        $('#EN_CIDADE').val('');
        $("#NM_CLIENTE").val('');
        $("#EN_ESTADO").val('');
        $("#EN_CEP").val('');
        $("#NR_CNPJ").val('');
        $("#TX_EMAIL").val('');
        $("#TX_TELEFONE").val('');
        $("#DS_SEGMENTO").val('');
        //var token = sessionStorage.getItem("token");
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
                    $("#DS_SEGMENTO").val(data.Segmento.DS_SEGMENTO);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", JSON.parse(res.responseText).Message);
            }
        });
    }
}

function PopularAtivoCliente(codigoCliente) {
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
                    option.appendTo($('#ddlAtivo'));
                }
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function AdicionarEquipamento() {
    var url = URLAPI + "WfPedidoEquipItemAPI/Adicionar";

    var cliente = $("#CD_CLIENTE option:selected").val();
    if (cliente == "" || cliente == null) {
        Alerta("Alerta","Favor selecionar um cliente!");
        return false;
    }

    var equipItemEntity = {
        ID_WF_PEDIDO_EQUIP: $("#ID_WF_PEDIDO_EQUIP").val(),
        CD_ATIVO_FIXO: $("#ddlAtivo option:selected").val()
    };
    //var token = sessionStorage.getItem("token");
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
            PopularGridEquipamentos();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function RemoverEquipamento(codigoEquipamento) {
    var url = URLAPI + "WfPedidoEquipItemAPI/Remover?id_wf_pedido_equip_item=" + codigoEquipamento + "&nidUsuario=" + nidUsuario;
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
            PopularGridEquipamentos();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

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

    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadDevolucao", function (data) {
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

            CarregarAnexos(idWFDevolucao);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function PopularGridEquipamentos() {
    $.ajax({
        type: 'POST',
        url: actionListarEquipamentos + '?codigoWorkflow=' + $('#ID_WF_PEDIDO_EQUIP').val(),
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
                $('#gridAtivoDevolucao').html(data.Html);
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

    pedidoEntity.CD_CLIENTE = $('#CD_CLIENTE option:selected').val();
    pedidoEntity.DS_CONTATO_NOME = $('#DS_CONTATO_NOME').val();
    pedidoEntity.DS_CONTATO_EMAIL = $('#TX_EMAIL').val();
    pedidoEntity.DS_CONTATO_TEL_NUM = $('#TX_TELEFONE').val();
    pedidoEntity.ID_USU_ULT_ATU = nidUsuario;

    pedidoEntity.CD_MOTIVO_DEVOLUCAO = $('#MotivoDevolucao option:selected').val();
    pedidoEntity.FL_COPIA_NF3M = $("#PossuiNotaFiscal3M").val();
    pedidoEntity.VL_NOTA_FISCAL_3M = FormatarValorJson($('#ValorNotaFiscal3M').val());
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

    pedidoEntity.TP_PEDIDO = 'D';

    return pedidoEntity;
}

function EnviarPedido(pedidoEntity, exibirMsg) {
    //WorkflowAPI/EnviarPedido

    //var pedidoEntity = ObterObjetoPedido();
    //pedidoEntity.ID_WF_PEDIDO_EQUIP = $('#ID_WF_PEDIDO_EQUIP').val();
    //pedidoEntity.CD_WF_PEDIDO_EQUIP = $('#CD_WF_PEDIDO_EQUIP').val();

    //pedidoEntity.ID_USU_SOLICITANTE = $('#ID_USU_SOLICITANTE option:selected').val();

    //pedidoEntity.CD_CLIENTE = $('#CD_CLIENTE option:selected').val();
    //pedidoEntity.DS_CONTATO_NOME = $('#DS_CONTATO_NOME').val();
    //pedidoEntity.DS_CONTATO_EMAIL = $('#TX_EMAIL').val();
    //pedidoEntity.DS_CONTATO_TEL_NUM = $('#TX_TELEFONE').val();
    //pedidoEntity.ID_USU_ULT_ATU = nidUsuario;

    //pedidoEntity.CD_MOTIVO_DEVOLUCAO = $('#MotivoDevolucao option:selected').val();
    //pedidoEntity.FL_COPIA_NF3M = $("#PossuiNotaFiscal3M").val();
    //pedidoEntity.VL_NOTA_FISCAL_3M = FormatarValorJson($('#ValorNotaFiscal3M').val());

    ////pedidoEn.DS_ARQ_ANEXO = "";

    var url = URLAPI + "WorkflowAPI/EnviarPedido";
    //var token = sessionStorage.getItem("token");
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
            //Alerta("Sucesso", JSON.parse(response.Mensagem));
            if (exibirMsg == true) {
                MensagemSucesso();
            }
            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#fileUpload').click(function () {
    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaWorkflowUploadDevolucao", function (data) {
        $('#DS_ARQUIVO').val(data.file[0]);

        Alerta("Sucesso", "Upload efetuado, favor salvar rascunho ou submeter solicitação!");
    });
    return false;
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

    var contato = $('#DS_CONTATO_NOME').val();
    if (contato == "" || contato == null || contato == undefined) {
        Alerta("Atenção", "Favor informar um contato");
        return false;
    }

    var email = $('#TX_EMAIL').val();
    if (email == "" || email == null || email == undefined) {
        Alerta("Atenção", "Favor informar um e-mail de contato");
        return false;
    }

    var tel = $('#TX_TELEFONE').val();
    if (tel == "" || tel == null || tel == undefined) {
        Alerta("Atenção", "Favor informar um telefone de contato");
        return false;
    }

    var motivo = $("#MotivoDevolucao option:selected").val();
    if (motivo == "" || motivo == null || motivo == undefined || motivo == 0) {
        Alerta("Atenção", "Favor informar o motivo da devolução");
        return false;
    }

    var nota = document.getElementsByName('PossuiNotaFiscal3M');

    if (nota[0].checked) { //Sim selecionado
        var valor = $("#ValorNotaFiscal3M").val();
        if (valor == "" || valor == null || valor == undefined || valor == 0) {
            Alerta("Atenção", "Favor informar o valor da nota fiscal");
            return false;
        }
    }

    return true;
}