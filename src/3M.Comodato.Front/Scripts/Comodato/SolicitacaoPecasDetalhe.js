jQuery(document).ready(function () {
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();

    $('.js-example-basic-single').select2();

    $('#txtEstoque3M1').ForceNumericOnly();
    $('#txtEstoque3M2').ForceNumericOnly();

    OcultarCampo($('#validaTXOBS'));

    //$('#DT_DATA_LOG_OS').mask('00/00/0000');

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    if (TP_TIPO_PEDIDO == statusTipoPedidoCliente)
        carregarComboCliente();

    carregarDados();
    carregarGRIDMensagem();
    
    carregarGridMVCPedidoItem();

    var checkboxes = document.querySelectorAll('[name=checkLote]');
    for (var i = 0; i < checkboxes.length; i++) {
        // somente nome da função, sem executar com ()
        checkboxes[i].addEventListener('click', getValues, false);
    }

    carregarGRIDNotas();
});

$('#btnGravar').click(function () {
    var URL = URLAPI + "MensagemAPI/Incluir";
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var TX_OBS = $("#TX_OBS").val();

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (TX_OBS.trim() == "") {
        ExibirCampo($('#validaTXOBS'));
        return;
    }

    var mensagemEntity = {
        pedido: {
            ID_PEDIDO: ID_PEDIDO,
        },
        usuario: {
            nidUsuario: nidUsuario,
        },
        DS_MENSAGEM: TX_OBS,
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(mensagemEntity),
        cache: false,
        async: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.ID_MENSAGEM != null) {
                //Alerta("Aviso", MensagemGravacaoSucesso);
                $("#TX_OBS").val('');
                carregarGRIDMensagem();
                carregarGRIDNotas();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$('#btnAcao').click(function () {
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var CD_CLIENTE = null;
    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();
    var estoqueAbaixoSolicitado = $("#estoqueAbaixoSolicitado").val();
    var pecaNaoEncontradaEstoque = $("#pecaNaoEncontradaEstoque").val();
    var mensagemGRID = '';

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_STATUS_PEDIDO == "" || ID_STATUS_PEDIDO == "0" || ID_STATUS_PEDIDO == 0) {
        ExibirCampo($('#validaIDSTATUSPEDIDO'));
        return;
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
        if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == null) {
            ExibirCampo($('#validaCDCLIENTE'));
            return;
        }
    }

    if (estoqueAbaixoSolicitado == 'S') {
        mensagemGRID = mensagemGRID + "<span class='text-danger font-weight-bold font-italic'>*Itens do pedido identificados em vermelho</span>&nbsp;<br/>Não há estoque interno suficiente para atender este pedido!<br/>";
    }
    if (pecaNaoEncontradaEstoque == 'S') {
        mensagemGRID = mensagemGRID + "<span class='text-primary font-weight-bold font-italic'>*Itens do pedido identificados em azul</span>&nbsp;<br/>Não encontrado estoque interno 3M! Por favor, contate o suporte.";
    }
    if (mensagemGRID != '' && ID_STATUS_PEDIDO == statusAprovado) {
        Alerta('Aviso', mensagemGRID);
        return;
    }

    ConfirmarSimNao('Aviso', 'Confirma ação no Pedido?', 'btnAcaoConfirmada()');
});

$('#btnAcaoEnvio').click(function () {
    var URL = URLAPI + "PedidoAPI/AlterarPedidoBPCS";
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var tipodePecas = $('#TP_Especial').val();
    
    var pedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
        TP_Especial: tipodePecas
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '') {
                Alerta("Aviso", res.MENSAGEM);
                //carregarPedido(res.ID_PEDIDO);
            }
            else {
                //Alerta("Aviso", MensagemGravacaoSucesso);
                //carregarPedido(res.ID_PEDIDO);
                $('#btnAcaoEnvio').hide();
                //location.reload(); 
            }

            

            location.reload();
            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseJSON.MENSAGEM);
        }
    });

    carregarGridMVCPedidoItem();
})



