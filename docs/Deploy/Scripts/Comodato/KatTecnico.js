$(document).ready(function () {
    $('#DT_INICIAL').mask('00/00/0000');
    $('#DT_FINAL').mask('00/00/0000');

    $('#ddlTecnico').select2({
        placeholder: "Selecione...",
        templateSelection: function (data) { return data.id; }
    });

    $('#ddlCliente').select2({
        //minimumInputLength: 3,
        placeholder: "Selecione...",
        templateSelection: function (data) { return data.id; }
    });


   popularComboClientes();
    popularComboTecnico();

});

$('#DT_DATA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#ddlCliente").change(function () {

    popularComboTecnico();

});


$('#btnLimpar').click(function () {
    popularComboClientes();
    popularComboTecnico();

});

$('#btnImprimir').click(function () {

    var DT_INICIAL = $('#DT_INICIAL').val();
    var DT_FINAL = $('#DT_FINAL').val();

    var codigoTecnicos = '';
    if (null != $("#ddlTecnico").val() && $("#ddlTecnico").val() != '')
        codigoTecnicos = $("#ddlTecnico").val();

    var codigoClientes = '';
    if (null != $("#ddlCliente").val() && $("#ddlCliente").val() != '')
        codigoClientes = $("#ddlCliente").val();

    var ExibirClientes = 'N';
    if ($("#chkExibirClientes").prop('checked')) {
        ExibirClientes = 'S';
    }

    var partsDT_INICIAL = DT_INICIAL.split('/');
    var partsDT_FINAL = DT_FINAL.split('/');
    var AnoOuMes = 'A';
    if (partsDT_INICIAL[1] == partsDT_FINAL[1]) {
        AnoOuMes = 'M'; //Se o intervalo for no mesmo mês
    }


    var periodoValido = checarDatas(DT_INICIAL, DT_FINAL);
    if (periodoValido) {

        var parametros = DT_INICIAL + "|" + DT_FINAL + '|' + codigoTecnicos + "|" + codigoClientes + "|" + ExibirClientes + "|" + AnoOuMes;

        //window.open(actionRelatorio + '?idKey=' + parametros, '_blank');
        var URL = URLCriptografarChave + "?Conteudo=" + parametros;

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
                    window.open(URLSite + '/RelatorioKatTecnico.aspx?idKey=' + res.idKey, '_blank');
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



function popularComboClientes() {
    var URL = URLAPI + "ClienteAPI/";

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
        contentType: "application/json",
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.clientes != null) {
                preencherComboClientes(res.clientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}
function preencherComboClientes(clientes) {
    if (nidPerfil == perfilCliente)
        $("#ddlCliente").empty();
    else
        LimparCombo($("#ddlCliente"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#ddlCliente"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function popularComboTecnico() {
    var CD_CLIENTE = $("#ddlCliente option:selected").val();
    var URL = URLAPI;

    if (null == CD_CLIENTE || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

        $.ajax({
            type: 'POST',
            url: URL,
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
    else {
        URL = URLAPI + "TecnicoXClienteAPI/ObterLista";
        var tecnicoClienteEntity = {
            cliente: {
                CD_CLIENTE: CD_CLIENTE,
            }
        };

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            contentType: "application/json",
            data: JSON.stringify(tecnicoClienteEntity),
            beforeSend: function () {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                $("#loader").css("display", "none");
                if (res.tecnicos != null) {
                    preencherComboTecnicosCliente(res.tecnicos);
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                Alerta("ERRO", res.responseText);
            }
        });
    }
}
function preencherComboTecnicos(tecnicosJO) {
    LimparCombo($("#ddlTecnico"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO + " (" + tecnicos[i].CD_TECNICO + ")");
    }
}
function preencherComboTecnicosCliente(tecnicos) {
    LimparCombo($("#ddlTecnico"));
    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO + " (" + tecnicos[i].tecnico.CD_TECNICO + ")");
    }
}


function checarDatas(DT_INICIAL, DT_FINAL) {
    var data_1 = new Date(formatarData(DT_INICIAL));
    var data_2 = new Date(formatarData(DT_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
    }
    //} else if (data_1 > data) {
    //    Alerta("Aviso", "A data inicial não pode ser posterior a hoje!");
    //    return false;

    //} else if (data_2 > data) {
    //    Alerta("Aviso", "A data final não pode ser posterior a hoje!");
    //    return false;
    //}

    return true;
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

