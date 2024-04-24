var casasDecimais = 0;

$("#pedidoPecaAvulso_peca_CD_PECA").change(function () {
    var CD_PECA = $("#pedidoPecaAvulso_peca_CD_PECA option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_ITEM_PEDIDO = $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    OcultarCampo($('#validaCDPECA_Avulso'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Avulso'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        Alerta("Aviso", "Técnico inválido ou não informado!");
        return;
    }

    CarregarEstoquePeca(CD_TECNICO, CD_PECA, '');

    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Avulso'));
    }

});

//SL00035668
jQuery(document).ready(function () {
    $('#btnSalvarContinuarPedidoPecaAvulsoModal').hide();
});

//SL00035668
function SalvarPedidoPecaAvulsoModal() {
    OcultarCampo($('#validaIDESTOQUE_Avulso_3M1'));
    OcultarCampo($('#validaIDESTOQUE_Avulso_3M2'));

    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    var URL = URLAPI + "PedidoPecaAPI/";
    var ID_ITEM_PEDIDO = $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_PECA = $("#pedidoPecaAvulso_peca_CD_PECA option:selected").val();
    var QTD_SOLICITADA;
    if (casasDecimais > 0)
        QTD_SOLICITADA = $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").maskMoney('unmasked')[0];
    else
        QTD_SOLICITADA = $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val();

    var QTD_APROVADA;
    if (casasDecimais > 0)
        QTD_APROVADA = $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").maskMoney('unmasked')[0];
    else
        QTD_APROVADA = $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val();

    var QTD_RECEBIDA;
    if (casasDecimais > 0)
        QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").maskMoney('unmasked')[0];
    else
        QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val();

    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();
    var permiteInteragir = false;


    //if (tipoOrigemPagina == "Aprovacao") {
    var ID_ESTOQUE_3M1;
    var ID_ESTOQUE_3M2;

    //var QTD_APROVADA_3M1;
    //if (casasDecimais > 0)
    //    QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").maskMoney('unmasked')[0];
    //else
    //    QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").val();

    //var QTD_APROVADA_3M2;
    //if (casasDecimais > 0)
    //    QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").maskMoney('unmasked')[0];
    //else
    //    QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").val();

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
            QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").val();

        if (casasDecimais > 0)
            QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").val();
    }

    if (QTD_APROVADA_3M1 > 0) {
        ID_ESTOQUE_3M1 = $('#txtEstoque3M1Avulso').data("id");
    }
    if (QTD_APROVADA_3M2 > 0) {
        ID_ESTOQUE_3M2 = $('#txtEstoque3M2Avulso').data("id");
    }

    //if (QTD_RECEBIDA == '' || QTD_RECEBIDA == null) {
    //    QTD_RECEBIDA = 0;
    //}


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
    else if ((ID_STATUS_PEDIDO != statusNovoRascunho && ID_STATUS_PEDIDO != statusPendente) && (ID_ITEM_PEDIDO == "" || ID_ITEM_PEDIDO == "0" || ID_ITEM_PEDIDO == 0)) {
        Alerta("Aviso", "Só é permitido <strong>incluir Peça</strong> para Pedido com status <strong>NOVO/RASCUNHO</strong> ou <strong>PENDENTE</strong>!");
        return false;
    }
    else if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        ExibirCampo($('#validaCDPECA_Avulso'));
        return false;
    }
    else if (QTD_SOLICITADA == "" || QTD_SOLICITADA == "0" || QTD_SOLICITADA == 0) {
        ExibirCampo($('#validaQTDSOLICITADA_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) <= 0) {
        ExibirCampo($('#validaQTDSOLICITADARange_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) > 999999999) {
        ExibirCampo($('#validaQTDSOLICITADARangeMax_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) < 0) {
        ExibirCampo($('#validaQTDAPROVADARange_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) > 999999999) {
        ExibirCampo($('#validaQTDAPROVADARangeMax_Avulso'));
        return false;
    }
    //SL00036804
    else if (parseFloat(QTD_RECEBIDA) <= 0 && tipoOrigemPagina != "Aprovacao") {
        ExibirCampo($('#validaQTDRECEBIDARange_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) > 999999999) {
        ExibirCampo($('#validaQTDRECEBIDARangeMax_Avulso'));
        return false;
    }

    if (tipoOrigemPagina == "Aprovacao") {

        var QTD_ESTOQUE_3M1 = parseInt('0' + $("#pedidoPecaAvulso_QTD_ESTOQUE_3M1").val().replace('.', ''));

        var QTD_ESTOQUE_3M2 = parseInt('0' + $("#pedidoPecaAvulso_QTD_ESTOQUE_3M2").val().replace('.', ''));

        if (parseInt("0" + ID_ESTOQUE_3M1) != 0) {
            if (QTD_APROVADA_3M1 > QTD_ESTOQUE_3M1) {
                ExibirCampo($('#validaIDESTOQUE_Avulso_3M1'));
                return false;
            }
        }

        if (parseInt("0" + ID_ESTOQUE_3M2) != 0) {
            if (QTD_APROVADA_3M2 > QTD_ESTOQUE_3M2) {
                ExibirCampo($('#validaIDESTOQUE_Avulso_3M2'));
                return false;
            }
        }
    }

    if (tipoOrigemPagina == "Solicitacao") {
        ID_ESTOQUE = $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").val();
    }

    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Avulso'));
        return false;
    }

    if (ID_ITEM_PEDIDO == "" || ID_ITEM_PEDIDO == "0" || ID_ITEM_PEDIDO == 0)
        URL = URL + "Incluir";
    else
        URL = URL + "Alterar";

    //Novas verificações para Aprov. Parcial:
    var somaQtdAp = parseInt(QTD_APROVADA_3M1) + parseInt(QTD_APROVADA_3M2);

    //Chamado SL00033225
    //if (somaQtdAp > parseInt(QTD_SOLICITADA) || parseInt(QTD_APROVADA) > parseInt(QTD_SOLICITADA)) {
    //    //Alerta("Aviso", "Quantidade aprovada é maior do que a solicitada, favor ajustar!");
    //    //return false;
    //    Alerta("Aviso", "Atenção! Aprovando uma quantidade maior do que a solicitada.");
    //}

    if (QTD_RECEBIDA > QTD_APROVADA) {
        Alerta("Aviso", "Favor alertar a administração sobre as peças 'excedentes'.");
    }

    var statusItem = 1;
    //if (somaQtdAp > 0) {
    //    statusItem = 3;
    //}

    if (tipoOrigemPagina == "Confirmacao"
        && QTD_SOLICITADA > 0 && QTD_APROVADA > 0 && somaQtdAp > 0
        && (QTD_RECEBIDA < 1 || QTD_RECEBIDA == null || QTD_RECEBIDA == '' || QTD_RECEBIDA == undefined)) {
        statusItem = 2; //pendente
    }

    if (QTD_SOLICITADA == 0 || ((somaQtdAp == 0 || somaQtdAp == null) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M) && tipoOrigemPagina != "Solicitacao")) {
        statusItem = 4; //cancelado
    }
    if (QTD_APROVADA > 0 && QTD_RECEBIDA > 0) {
        statusItem = 5; //recebido
    }

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
        QTD_APROVADA_3M1: QTD_APROVADA_3M1,
        QTD_APROVADA_3M2: QTD_APROVADA_3M2,
        QTD_APROVADA: QTD_APROVADA,
        QTD_RECEBIDA: QTD_RECEBIDA,
        TX_APROVADO: "S",
        NR_DOC_ORI: null,
        ST_STATUS_ITEM: statusItem,
        DS_OBSERVACAO: $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val(),
        DS_DIR_FOTO: null,
        estoque3M1: { ID_ESTOQUE: ID_ESTOQUE_3M1 },
        estoque3M2: { ID_ESTOQUE: ID_ESTOQUE_3M2 },
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoPecaEntity),
        cache: false,
        async: false,
        beforeSend: function () {
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

            //SL00035668
            carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

            $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(0);
            $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val('');
            $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val('');
            $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val('');
            $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val('')

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });


}

//SL00035668
$('#btnSalvarPedidoPecaAvulsoModal').click(function () {
    return SalvarPedidoPecaAvulsoModal();
});

$('#btnNovaPeca').click(function () {

    //SL00035668
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    if (tipoOrigemPagina == "Solicitacao") {
        $('#btnSalvarContinuarPedidoPecaAvulsoModal').show();
    }

    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();

    if (ID_STATUS_PEDIDO != statusNovoRascunho && ID_STATUS_PEDIDO != statusPendente) {
        Alerta("Aviso", "Só é permitido adicionar <strong>Nova Peça</strong> para Pedido com status <strong>NOVO/RASCUNHO</strong> ou <strong>PENDENTE</strong>!");
        return false;
    }

    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(0);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val('')

    OcultarCampo($('#validaCDPECA_Avulso'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADA_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARange_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Avulso'));
    OcultarCampo($('#validaQTDAPROVADARange_Avulso'));
    OcultarCampo($('#validaQTDAPROVADARangeMax_Avulso'));
    OcultarCampo($('#validaQTDRECEBIDARange_Avulso'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Avulso'));

    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: true });
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    casasDecimais = 0;

    HabilitarDesabilitarQtdes();

    $('#PedidoPecaAvulsoModal').modal({
        show: true
    });
});


function EditarPeca(ID_ITEM_PEDIDO) {

    //SL00035668
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    if (tipoOrigemPagina == "Solicitacao") {
        $('#btnSalvarContinuarPedidoPecaAvulsoModal').hide();
    }

    var URL = URLAPI + "PedidoPecaAPI/Obter?ID_ITEM_PEDIDO=" + ID_ITEM_PEDIDO;

    btnSalvarContinuarPedidoPecaAvulsoModal
    $.ajax({
        type: 'GET',
        url: URL,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        cache: false,
        async: false,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.pedidoPecaModel != null) {
                LoadPedidoItem(res.pedidoPecaModel, true);
                $('#PedidoPecaAvulsoModal').modal({
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

function LoadPedidoItem(pedidoPecaModel) {
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    OcultarCampo($('#validaQTDSOLICITADA_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARange_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Avulso'));

    OcultarCampo($('#validaQTDRECEBIDA_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));

    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    if (tipoOrigemPagina == "Aprovacao") {
        OcultarCampo($('#validaIDESTOQUE_Avulso_3M1'));
        OcultarCampo($('#validaIDESTOQUE_Avulso_3M2'));

        //SL00038287
        //ST_STATUS_ITEM -> 4(cancelado); 3(Aprovado); 1(Aguardando)
        if (pedidoPecaModel.ST_STATUS_ITEM == "3" || pedidoPecaModel.ST_STATUS_ITEM == "4" ) {
            $('#btnSalvarPedidoPecaAvulsoModal').hide();
        }
        else {
            $('#btnSalvarPedidoPecaAvulsoModal').show();
        }

        carregarComboEstoque($("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE"));
        //$("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);
        //$("#pedidoPecaAvulso_QTD_ESTOQUE_3M").val(pedidoPecaModel.QT_PECA_ATUAL);

        //Estoque 3M1
        var objEstoque = $('#txtEstoque3M1Avulso');
        if (null != pedidoPecaModel.QTD_SOLICITADA && !isNaN(pedidoPecaModel.QTD_SOLICITADA)) {
            objEstoque.val(pedidoPecaModel.QTD_SOLICITADA);
        }
        if (null != pedidoPecaModel.QTD_APROVADA_3M1 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M1)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M1);
        }
        if (objEstoque.val() == '') {
            objEstoque.val(0);
        }

        //3M1 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M1'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA);
        if ($('#pedidoPecaAvulso_QTD_ESTOQUE_3M1').val() == '') {
            $('#pedidoPecaAvulso_QTD_ESTOQUE_3M1').val(0);
        }

        //Estoque 3M2
        objEstoque = $('#txtEstoque3M2Avulso');
        if (null != pedidoPecaModel.QTD_APROVADA_3M2 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M2)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M2);
        }
        if (objEstoque.val() == '') {
            objEstoque.val(0);
        }
        //3M2 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA);
        if ($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val() == '') {
            $('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val(0);
        }

        AtualizarQuantidade();

    }
    //if (parseInt(pedidoPecaModel.estoque.ID_ESTOQUE) > 0)
    //    $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);

    $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(pedidoPecaModel.ID_ITEM_PEDIDO);
    $("#pedidoPecaAvulso_peca_CD_PECA").val(pedidoPecaModel.peca.CD_PECA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val(pedidoPecaModel.QTD_SOLICITADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val(pedidoPecaModel.QTD_APROVADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val(pedidoPecaModel.QTD_RECEBIDA);
    $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val(pedidoPecaModel.DS_OBSERVACAO);

    CarregarEstoquePeca(CD_TECNICO, pedidoPecaModel.peca.CD_PECA, '');
    HabilitarDesabilitarQtdes();

    if (tipoOrigemPagina == "Aprovacao") {
        if (($('#txtEstoque3M1Avulso').val() == '' || $('#txtEstoque3M1Avulso').val() == '0') && ($('#txtEstoque3M2Avulso').val() == '' || $('#txtEstoque3M2Avulso').val() == '0')) {
            $('#txtEstoque3M1Avulso').val($("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val());
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

$("#txtEstoque3M1Avulso").change(function () {
    AtualizarQuantidade();
});
$("#txtEstoque3M2Avulso").change(function () {
    AtualizarQuantidade();
});

function AtualizarQuantidade() {
    var quantidade3M1 = 0;
    var quantidade3M2 = 0;

    if (casasDecimais > 0)
        quantidade3M1 = $("#txtEstoque3M1Avulso").maskMoney('unmasked')[0];
    else
        quantidade3M1 = $("#txtEstoque3M1Avulso").val();

    if (casasDecimais > 0)
        quantidade3M2 = $("#txtEstoque3M2Avulso").maskMoney('unmasked')[0];
    else
        quantidade3M2 = $("#txtEstoque3M2Avulso").val();

    $('#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA').val(parseInt('0' + quantidade3M1.toString()) + parseInt('0' + quantidade3M2.toString()));
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

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoPecaEntity),
        beforeSend: function () {
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

//$("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").change(function () {
//    var CD_PECA = $("#pedidoPecaAvulso_peca_CD_PECA option:selected").val();
//    var ID_ESTOQUE = $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE option:selected").val();

//    OcultarCampo($('#validaIDESTOQUE_Avulso'));
//    $("#pedidoPecaAvulso_QTD_ESTOQUE_3M").val('');

//    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
//        return;
//    }
//    else if (ID_ESTOQUE == "" || ID_ESTOQUE == "0" || ID_ESTOQUE == 0) {
//        return;
//    }

//    CarregarEstoque($("#pedidoPecaAvulso_QTD_ESTOQUE_3M"), ID_ESTOQUE, CD_PECA);
//});

$('#btnMarcarTodos').click(function (e) {
    e.preventDefault();
    $("INPUT[name*='checkLote']").prop('checked', true);
    // $("INPUT[id*='ClientesSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarTodos').click(function (e) {
    e.preventDefault();
    $("INPUT[name*='checkLote']").prop('checked', false);
    //$("INPUT[id*='ClientesSelecionados']").prop('disabled', false);
});