$('#btnImprimir').click(function () {
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }

    var NR_LOTE = $("#listaLotes option:selected").text();
    if (NR_LOTE == "Todos") {
        NR_LOTE = 0;
    }

    var URL = URLCriptografarChave + "?Conteudo=" + ID_PEDIDO + "|" + NR_LOTE;

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
            if (res.idKey != null && res.idKey != '') {
                window.open(URLSite + '/RelatorioLote.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
    //window.open(URLSite + '/RelatorioSolicitacaoPecas.aspx?idKey=' + ID_PEDIDO, '_blank');
});

function btnAcaoConfirmada() {
    var URL = URLAPI + "PedidoAPI/Alterar";
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var CD_CLIENTE = null;
    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();
    var ID_STATUS_PEDIDO_ATUAL = $("#statusPedidoAtual_DS_STATUS_PEDIDO option:selected").val();
    var QTD_ULTIMO_RECEBIMENTO = $('#qtdRecebimentoParcial').val();
    var TP_Especial = $('#TP_Especial').val();
    var Responsavel = $('#Responsavel').val();
    var Telefone = $('#Telefone').val();
    var BPCS = $('#selectBPCS').val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    console.log("Origem", tipoOrigemPagina);
    if (tipoOrigemPagina == "Aprovacao") {
        if (BPCS == null || BPCS == undefined || BPCS == "") {
            Alerta("Atenção!", "Informe se o pedido será enviado ao BPCS!!");
            return;
        }
    }
    
    //var token = sessionStorage.getItem("token");
    var FL_EMERGENCIA = 'N';
    if ($("#chkEmergencia").prop('checked')) {
        FL_EMERGENCIA = 'S';
    }

    if (ID_STATUS_PEDIDO === "3" && (Responsavel == "" || Responsavel == null || Responsavel == undefined)) {
        Alerta("Preencha o campo Responsável ou Telefone!", "O campo Responsável ou Telefone devem ser preenchidos");
        return;
    }

    if (ID_STATUS_PEDIDO === "3" && (Telefone == "" || Telefone == null || Telefone == undefined)) {
        Alerta("Preencha o campo Responsável ou Telefone!", "O campo Responsável ou Telefone devem ser preenchidos");
        return;
    }
    
    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_STATUS_PEDIDO == "" || ID_STATUS_PEDIDO == "0" || ID_STATUS_PEDIDO == 0) {
        ExibirCampo($('#validaIDSTATUSPEDIDO'));
        return;
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
        if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == null) {
            ExibirCampo($('#validaCDCLIENTE'));
            return;
        }
    }

    var pecas = document.querySelectorAll('[name=checkLote]:checked');
    //var values = [];
    var lote = "";
    for (var i = 0; i < pecas.length; i++) {
        //values.push(pecas[i].value);
        lote = lote + pecas[i].value + ",";
    }

    if (lote == "" && (ID_STATUS_PEDIDO == 3 || ID_STATUS_PEDIDO == 4 || ID_STATUS_PEDIDO == 5 || ID_STATUS_PEDIDO == 6 || ID_STATUS_PEDIDO == 7)) {
        Alerta('Aviso', 'Não há itens selecionados!');
        return;
    }

    var pedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
        tecnico: {
            CD_TECNICO: CD_TECNICO
        },
        NR_DOCUMENTO: 0,
        DT_CRIACAO: null,
        statusPedido: {
            ID_STATUS_PEDIDO: ID_STATUS_PEDIDO,
            ID_STATUS_PEDIDO_ATUAL: ID_STATUS_PEDIDO_ATUAL
        },
        TP_TIPO_PEDIDO: TP_TIPO_PEDIDO,
        FL_EMERGENCIA: FL_EMERGENCIA,
        cliente: {
            CD_CLIENTE: CD_CLIENTE
        },
        nidUsuarioAtualizacao: nidUsuario,
        pecasLote: lote,
        TP_Especial: TP_Especial,
        Responsavel: Responsavel,
        Telefone: Telefone,
        EnviaBPCS: BPCS
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '') {
                Alerta("Aviso", res.MENSAGEM);
                //carregarPedido(res.ID_PEDIDO);
            }                
            else {
                //Alerta("Aviso", MensagemGravacaoSucesso);
                //carregarPedido(res.ID_PEDIDO);
                carregarGridMVCPedidoItem();
                //location.reload(); 
            }
            carregarPedido(res.ID_PEDIDO);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

    carregarGridMVCPedidoItem();

    //setTimeout(function () {
    //    location = '';
    //    console.log("Recarrega");
    //}, 4000)
}

