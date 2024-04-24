var casasDecimais = 0;

$("#pedidoPecaAvulso_peca_CD_PECA").change(function () {
    var CD_PECA = $("#pedidoPecaAvulso_peca_CD_PECA option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_ITEM_PEDIDO = $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    OcultarCampo($('#validaid_item_pedido_Avulso'));
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

    ConfigurarComponenteTela();
});

function ConfigurarComponenteTela() {

    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    if (tipoOrigemPagina == "Confirmacao") {
        OcultarCampo($("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA"));
    }
    else {
        OcultarCampo($("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO"));
    }
}


//SL00035668
function SalvarPedidoPecaAvulsoModal() {
    var tipodePecas = $('#TP_Especial').val();

    var valorPC = $('#txtVLPeca').val();

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

    if (tipoOrigemPagina == "Confirmacao") {
        if (casasDecimais > 0)
            QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").maskMoney('unmasked')[0];
        else
            QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val();
    }
    else {
        if (casasDecimais > 0)
            QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").maskMoney('unmasked')[0];
        else
            QTD_RECEBIDA = $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val();
    }

    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();
    var permiteInteragir = false;
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



    if (tipoOrigemPagina == "Aprovacao") {

        if (casasDecimais > 0)
            QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M1 = $("#txtEstoque3M1Avulso").val();

        if (casasDecimais > 0)
            QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M2 = $("#txtEstoque3M2Avulso").val();

        
        if (tipodePecas == "Especial") {
            QTD_APROVADA_3M2 = parseInt($("#txtAprovaEspecial3M2Avulso").val());
            QTD_APROVADA_3M1 = 0;
        }

    }

    if (tipoOrigemPagina == "Aprovacao") {


        if (tipodePecas == "Especial") {
            if (valorPC == "" || valorPC == undefined || valorPC == null) {
                Alerta("Atenção", "Para peças especiais o valor da peça é obrigatório!")
                return;
            }
        }
    }

    if (QTD_APROVADA_3M1 > 0) {
        ID_ESTOQUE_3M1 = $('#txtEstoque3M1Avulso').data("id");
    }
    if (QTD_APROVADA_3M2 > 0) {
        ID_ESTOQUE_3M2 = $('#txtEstoque3M2Avulso').data("id");
    }

    if ((ID_STATUS_PEDIDO == statusNovoRascunho || ID_STATUS_PEDIDO == statusPendente) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusSolicitado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusAprovado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusRecebidoComPendencia && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica)) {
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
        ExibirCampo($('#validaid_item_pedido_Avulso'));
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
        ExibirCampo($('#validaQTDULTIMORECEBIMENTORange_Avulso'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) > 999999999) {
        ExibirCampo($('#validaQTDRECEBIDARangeMax_Avulso'));
        ExibirCampo($('#validaQTDULTIMORECEBIMENTORangeMax_Avulso'));
        return false;
    }

    if (tipoOrigemPagina == "Aprovacao") {
        
        if (tipodePecas == "Especial") {
            var QTD_APROVADA_3M2 = parseInt($("#txtAprovaEspecial3M2Avulso").val());
            var QTD_APROVADA_3M1 = 0;
        }
        else {
            var QTD_ESTOQUE_3M1 = parseInt('0' + $("#pedidoPecaAvulso_QTD_ESTOQUE_3M1").val().replace('.', ''));

            var QTD_ESTOQUE_3M2 = parseInt('0' + $("#pedidoPecaAvulso_QTD_ESTOQUE_3M2").val().replace('.', ''));

            if (parseInt("0" + ID_ESTOQUE_3M1) != 0) {
                if (QTD_APROVADA_3M1 > QTD_ESTOQUE_3M1) {
                    ExibirCampo($('#validaIDESTOQUE_Avulso_3M1'));
                    $('#Campo3M1').val(CD_PECA);
                    return false;
                }
            }

            if (parseInt("0" + ID_ESTOQUE_3M2) != 0) {
                if (QTD_APROVADA_3M2 > QTD_ESTOQUE_3M2) {
                    ExibirCampo($('#validaIDESTOQUE_Avulso_3M2'));
                    $('#Campo3M2').val(CD_PECA);
                    return false;
                }
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

    //if (QTD_RECEBIDA > QTD_APROVADA) {
    //    Alerta("Aviso", "Favor alertar a administração sobre as peças 'excedentes'.");
    //}

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
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!</strong>!");
        return false;
    }

    if (parseInt(somaQtdAp) > 0 && parseInt(somaQtdAp) > parseInt(QTD_SOLICITADA)) {
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!</strong>!");
        return false;
    }

    if (tipoOrigemPagina == "Confirmacao") {
        if (QTD_RECEBIDA == "" || QTD_RECEBIDA == null || QTD_RECEBIDA == undefined) {
            statusItem = statusItemAprovado;
            Alerta("Aviso", "Por favor preencha a quantidade recebida!</strong>!");
            return false;
        }

    }

    if (parseInt($("#qtdRecebimentoParcial").val()) < 0) {
        Alerta("Aviso", "Quantidade recebida não pode ser menor que 0!</strong>!");
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
            Alerta("Aviso", "Quantidade recebida maior que a quantidade aprovada!</strong>!");
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
    if (tipoOrigemPagina == "Solicitacao" && ID_STATUS_PEDIDO == statusSolicitado) {
        statusItem = statusItemSolicitado;
    }
    if (tipoOrigemPagina == "Solicitacao" && ID_STATUS_PEDIDO == statusNovoRascunho) {
        statusItem = statusItemNovoRascunho;
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
            ID_PEDIDO: ID_PEDIDO,
            TP_Especial: tipodePecas
        },
        peca: {
            CD_PECA: CD_PECA
        },
        QTD_SOLICITADA: QTD_SOLICITADA,
        DESCRICAO_PECA: $('#desc_peca').val(),
        QTD_APROVADA_3M1: QTD_APROVADA_3M1,
        QTD_APROVADA_3M2: QTD_APROVADA_3M2,
        QTD_APROVADA: QTD_APROVADA,
        QTD_RECEBIDA: QTD_RECEBIDA,
        QTD_ULTIMO_RECEBIMENTO: $("#qtdRecebimentoParcial").val(),
        TX_APROVADO: "S",
        NR_DOC_ORI: null,
        ST_STATUS_ITEM: statusItem,
        DS_OBSERVACAO: $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val(),
        DS_DIR_FOTO: null,
        estoque3M1: { ID_ESTOQUE: ID_ESTOQUE_3M1 },
        estoque3M2: { ID_ESTOQUE: ID_ESTOQUE_3M2 },
        VL_PECA: valorPC,
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
            $("#qtdRecebimentoParcial").val(0);
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
            $("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val('');
            $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val('')
            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });


}

$("#txtVLPeca").blur(function () {
    this.value = parseFloat(this.value).toFixed(2);
});

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
    $('#desc_peca').val("");
    $('#tipopecaSel').val("N");

    if (BloquearIncluirNovaPeca()) {
        Alerta("Aviso", ObterMensagemBloqueioIncluirNovaPeca());
        return false;
    }

    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(0);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val('');
    $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val('')

    OcultarCampo($('#validaid_item_pedido_Avulso'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADA_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARange_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Avulso'));
    OcultarCampo($('#validaQTDAPROVADARange_Avulso'));
    OcultarCampo($('#validaQTDAPROVADARangeMax_Avulso'));
    OcultarCampo($('#validaQTDRECEBIDARange_Avulso'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORange_Avulso'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Avulso'));
    OcultarCampo($('#validaQTDULTIMORECEBIMENTORangeMax_Avulso'));

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
    //var token = sessionStorage.getItem("token");
    //btnSalvarContinuarPedidoPecaAvulsoModal
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
                LoadPedidoItem(res.pedidoPecaModel, true);
                $('#PedidoPecaAvulsoModal').modal().show();
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

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

$('#btnSalvarDadosPedido').click(function (){
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

function mask(o, f) {
    setTimeout(function () {
        var v = mphone(o.value);
        if (v != o.value) {
            o.value = v;
        }
    }, 1);
}

function mphone(v) {
    var r = v.replace(/\D/g, "");
    r = r.replace(/^0/, "");
    if (r.length > 10) {
        r = r.replace(/^(\d\d)(\d{5})(\d{4}).*/, "($1) $2-$3");
    } else if (r.length > 5) {
        r = r.replace(/^(\d\d)(\d{4})(\d{0,4}).*/, "($1) $2-$3");
    } else if (r.length > 2) {
        r = r.replace(/^(\d\d)(\d{0,5})/, "($1) $2");
    } else {
        r = r.replace(/^(\d*)/, "($1");
    }
    return r;
}

function LoadPedidoItemBPCS(pedidoPecaModel) {
    console.log("Item", pedidoPecaModel)
    $('#desc_peca').prop('disabled', true);
    $('#tipopecaSel').prop('disabled', true);
    console.log("Teste Exibição", pedidoPecaModel.DESCRICAO_PECA)
    $('#desc_peca').val(pedidoPecaModel.DESCRICAO_PECA);
    var descPed = $('#desc_peca').val();
    if (descPed != "" && descPed != undefined && descPed != null) {
        $('#tipopecaSel').val('E');
    }
    else {
        $('#tipopecaSel').val('N');
    }

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    OcultarCampo($('#validaQTDSOLICITADA_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARange_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Avulso'));

    OcultarCampo($('#validaQTDRECEBIDA_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));

    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    if (parseInt(pedidoPecaModel.ST_STATUS_ITEM) == 7) {
        $("#DivqtdRecebimentoParcial").css("display", "block");
        $('#btnSalvarPedidoPecaAvulsoModal').show();
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').val(pedidoPecaModel.QTD_RECEBIDA);
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').prop('disabled', true);
    }


    if (tipoOrigemPagina == "Aprovacao") {
        OcultarCampo($('#validaIDESTOQUE_Avulso_3M1'));
        OcultarCampo($('#validaIDESTOQUE_Avulso_3M2'));

        //SL00038287
        //ST_STATUS_ITEM -> 4(cancelado); 3(Aprovado); 1(Aguardando)
        if (pedidoPecaModel.ST_STATUS_ITEM == "3" || pedidoPecaModel.ST_STATUS_ITEM == "4") {
            $('#btnSalvarPedidoPecaAvulsoModal').hide();
        }
        else {
            $('#btnSalvarPedidoPecaAvulsoModal').show();
        }

        //carregarComboEstoque($("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE"));
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
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M1'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());


        //Estoque 3M2
        objEstoque = $('#txtEstoque3M2Avulso');
        if (null != pedidoPecaModel.QTD_APROVADA_3M2 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M2)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M2);
        }
        if (objEstoque.val() == '') {
            objEstoque.val(0);
        }
        //3M2 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());

        if ($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val() == '') {
            $('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val(0);
        }

        AtualizarQuantidade();

    }
    //if (parseInt(pedidoPecaModel.estoque.ID_ESTOQUE) > 0)
    //    $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);

    $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(pedidoPecaModel.ID_ITEM_PEDIDO);
    $("#pedidoPecaAvulso_peca_CD_PECA").val(pedidoPecaModel.peca.CD_PECA.toUpperCase());
    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val(pedidoPecaModel.QTD_SOLICITADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val(pedidoPecaModel.QTD_APROVADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val(pedidoPecaModel.QTD_RECEBIDA);
    $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val(pedidoPecaModel.DS_OBSERVACAO);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val(pedidoPecaModel.QTD_RECEBIDA);
    CarregarEstoquePeca(CD_TECNICO, pedidoPecaModel.peca.CD_PECA.toUpperCase(), '');
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

    if (pedidoPecaModel.ST_STATUS_ITEM == '5') {
        $('#btnSalvarPedidoPecaAvulsoModal').hide();
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').val(pedidoPecaModel.QTD_RECEBIDA);
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').prop('disabled', true);
    }
}

function LoadPedidoItem(pedidoPecaModel) {
    console.log("Item", pedidoPecaModel)
    $('#desc_peca').prop('disabled', true);
    $('#tipopecaSel').prop('disabled', true);
    console.log("Teste Exibição", pedidoPecaModel.DESCRICAO_PECA)
    $('#desc_peca').val(pedidoPecaModel.DESCRICAO_PECA);
    var descPed = $('#desc_peca').val();
    if (descPed != "" && descPed != undefined && descPed != null) {
        $('#tipopecaSel').val('E');
    }
    else {
        $('#tipopecaSel').val('N');
    }

    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    OcultarCampo($('#validaQTDSOLICITADA_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARange_Avulso'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Avulso'));

    OcultarCampo($('#validaQTDRECEBIDA_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));

    carregarComboPeca($("#pedidoPecaAvulso_peca_CD_PECA"));

    if (parseInt(pedidoPecaModel.ST_STATUS_ITEM) == 7) {
        $("#DivqtdRecebimentoParcial").css("display", "block");
        $('#btnSalvarPedidoPecaAvulsoModal').show();
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').val(pedidoPecaModel.QTD_RECEBIDA);
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').prop('disabled', true);
    }
    

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

        //carregarComboEstoque($("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE"));
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
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M1'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());

        
        //Estoque 3M2
        objEstoque = $('#txtEstoque3M2Avulso');
        if (null != pedidoPecaModel.QTD_APROVADA_3M2 && !isNaN(pedidoPecaModel.QTD_APROVADA_3M2)) {
            objEstoque.val(pedidoPecaModel.QTD_APROVADA_3M2);
        }
        if (objEstoque.val() == '') {
            objEstoque.val(0);
        }
        //3M2 - Quandidade disponível
        CarregarEstoque($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());

        if ($('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val() == '') {
            $('#pedidoPecaAvulso_QTD_ESTOQUE_3M2').val(0);
        }

        AtualizarQuantidade();

    }
    //if (parseInt(pedidoPecaModel.estoque.ID_ESTOQUE) > 0)
    //    $("#pedidoPecaAvulso_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);

    $("#pedidoPecaAvulso_pedidoPeca_ID_ITEM_PEDIDO").val(pedidoPecaModel.ID_ITEM_PEDIDO);
    $("#pedidoPecaAvulso_peca_CD_PECA").val(pedidoPecaModel.peca.CD_PECA.toUpperCase());
    $("#pedidoPecaAvulso_pedidoPeca_QTD_SOLICITADA").val(pedidoPecaModel.QTD_SOLICITADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_APROVADA").val(pedidoPecaModel.QTD_APROVADA);
    $("#pedidoPecaAvulso_pedidoPeca_QTD_RECEBIDA").val(pedidoPecaModel.QTD_RECEBIDA);
    $("#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO").val(pedidoPecaModel.DS_OBSERVACAO); 
    $("#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO").val(pedidoPecaModel.QTD_RECEBIDA);
    CarregarEstoquePeca(CD_TECNICO, pedidoPecaModel.peca.CD_PECA.toUpperCase(), '');
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

    if (pedidoPecaModel.ST_STATUS_ITEM == '5') {
        $('#btnSalvarPedidoPecaAvulsoModal').hide();
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').val(pedidoPecaModel.QTD_RECEBIDA);
        $('#pedidoPecaAvulso_pedidoPeca_QTD_ULTIMO_RECEBIMENTO').prop('disabled', true);
    }

    if (tipoOrigemPagina == "Solicitacao" && nidPerfil == perfilTecnicoEmpresaTerceira) {
        $('#pedidoPecaAvulso_pedidoPeca_DS_OBSERVACAO').prop('disabled', true);
        $('#btnSalvarPedidoPecaAvulsoModal').hide();
    }

    console.log("Pagina: ", tipoOrigemPagina + "//" + "Perfil: ", nidPerfil + "//" + "Perfil Tecnico: ", perfilTecnicoEmpresaTerceira);

}

$("#txtEstoque3M1Avulso").change(function () {
    AtualizarQuantidade();
});
$("#txtEstoque3M2Avulso").change(function () {
    AtualizarQuantidade();
});
$("#txtAprovaEspecial3M2Avulso").change(function () {
    AtualizarQuantidade();
});

function AtualizarQuantidade() {
    var quantidade3M1 = 0;
    var quantidade3M2 = 0;
    var tipodePecas = $('#TP_Especial').val();
    

    if (casasDecimais > 0)
        quantidade3M1 = $("#txtEstoque3M1Avulso").maskMoney('unmasked')[0];
    else if ($("#txtEstoque3M1Avulso").val() != null && $("#txtEstoque3M1Avulso").val() != undefined && $("#txtEstoque3M1Avulso").val() != null)
        quantidade3M1 = $("#txtEstoque3M1Avulso").val();
    else
        quantidade3M1 = 0;

    if (casasDecimais > 0)
        quantidade3M2 = $("#txtEstoque3M2Avulso").maskMoney('unmasked')[0];
    else if ($("#txtEstoque3M2Avulso").val() != null && $("#txtEstoque3M2Avulso").val() != undefined && $("#txtEstoque3M2Avulso").val() != null)
        quantidade3M2 = $("#txtEstoque3M2Avulso").val();
    else
        quantidade3M2 = 0;

    if (tipodePecas == "Especial") {
        quantidade3M2 = parseInt($("#txtAprovaEspecial3M2Avulso").val());
        quantidade3M1 = 0;
    }

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

function CalcularRestante() {
    var ID_PEDIDO = $("#ID_PEDIDO").val();

    var URL = URLAPI + "PedidoAPI/CalcularValorPedidoEntity?CD_PEDIDO=" + ID_PEDIDO;

    $.ajax({
        url: URL,
        processData: true,
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
            $("#VL_TOTAL_PECA_Restante").val(res.Valor);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

    
}
