var casasDecimais = 0;

$("#pedidoPecaCliente_peca_CD_PECA").change(function () {
    var CD_PECA = $("#pedidoPecaCliente_peca_CD_PECA option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    var ID_ITEM_PEDIDO = $("#pedidoPecaCliente_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    OcultarCampo($('#validaid_item_pedido_Cliente'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Cliente'));

    if (CD_PECA == "" || CD_PECA == "0" || CD_PECA == 0) {
        return;
    }
    else if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        Alerta("Aviso", "Técnico inválido ou não informado!");
        return;
    }

    CarregarEstoquePeca(CD_TECNICO, CD_PECA, '0');
    
    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Cliente'));
    }

    if (CD_PECA != "" && CD_PECA != "0" && CD_PECA != 0) {
        if (CD_CLIENTE != "" || CD_CLIENTE != "0" || CD_CLIENTE != 0) {
            CarregarQTDPecaCli(CD_CLIENTE, CD_PECA);
        }
    }

});

function CarregarQTDPecaCli(CD_CLIENTE, CD_PECA) {
    var URL = URLAPI + "EstoquePecaAPI/ObterQtdEstoqueCliente?CD_CLIENTE=" + CD_CLIENTE + "&CD_PECA=" + CD_PECA;
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

                $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val(res.estoquePeca.QT_PECA_ATUAL);

            } else if (ExisteEstoque == false) {
                $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val('0');
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

//SL00035668
jQuery(document).ready(function () {
    $('#btnSalvarContinuarPedidoPecaClienteModal').hide();
});

$("#txtVLPeca").blur(function () {
    this.value = parseFloat(this.value).toFixed(2);
});
//SL00035668
function SalvarPedidoPecaClienteModal() {
    var tipodePecas = $('#TP_Especial').val();
    var valorPC = $('#txtVLPeca').val();
    OcultarCampo($('#validaIDESTOQUE_Cliente_3M1'));
    OcultarCampo($('#validaIDESTOQUE_Cliente_3M2'));

    var tipoOrigemPagina = $("#tipoOrigemPagina").val();

    var URL = URLAPI + "PedidoPecaAPI/";
    var ID_ITEM_PEDIDO = $("#pedidoPecaCliente_pedidoPeca_ID_ITEM_PEDIDO").val();
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var CD_PECA = $("#pedidoPecaCliente_peca_CD_PECA option:selected").val();
    var QTD_SOLICITADA;
    if (casasDecimais > 0)
        QTD_SOLICITADA = $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").maskMoney('unmasked')[0];
    else
        QTD_SOLICITADA = $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").val();

    var QTD_APROVADA;
    if (casasDecimais > 0)
        QTD_APROVADA = $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").maskMoney('unmasked')[0];
    else
        QTD_APROVADA = $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").val();

    var QTD_RECEBIDA;
    if (casasDecimais > 0)
        QTD_RECEBIDA = $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").maskMoney('unmasked')[0];
    else
        QTD_RECEBIDA = $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").val();

    var ID_STATUS_PEDIDO = $("#statusPedidoAtual_ID_STATUS_PEDIDO").val();
    var permiteInteragir = false;


    //if (tipoOrigemPagina == "Aprovacao") {
    var ID_ESTOQUE_3M1;
    var ID_ESTOQUE_3M2;

    //var QTD_APROVADA_3M1;
    //if (casasDecimais > 0)
    //    QTD_APROVADA_3M1 = $("#txtEstoqueCliente3M1").maskMoney('unmasked')[0];
    //else
    //    QTD_APROVADA_3M1 = $("#txtEstoqueCliente3M1").val();

    //var QTD_APROVADA_3M2;
    //if (casasDecimais > 0)
    //    QTD_APROVADA_3M2 = $("#txtEstoqueCliente3M2").maskMoney('unmasked')[0];
    //else
    //    QTD_APROVADA_3M2 = $("#txtEstoqueCliente3M2").val();

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
            QTD_APROVADA_3M1 = $("#txtEstoqueCliente3M1").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M1 = $("#txtEstoqueCliente3M1").val();

        if (casasDecimais > 0)
            QTD_APROVADA_3M2 = $("#txtEstoqueCliente3M2").maskMoney('unmasked')[0];
        else
            QTD_APROVADA_3M2 = $("#txtEstoqueCliente3M2").val();

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
        ID_ESTOQUE_3M1 = $('#txtEstoqueCliente3M1').data("id");
    }
    if (QTD_APROVADA_3M2 > 0) {
        ID_ESTOQUE_3M2 = $('#txtEstoqueCliente3M2').data("id");
    }


    if ((ID_STATUS_PEDIDO == statusNovoRascunho || ID_STATUS_PEDIDO == statusPendente) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica || nidPerfil == perfilCliente)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusSolicitado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M)) {
        permiteInteragir = true;
    }
    else if (ID_STATUS_PEDIDO == statusAprovado && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilTecnico3M || nidPerfil == perfilTecnicoEmpresaTerceira || nidPerfil == perfilLiderEmpresaTecnica || nidPerfil == perfilCliente)) {
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
        ExibirCampo($('#validaid_item_pedido_Cliente'));
        return false;
    }
    else if (QTD_SOLICITADA == "" || QTD_SOLICITADA == "0" || QTD_SOLICITADA == 0) {
        ExibirCampo($('#validaQTDSOLICITADA_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) <= 0) {
        ExibirCampo($('#validaQTDSOLICITADARange_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_SOLICITADA) > 999999999) {
        ExibirCampo($('#validaQTDSOLICITADARangeMax_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) < 0) {
        ExibirCampo($('#validaQTDAPROVADARange_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_APROVADA) > 999999999) {
        ExibirCampo($('#validaQTDAPROVADARangeMax_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) <= 0 && tipoOrigemPagina != "Aprovacao") {
        ExibirCampo($('#validaQTDRECEBIDARange_Cliente'));
        return false;
    }
    else if (parseFloat(QTD_RECEBIDA) > 999999999) {
        ExibirCampo($('#validaQTDRECEBIDARangeMax_Cliente'));
        return false;
    }

    //if (tipoOrigemPagina == "Aprovacao" && ID_ESTOQUE != "" && ID_ESTOQUE != "0" && ID_ESTOQUE != 0 && QTD_ESTOQUE_3M == "") {
    //    ExibirCampo($('#validaIDESTOQUE_Cliente'));
    //    return false;
    //}

    if (tipoOrigemPagina == "Aprovacao") {

        
        if (tipodePecas == "Especial") {
            var QTD_APROVADA_3M2 = parseInt($("#txtAprovaEspecial3M2Avulso").val());
            var QTD_APROVADA_3M1 = 0;
        } else {
            var QTD_ESTOQUE_3M1 = parseInt('0' + $("#pedidoPecaCliente_QTD_ESTOQUE_3M1").val().replace('.', ''));

            var QTD_ESTOQUE_3M2 = parseInt('0' + $("#pedidoPecaCliente_QTD_ESTOQUE_3M2").val().replace('.', ''));

            if (parseInt("0" + ID_ESTOQUE_3M1) != 0) {
                if (parseInt(QTD_APROVADA_3M1) > QTD_ESTOQUE_3M1) {
                    ExibirCampo($('#validaIDESTOQUE_Avulso_3M1'));
                    $('#Campo3M1').val(CD_PECA);
                    return false;
                }
            }

            if (parseInt("0" + ID_ESTOQUE_3M2) != 0) {
                if (parseInt(QTD_APROVADA_3M2) > QTD_ESTOQUE_3M2) {
                    ExibirCampo($('#validaIDESTOQUE_Avulso_3M2'));
                    $('#Campo3M2').val(CD_PECA);
                    return false;
                }
            }

        }
    }


    if (tipoOrigemPagina == "Solicitacao") {
        ID_ESTOQUE = $("#pedidoPecaCliente_pedidoPeca_estoque_ID_ESTOQUE").val();
    }

    if (ValidaPecaDuplicadaPedido(ID_PEDIDO, ID_ITEM_PEDIDO, CD_PECA) == true) {
        ExibirCampo($('#validaPecaDuplicadaPedido_Cliente'));
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

    if (parseInt(QTD_RECEBIDA) > parseInt(QTD_APROVADA)) {
        Alerta("Aviso", "Favor alertar a administração sobre as peças 'excedentes'.");
    }

    if (parseInt(QTD_APROVADA) > 0 && parseInt(QTD_APROVADA) > parseInt(QTD_SOLICITADA)) {
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!</strong>!");
        return false;
    }

    if (parseInt(somaQtdAp) > 0 && parseInt(somaQtdAp) > parseInt(QTD_SOLICITADA)) {
        Alerta("Aviso", "Não é permitido Salvar a quantidade aprovada maior que a quantidade solicitada!</strong>!");
        return false;
    }

    var statusItem = statusItemNovoRascunho;

    if (tipoOrigemPagina == "Confirmacao"
        && QTD_SOLICITADA > 0 && QTD_APROVADA > 0 && somaQtdAp > 0
        && (QTD_RECEBIDA < 1 || QTD_RECEBIDA == null || QTD_RECEBIDA == '' || QTD_RECEBIDA == undefined)) {
        statusItem = statusItemPendente;
    }

    if (QTD_SOLICITADA == 0 || ((somaQtdAp == 0 || somaQtdAp == null) && (nidPerfil == perfilAdministrador3M || nidPerfil == perfilAssistênciaTecnica3M) && tipoOrigemPagina != "Solicitacao")) {
        statusItem = statusItemCancelado; 
    }

    if (QTD_APROVADA > 0 && QTD_RECEBIDA >= QTD_APROVADA) {
        statusItem = statusItemRecebido;
    }

    if (QTD_APROVADA > 0 && QTD_RECEBIDA > 0 && QTD_RECEBIDA < QTD_APROVADA) {
        statusItem = statusItemRecebidoPendencia;
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
            CD_PECA: CD_PECA,
        },
        QTD_SOLICITADA: QTD_SOLICITADA,
        DESCRICAO_PECA: $('#desc_peca').val(),
        QTD_APROVADA: QTD_APROVADA,
        QTD_APROVADA_3M1: QTD_APROVADA_3M1,
        QTD_APROVADA_3M2: QTD_APROVADA_3M2,
        QTD_RECEBIDA: QTD_RECEBIDA,
        TX_APROVADO: null,
        NR_DOC_ORI: null,
        ST_STATUS_ITEM: statusItem,
        DS_OBSERVACAO: $("#pedidoPecaCliente_pedidoPeca_DS_OBSERVACAO").val(),
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

            //SL00035389
            if (res.QTD_APROVADA == 0) {
                carregarPedido(ID_PEDIDO);
            }

            carregarGridMVCPedidoItem();

            //SL00035668
            carregarComboPeca($("#pedidoPecaCliente_peca_CD_PECA"));

            $("#pedidoPecaCliente_pedidoPeca_ID_ITEM_PEDIDO").val(0);
            $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").val('');
            $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").val('');
            $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").val('');
            $("#pedidoPecaCliente_pedidoPeca_DS_OBSERVACAO").val('')

            //$("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val('')
            $("#pedidoPecaCliente_QTD_ESTOQUE").val('')
            $("#pedidoPecaCliente_QTD_SUGERIDA_PZ").val('')

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

//SL00035668
$('#btnSalvarPedidoPecaClienteModal').click(function () {
    return SalvarPedidoPecaClienteModal();
});

$('#btnNovaPeca').click(function () {

    //SL00035668
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    if (tipoOrigemPagina == "Solicitacao") {
        $('#btnSalvarContinuarPedidoPecaClienteModal').show();
    }
    $('#desc_peca').val("");
    $('#tipopecaSel').val("N");
    if (BloquearIncluirNovaPeca()) {
        Alerta("Aviso", ObterMensagemBloqueioIncluirNovaPeca());
        return false;
    }

    carregarComboPeca($("#pedidoPecaCliente_peca_CD_PECA"));

    $("#pedidoPecaCliente_pedidoPeca_ID_ITEM_PEDIDO").val(0);
    $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").val('');
    $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").val('');
    $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").val('');
    $("#pedidoPecaCliente_pedidoPeca_DS_OBSERVACAO").val('')

    //$("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val('')
    $("#pedidoPecaCliente_QTD_ESTOQUE").val('')
    $("#pedidoPecaCliente_QTD_SUGERIDA_PZ").val('')

    OcultarCampo($('#validaid_item_pedido_Cliente'));
    OcultarCampo($('#validaPecaDuplicadaPedido_Cliente'));
    OcultarCampo($('#validaQTDSOLICITADA_Cliente'));
    OcultarCampo($('#validaQTDSOLICITADARange_Cliente'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Cliente'));
    OcultarCampo($('#validaQTDAPROVADARange_Cliente'));
    OcultarCampo($('#validaQTDAPROVADARangeMax_Cliente'));
    OcultarCampo($('#validaQTDRECEBIDARange_Cliente'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Cliente'));

    $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: true });
    $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: false });
    casasDecimais = 0;

    HabilitarDesabilitarQtdes();

    $('#PedidoPecaClienteModal').modal({
        show: true
    })
    $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val('0');
});

function EditarPeca(ID_ITEM_PEDIDO) {

    //SL00035668
    var tipoOrigemPagina = $("#tipoOrigemPagina").val();
    if (tipoOrigemPagina == "Solicitacao") {
        $('#btnSalvarContinuarPedidoPecaClienteModal').hide();
    }

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
                $('#PedidoPecaClienteModal').modal({
                    show: true
                })
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadPedidoItem(pedidoPecaModel) {
    $('#desc_peca').prop('disabled', true);
    $('#tipopecaSel').prop('disabled', true);
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

    OcultarCampo($('#validaQTDSOLICITADA_Cliente'));
    OcultarCampo($('#validaQTDSOLICITADARange_Cliente'));
    OcultarCampo($('#validaQTDSOLICITADARangeMax_Cliente'));

    OcultarCampo($('#validaQTDRECEBIDA_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARange_Tecnico'));
    OcultarCampo($('#validaQTDRECEBIDARangeMax_Tecnico'));

    carregarComboPeca($("#pedidoPecaCliente_peca_CD_PECA"));

    
    if (tipoOrigemPagina == "Aprovacao") {
        //carregarComboEstoque($("#pedidoPecaCliente_pedidoPeca_estoque_ID_ESTOQUE"));
        //$("#pedidoPecaCliente_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);
        //$("#pedidoPecaCliente_QTD_ESTOQUE_3M").val(pedidoPecaModel.QT_PECA_ATUAL);
        OcultarCampo($('#validaIDESTOQUE_Cliente_3M1'));
        OcultarCampo($('#validaIDESTOQUE_Cliente_3M2'));

        //SL00038287
        //ST_STATUS_ITEM -> 4(cancelado); 3(Aprovado); 1(Aguardando)
        if (pedidoPecaModel.ST_STATUS_ITEM == "3" || pedidoPecaModel.ST_STATUS_ITEM == "4") {
            $('#btnSalvarPedidoPecaClienteModal').hide();
        }
        else {
            $('#btnSalvarPedidoPecaClienteModal').show();
        }

        if (pedidoPecaModel.ST_STATUS_ITEM == "5" || pedidoPecaModel.ST_STATUS_ITEM == "7") {
            $('#btnSalvarPedidoPecaClienteModal').hide();
            $("#txtEstoqueCliente3M1").prop('disabled', true);
            $("#txtEstoqueCliente3M2").prop('disabled', true);
        }
        

        //Estoque 3M1 
        var objEstoque = $('#txtEstoqueCliente3M1');
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
        CarregarEstoque($('#pedidoPecaCliente_QTD_ESTOQUE_3M1'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());
        if ($('#pedidoPecaCliente_QTD_ESTOQUE_3M1').val() == '') {
            $('#pedidoPecaCliente_QTD_ESTOQUE_3M1').val(0);
        }

        //Estoque 3M2
        objEstoque = $('#txtEstoqueCliente3M2');
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
        CarregarEstoque($('#pedidoPecaCliente_QTD_ESTOQUE_3M2'), objEstoque.data("id"), pedidoPecaModel.peca.CD_PECA.toUpperCase());
        if ($('#pedidoPecaCliente_QTD_ESTOQUE_3M2').val() == '') {
            $('#pedidoPecaCliente_QTD_ESTOQUE_3M2').val(0);
        }

        AtualizarQuantidade();
    }

    //if (parseInt(pedidoPecaModel.estoque.ID_ESTOQUE) > 0)
    //    $("#pedidoPecaCliente_pedidoPeca_estoque_ID_ESTOQUE").val(pedidoPecaModel.estoque.ID_ESTOQUE);

    $("#pedidoPecaCliente_pedidoPeca_ID_ITEM_PEDIDO").val(pedidoPecaModel.ID_ITEM_PEDIDO);
    $("#pedidoPecaCliente_peca_CD_PECA").val(pedidoPecaModel.peca.CD_PECA.toUpperCase());
    $("#pedidoPecaCliente_pedidoPeca_QTD_SOLICITADA").val(pedidoPecaModel.QTD_SOLICITADA);
    $("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").val(pedidoPecaModel.QTD_APROVADA);
    $("#pedidoPecaCliente_pedidoPeca_QTD_RECEBIDA").val(pedidoPecaModel.QTD_RECEBIDA);
    $("#pedidoPecaCliente_pedidoPeca_DS_OBSERVACAO").val(pedidoPecaModel.DS_OBSERVACAO);

    CarregarEstoquePeca(CD_TECNICO, pedidoPecaModel.peca.CD_PECA.toUpperCase(), pedidoPecaModel.QT_SUGERIDA_PZ);
    HabilitarDesabilitarQtdes();


    if (tipoOrigemPagina == "Aprovacao") {
        if (($('#txtEstoqueCliente3M1').val() == '' || $('#txtEstoqueCliente3M1').val() == '0') && ($('#txtEstoqueCliente3M2').val() == '' || $('#txtEstoqueCliente3M2').val() == '0')) {
            $('#txtEstoqueCliente3M1').val($("#pedidoPecaCliente_pedidoPeca_QTD_APROVADA").val());
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
    $("#pedidoPecaCliente_DT_ULTIMA_UTILIZACAO").val(pedidoPecaModel.QTD_ESTOQUE_CLIENTE);
}

$("#txtEstoqueCliente3M1").change(function () {
    AtualizarQuantidade();
});
$("#txtEstoqueCliente3M2").change(function () {
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
        quantidade3M1 = $("#txtEstoqueCliente3M1").maskMoney('unmasked')[0];
    else
        quantidade3M1 = $("#txtEstoqueCliente3M1").val();

    if (casasDecimais > 0)
        quantidade3M2 = $("#txtEstoqueCliente3M2").maskMoney('unmasked')[0];
    else
        quantidade3M2 = $("#txtEstoqueCliente3M2").val();

    if (tipodePecas == "Especial") {
        quantidade3M2 = parseInt($("#txtAprovaEspecial3M2Avulso").val());
        quantidade3M1 = 0;
    }

    $('#pedidoPecaCliente_pedidoPeca_QTD_APROVADA').val(parseInt('0' + quantidade3M1.toString()) + parseInt('0' + quantidade3M2.toString()));
}
function ExcluirPeca(ID_ITEM_PEDIDO) {
    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> do Item de Pedido?', 'ExcluirPecaConfirmada(' + ID_ITEM_PEDIDO + ')');
}

function ExcluirPecaConfirmada(ID_ITEM_PEDIDO) {
    var URL = URLAPI + "PedidoPecaAPI/Excluir";
    //var token = sessionStorage.getItem("token");
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

$("#cliente_CD_CLIENTE").change(function () {
    OcultarCampo($('#validaCDCLIENTE'));
});

$('#btnGravarCliente').click(function () {
    var URL = URLAPI + "PedidoAPI/Alterar";
    var ID_PEDIDO = $("#ID_PEDIDO").val();
    var TP_TIPO_PEDIDO = $("#TP_TIPO_PEDIDO").val();
    var CD_CLIENTE = null;
    //var token = sessionStorage.getItem("token");
    if (ID_PEDIDO == "" || ID_PEDIDO == "0" || ID_PEDIDO == 0) {
        AlertaRedirect("Aviso", "Pedido inválido ou não informado!", "window.history.back();");
        return;
    }
    else if (TP_TIPO_PEDIDO == statusTipoPedidoCliente) {
        CD_CLIENTE = $("#cliente_CD_CLIENTE").val();
        if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == null) {
            CD_CLIENTE = -1;
        }
    }

    var Responsavel = $('#Responsavel').val();
    var Telefone = $('#Telefone').val();
    var BPCS = $('#selectBPCS').val();

    var pedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
        cliente: {
            CD_CLIENTE: CD_CLIENTE,
        },
        nidUsuarioAtualizacao: nidUsuario,
        Responsavel: Responsavel,
        Telefone: Telefone
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
            if (res.MENSAGEM != '' || res.MENSAGEM != '')
                Alerta("Aviso", res.MENSAGEM);
            else {
                Alerta("Aviso", MensagemGravacaoSucesso);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

});

function carregarComboCliente() {
    //var URL = URLAPI + "ClienteAPI/";
    var CD_TECNICO = $("#tecnico_CD_TECNICO").val();
    //var token = sessionStorage.getItem("token");
    //if (nidPerfil == perfilCliente) {
    //    URL = URL + "ObterListaPerfilCliente?nidUsuario=" + nidUsuario;
    //}
    //else {
    //    if (nidPerfil == perfilAdministrador3M)
    //        URL = URL + "ObterListaPorTecnicoPerfil?CD_TECNICO=" + CD_TECNICO + "&SomenteAtivos=true";
    //    else
    //        URL = URL + "ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario + "&SomenteAtivos=true";
    //}

    //var URL = URLAPI + "ClienteAPI/ObterListaPorTecnicoPerfil?CD_TECNICO=" + CD_TECNICO + "&SomenteAtivos=true";
    var URL = URLAPI + "TecnicoXClienteAPI/ObterLista";

    var tecnicoClienteEntity = {
        tecnico: {
            CD_TECNICO: CD_TECNICO,
        }
    };

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(tecnicoClienteEntity),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            //if (res.clientes != null) {
            //    LoadClientes(res.clientes);
            //}
            if (res.tecnicos != null) {
                LoadClientes(res.tecnicos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadClientes(tecnicos) {
    //if (nidPerfil == perfilCliente) {
    //    if (clientes.length == 0)
    //        LimparCombo($("#cliente_CD_CLIENTE"));
    //    else
    //        $("#cliente_CD_CLIENTE").empty();
    //}
    //else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < tecnicos.length; i++) {
        //var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), tecnicos[i].cliente.CD_CLIENTE, tecnicos[i].cliente.NM_CLIENTE);
    }

    totalClientes = tecnicos.length;
}


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
    var tipodePecas = $('#TP_Especial').val();
    //var token = sessionStorage.getItem("token");
    console.log("Tipo de Peça", tipodePecas)
    if (tipodePecas == "Especial") {
        $('#divBruto').show();
        $('#divLiquido').show();
    }
    else {
        $('#divBruto').hide();
        $('#divLiquido').hide();
    }
    console.log("Lote", lote);

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