$("#TX_OBS").blur(function () {
    OcultarCampo($('#validaTXOBS'));
});
function btnAcaoConfirmadaBPCS() {
    var URL = URLAPI + "PedidoAPI/Alterar";
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var CD_CLIENTE = null;
    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();
    var ID_STATUS_PEDIDO_ATUAL = $("#statusPedidoAtual_DS_STATUS_PEDIDO option:selected").val();
    var QTD_ULTIMO_RECEBIMENTO = $('#qtdRecebimentoParcial').val();
    //var token = sessionStorage.getItem("token");
    var FL_EMERGENCIA = 'N';
    if ($("#chkEmergencia").prop('checked')) {
        FL_EMERGENCIA = 'S';
    }


    //var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (ID_STATUS_PEDIDO == "" || ID_STATUS_PEDIDO == "0" || ID_STATUS_PEDIDO == 0) {
        ExibirCampo($('#validaIDSTATUSPEDIDO'));
        return;
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
        if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == null) {
            ExibirCampo($('#validaCDCLIENTE'));
            return;
        }
    }

    var pecas = document.querySelectorAll('[name=checkLote]:checked');
    //var values = [];
    var lote = "";
    for (var i = 0; i < pecas.length; i++) {
        //values.push(pecas[i].value);
        lote = lote + pecas[i].value + ",";
    }

    if (lote == "" && (ID_STATUS_PEDIDO == 3 || ID_STATUS_PEDIDO == 4 || ID_STATUS_PEDIDO == 5 || ID_STATUS_PEDIDO == 6 || ID_STATUS_PEDIDO == 7)) {
        Alerta('Aviso', 'Não há itens selecionados!');
        return;
    }

    var pedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
        tecnico: {
            CD_TECNICO: CD_TECNICO
        },
        NR_DOCUMENTO: 0,
        DT_CRIACAO: null,
        statusPedido: {
            ID_STATUS_PEDIDO: ID_STATUS_PEDIDO,
            ID_STATUS_PEDIDO_ATUAL: ID_STATUS_PEDIDO_ATUAL
        },
        TP_TIPO_PEDIDO: TP_TIPO_PEDIDO,
        FL_EMERGENCIA: FL_EMERGENCIA,
        cliente: {
            CD_CLIENTE: CD_CLIENTE
        },
        nidUsuarioAtualizacao: nidUsuario,
        pecasLote: lote
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.MENSAGEM != '' || res.MENSAGEM != '') {
                Alerta("Aviso", res.MENSAGEM);
                //carregarPedido(res.ID_PEDIDO);
            }
            else {
                //Alerta("Aviso", MensagemGravacaoSucesso);
                //carregarPedido(res.ID_PEDIDO);
                carregarGridMVCPedidoItem();
                //location.reload(); 
            }
            if (ID_STATUS_PEDIDO > 2) {
                location.reload();
            } else {
                carregarPedido(res.ID_PEDIDO);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

    carregarGridMVCPedidoItem();
}

$("#TX_OBS").keypress(function () {
    OcultarCampo($('#validaTXOBS'));
});

$("#statusPedido_ID_STATUS_PEDIDO").change(function () {
    OcultarCampo($('#validaIDSTATUSPEDIDO'));
});

function carregarDados() {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        carregarPedidoINICIAR();
    } else {
        carregarPedido(ID_PEDIDO);
        //carregarGRIDMensagem();
    }
}

