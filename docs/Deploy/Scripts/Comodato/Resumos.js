jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarComboLinhaProduto();
    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    carregarComboRegiao();
    carregarComboExecutivo();    
});

$('#btnLimpar').click(function () {
    $('#produto_CD_LINHA_PRODUTO').val(null);
    $('#cliente_CD_CLIENTE').val(null);
    $('#vendedor_CD_VENDEDOR').val(null);
    $('#grupo_CD_GRUPO').val(null);
    $('#regiao_CD_REGIAO').val(null);
    $('#executivo_CD_EXECUTIVO').val(null);

    carregarComboLinhaProduto();
    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    carregarComboRegiao();
    carregarComboExecutivo();
    
});

$('#btnImprimir').click(function () {
    var CD_LINHA_PRODUTO = $("#produto_CD_LINHA_PRODUTO option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR option:selected").val();
    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
    var CD_REGIAO = $("#regiao_CD_REGIAO option:selected").val();
    var CD_EXECUTIVO = $("#executivo_CD_EXECUTIVO option:selected").val();    

    var URL = URLCriptografarChave + "?Conteudo=" + CD_CLIENTE + "|" + CD_VENDEDOR + '|' + CD_REGIAO + "|" + CD_GRUPO + "|" + CD_EXECUTIVO + "|" + CD_LINHA_PRODUTO;

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
                window.open(URLSite + '/RelatorioResumos.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

});

function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
    LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + ' (' + clientes[i].CD_CLIENTE + ') ' + clientes[i].EN_CIDADE + ' - ' + clientes[i].EN_ESTADO;
        //$("#cliente_CD_CLIENTE").append("<option value='" + clientes[i].CD_CLIENTE + "'>" + clientes[i].NM_CLIENTE + "</option>");
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function carregarComboVendedor() {
    var URL = URLAPI + "VendedorAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
            if (res.vendedores != null) {
                LoadVendedores(res.vendedores);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadVendedores(vendedores) {
    LimparCombo($("#vendedor_CD_VENDEDOR"));

    for (i = 0; i < vendedores.length; i++) {
        //SL00034720
        if (vendedores[i].FL_ATIVO == "S")
        MontarCombo($("#vendedor_CD_VENDEDOR"), vendedores[i].CD_VENDEDOR, vendedores[i].NM_VENDEDOR);
    }
}

function carregarComboGrupo() {
    var URL = URLAPI + "GrupoAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
            if (res.grupos != null) {
                LoadGrupos(res.grupos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadGrupos(grupos) {
    LimparCombo($("#grupo_CD_GRUPO"));

    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#grupo_CD_GRUPO"), grupos[i].CD_GRUPO, grupos[i].DS_GRUPO);
    }
}

function carregarComboRegiao() {
    var URL = URLAPI + "RegiaoAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
            if (res.regioes != null) {
                LoadRegioes(res.regioes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadRegioes(regioes) {
    LimparCombo($("#regiao_CD_REGIAO"));
    //var regioes = JSON.parse(regioesJO);
    for (i = 0; i < regioes.length; i++) {
        MontarCombo($("#regiao_CD_REGIAO"), regioes[i].CD_REGIAO, regioes[i].DS_REGIAO);
    }
}

function carregarComboExecutivo() {
    var URL = URLAPI + "ExecutivoAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
            if (res.executivos != null) {
                LoadExecutivos(res.executivos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadExecutivos(executivos) {
    LimparCombo($("#executivo_CD_EXECUTIVO"));

    for (i = 0; i < executivos.length; i++) {
        MontarCombo($("#executivo_CD_EXECUTIVO"), executivos[i].CD_EXECUTIVO, executivos[i].NM_EXECUTIVO);
    }
}

function carregarComboLinhaProduto() {
    var URL = URLAPI + "LinhaProdutoAPI/ObterLista";

    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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
            if (res.produtos != null) {
                LoadProdutos(res.produtos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadProdutos(produtos) {
    LimparCombo($("#produto_CD_LINHA_PRODUTO"));

    for (i = 0; i < produtos.length; i++) {
        MontarCombo($("#produto_CD_LINHA_PRODUTO"), produtos[i].CD_LINHA_PRODUTO, produtos[i].DS_LINHA_PRODUTO);
    }
}