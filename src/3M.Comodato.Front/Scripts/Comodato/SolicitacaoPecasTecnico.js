var casasDecimais = 0;

jQuery(document).ready(function () {

    ConfigurarComponenteTela();
});

function ConfigurarComponenteTela() {

    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    if (tipoOrigemPagina == "Confirmacao") {
        OcultarCampo($("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA"));
    }
    else {
        OcultarCampo($("#pedidoPecaTecnico_pedidoPeca_QTD_ULTIMO_RECEBIMENTO"));
    }
}

$("#pedidoPecaTecnico_peca_CD_PECA").change(function () {
    var CD_PECA = $("#pedidoPecaTecnico_peca_CD_PECA option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_ITEM_PEDIDO = $("#pedidoPecaTecnico_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    OcultarCampo($('#validaid_item_pedido_Tecnico'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Tecnico'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        Alerta("Aviso", "Técnico inválido ou não informado!");
        return;
    }

    CarregarEstoquePeca(CD_TECNICO, CD_PECA, '');

    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Tecnico'));
    }
    //var token = sessionStorage.getItem("token");
    var URL = URLAPI + "PlanoZeroAPI/ObterQtSugerida?CD_TECNICO=" + CD_TECNICO + "&CD_PECA=" + CD_PECA;
    $.ajax({
        type: 'GET',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
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

            if (res.QT_SUGERIDA_PZ != null) {
                $("#pedidoPecaTecnico_QTD_SUGERIDA_PZ").val(res.QT_SUGERIDA_PZ);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");

            Alerta("ERRO", res.responseText);
        }
    });

});

