jQuery(document).ready(function () {
    var recarregarGrid = false;
    $('.js-example-basic-single').select2();

    $('#DT_CRIACAO_INICIO').mask('00/00/0000');
    $('#DT_CRIACAO_FIM').mask('00/00/0000');

    OcultarCampo($('#validaTecnico'));

    carregarComboCliente();
    if (localStorage['SolicitacaoPecas_CD_CLIENTE'] != undefined && localStorage['SolicitacaoPecas_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['SolicitacaoPecas_CD_CLIENTE']).trigger('change');
        recarregarGrid = true;
    }
    else 
        carregarComboTecnico();

    if (localStorage['SolicitacaoPecas_CD_TECNICO'] != undefined && localStorage['SolicitacaoPecas_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['SolicitacaoPecas_CD_TECNICO']).trigger('change');
        recarregarGrid = true;
    }
    else 
        carregarComboPedido();

    if (localStorage['SolicitacaoPecas_CD_PEDIDO'] != undefined && localStorage['SolicitacaoPecas_CD_PEDIDO'] != "") {
        $('#CD_PEDIDO').val(localStorage['SolicitacaoPecas_CD_PEDIDO']).trigger('change');
        recarregarGrid = true;
    }

    if (localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO'] != undefined && localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO'] != "") {
        $("#DT_CRIACAO_INICIO").val(localStorage['SolicitacaoPecas_DT_CRIACAO_INICIO']);
        recarregarGrid = true;
    }

    if (localStorage['SolicitacaoPecas_DT_CRIACAO_FIM'] != undefined && localStorage['SolicitacaoPecas_DT_CRIACAO_FIM'] != "") {
        $("#DT_CRIACAO_FIM").val(localStorage['SolicitacaoPecas_DT_CRIACAO_FIM']);
        recarregarGrid = true;
    }

    if (carregarFiltroStatusPedido == true) {
        carregarComboStatusPedido();
        if (localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO'] != undefined && localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO'] != "") {
            $("#statusPedido_ID_STATUS_PEDIDO").val(localStorage['SolicitacaoPecas_ID_STATUS_PEDIDO']);
            recarregarGrid = true;
        }
    }

    //if (tipoOrigemPagina == "Aprovação" || tipoOrigemPagina == "Confirmação")
    //    carregarGridMVC();
    //else if (recarregarGrid == true)
    //    carregarGridMVC();

    if (nidPerfil == perfilAdministrador3M) {
        carregarGridMVC();
    }

    //SL00036007
    //if (nidPerfil == perfilTecnico3M) {
    //    $("#btnNovoPedidoTecnico").attr("disabled", true);
    //    $("#btnNovoPedidoCliente").attr("disabled", true);
    //    $("#tbnNovoPedidoAvulso").attr("disabled", true);
    //}
    
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

    $('#tecnico_CD_TECNICO').val(null).trigger('change');
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

    //carregarGridMVC();
});

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

    var URL = URLAPI + "PedidoAPI/GerarTodosPedidosTecnico";

    $.ajax({
        type: 'GET',
        url: URL,
        beforeSend: function () {
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
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    novoPedido(CD_TECNICO, tipoPedidoCliente, ID_STATUS_PEDIDO);
});

$('#tbnNovoPedidoAvulso').click(function () {
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();

    var ID_STATUS_PEDIDO = $("#statusPedido_ID_STATUS_PEDIDO option:selected").val();

    if (CD_TECNICO == "" || CD_TECNICO == "0" || CD_TECNICO == 0) {
        ExibirCampo($('#validaTecnico'));
        return;
    }

    novoPedido(CD_TECNICO, tipoPedidoAvulso, ID_STATUS_PEDIDO);
});

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
                //window.open(URLSite + '/RelatorioSolicitacaoPecas.aspx?idKey=' + res.idKey, '_blank');
                window.open(URLSite + '/RelatorioLote.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarComboCliente() {
    //var URL = URLAPI + "ClienteAPI/ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    var URL = URLAPI + "ClienteAPI/";

    //if (nidPerfil == perfilCliente) {
    //    URL = URL + "ObterListaPerfilCliente?nidUsuario=" + nidUsuario;
    //}
    //else {
    //    URL = URL + "ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    //}

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaComboPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    }


    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.clientes != null) {
                LoadClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadClientes(clientes) {
    if (nidPerfil == perfilCliente) {
        if (clientes.length == 0)
            LimparCombo($("#cliente_CD_CLIENTE"));
        else
            $("#cliente_CD_CLIENTE").empty();
    }
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }

    totalClientes = clientes.length;
}