function carregarPedidoINICIAR() {
    var URL = URLAPI + "PedidoAPI/Incluir";
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();

    var TP_Especial = "";

    var tipo = $('#TP_Especial').val();

    if ($('#TP_Especial').val() === "Especial") {
        TP_Especial = "E";
    }
    else {
        TP_Especial = "N";
    }
    var origem = "";
    if ($('#Origem').val() === "Web") {
        origem = "W";
    } else if ($('#Origem').val() === "App") {
        origem = "A";
    }
    var Responsavel = $('#Responsavel').val();
    var Telefone = $('#Telefone').val();

    console.log("Tipo", TP_Especial)
    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var CD_CLIENTE = null;
    if (TP_TIPO_PEDIDO == statusTipoPedidoCliente)
        CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
    //var token = sessionStorage.getItem("token");
    var pedidoEntity = {
        ID_PEDIDO: 0,
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        },
        NR_DOCUMENTO: 0,
        DT_CRIACAO: new Date($.now()),
        statusPedido: {
            ID_STATUS_PEDIDO: statusNovoRascunho,
        },
        TP_TIPO_PEDIDO: TP_TIPO_PEDIDO,
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        nidUsuarioAtualizacao: nidUsuario,
        TOKEN: ObterPrefixoTokenRegistro(nidUsuario),
        TP_Especial: TP_Especial,
        Responsavel: Responsavel,
        Telefone: Telefone,
        Origem: origem
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.ID_PEDIDO != null) {
                carregarPedido(res.ID_PEDIDO);
            }

        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function carregarPedido(ID_PEDIDO) {
    var URL = URLAPI + "PedidoAPI/Obter?ID_PEDIDO=" + ID_PEDIDO;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
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
            if (res.pedidoModel != null) {
                LoadPedido(res.pedidoModel);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadPedido(pedido) {
    $("#ID_PEDIDO").val(pedido.ID_PEDIDO);
    $("#CD_PEDIDO_Formatado").val(pedido.CD_PEDIDO_Formatado);
    $("#tecnico_CD_TECNICO").val(pedido.tecnico.CD_TECNICO);
    $("#tecnico_NM_TECNICO").val(pedido.tecnico.NM_TECNICO);
    $("#tecnico_empresa_NM_Empresa").val(pedido.tecnico.empresa.NM_Empresa);
    $("#TP_TIPO_PEDIDO").val(pedido.TP_TIPO_PEDIDO);
    $("#DS_TP_TIPO_PEDIDO").val(pedido.DS_TP_TIPO_PEDIDO);

    var BPCS = pedido.EnviaBPCS;

    if (BPCS != null && BPCS != "" && BPCS != undefined) {
        $('#selectBPCS').val(BPCS);
        $('#selectBPCS').prop("disabled", true);
    }

    if (pedido.statusPedido.ID_STATUS_PEDIDO != statusNovoRascunho) {
        $("#cliente_CD_CLIENTE").prop("disabled", true);
        OcultarCampo($('#btnGravarCliente'));
    }

    if (pedido.TP_TIPO_PEDIDO == statusTipoPedidoCliente)
        $("#cliente_CD_CLIENTE").val(pedido.cliente.CD_CLIENTE);

    $("#statusPedidoAtual_ID_STATUS_PEDIDO").val(pedido.statusPedido.ID_STATUS_PEDIDO);
    $("#statusPedidoAtual_DS_STATUS_PEDIDO").val(pedido.statusPedido.DS_STATUS_PEDIDO);

    $("#DT_CRIACAO").val(pedido.DT_CRIACAO);
    $("#DT_Aprovacao").val(pedido.DT_Aprovacao);
    $("#UsuarioSolicitante").val(pedido.UsuarioSolicitante);
    $("#UsuarioAprovador").val(pedido.UsuarioAprovador);
    if ((pedido.Responsavel == null || pedido.Responsavel == "") && pedido.DS_TP_TIPO_PEDIDO == "Pedido avulso")
        pedido.Responsavel = pedido.tecnico.NM_TECNICO;
    $("#Responsavel").val(pedido.Responsavel);
    $("#Telefone").val(pedido.Telefone);
    $("#Origem").val(pedido.Origem);

    if ($("#UsuarioSolicitante").val() == null || $("#UsuarioSolicitante").val() == "" || $("#UsuarioSolicitante").val() == undefined) {
        $("#divUserPedido").hide();
    }
    else {
        $("#divUserPedido").show();
    }

    if (pedido.FL_EMERGENCIA == 'S') {
        $("#chkEmergencia").prop('checked', true);
    }
    else {
        $("#chkEmergencia").prop('checked', false);
    }

    //if (pedido.ativoFixo.CD_ATIVO_FIXO != '' && pedido.ativoFixo.CD_ATIVO_FIXO != null)
    //    $("#ativoFixo_CD_ATIVO_FIXO").prop("disabled", true);

    //var classColorStatus = "";

    //if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusNova) {
    //    classColorStatus = "col-12 alert alert-success";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusAberta) {
    //    classColorStatus = "col-12 alert alert-success";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusFinalizada) {
    //    classColorStatus = "col-12 alert alert-primary";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusConfirmada) {
    //    classColorStatus = "col-12 alert alert-primary";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPausada) {
    //    classColorStatus = "col-12 alert alert-warning";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusPendente) {
    //    classColorStatus = "col-12 alert alert-warning";
    //}
    //else if (pedido.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS == statusCancelada) {
    //    classColorStatus = "col-12 alert alert-danger";
    //}
    //else {
    //    classColorStatus = "col-12 alert alert-info";
    //}

    //$('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').removeClass();
    //$('#tpStatusVisitaOSAtual_DS_TP_STATUS_VISITA_OS').addClass(classColorStatus);

    definirFluxoStatusPedido();
    //carregarGRIDMensagem();
}

function definirFluxoStatusPedido() {
    var ID_STATUS_PEDIDO = parseInt($("#statusPedidoAtual_ID_STATUS_PEDIDO").val());
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    var statusCarregar = '';
    statusRecebidoComPendencia = 6;

    LimparCombo($("#statusPedido_ID_STATUS_PEDIDO"));

    if (ID_STATUS_PEDIDO == statusNovoRascunho && tipoOrigemPagina == "Solicitacao") {
        statusCarregar = statusSolicitado + ',' + statusCancelado;
    } else if (ID_STATUS_PEDIDO == statusSolicitado && tipoOrigemPagina == "Aprovacao") {
        if (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M) {
            statusCarregar = statusAprovado + ',' + statusCancelado; //statusPendente
        }
    } else if (ID_STATUS_PEDIDO == statusPendente && tipoOrigemPagina == "Solicitacao") {
        statusCarregar = statusSolicitado;
    } else if (ID_STATUS_PEDIDO == statusPendente && tipoOrigemPagina == "Aprovacao") {
        statusCarregar = statusAprovado + ',' + statusCancelado;
    } else if (ID_STATUS_PEDIDO == statusPendente && tipoOrigemPagina == "Confirmacao") {
        statusCarregar = statusRecebido;// + ',' + statusRecebidoComPendencia;
    } else if (ID_STATUS_PEDIDO == statusAprovado && tipoOrigemPagina == "Confirmacao") {
        statusCarregar = statusRecebido;// + ',' + statusRecebidoComPendencia;
    } else if (ID_STATUS_PEDIDO == statusRecebidoComPendencia && tipoOrigemPagina == "Confirmacao") {
        statusCarregar = statusRecebido;// + ',' + statusRecebidoComPendencia;
    } else {
        return;
    }

    var URL = URLAPI + "StatusPedidoAPI/ObterListaStatus?statusCarregar=" + statusCarregar;
    //var token = sessionStorage.getItem("token");
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
            if (res.tiposStatusPedidos != null) {
                LoadStatusPedido(res.tiposStatusPedidos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function LoadStatusPedido(tiposStatusPedidos) {
    for (i = 0; i < tiposStatusPedidos.length; i++) {
        MontarCombo($("#statusPedido_ID_STATUS_PEDIDO"), tiposStatusPedidos[i].ID_STATUS_PEDIDO, tiposStatusPedidos[i].DS_STATUS_PEDIDO_ACAO);
    }
}

function carregarGRIDMensagem() {
    //ExibirCampo($('#gridmvc'));
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        //AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLObterListaMensagem + "?ID_PEDIDO=" + ID_PEDIDO;

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
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarGRIDNotas() {
    //ExibirCampo($('#gridmvc'));
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        //AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLObterListaNotas + "?ID_PEDIDO=" + ID_PEDIDO;

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
                $('#gridmvcNotas').html(res.Html);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}


function carregarGridMVCPedidoItem(manterLinhasEditadas = true, manterScroll = true) {
    //ExibirCampo($('#gridmvc'));
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    var idStatusPedido = $('#statusPedidoAtual_ID_STATUS_PEDIDO').val();

    if (idStatusPedido == '2' || idStatusPedido == 2) {
        $('#btnNovaPeca').css("display", "none");
    }

    if (idStatusPedido == '9' || idStatusPedido == 9) {
        $('#btnAcaoEnvio').css("display", "none");
        $('#btnDadosBPCS').css("display", "none");
    }

    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        //AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        //AlertaRedirect("Aviso", "Técnico inválido ou não informado!", "window.history.back();");
        return;
    }

    var URL = URLObterListaPedidoItem + "?ID_PEDIDO=" + ID_PEDIDO + "&CD_TECNICO=" + CD_TECNICO + "&TP_TIPO_PEDIDO=" + TP_TIPO_PEDIDO + "&tipoOrigemPagina=" + tipoOrigemPagina + "&idStatusPedido=" + idStatusPedido;

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
                var linhasEditadas = GetLinhasEditadas()
                var scrollAnterior = $('#hdScrollgridPecas').scrollTop()

                $('#gridmvcPedidoItem').html(res.Html);
                $("#gridPecas").freezeHeader({ 'height': '400px' });
                $("#estoqueAbaixoSolicitado").val(res.estoqueAbaixoSolicitado);
                $("#pecaNaoEncontradaEstoque").val(res.pecaNaoEncontradaEstoque);

                if (manterLinhasEditadas) {
                    MarcarLinhasEditadas(linhasEditadas)
                }
                if (manterScroll) {
                    $('#hdScrollgridPecas').scrollTop(scrollAnterior)
                }
            }

            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

        

    });

}

function GetLinhasEditadas() {
    var linhas = $(".linha_alterada .grid-cell[data-name='peca.CD_PECA']")
    var arr = linhas.map((i, v) => {
        return $(v).text()
    })
    return arr;
}

function MarcarLinhasEditadas(codigos) {
    for (var x = 0; x <= codigos.length; x++) {
        var valor = codigos[x]
        var coluna = $(".grid-cell:contains(" + valor + ")")
        if (coluna != undefined) {
            $(coluna).parent('.grid-row').addClass('linha_alterada')
        }
    }
}


function MudarComboPeca() {
    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    var tipoPeca = $('#tipopecaSel').val();
    if (tipoPeca == "E") {
        $('#desc_peca').prop('disabled', false);
        $('#desc_peca').val("");
    } else {
        $('#desc_peca').prop('disabled', true);
        $('#desc_peca').val("");
    }
}


function carregarComboPeca(Obj) {
    var tipoPeca = "";
    if ($('#TP_Especial').val() == "Especial") {
        tipoPeca = "E";
    }
    else {
        tipoPeca = "N";
    }
    var URL = URLAPI + "PecaAPI/ObterListaAtivos";
    filtroPeca = {
        TP_PECA: tipoPeca
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(filtroPeca),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.PECA != null) {
                LoadPecas(Obj, res.PECA);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadPecas(Obj, PecaJO) {
    LimparCombo(Obj);

    var listaPecas = JSON.parse(PecaJO);

    for (i = 0; i < listaPecas.length; i++) {
        if (listaPecas[i].TP_PECA == "E") {
            listaPecas[i].DS_PECA = listaPecas[i].DS_PECA + " (Ref.:" + listaPecas[i].CD_PECA + ")";
        }
        MontarCombo(Obj, listaPecas[i].CD_PECA, listaPecas[i].DS_PECA);
    }
}

function carregarComboEstoque(Obj) {
    var URL = URLAPI + "EstoqueAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    var estoqueEntity = {
        TP_ESTOQUE_TEC_3M: '3M%',
        FL_ATIVO: 'S'
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(estoqueEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.estoques != null) {
                LoadEstoques(Obj, res.estoques);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

}

function LoadEstoques(Obj, estoques) {
    LimparCombo(Obj);

    //var estoques = JSON.parse(estoquesJO);

    for (i = 0; i < estoques.length; i++) {
        MontarCombo(Obj, estoques[i].ID_ESTOQUE, estoques[i].DS_ESTOQUE + ' - ' + estoques[i].CD_ESTOQUE);
    }
}

function CarregarEstoquePeca(CD_TECNICO, CD_PECA, QT_SUGERIDA_PZ) {
    var URL = URLAPI + "EstoquePecaAPI/ObterTecnico?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA;
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();

    if (TP_TIPO_PEDIDO == statusTipoPedidoAvulso) {
        var ObjSolicitada = $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA");
        var ObjRecebida = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA");
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        var ObjSolicitada = $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA");
        var ObjRecebida = $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA");

        var ObjDtUltimaUtilizacao = $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO");
        var ObjQtdEstoque = $("#pedidoPecaCliente_QTD_ESTOQUE");
        var ObjQtdSugeridaPZ = $("#pedidoPecaCliente_QTD_SUGERIDA_PZ");
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoTecnico) {
        var ObjSolicitada = $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA");
        var ObjRecebida = $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA");

        var ObjDtUltimaUtilizacao = $("#pedidoPecaTecnico_DT_ULTIMA_UTILIZACAO");
        var ObjQtdEstoque = $("#pedidoPecaTecnico_QTD_ESTOQUE");
        var ObjQtdSugeridaPZ = $("#pedidoPecaTecnico_QTD_SUGERIDA_PZ");
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
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

            if (res.estoquePeca != null) {
                //if (res.estoquePeca.peca.TX_UNIDADE == 'MT') {
                //    ObjSolicitada.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                //    ObjAprovada.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: true });
                //    ObjRecebida.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 3, allowZero: false });
                //}
                //else {
                    ObjSolicitada.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                    ObjAprovada.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: true });
                    ObjRecebida.maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
                //}

                if (TP_TIPO_PEDIDO == statusTipoPedidoCliente || TP_TIPO_PEDIDO == statusTipoPedidoTecnico) {
                    ObjDtUltimaUtilizacao.val(res.estoquePeca.DT_MOVIMENTACAO_AJUSTE_SAIDA);
                    ObjQtdEstoque.val(res.estoquePeca.QT_PECA_ATUAL);
                    ObjQtdSugeridaPZ.val(QT_SUGERIDA_PZ);
                }

                HabilitarDesabilitarQtdes();
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

}

function CarregarEstoque(Obj, ID_ESTOQUE, CD_PECA) {
    if (ID_ESTOQUE == null || ID_ESTOQUE == undefined) {
        ID_ESTOQUE = 0;
    }
    var URL = URLAPI + "EstoquePecaAPI/ObterQuantidadeEmEstoque?nidEstoque=" + ID_ESTOQUE + "&ccdPeca=" + CD_PECA;
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();

    var tipodePecas = $('#TP_Especial').val();
    //var token = sessionStorage.getItem("token");
    if (tipodePecas != "Especial") {
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

                if (res.QTD_PECAS_FORMATADO != null) {
                    var QTD_PECAS_FORMATADO = JSON.parse(res.QTD_PECAS_FORMATADO);
                    var tipoEstoque = Obj.attr('id').substr(Obj.attr('id').indexOf('_3', 0), 4);
                    var QTD_PECAS_APROV = parseInt('0' + $('#txtEstoque' + tipoEstoque.substr(1, 3)).val());

                    if (TP_TIPO_PEDIDO == statusTipoPedidoAvulso) {
                        Obj.val(QTD_PECAS_FORMATADO);
                        //$("#pedidoPecaAvulso_QTD_ESTOQUE_3M").val(QTD_PECAS_FORMATADO);
                        //if (QTD_PECAS_FORMATADO == "") {

                        if (QTD_PECAS_FORMATADO != "" && QTD_PECAS_FORMATADO != "0") {
                            if (QTD_PECAS_APROV > parseInt('0' + QTD_PECAS_FORMATADO.replace('.', ''))) {
                                ExibirCampo($('#validaIDESTOQUE_Avulso' + tipoEstoque));
                            }
                        }
                    }
                    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
                        //$("#pedidoPecaCliente_QTD_ESTOQUE_3M").val(QTD_PECAS_FORMATADO);
                        Obj.val(QTD_PECAS_FORMATADO);
                        //if (QTD_PECAS_FORMATADO == "") {
                        if (QTD_PECAS_FORMATADO != "" && QTD_PECAS_FORMATADO != "0") {
                            if (QTD_PECAS_APROV > parseInt('0' + QTD_PECAS_FORMATADO.replace('.', ''))) {
                                ExibirCampo($('#validaIDESTOQUE_Cliente' + tipoEstoque));
                            }
                        }
                    }
                    else if (TP_TIPO_PEDIDO == statusTipoPedidoTecnico) {
                        //$("#pedidoPecaTecnico_QTD_ESTOQUE_3M").val(QTD_PECAS_FORMATADO);
                        Obj.val(QTD_PECAS_FORMATADO);
                        if (QTD_PECAS_FORMATADO != "" && QTD_PECAS_FORMATADO != "0") {
                            if (QTD_PECAS_APROV > parseInt('0' + QTD_PECAS_FORMATADO.replace('.', ''))) {
                                ExibirCampo($('#validaIDESTOQUE_Tecnico' + tipoEstoque));
                            }
                        }
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
    

}

function HabilitarDesabilitarQtdes() {
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    if (TP_TIPO_PEDIDO == statusTipoPedidoAvulso) {
        var ObjSolicitada = $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA");
        var ObjRecebida = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA");
        var ObjPeca = $("#pedidoPecaAvulso_peca_CD_PECA");
        var ObjEstoqueDebitar = $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE");

    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        var ObjSolicitada = $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA");
        var ObjRecebida = $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA");
        var ObjPeca = $("#pedidoPecaCliente_peca_CD_PECA");

        var ObjEstoqueDebitar = $("#pedidoPecaCliente_pedidoPeca_estoque_ID_ESTOQUE");
        var ObjEstoqueDebitar3M1 = '';
        var ObjEstoqueDebitar3M2 = '';


        $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").prop("disabled", true);
        $("#pedidoPecaCliente_QTD_ESTOQUE").prop("disabled", true);
        $("#pedidoPecaCliente_QTD_SUGERIDA_PZ").prop("disabled", true);
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoTecnico) {
        var ObjSolicitada = $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA");
        var ObjAprovada = $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA");

        var ObjRecebida = $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA");
        var ObjPeca = $("#pedidoPecaTecnico_peca_CD_PECA");
        var ObjEstoqueDebitar = $("#pedidoPecaTecnico_pedidoPeca_estoque_ID_ESTOQUE");

        $("#pedidoPecaTecnico_DT_ULTIMA_UTILIZACAO").prop("disabled", true);
        $("#pedidoPecaTecnico_QTD_ESTOQUE").prop("disabled", true);
        $("#pedidoPecaTecnico_QTD_SUGERIDA_PZ").prop("disabled", true);
    }

    if ((ID_STATUS_PEDIDO == statusNovoRascunho || ID_STATUS_PEDIDO == statusPendente) && tipoOrigemPagina == "Solicitacao" && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
        ObjSolicitada.prop("disabled", false);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", true);
        ObjPeca.prop("disabled", false);
    }
    else if (ID_STATUS_PEDIDO == statusSolicitado && tipoOrigemPagina == "Solicitacao" && UsuarioTecnico.toLowerCase() === 'false') {
        ObjSolicitada.prop("disabled", false);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", true);
        ObjPeca.prop("disabled", false);
    }
    else if (ID_STATUS_PEDIDO == statusSolicitado && tipoOrigemPagina == "Aprovacao" && (nidPerfil == perfilAdministrador3M || nidPerfil == AssistênciaTecnica3M)) {
        ObjSolicitada.prop("disabled", true);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", true);
        ObjPeca.prop("disabled", true);

        if (ObjAprovada.val() == '') {
            ObjAprovada.val(ObjSolicitada.val());
        }
    }
    else if (ID_STATUS_PEDIDO == statusPendente && tipoOrigemPagina == "Confirmacao" && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica || nidPerfil == perfilCliente)) {
        ObjSolicitada.prop("disabled", true);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", false);
        ObjPeca.prop("disabled", true);
    }
    else if (ID_STATUS_PEDIDO == statusAprovado && tipoOrigemPagina == "Confirmacao" && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica || nidPerfil == perfilCliente)) {
        ObjSolicitada.prop("disabled", true);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", false);
        ObjPeca.prop("disabled", true);
    }
    else {
        ObjSolicitada.prop("disabled", true);
        ObjAprovada.prop("disabled", true);
        ObjRecebida.prop("disabled", true);
        ObjPeca.prop("disabled", true);
        ObjEstoqueDebitar.prop("disabled", true);
    }

}

function ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) {
    var URL = URLAPI + "PedidoPecaAPI/ValidaPecaDuplicidadePedido?ID_PEDIDO=" + ID_PEDIDO + "&ID_ITEM_PEDIDO=" + ID_ITEM_PEDIDO + "&CD_PECA=" + CD_PECA;
    var retornoValidacao = false;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
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

            if (res.retorno != null) {
                retornoValidacao = res.retorno;
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

    return retornoValidacao;
}

function checkAll(event) {
    $("INPUT[name*='checkLote']").prop('checked', true);
    $("a").unbind();
}

function uncheckAll(event) {
    $("INPUT[name*='checkLote']").prop('checked', false);
    $("a").unbind();
}

function getLotes() {
    var pecas = document.querySelectorAll('[name=checkLote]:checked');
    var values = [];
    for (var i = 0; i < pecas.length; i++) {
        values.push(pecas[i].value);
    }
    //return values;
    //alert(values);
}

function checkAndUncheckAll(event) {
    var pecas = document.querySelectorAll('[name=checkLote]:checked');
    if (pecas.length > 0) {
        $("INPUT[name*='checkLote']").prop('checked', false);
    } else {
        $("INPUT[name*='checkLote']").prop('checked', true);
    }        
    $("a").unbind();
}

$("#listaLotes").change(function () {
    var NR_LOTE = $("#listaLotes option:selected").text();
    var guid = $("#listaLotes option:selected").val();
    if (guid == null || guid == '') {
        //$('#lnkDownload').prop('href', '');
        $('#lnkDownload').removeAttr("style").hide();
    } else {
        $('#lnkDownload').show();
        $('#lnkDownload').prop('href', URL_DOWNLOAD + '?pastaConstante=PastaNFLoteUpload&fileName=' + guid);
    }

    if (NR_LOTE == "Todos") {
        $("DIV[id*='anexo']").prop('style', 'display:none');
    } else if (NR_LOTE >= 1) {
        $("DIV[id*='anexo']").prop('style', 'display:block');
    }
});

$('#fileUpload').click(function () {

    var NR_LOTE = $("#listaLotes option:selected").text();
    if (NR_LOTE < 1) {
        Alerta("Aviso", "Selecione um lote!");
        return false;
    }

    UploadFiles(URL_UPLOAD + "?pastaConstante=PastaNFLoteUpload", function (data) {
        $('#DS_ARQUIVO').val(data.file[0]);
        var guid = $('#DS_ARQUIVO').val();
        if (guid == "") {
            Alerta("Aviso", "Falha na operação! Favor tentar novamente.");
            return false;
        }
        //var token = sessionStorage.getItem("token");
        var URL = URLAPI + "LoteAPI/AtualizaRefNF?ID_LOTE=" + NR_LOTE + "&GUID=" + guid;
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

                $("#listaLotes option:selected").val(guid);
                Alerta("Sucesso", "Upload efetuado com sucesso!");
                location.reload(); 
            },
            error: function (res) {
                //atualizarPagina();
                $("#loader").css("display", "none");
                Alerta("ERRO", JSON.parse(res.responseText).Message);
            }

        });

        //Alerta("Sucesso", "Upload efetuado com sucesso!");
    });
    return false;
}); 

function MostrarFotoPeca(arqImgPeca) {
    //alert("Oi");
    var URL = URLAPI + "PedidoPecaAPI/getimage?ImgFileName=" + arqImgPeca;
    var win = window.open(URL, '_blank');
    win.focus();
}

function BloquearIncluirNovaPeca() {

    const ID_PEDIDO = $("#ID_PEDIDO").val();
    const semIdPedido = ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0 ? true : false;
    const ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();

    if (UsuarioTecnico.toLowerCase() === 'false') {

        return (ID_STATUS_PEDIDO != statusNovoRascunho && ID_STATUS_PEDIDO != statusPendente && ID_STATUS_PEDIDO != statusSolicitado && semIdPedido) ? true : false;
    }
    else {
        return (ID_STATUS_PEDIDO != statusNovoRascunho && ID_STATUS_PEDIDO != statusPendente && semIdPedido) ? true : false;
    }
}

function ObterMensagemBloqueioIncluirNovaPeca() {

    const textoPadrao = "Só é permitido adicionar <strong>Nova Peça</strong> para Pedido com status";

    if (UsuarioTecnico.toLowerCase() === 'false') {

        return `${textoPadrao} "<strong>NOVO/RASCUNHO</strong>, <strong>PENDENTE</strong> ou <strong>SOLICITADO</strong>!"`;
    }
    else {
        return `${textoPadrao} <strong>NOVO/RASCUNHO</strong> ou <strong>PENDENTE</strong>!`;
    }
}
