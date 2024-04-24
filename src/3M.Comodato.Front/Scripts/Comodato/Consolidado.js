jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#ddlLinhaProduto').select2({
        placeholder: "Selecione...",
        allowClear: true
    });
    
    carregarComboCliente();

    carregarComboGrupo();
    popularComboLinhaProduto();
});

$('#btnLimpar').click(function () {    
    $('#cliente_CD_CLIENTE').val(null);    
    $('#grupo_CD_GRUPO').val(null);   
    $('#ddlLinhaProduto').val(null).trigger('change');
    
    carregarComboCliente();    

    carregarComboGrupo();        
    
});

$('#btnImprimir').click(function () {
    //var CD_LINHA_PRODUTO = $("#produto_CD_LINHA_PRODUTO option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    //var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR option:selected").val();
    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
    //var CD_REGIAO = $("#regiao_CD_REGIAO option:selected").val();
    //var CD_EXECUTIVO = $("#executivo_CD_EXECUTIVO option:selected").val();    
    var codigoLinhaProduto = '';
    if (null != $('#ddlLinhaProduto').val()) {
        codigoLinhaProduto = $('#ddlLinhaProduto').val()
    }
    var params = CD_CLIENTE + "|" + CD_GRUPO + "|" + codigoLinhaProduto;
    var URL = URLCriptografarChave + "?Conteudo=" + params;

    if ((CD_CLIENTE == "" || CD_CLIENTE == null || CD_CLIENTE == undefined)
        && (CD_GRUPO == "" || CD_GRUPO == null || CD_GRUPO == undefined)
        && (codigoLinhaProduto == "" || codigoLinhaProduto == null || codigoLinhaProduto == undefined)) {
        Alerta("ERRO", "Selecione um Cliente ou Grupo de Cliente");
    }
    else if ((CD_CLIENTE == "" || CD_CLIENTE == null || CD_CLIENTE == undefined)
        && (CD_GRUPO == "" || CD_GRUPO == null || CD_GRUPO == undefined)
        && (codigoLinhaProduto != "" && codigoLinhaProduto != null && codigoLinhaProduto != undefined)) {
        Alerta("ERRO", "Selecione um Cliente ou Grupo de Cliente");
    }
    else if ((CD_CLIENTE != "" && CD_CLIENTE != null && CD_CLIENTE != undefined)
        && (CD_GRUPO != "" && CD_GRUPO != null && CD_GRUPO != undefined)) {
        Alerta("ERRO", "Selecione um Cliente ou Grupo de Cliente");
    }
    else {
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
                    window.open(URLSite + '/RelatorioConsolidadoVendas.aspx?idKey=' + res.idKey, '_blank');
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });
    }

    

});

function carregarComboCliente() {
    var URL = URLAPI + "ClienteAPI/ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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

function carregarComboGrupo() {
    var URL = URLAPI + "GrupoAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
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

function popularComboLinhaProduto() {
    var URL = URLAPI + "LinhaProdutoAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
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
            if (res.produtos != null) {
                preencherComboLinhaProduto(res.produtos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboLinhaProduto(produtos) {
    LimparCombo($("#ddlLinhaProduto"));
    for (i = 0; i < produtos.length; i++) {
        MontarCombo($("#ddlLinhaProduto"), produtos[i].CD_LINHA_PRODUTO, produtos[i].DS_LINHA_PRODUTO);
    }
}