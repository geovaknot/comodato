var filtros = '?CD_GRUPO=&CLIENTE=&CD_MODELO=&TECNICO=&nidUsuario=&CD_VENDEDOR=&LINHA_PRODUTO='; 

$('#grupo_CD_GRUPO').change(function () {
    definirCorFiltro();
});

$('#usuarioRegional_nidUsuario').change(function () {
    definirCorFiltro();
    carregarComboVendedor();
});

$('#vendedor_CD_VENDEDOR').change(function () {
    definirCorFiltro();
});

$('#modelo_CD_MODELO').change(function () {
    definirCorFiltro();
});

$('#CLIENTE').focusout(function () {
    definirCorFiltro();
});

$('#TECNICO').focusout(function () {
    definirCorFiltro();
});

$('#btnAutoRefresh').click(function () {
    AutoRefresh = !AutoRefresh;
    var Class = '';

    if (AutoRefresh == true)
        Class = 'btn btn-success btn-sm';
    else
        Class = 'btn btn-primary btn-sm';

    $("#btnAutoRefresh").removeClass();
    $("#btnAutoRefresh").addClass(Class);

});

$('#tipoDashboard').change(function () {
    var tipoDashboard = $("#tipoDashboard").val();

    switch (tipoDashboard) {
        case "P":
            window.location = 'Planejamento';
            break;
        case "T":
            window.location = 'AreaTecnica';
            break;
        case "N":
            window.location = 'Negocios';
            break;
        case "V":
            window.location = 'Vendas';
            break;
    }

});

$('#btnAplicar').click(function () {
    AplicarFiltros();
});

$('#btnLimpar').click(function () {
    $('#grupo_CD_GRUPO').val('').trigger('change');
    $('#CLIENTE').val('');
    $('#modelo_CD_MODELO').val('').trigger('change');
    $('#TECNICO').val('');
    $('#usuarioRegional_nidUsuario').val('').trigger('change');
    $('#vendedor_CD_VENDEDOR').val('').trigger('change');
    $('#linhaProduto_CD_LINHA_PRODUTO').val('').trigger('change');

    definirCorFiltro();
    AplicarFiltros();
});

function AplicarFiltros() {
    var CD_GRUPO = $('#grupo_CD_GRUPO').val();
    var CLIENTE = $('#CLIENTE').val();
    var CD_MODELO = $('#modelo_CD_MODELO').val();
    var TECNICO = $('#TECNICO').val();
    var nidUsuario = $('#usuarioRegional_nidUsuario').val();
    var CD_VENDEDOR = $('#vendedor_CD_VENDEDOR').val();
    var CD_LINHA_PRODUTO = $('#linhaProduto_CD_LINHA_PRODUTO').val();

    if (CD_GRUPO == undefined)
        CD_GRUPO = "";
    if (CLIENTE == undefined)
        CLIENTE = "";
    if (CD_MODELO == undefined)
        CD_MODELO = "";
    if (TECNICO == undefined)
        TECNICO = "";
    if (nidUsuario == undefined)
        nidUsuario = "";
    if (CD_VENDEDOR == undefined)
        CD_VENDEDOR = "";
    if (CD_LINHA_PRODUTO == undefined)
        CD_LINHA_PRODUTO = "";

    filtros = '?CD_GRUPO=' + CD_GRUPO + '&CLIENTE=' + CLIENTE + '&CD_MODELO=' + CD_MODELO + '&TECNICO=' + TECNICO + '&nidUsuario=' + nidUsuario + '&CD_VENDEDOR=' + CD_VENDEDOR + '&CD_LINHA_PRODUTO=' + CD_LINHA_PRODUTO; 

    var AutoRefresh_SituacaoInicial = AutoRefresh;
    AutoRefresh = true;
    carregarPagina();
    AutoRefresh = AutoRefresh_SituacaoInicial;
}

function definirCorFiltro() {
    var CD_GRUPO = $('#grupo_CD_GRUPO').val();
    var CLIENTE = $('#CLIENTE').val();
    var CD_MODELO = $('#modelo_CD_MODELO').val();
    var TECNICO = $('#TECNICO').val();
    var nidUsuario = $('#usuarioRegional_nidUsuario').val();
    var CD_VENDEDOR = $('#vendedor_CD_VENDEDOR').val();
    var CD_LINHA_PRODUTO = $('#linhaProduto_CD_LINHA_PRODUTO').val();

    var Class = 'btn btn-primary btn-sm';

    if (CD_GRUPO != "" && CD_GRUPO != "0" && CD_GRUPO != 0 && CD_GRUPO != undefined) 
        Class = 'btn btn-success btn-sm';

    if (CLIENTE != "" && CLIENTE != undefined) 
        Class = 'btn btn-success btn-sm';

    if (CD_MODELO != "" && CD_MODELO != "0" && CD_MODELO != 0 && CD_MODELO != undefined) 
        Class = 'btn btn-success btn-sm';

    if (TECNICO != "" && TECNICO != undefined)
        Class = 'btn btn-success btn-sm';

    if (nidUsuario != "" && nidUsuario != "0" && nidUsuario != 0 && nidUsuario != undefined)
        Class = 'btn btn-success btn-sm';

    if (CD_VENDEDOR != "" && CD_VENDEDOR != "0" && CD_VENDEDOR != 0 && CD_VENDEDOR != undefined)
        Class = 'btn btn-success btn-sm';

    if (CD_LINHA_PRODUTO != "" && CD_LINHA_PRODUTO != "0" && CD_LINHA_PRODUTO != 0 && CD_LINHA_PRODUTO != undefined)
        Class = 'btn btn-success btn-sm';

    $("#btnFiltro").removeClass();
    $("#btnFiltro").addClass(Class);
}