function carregarComboTecnico() {
    //var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var URL = URLAPI;

    //if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

        var tecnicoEntity = {};
        tecnicoEntity = {
            usuario: {
                nidUsuario: nidUsuario
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
            //data: null,
            data: JSON.stringify(tecnicoEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    LoadTecnicos(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    //}
    //else {
    //    URL = URLAPI + "TecnicoXClienteAPI/ObterLista"; //?CD_CLIENTE=" + CD_CLIENTE;

    //    var tecnicoClienteEntity = {
    //        cliente: {
    //            CD_CLIENTE: CD_CLIENTE,
    //        }
    //    };

    //    $.ajax({
    //        type: 'POST',
    //        url: URL,
    //        dataType: "json",
    //        cache: false,
    //        async: false,
    //        contentType: "application/json",
    //        //headers: { "Authorization": "Basic " + localStorage.token },
    //        data: JSON.stringify(tecnicoClienteEntity),
    //        beforeSend: function () {
    //            $("#loader").css("display", "block");
    //        },
    //        complete: function () {
    //            $("#loader").css("display", "none");
    //        },
    //        success: function (res) {
    //            $("#loader").css("display", "none");
    //            if (res.tecnicos != null) {
    //                LoadTecnicosCliente(res.tecnicos);
    //            }
    //        },
    //        error: function (res) {
    //            $("#loader").css("display", "none");
    //            //atualizarPagina();
    //            Alerta("ERRO", res.responseText);
    //        }

    //    });
    //}
}

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    //SL00036007
    //if (UsuarioTecnico == "True") {
    //    var cdTecnico = '';
    //    var contador = 0;

    //    for (i = 0; i < tecnicos.length; i++) {
    //        if (nidUsuario == tecnicos[i].usuario.nidUsuario) {
    //            MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);

    //            cdTecnico = tecnicos[i].CD_TECNICO;
    //            contador++;
    //        }
    //    }

    //    if (contador == 1) {
    //        $('#tecnico_CD_TECNICO').val(cdTecnico).trigger('change');
    //    }
    //}
    //else {
        for (i = 0; i < tecnicos.length; i++) {
            MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
        }
    //}
}

//function LoadTecnicos(tecnicosJO) {
//    LimparCombo($("#tecnico_CD_TECNICO"));
//    var tecnicos = JSON.parse(tecnicosJO);

//    for (i = 0; i < tecnicos.length; i++) {
//        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
//        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
//    }
//}

function LoadTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));

    for (i = 0; i < tecnicos.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}

function carregarComboStatusPedido() {
    var URL = URLAPI + "StatusPedidoAPI/ObterLista";

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function () {
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
            beforeSend: function () {
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

    $.ajax({
        type: 'POST',
        url: URL,
        contentType: "application/json",
        cache: false,
        async: false,
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(pedidoEntity),
        beforeSend: function () {
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

function novoPedido(CD_TECNICO, tipoPedido, ID_STATUS_PEDIDO) {
    var URL = URLCriptografarChave + "?Conteudo=" + CD_TECNICO + "|0|" + tipoPedido + "|" + tipoOrigemPagina + "|" + ID_STATUS_PEDIDO;

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