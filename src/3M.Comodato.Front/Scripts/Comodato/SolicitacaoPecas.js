jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_CRIACAO_INICIO').mask('00/00/0000');
    $('#DT_CRIACAO_FIM').mask('00/00/0000');

    var pedidoCliente = false;
    var pedidoAvulso = false;

    OcultarCampo($('#validaTecnico'));

    carregarComboCliente();
    if (localStorage['SolicitacaoPecas_CD_CLIENTE'] != undefined && localStorage['SolicitacaoPecas_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['SolicitacaoPecas_CD_CLIENTE']).trigger('change');
    }
    else 
        carregarComboTecnico();

    if (localStorage['SolicitacaoPecas_CD_TECNICO'] != undefined && localStorage['SolicitacaoPecas_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['SolicitacaoPecas_CD_TECNICO']).trigger('change');
    }
    else 
        carregarComboPedido();

    if (localStorage['SolicitacaoPecas_CD_PEDIDO'] != undefined && localStorage['SolicitacaoPecas_CD_PEDIDO'] != "") {
        $('#CD_PEDIDO').val(localStorage['SolicitacaoPecas_CD_PEDIDO']).trigger('change');
    }

    if (localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO'] != undefined && localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO'] != "") {
        $("#DT_CRIACAO_INICIO").val(localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO']);
    }

    if (localStorage['SolicitacaoPecas_DT_CRIACAO_FIM'] != undefined && localStorage['SolicitacaoPecas_DT_CRIACAO_FIM'] != "") {
        $("#DT_CRIACAO_FIM").val(localStorage['SolicitacaoPecas_DT_CRIACAO_FIM']);
    }

    if (carregarFiltroStatusPedido == true) {
        carregarComboStatusPedido();
        if (localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO'] != undefined && localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO'] != "") {
            $("#statusPedido_ID_STATUS_PEDIDO").val(localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO']);
        }
    }

    carregarGridMVC();
});

$('#DT_CRIACAO-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#cliente_CD_CLIENTE").change(function () {
    carregarComboTecnico();
    carregarComboPedido();
});

$("#tecnico_CD_TECNICO").change(function () {
    OcultarCampo($('#validaTecnico'));
    carregarComboPedido();
});

$('#btnLimpar').click(function () {
    OcultarCampo($('#validaTecnico'));

    if (totalClientes > 1)
        $('#cliente_CD_CLIENTE').val(null).trigger('change');

    if (UsuarioTecnico.toLowerCase() === 'false') {
        $('#tecnico_CD_TECNICO').val(null).trigger('change');
    }

    $('#CD_PEDIDO').val(null).trigger('change');
    $('#statusPedido_ID_STATUS_PEDIDO').val(null);
    $('#DT_CRIACAO_INICIO').val(periodoINICIAL);
    $('#DT_CRIACAO_FIM').val(periodoFINAL);

    localStorage.removeItem('SolicitacaoPecas_CD_CLIENTE');
    localStorage.removeItem('SolicitacaoPecas_CD_TECNICO');
    localStorage.removeItem('SolicitacaoPecas_CD_PEDIDO');
    localStorage.removeItem('SolicitacaoPecas_DT_CRIACAO_INICIO');
    localStorage.removeItem('SolicitacaoPecas_DT_CRIACAO_FIM');
    localStorage.removeItem('SolicitacaoPecas_ID_STATUS_PEDIDO');
});

function CancelarPlanoZero() {
    ConfirmarSimNao('Aviso', 'Confirma o cancelamento do Plano Zero?', 'btnCancelarPlanoZeroConfirmada()');
}