function dynamicColors() {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgba(" + r + "," + g + "," + b + ", 0.5)";

    //var hex = "0123456789ABCDEF",
    //    color = "#";
    //for (var i = 1; i <= 6; i++) {
    //    color += hex[Math.floor(Math.random() * 16)];
    //}
    //return color;
}

function carregarComboRegional() {
    var URL = URLAPI + "UsuarioAPI/ObterListaUsuarioRegional";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.usuarios != null) {
                LoadUsuariosRegionais(res.usuarios);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadUsuariosRegionais(usuariosJO) {
    LimparCombo($("#usuarioRegional_nidUsuario"));
    var usuarios = JSON.parse(usuariosJO);

    for (i = 0; i < usuarios.length; i++) {
        MontarCombo($("#usuarioRegional_nidUsuario"), usuarios[i].nidUsuario, usuarios[i].cnmNome);
    }
}

function carregarComboVendedor() {
    var usuarioRegional = $("#usuarioRegional_nidUsuario").val();

    var URL = URLAPI + "VendedorAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    var VendedorEntity = {
        usuarioGerenteRegional: {
            nidUsuario: usuarioRegional,
        }
    };
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        data: JSON.stringify(VendedorEntity),
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
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
        MontarCombo($("#vendedor_CD_VENDEDOR"), vendedores[i].CD_VENDEDOR, vendedores[i].NM_VENDEDOR);
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
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
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

function carregarComboModelo() {
    var URL = URLAPI + "ModeloAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (res) {
            if (res.MODELO != null) {
                LoadModelos(res.MODELO);
            }
        },
        error: function (res) {
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });
}

function LoadModelos(modeloJO) {
    LimparCombo($("#modelo_CD_MODELO"));
    var modelos = JSON.parse(modeloJO);

    for (i = 0; i < modelos.length; i++) {
        MontarCombo($("#modelo_CD_MODELO"), modelos[i].CD_MODELO, modelos[i].DS_MODELO);
    }
}

function carregarComboLinhaProduto() {
    var URL = URLAPI + "LinhaProdutoAPI/ObterLista";
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
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
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
    LimparCombo($("#linhaProduto_CD_LINHA_PRODUTO"));

    for (i = 0; i < produtos.length; i++) {
        MontarCombo($("#linhaProduto_CD_LINHA_PRODUTO"), produtos[i].CD_LINHA_PRODUTO, produtos[i].DS_LINHA_PRODUTO);
    }
}

function carregarComboLinhaProdutoGp() {
    var URL = URLAPI + "LinhaProdutoAPI/ObterListaGp";
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
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.produtos != null) {
                LoadProdutosGp(res.produtos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadProdutosGp(produtos) {
    LimparCombo($("#linhaProduto_CD_LINHA_PRODUTO"));

    for (i = 0; i < produtos.length; i++) {
        MontarCombo($("#linhaProduto_CD_LINHA_PRODUTO"), produtos[i].CD_LINHA_PRODUTO, produtos[i].DS_LINHA_PRODUTO);
    }
}
// Define um plugin para incluir conteúdo aos labels 
Chart.plugins.register({
    afterDatasetsDraw: function (chart) {
        var ctx = chart.ctx;

        // Definir aqui os gráficos que não terão conteúdo nos labels
        if (chart.canvas.id == 'chartValorPecaEnviadaMes')
            return;
        else if (chart.canvas.id == 'chartFamiliaModelo')
            return;

        chart.data.datasets.forEach(function (dataset, i) {
            var meta = chart.getDatasetMeta(i);
            if (!meta.hidden) {
                meta.data.forEach(function (element, index) {
                    // Draw the text in black, with the specified font
                    ctx.fillStyle = 'rgb(0, 0, 0)';

                    var fontSize = 10;
                    var fontStyle = 'normal';
                    var fontFamily = 'Helvetica Neue';
                    ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

                    // Just naively convert to string for now
                    var dataString = dataset.data[index].toString();

                    // Make sure alignment settings are correct
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';

                    var padding = 5;
                    var position = element.tooltipPosition();
                    ctx.fillText(dataString, position.x, position.y - (fontSize / 2) - padding);
                });
            }
        });
    }
});