$('#btnSalvarPedidoPecaTecnicoModal').click(function () {
    OcultarCampo($('#validaIDESTOQUE_Tecnico_3M1')); //estava c 3M2 igual a linha abaixo
    OcultarCampo($('#validaIDESTOQUE_Tecnico_3M2'));

    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    var URL = URLAPI + "PedidoPecaAPI/";
    var ID_ITEM_PEDIDO = $("#pedidoPecaTecnico_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_PECA = $("#pedidoPecaTecnico_peca_CD_PECA option:selected").val();
    var QTD_SOLICITADA;
    if (casasDecimais > 0)
        QTD_SOLICITADA = $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA").maskMoney('unmasked')[0];
    else
        QTD_SOLICITADA = $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA").val();

    var QTD_APROVADA;
    if (casasDecimais > 0)
        QTD_APROVADA = $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").maskMoney('unmasked')[0];
    else
        QTD_APROVADA = $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").val();


    //if (tipoOrigemPagina == "Aprovacao") {
        var ID_ESTOQUE_3M1;
        var ID_ESTOQUE_3M2;

        var QTD_APROVADA_3M1;
        if (casasDecimais > 0)
            QTD_APROVADA_3M1 = $("#txtQtdAprovada3M1").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M1 = $("#txtQtdAprovada3M1").val();


        var QTD_APROVADA_3M2;
        if (casasDecimais > 0)
            QTD_APROVADA_3M2 = $("#txtQtdAprovada3M2").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M2 = $("#txtQtdAprovada3M2").val();
    //}

    if (tipoOrigemPagina == "Aprovacao") {
        if (casasDecimais > 0)
            QTD_APROVADA_3M1 = $("#txtEstoque3M1").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M1 = $("#txtEstoque3M1").val();

        if (casasDecimais > 0)
            QTD_APROVADA_3M2 = $("#txtEstoque3M2").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M2 = $("#txtEstoque3M2").val();
    }

    if (QTD_APROVADA_3M1 > 0) {
        ID_ESTOQUE_3M1 = $('#txtEstoque3M1').data("id");
    }
    if (QTD_APROVADA_3M2 > 0) {
        ID_ESTOQUE_3M2 = $('#txtEstoque3M2').data("id");
    }

    var QTD_RECEBIDA;

    if (tipoOrigemPagina == "Confirmacao") {
        if (casasDecimais > 0)
            QTD_RECEBIDA = $("#pedidoPecaTecnico_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").maskMoney('unmasked')[0];
        else
            QTD_RECEBIDA = $("#pedidoPecaTecnico_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val();
    }
    else {

        if (casasDecimais > 0)
            QTD_RECEBIDA = $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA").maskMoney('unmasked')[0];
        else
            QTD_RECEBIDA = $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA").val();
    }

    //if (QTD_RECEBIDA == '' || QTD_RECEBIDA == null) {
    //    QTD_RECEBIDA = 0;
    //}

    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();
    var permiteInteragir = false;

    if ((ID_STATUS_PEDIDO == statusNovoRascunho || ID_STATUS_PEDIDO == statusPendente) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusSolicitado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusAprovado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
        permiteInteragir = true;
    }

    if (permiteInteragir == false) {
        Alerta("Aviso", "Não é permitido Salvar peça para o <strong>Perfil do usuário e Status atual do Pedido</strong>!");
        return false;
    }
    else if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (BloquearIncluirNovaPeca()) {
        Alerta("Aviso", ObterMensagemBloqueioIncluirNovaPeca());
        return false;
    }
    else if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        ExibirCampo($('#validaid_item_pedido_Tecnico'));
        return false;
    }
    else if ((QTD_SOLICITADA == "" || QTD_SOLICITADA == "0" || QTD_SOLICITADA == 0) && (nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira)) {
        ExibirCampo($('#validaQTDSOLICITADA_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) <= 0) {
        ExibirCampo($('#validaQTDSOLICITADARange_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) > 999999999) {
        ExibirCampo($('#validaQTDSOLICITADARangeMax_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) < 0) {
        ExibirCampo($('#validaQTDAPROVADARange_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) > 999999999) {
        ExibirCampo($('#validaQTDAPROVADARangeMax_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) <= 0 && tipoOrigemPagina == "Confirmacao") {
        ExibirCampo($('#validaQTDRECEBIDARange_Tecnico'));
        ExibirCampo($('#validaQTDULTIMORECEBIMENTORange_Tecnico'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) > 999999999) {
        ExibirCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));
        ExibirCampo($('#validaQTDULTIMORECEBIMENTORangeMax_Tecnico'));
        return false;
    }

    if (tipoOrigemPagina == "Aprovacao") {

        var QTD_ESTOQUE_3M1 = parseInt('0' + $("#pedidoPecaTecnico_QTD_ESTOQUE_3M1").val().replace('.', ''));

        var QTD_ESTOQUE_3M2 = parseInt('0' + $("#pedidoPecaTecnico_QTD_ESTOQUE_3M2").val().replace('.', ''));

        if (parseInt("0" + ID_ESTOQUE_3M1) != 0) {
            if (QTD_APROVADA_3M1 > QTD_ESTOQUE_3M1) {
                ExibirCampo($('#validaIDESTOQUE_Tecnico_3M1'));
                return false;
            }
        }

        if (parseInt("0" + ID_ESTOQUE_3M2) != 0) {
            if (QTD_APROVADA_3M2 > QTD_ESTOQUE_3M2) {
                ExibirCampo($('#validaIDESTOQUE_Tecnico_3M2'));
                return false;
            }
        }
    }

    //if (tipoOrigemPagina == "Solicitacao") {
    //    ID_ESTOQUE = $("#pedidoPecaTecnico_pedidoPeca_estoque_ID_ESTOQUE").val();
    //}

    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Tecnico'));
        return false;
    }

    if (ID_ITEM_PEDIDO == "" || ID_ITEM_PEDIDO == "0" || ID_ITEM_PEDIDO == 0)
        URL = URL + "Incluir";
    else
        URL = URL + "Alterar";

    //Novas verificações para Aprov. Parcial:
    var qtd3m1 = parseInt(QTD_APROVADA_3M1) || 0;
    var qtd3m2 = parseInt(QTD_APROVADA_3M2) || 0;
    var somaQtdAp = qtd3m1 + qtd3m2;

    if (somaQtdAp > QTD_SOLICITADA || QTD_APROVADA > QTD_SOLICITADA) {
        //Alerta("Aviso", "Quantidade aprovada é maior do que a solicitada, favor ajustar!");
        //return false;
        //Alerta("Aviso", "Atenção! Aprovando uma quantidade maior do que a solicitada.");
    } 

    if (QTD_RECEBIDA > QTD_APROVADA) {
        Alerta("Aviso", "Favor alertar a administração sobre as peças 'excedentes'.");
    }

    if (QTD_APROVADA > QTD_SOLICITADA) {
        Alerta("Aviso", "Aprovar somente a quantidade de peças igual ou superior a quantidade solicitada.");
    }

    //Inicio 
    var statusItem = statusItemNovoRascunho;

    if (tipoOrigemPagina == "Confirmacao"
        && QTD_SOLICITADA > 0 && QTD_APROVADA > 0 && somaQtdAp > 0
        && (QTD_RECEBIDA < 1 || QTD_RECEBIDA == null || QTD_RECEBIDA == '' || QTD_RECEBIDA == undefined)) {
        //statusItem = statusItemPendente;
    }

    if (QTD_SOLICITADA == 0 || ((somaQtdAp == 0 || somaQtdAp == null) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M) && tipoOrigemPagina != "Solicitacao")) {
        statusItem = statusItemCancelado;
    }

    if (QTD_APROVADA > 0 && QTD_RECEBIDA == QTD_APROVADA) {
        statusItem = statusItemAprovado; 
    }

    if (parseInt(QTD_RECEBIDA) > parseInt(QTD_APROVADA)) {
        Alerta("Aviso", "Favor alertar a administração sobre as peças 'excedentes'.");
        return false;
    }

    if (parseInt(QTD_APROVADA) > 0 && parseInt(QTD_APROVADA) > parseInt(QTD_SOLICITADA)) {
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!");
        return false;
    }

    if (parseInt(somaQtdAp) > 0 && parseInt(somaQtdAp) > parseInt(QTD_SOLICITADA)) {
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!");
        return false;
    }

    if (tipoOrigemPagina == "Confirmacao") {
        if (QTD_RECEBIDA == "" || QTD_RECEBIDA == null || QTD_RECEBIDA == undefined) {
            statusItem = statusItemAprovado;
            Alerta("Aviso", "Por favor preencha a quantidade recebida!");
            return false;
        }

    }

    if (parseInt($("#qtdRecebimentoParcial").val()) < 0) {
        Alerta("Aviso", "Quantidade recebida não pode ser menor que 0!");
        return false;
    }

    if (parseInt(QTD_RECEBIDA) > 0 && parseInt(QTD_SOLICITADA) > 0) {
        if (parseInt(QTD_RECEBIDA) < parseInt(QTD_APROVADA)) {
            statusItem = statusItemRecebidoPendencia;
        }
        if (parseInt(QTD_RECEBIDA) == parseInt(QTD_APROVADA)) {
            statusItem = statusItemRecebido;
        }
    }

    if (parseInt($("#qtdRecebimentoParcial").val()) > 0 && statusItem == statusItemRecebidoPendencia) {
        var somaRecebimento = parseInt($("#qtdRecebimentoParcial").val()) + parseInt(QTD_RECEBIDA);
        if (parseInt(somaRecebimento) > parseInt(QTD_APROVADA)) {
            Alerta("Aviso", "Quantidade recebida maior que a quantidade aprovada!");
            return false;
        }
        else {
            QTD_RECEBIDA = somaRecebimento;

        }
    }

    if ($("#qtdRecebimentoParcial").val() == "" || $("#qtdRecebimentoParcial").val() == null) {
        if (parseInt(QTD_RECEBIDA) > 0) {
            if (statusItem == statusItemRecebidoPendencia) {
                $("#qtdRecebimentoParcial").val(QTD_RECEBIDA);
            }
        }
    }

    if (QTD_APROVADA > 0 && QTD_RECEBIDA > 0 && QTD_RECEBIDA < QTD_APROVADA && statusItem != statusItemRecebidoPendencia) {
        statusItem = statusItemAprovado;
    }

    if (tipoOrigemPagina == "Aprovacao" && QTD_APROVADA > 0) {
        statusItem = statusItemAprovado;
    }
    if (tipoOrigemPagina == "Aprovacao" && somaQtdAp > 0) {
        statusItem = statusItemSolicitado;
    }
    if (tipoOrigemPagina == "Aprovacao" && somaQtdAp == 0) {
        statusItem = statusItemCancelado;
    }
    if (tipoOrigemPagina == "Solicitacao" && ID_STATUS_PEDIDO == statusSolicitado) {
        statusItem = statusItemSolicitado;
    }
    if (tipoOrigemPagina == "Solicitacao" && ID_STATUS_PEDIDO == statusNovoRascunho) {
        statusItem = statusItemNovoRascunho;
    }

    //Final

    if (tipoOrigemPagina == "Aprovacao" || tipoOrigemPagina == "Confirmacao") {
        if (QTD_APROVADA_3M1 <= 0) {
            ID_ESTOQUE_3M1 = null;
            QTD_APROVADA_3M1 = null;
        }
        if (QTD_APROVADA_3M2 <= 0) {
            ID_ESTOQUE_3M2 = null;
            QTD_APROVADA_3M2 = null;
        }
    }

    var pedidoPecaEntity = {
        ID_ITEM_PEDIDO: ID_ITEM_PEDIDO,
        pedido: {
            ID_PEDIDO: ID_PEDIDO
        },
        peca: {
            CD_PECA: CD_PECA
        },
        QTD_SOLICITADA: QTD_SOLICITADA,
        QTD_APROVADA: QTD_APROVADA,
        QTD_ULTIMO_RECEBIMENTO: QTD_RECEBIDA,
        QTD_APROVADA_3M1: QTD_APROVADA_3M1,
        QTD_APROVADA_3M2: QTD_APROVADA_3M2,

        //TX_APROVADO: null,
        TX_APROVADO: "S",
        NR_DOC_ORI: null,
        ST_STATUS_ITEM: statusItem,
        DS_OBSERVACAO: $("#pedidoPecaTecnico_pedidoPeca_DS_OBSERVACAO").val(),
        DS_DIR_FOTO: null,
        estoque3M1: { ID_ESTOQUE: ID_ESTOQUE_3M1 },
        estoque3M2: { ID_ESTOQUE: ID_ESTOQUE_3M2 },
        nidUsuarioAtualizacao: nidUsuario,
        TOKEN: ObterPrefixoTokenRegistro(nidUsuario)
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoPecaEntity),
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
            //Alerta("Aviso", MensagemGravacaoSucesso);
            MarcarLinhasEditadas([pedidoPecaEntity.peca.CD_PECA]);

            //SL00035389
            if (res.QTD_APROVADA == 0) {
                carregarPedido(ID_PEDIDO);
            }                

            carregarGridMVCPedidoItem();

            $("#pedidoPecaTecnico_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val('');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

});

$('#btnNovaPeca').click(function () {

    if (BloquearIncluirNovaPeca()) {
        Alerta("Aviso", ObterMensagemBloqueioIncluirNovaPeca());
        return false;
    }

    carregarComboPeca($("#pedidoPecaTecnico_peca_CD_PECA"));

    $("#pedidoPecaTecnico_pedidoPeca_ID_ITEM_PEDIDO").val(0);
    $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA").val('');
    $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").val('');
    $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA").val('');
    $("#pedidoPecaTecnico_pedidoPeca_DS_OBSERVACAO").val('');

    $("#pedidoPecaTecnico_DT_ULTIMA_UTILIZACAO").val('');
    $("#pedidoPecaTecnico_QTD_ESTOQUE").val('');
    $("#pedidoPecaTecnico_QTD_SUGERIDA_PZ").val('');

    OcultarCampo($('#validaid_item_pedido_Tecnico'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Tecnico'));
    OcultarCampo($('#validaQTDSOLICITADA_Tecnico'));
    OcultarCampo($('#validaQTDSOLICITADARange_Tecnico'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Tecnico'));
    OcultarCampo($('#validaQTDAPROVADARange_Tecnico'));
    OcultarCampo($('#validaQTDAPROVADARangeMax_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORangeMax_Tecnico'));

    $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: true });
    $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    casasDecimais = 0;

    HabilitarDesabilitarQtdes();

    $('#PedidoPecaTecnicoModal').modal({
        show: true
    });
});

function EditarPeca(ID_ITEM_PEDIDO) {
    var URL = URLAPI + "PedidoPecaAPI/Obter?ID_ITEM_PEDIDO=" + ID_ITEM_PEDIDO;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
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
            if (res.pedidoPecaModel != null) {
                LoadPedidoItem(res.pedidoPecaModel);
                $('#PedidoPecaTecnicoModal').modal({
                    show: true
                });
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function AtualizarQuantidade() {
    var quantidade3M1 = 0;
    var quantidade3M2 = 0;

    if (casasDecimais > 0)
        quantidade3M1 = $("#txtEstoque3M1").maskMoney('unmasked')[0];
    else
        quantidade3M1 = $("#txtEstoque3M1").val();

    if (casasDecimais > 0)
        quantidade3M2 = $("#txtEstoque3M2").maskMoney('unmasked')[0];
    else
        quantidade3M2 = $("#txtEstoque3M2").val();

    $('#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA').val(parseInt('0' + quantidade3M1.toString()) + parseInt('0' + quantidade3M2.toString()));
}

$("#txtEstoque3M1").change(function () {
    AtualizarQuantidade();
});
$("#txtEstoque3M2").change(function () {
    AtualizarQuantidade();
});

function LoadPedidoItem(pedidoPecaModel) {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    OcultarCampo($('#validaQTDSOLICITADA_Tecnico'));
    OcultarCampo($('#validaQTDSOLICITADARange_Tecnico'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Tecnico'));

    OcultarCampo($('#validaQTDRECEBIDA_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORange_Tecnico'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORangeMax_Tecnico'));

    carregarComboPeca($("#pedidoPecaTecnico_peca_CD_PECA"));

    if (tipoOrigemPagina == "Aprovacao") {
        OcultarCampo($('#validaIDESTOQUE_Tecnico_3M1'));
        OcultarCampo($('#validaIDESTOQUE_Tecnico_3M2'));

        //SL00038287
        //ST_STATUS_ITEM -> 4(cancelado); 3(Aprovado); 1(Aguardando)
        if (pedidoPecaModel.ST_STATUS_ITEM == "3" || pedidoPecaModel.ST_STATUS_ITEM == "4") {
            $('#btnSalvarPedidoPecaTecnicoModal').hide();
        }
        else {
            $('#btnSalvarPedidoPecaTecnicoModal').show();
        }

        //Estoque 3M1
        var objEstoque = $('#txtEstoque3M1');
        if (null != pedidoPecaModel.QTD_SOLICITADA && !isNaN(pedidoPecaModel.QTD_SOLICITADA)) {
            objEstoque.val(pedidoPecaModel.QTD_SOLICITADA);
        }
        if (null != pedidoPecaModel.QTD_APROVADA_3M1 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M1)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M1);
        }
        //if (objEstoque.val() == '') {
        //    objEstoque.val(0);
        //}

        //3M1 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaTecnico_QTD_ESTOQUE_3M1'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA);
        if ($('#pedidoPecaTecnico_QTD_ESTOQUE_3M1').val() == '') {
            $('#pedidoPecaTecnico_QTD_ESTOQUE_3M1').val(0);
        }

        //Estoque 3M2
        objEstoque = $('#txtEstoque3M2');
        //if (null != pedidoPecaModel.QTD_SOLICITADA && !isNaN(pedidoPecaModel.QTD_SOLICITADA)) {
        //    objEstoque.val(pedidoPecaModel.QTD_SOLICITADA);
        //}
        if (null != pedidoPecaModel.QTD_APROVADA_3M2 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M2)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M2);
        }
        if (objEstoque.val() == '') {
            objEstoque.val(0);
        }
        //3M2 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaTecnico_QTD_ESTOQUE_3M2'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA);
        if ($('#pedidoPecaTecnico_QTD_ESTOQUE_3M2').val() == '') {
            $('#pedidoPecaTecnico_QTD_ESTOQUE_3M2').val(0);
        }

        AtualizarQuantidade();
    }
    //if (parseInt(pedidoPecaModel.estoque.ID_ESTOQUE) > 0)
    //    $("#pedidoPecaTecnico_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);

    $("#pedidoPecaTecnico_pedidoPeca_ID_ITEM_PEDIDO").val(pedidoPecaModel.ID_ITEM_PEDIDO);
    $("#pedidoPecaTecnico_peca_CD_PECA").val(pedidoPecaModel.peca.CD_PECA);
    $("#pedidoPecaTecnico_pedidoPeca_QTD_SOLICITADA").val(pedidoPecaModel.QTD_SOLICITADA);
    $("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").val(pedidoPecaModel.QTD_APROVADA);
    $("#pedidoPecaTecnico_pedidoPeca_QTD_RECEBIDA").val(pedidoPecaModel.QTD_RECEBIDA);
    $("#pedidoPecaTecnico_pedidoPeca_DS_OBSERVACAO").val(pedidoPecaModel.DS_OBSERVACAO);
    $("#pedidoPecaTecnico_QTD_ESTOQUE_CLIENTE").val(pedidoPecaModel.QTD_ESTOQUE_CLIENTE);



    CarregarEstoquePeca(CD_TECNICO, pedidoPecaModel.peca.CD_PECA, pedidoPecaModel.QT_SUGERIDA_PZ);
    HabilitarDesabilitarQtdes();

    if (tipoOrigemPagina == "Aprovacao") {
        if (($('#txtEstoque3M1').val() == '' || $('#txtEstoque3M1').val() == '0') && ($('#txtEstoque3M2').val() == '' || $('#txtEstoque3M2').val() == '0')) {
            $('#txtEstoque3M1').val($("#pedidoPecaTecnico_pedidoPeca_QTD_APROVADA").val());
        }
    }


    //Inicio testes 
    objQtdAprov3M1 = $('#txtQtdAprovada3M1');

    if (null != pedidoPecaModel.QTD_APROVADA_3M1 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M1)) {
        objQtdAprov3M1.val(pedidoPecaModel.QTD_APROVADA_3M1);
    }
    if (objQtdAprov3M1.val() == '') {
        objQtdAprov3M1.val(0);
    }

    objQtdAprov3M2 = $('#txtQtdAprovada3M2');

    if (null != pedidoPecaModel.QTD_APROVADA_3M2 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M2)) {
        objQtdAprov3M2.val(pedidoPecaModel.QTD_APROVADA_3M2);
    }
    if (objQtdAprov3M2.val() == '') {
        objQtdAprov3M2.val(0);
    }

}

function ExcluirPeca(ID_ITEM_PEDIDO) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> do Item de Pedido?', 'ExcluirPecaConfirmada(' + ID_ITEM_PEDIDO + ')');
}

function ExcluirPecaConfirmada(ID_ITEM_PEDIDO) {
    var URL = URLAPI + "PedidoPecaAPI/Excluir";

    if (ID_ITEM_PEDIDO == "" || ID_ITEM_PEDIDO == "0" || ID_ITEM_PEDIDO == 0) {
        Alerta("Aviso", "Item de Pedido inválido ou não informado!");
        return;
    }

    var pedidoPecaEntity = {
        ID_ITEM_PEDIDO: ID_ITEM_PEDIDO,
        nidUsuarioAtualizacao: nidUsuario
    };
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoPecaEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            Alerta("Aviso", MensagemExclusaoSucesso);
            carregarGridMVCPedidoItem();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

$("#pedidoPecaTecnico_pedidoPeca_estoque_ID_ESTOQUE").change(function () {
    var CD_PECA = $("#pedidoPecaTecnico_peca_CD_PECA option:selected").val();
    var ID_ESTOQUE = $("#pedidoPecaTecnico_pedidoPeca_estoque_ID_ESTOQUE option:selected").val();

    OcultarCampo($('#validaIDESTOQUE_Tecnico'));
    $("#pedidoPecaTecnico_QTD_ESTOQUE_3M").val('');

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        return;
    }
    else if (ID_ESTOQUE == "" || ID_ESTOQUE == "0" || ID_ESTOQUE == 0) {
        return;
    }

    CarregarEstoque($("#pedidoPecaTecnico_QTD_ESTOQUE_3M"), ID_ESTOQUE, CD_PECA);
});

$("#cliente_CD_CLIENTE").change(function () {
    OcultarCampo($('#validaCDCLIENTE'));
});

function InformarDadosBPCS() {

    var pecas = document.querySelectorAll('[name=checkLote]:checked');
    //var values = [];
    var lote = "";
    for (var i = 0; i < pecas.length; i++) {
        //values.push(pecas[i].value);
        lote = lote + pecas[i].value + ",";
    }

    if (lote == "") {
        Alerta('Aviso', 'Não há itens selecionados!');
        return;
    }

    console.log("Lote", lote);

    var tipodePecas = $('#TP_Especial').val();
    if (tipodePecas == "Especial") {
        $('#divBruto').show();
        $('#divLiquido').show();
    }
    else {
        $('#divBruto').hide();
        $('#divLiquido').hide();
    }
    var user = $('#UsuarioAprovador').val();

    $('#ped_AP').val(user);


    $('#ReclamacaoModal').modal().show();
}

$('#btnSalvarDadosPedido').click(function () {
    var user = $('#ped_AP').val();
    var vol = $('#ped_VOL').val();
    var pes_liquido = $('#ped_PESOLQ').val();
    var pes_bruto = $('#ped_PESOBT').val();
    var resp_cli = $('#Responsavel').val();
    var telefone = $('#Telefone').val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var ramal = $("#ped_RAMAL").val();

    var pecas = document.querySelectorAll('[name=checkLote]:checked');

    var lote = "";
    for (var i = 0; i < pecas.length; i++) {
        //values.push(pecas[i].value);
        lote = lote + pecas[i].value + ",";
    }

    if (vol == "" || ramal == "") {
        if (vol == "") {
            $("#ped_VOL_Obrigatorio").show()
        } else {
            $("#ped_VOL_Obrigatorio").hide()
        }

        if (ramal == "") {
            $("#ped_RAMAL_Obrigatorio").show()
        } else {
            $("#ped_RAMAL_Obrigatorio").hide()
        }
        return;
    } else {
        $("#ped_VOL_Obrigatorio").hide()
        $("#ped_RAMAL_Obrigatorio").hide()
    }
    

    if (lote == "") {
        Alerta('Aviso', 'Não há itens selecionados!');
        return;
    }

    var dadosPedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
        VOLUME: vol,
        RAMAL: ramal,
        PESOLIQUIDO: pes_liquido,
        PESOBRUTO: pes_bruto,
        DS_TELEFONE: telefone,
        DS_APROVADOR: user,
        RESP_CLIENTE: resp_cli,
        pecasLote: lote
    };

    var URL = URLAPI + "PedidoPecaAPI/InserirInformacoesPedidoPeca";
    //var token = sessionStorage.getItem("token");
    //btnSalvarContinuarPedidoPecaAvulsoModal
    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(dadosPedidoEntity),
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
            $('#ped_PESOLQ').val("");
            $('#ped_PESOBT').val("");
            $('#ped_VOL').val("");
            window.location.reload();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

})