function btnCancelarPlanoZeroConfirmada() {

    var URL = URLAPI + "PlanoZeroAPI/CancelarPlanoZero?idUsuario=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            console.log("res", res);
            if (res == "") {
                carregarGridMVC();
                Alerta("Aviso", res.STATUS);
            }
            else {
                Alerta("Aviso", "Plano Zero cancelado com sucesso!");

                setTimeout(function () {

                    location.reload();
                }, 9000);
            }
            
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

$('#btnFiltrar').click(function () {
    //var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    //if (tipoOrigemPagina == "Solicitação") {
    //    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
    //        ExibirCampo($('#validaTecnico'));
    //        return;
    //    }
    //}
    var cliente = $("#cliente_CD_CLIENTE option:selected").val();
    var tecnico = $("#tecnico_CD_TECNICO option:selected").val();

    if (cliente == '' || cliente == null || cliente == undefined) {
        cliente = 0;
    }

    if (tecnico == '' || tecnico == null || tecnico == undefined) {
        tecnico = 0;
    }

    if ((nidPerfil != perfilAdministrador3M) && cliente == 0 && tecnico == 0) {
        Alerta("Alerta", "Informe um cliente e(ou) um técnico!");
        return false;
    }

    carregarGridMVC();

    localStorage['SolicitacaoPecas_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
    localStorage['SolicitacaoPecas_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['SolicitacaoPecas_CD_PEDIDO'] = $("#CD_PEDIDO option:selected").val();
    localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO'] = $("#DT_CRIACAO_INICIO").val();
    localStorage['SolicitacaoPecas_DT_CRIACAO_FIM'] = $("#DT_CRIACAO_FIM").val();
    localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO'] = $("#statusPedido_ID_STATUS_PEDIDO").val();
});

$('#btnNovoPedidoTecnico').click(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    novoPedido(CD_TECNICO, tipoPedidoTecnico, ID_STATUS_PEDIDO);
});


$('#btnGerarTodosPedidosTecnico').click(function () {

    ConfirmarSimNao('Aviso', 'Confirma Geração de TODOS Pedidos Técnico?', 'btnGerarTodosPedidosTecnicoConfirmada()');

});

function btnGerarTodosPedidosTecnicoConfirmada() {

    var URL = URLAPI + "PedidoAPI/GerarTodosPedidosTecnico?idUsuario=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.STATUS != null) {
                carregarGridMVC();
                Alerta("Aviso", res.STATUS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}



$('#btnNovoPedidoCliente').click(function () {
    //var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    //var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();

    //if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
    //    ExibirCampo($('#validaTecnico'));
    //    return;
    //}

    //novoPedido(CD_TECNICO, tipoPedidoCliente, ID_STATUS_PEDIDO);
    pedidoCliente = true;
    pedidoAvulso = false;
    $('#PedidoEspecial').modal({
        show: true
    });
});

$('#tbnNovoPedidoAvulso').click(function () {
    pedidoCliente = false;
    pedidoAvulso = true;
    $('#PedidoEspecial').modal({
        show: true
    });
});

$('#btnSalvarTipoPedido').click(function () {
    var selecionado = $('#tipoPedidoselect').val();
    console.log("Selecionado", selecionado)
    if (selecionado == "" || selecionado == null || selecionado == undefined) {
        Alerta("ERRO", "Informe se o pedido é normal ou especial");
        return;
    }

    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }
    if (pedidoAvulso == true) {
        novoPedido(CD_TECNICO, tipoPedidoAvulso, ID_STATUS_PEDIDO, selecionado, 'W');
    }
    else if (pedidoCliente == true) {
        novoPedido(CD_TECNICO, tipoPedidoCliente, ID_STATUS_PEDIDO, selecionado, 'W');
    }     
})

function Imprimir(ID_PEDIDO) {
    var URL = URLCriptografarChave + "?Conteudo=" + ID_PEDIDO + "|0";

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
            Alerta("ERRO", res.responseText);
        }

    });
}

