$('#DT_DATAS-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_INICIAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_FINAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

jQuery(document).ready(function () {
    $('#ddlTecnico').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlLinhaProduto').select2({
        placeholder: "Selecione...",
        allowClear: true
    });


    $('.js-example-basic-single').select2();

    //SL00033191
    $('.js-example-basic-single2').select2({
        placeholder: "Selecione...",
        templateSelection: function (data) { return data.id; }
    });

    $('#DT_INICIAL').mask('00/00/0000');
    $('#DT_FINAL').mask('00/00/0000');
    //$('#DT_DEV_FINAL').val(Date.now.toString("dd/MM/yyyy"));

    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    popularComboTecnico();
    popularComboLinhaProduto();



});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null);
    $('#vendedor_CD_VENDEDOR').val(null);
    $('#grupo_CD_GRUPO').val(null);
    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    $('#DT_INICIAL').val('');
    $('#DT_FINAL').val('');
});

$('#btnImprimir').click(function () {

    //SL00033191
    //var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_CLIENTE = $("#cliente_CD_CLIENTE").val();

    var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR option:selected").val();
    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var check = checarDatas(DT_INICIAL, DT_FINAL);

    if (CD_CLIENTE == undefined)
        CD_CLIENTE = '';
    if (CD_VENDEDOR == undefined)
        CD_VENDEDOR = '';
    if (CD_GRUPO == undefined)
        CD_GRUPO = '';

    var codigoTecnico = '';
    if (null != $("#ddlTecnico").val() && $("#ddlTecnico").val() != '')
        codigoTecnico = $("#ddlTecnico").val();

    var codigoLinhaProduto = '';
    if (null != $('#ddlLinhaProduto').val()) {
        codigoLinhaProduto = $('#ddlLinhaProduto').val()
    }

    var ModeloTabela = 'N';
    if ($("#chkModeloTabela").prop('checked')) {
        ModeloTabela = 'S';
    }

    var sitUltManutencao = '';
    if (null != $('#sitUltManutencao').val()) {
        sitUltManutencao = $('#sitUltManutencao').val()
    }

    //Código Original
    //if (CD_CLIENTE == '' && CD_VENDEDOR == '' && (DT_INICIAL == '' || DT_FINAL == '')) {
    //    Alerta("Aviso", "Informe o período (de/até) ou selecione um cliente ou vendendor como filtro para o relatório!");
    //    return false;
    //}

    //Chamado AMS|SL00033219
    //Nenhum parâmetro deve ser obrigatório

    if (check) {
        //window.open(URLSite + '/RelatorioInventarios.aspx?idKey=' + DT_INICIAL + "|" + DT_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|", '_blank');

        var URL = URLCriptografarChave + "?Conteudo=" + DT_INICIAL + "|" + DT_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + codigoTecnico + "|" + codigoLinhaProduto + "|" + ModeloTabela + "|" + sitUltManutencao;

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
                    window.open(URLSite + '/RelatorioInventarios.aspx?idKey=' + res.idKey, '_blank');
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

function checarDatas(DT_INICIAL, DT_FINAL) {
    var data_1 = new Date(formatarData(DT_INICIAL));
    var data_2 = new Date(formatarData(DT_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
        //} else if (data_1.getFullYear() < data.getFullYear() - 5) {
        //Alerta("Aviso", "A data inicial excedeu o limite de 5 anos!");
        //return false;
        //} else if (data_2.getFullYear() < data.getFullYear() - 5) {
        //Alerta("Aviso", "A data final excedeu o limite de 5 anos!");
        //return false;
        //} else if (data_1 > data) {
        //    Alerta("Aviso", "A data inicial não pode ser posterior a hoje!");
        //    return false;
        //} else if (data_2 > data) {
        //    Alerta("Aviso", "A data final não pode ser posterior a hoje!");
        //    return false;
    } else {
        return true;
    }
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

function carregarComboVendedor() {
    var URL = URLAPI + "VendedorAPI/ObterLista";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            //$("#loader").css("display", "block");
        },
        complete: function () {
            //$("#loader").css("display", "none");
        },
        success: function (res) {
            //$("#loader").css("display", "none");
            if (res.vendedores != null) {
                LoadVendedores(res.vendedores);
            }
        },
        error: function (res) {
            //$("#loader").css("display", "none");
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
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
        cache: false,
        contentType: "application/json",
        //headers: { "Authorization": "Basic " + localStorage.token },
        //data: null,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            //$("#loader").css("display", "block");
        },
        complete: function () {
            //$("#loader").css("display", "none");
        },
        success: function (res) {
            //$("#loader").css("display", "none");
            if (res.grupos != null) {
                LoadGrupos(res.grupos);
            }
        },
        error: function (res) {
            //$("#loader").css("display", "none");
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


function popularComboTecnico() {

    var URL = URLAPI + "TecnicoAPI/ObterListaAtivos";
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
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
            if (res.tecnicos != null) {
                preencherComboTecnicos(res.tecnicos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });
}
function preencherComboTecnicos(tecnicosJO) {
    LimparCombo($("#ddlTecnico"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
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