function carregarComboStatusPedido() {
    var URL = URLAPI + "StatusPedidoAPI/ObterLista";
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
            if (res.statusPedidos != null) {
                LoadStatusPedido(res.statusPedidos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadStatusPedido(statusPedidos) {
    LimparCombo($("#statusPedido_ID_STATUS_PEDIDO"));

    for (i = 0; i < statusPedidos.length; i++) {
        MontarCombo($("#statusPedido_ID_STATUS_PEDIDO"), statusPedidos[i].ID_STATUS_PEDIDO, statusPedidos[i].DS_STATUS_PEDIDO);
    }
}

function carregarComboPedido() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var ID_STATUS_PEDIDO = 0;
    var URL = URLAPI + "PedidoAPI/ObterLista";

    if (tipoOrigemPagina == "Aprovacao") {
        ID_STATUS_PEDIDO = statusSolicitado;
    }

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        CD_CLIENTE = 0;
    }
    //var token = sessionStorage.getItem("token");
    //Chamado #AMS|SL00034522
    //Alterada tb prcPedidoSelect - TB_PEDIDO.DT_CRIACAO >= @p_DT_CRIACAO
    var strDT_CRIACAO_INICIO = $('#DT_CRIACAO_INICIO').val();
    var parts = strDT_CRIACAO_INICIO.split('/');
    var DT_CRIACAO_INICIO = parts[2] + "-" + parts[1] + "-" + parts[0]; //Data no formato Universal


    //If Original
    //if (CD_CLIENTE != 0 || CD_TECNICO != 0 ) {
    //Abaixo o if para Chamado #AMS|SL00034522
    if (CD_CLIENTE != 0 || CD_TECNICO != 0 || CD_TECNICO == "") {


        var pedidoEntity = {
            cliente: {
                CD_CLIENTE: CD_CLIENTE,
            },
            tecnico: {
                CD_TECNICO: CD_TECNICO,
            },
            statusPedido: {
                ID_STATUS_PEDIDO: ID_STATUS_PEDIDO,
            },
            DT_CRIACAO: DT_CRIACAO_INICIO //Chamado #AMS|SL00034522 (Adicionado esse campo como filtro)

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
                if (res.pedidosModel != null) {
                    LoadPedido(res.pedidosModel);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    }
}

function LoadPedido(pedidos) {
    LimparCombo($("#CD_PEDIDO"));

    for (i = 0; i < pedidos.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
        MontarCombo($("#CD_PEDIDO"), pedidos[i].CD_PEDIDO, pedidos[i].CD_PEDIDO_Formatado);
    }
}

function carregarGridMVC() {
    //ExibirCampo($('#gridmvc'));
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var CD_PEDIDO = $("#CD_PEDIDO").val();
    var DT_CRIACAO_INICIO = $('#DT_CRIACAO_INICIO').val();
    var DT_CRIACAO_FIM = $('#DT_CRIACAO_FIM').val();
    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO").val();
    var ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA = 0;

    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0)
        CD_CLIENTE = 0;

    if (CD_PEDIDO == "" || CD_PEDIDO == "0" || CD_PEDIDO == 0 || CD_PEDIDO == null)
        CD_PEDIDO = 0;

    if (carregarFiltroStatusPedido == true) {
        if (ID_STATUS_PEDIDO == "" || ID_STATUS_PEDIDO == "0" || ID_STATUS_PEDIDO == 0 || ID_STATUS_PEDIDO == null)
            ID_STATUS_PEDIDO = 0;
    }
    else {
        if (tipoOrigemPagina == "Aprovacao") {
            ID_STATUS_PEDIDO = statusSolicitado;
        }
        else if (tipoOrigemPagina == "Confirmacao") {
            ID_STATUS_PEDIDO = statusAprovado;
            ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA = statusPendente; // statusRecebidoComPendencia;
        }
    }
    var URL = URLObterListaSolicitacao + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&CD_PEDIDO=" + CD_PEDIDO + "&ID_STATUS_PEDIDO=" + ID_STATUS_PEDIDO + "&ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA=" + ID_STATUS_PEDIDO_RECEBIDO_PENDENCIA + "&DT_CRIACAO_INICIO=" + DT_CRIACAO_INICIO + "&DT_CRIACAO_FIM=" + DT_CRIACAO_FIM;

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
                $('#gridmvc').html(res.Html);
                $('#VL_TOTAL_PECA_PEDIDOS').text('Valor Total Pedidos: ' + res.VL_TOTAL_PECA_PEDIDOS);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function ExcluirConfirmar(ID_PEDIDO, ID_STATUS_PEDIDO) {
    //if (ID_STATUS_PEDIDO == statusNovoRascunho) {
    //    ConfirmarSimNao('Aviso', 'Confirma a <strong>EXCLUSÃO</strong> do registro?', 'Excluir(' + ID_PEDIDO + ',' + ID_STATUS_PEDIDO + ')');
    //}
    //else {
    ConfirmarSimNao('Aviso', 'Confirma <strong>EXCLUSÃO/CANCELAMENTO</strong> do registro?', 'Excluir(' + ID_PEDIDO + ',' + ID_STATUS_PEDIDO + ')');
    //}
}

function Excluir(ID_PEDIDO, ID_STATUS_PEDIDO) {
    var URL = URLAPI + "PedidoAPI/Excluir";

    var pedidoEntity = {
        ID_PEDIDO: ID_PEDIDO,
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
            //if (ID_STATUS_PEDIDO == statusNovoRascunho) {
            //    Alerta("Aviso", MensagemExclusaoSucesso);
            //}
            //else {
            Alerta("Aviso", "Registro excluído/cancelado com sucesso!");
            //}
            carregarGridMVC();
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });

}

function novoPedido(CD_TECNICO, tipoPedido, ID_STATUS_PEDIDO, selecionado, origem) {
    var URL = URLCriptografarChave + "?Conteudo=" + CD_TECNICO + "|0|" + tipoPedido + "|" + tipoOrigemPagina + "|" + ID_STATUS_PEDIDO + "|" + selecionado + "|" + origem;

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
                window.location = URLSite + '/SolicitacaoPecas/Editar?idKey=' + res.idKey;
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}