$(document).ready(function () {
    $('#DT_INICIAL').mask('00/00/0000');
    $('#DT_FINAL').mask('00/00/0000');

    $('#ddlTecnico').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlEquipamento').select2({
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlCliente').select2({
        //minimumInputLength: 3,
        placeholder: "Selecione...",
        allowClear: true
    });

    $('#ddlGrupo').select2({
        placeholder: "Selecione...",
        allowClear: true
    });


    $('#ddlVisita').select2({
        //placeholder: "Selecione...",
        placeholder: "Selecione Cliente ou Técnico...",
        allowClear: true
    });
    $('#ddlOS').select2({
        //placeholder: "Selecione...",
        placeholder: "Selecione Cliente ou Técnico...",
        allowClear: true
    });

    popularComboGrupo();
    popularComboModelo();
    popularComboClientes();
    popularComboTecnico();

    $('#ddlVisita').prop("disabled", "disabled");
    $('#ddlOS').prop("disabled", "disabled");
    //popularComboVisitas();
    //popularComboOSVisita();
});

$('#DT_DATA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$("#ddlCliente").change(function () {
    var codigoCliente = $(this).val();

    popularComboTecnico();
    //popularComboAtivosCliente(codigoCliente);

    var CD_CLIENTE = $("#ddlCliente option:selected").val();
    if (CD_CLIENTE != null && CD_CLIENTE != "" && CD_CLIENTE != "0" && CD_CLIENTE != 0 && CD_CLIENTE != undefined) {
        $('#ddlVisita').prop("disabled", "");
        $('#ddlOS').prop("disabled", "");
        $('#ddlVisita').select2({
            placeholder: "Selecione...",
            allowClear: true
        });
        $('#ddlOS').select2({
            placeholder: "Selecione...",
            allowClear: true
        });
        popularComboVisitas();
        popularComboOSVisita();
    }

});
$("#ddlTecnico").change(function () {
    var CD_TECNICO = $("#ddlTecnico option:selected").val();

    if (CD_TECNICO != null && CD_TECNICO != "" && CD_TECNICO != "0" && CD_TECNICO != 0 && CD_TECNICO != undefined) {
        $('#ddlVisita').prop("disabled", "");
        $('#ddlOS').prop("disabled", "");
        $('#ddlVisita').select2({
            placeholder: "Selecione...",
            allowClear: true
        });
        $('#ddlOS').select2({
            placeholder: "Selecione...",
            allowClear: true
        });
        popularComboVisitas();
        popularComboOSVisita();
    }
});

$("#ddlVisita").change(function () {
    var ID_VISITA = $("#ddlVisita option:selected").val();

    if (ID_VISITA != null && ID_VISITA != "" && ID_VISITA != "0" && ID_VISITA != 0 && ID_VISITA != undefined) {
        $('#ddlOS').prop("disabled", "");
        $('#ddlOS').select2({
            placeholder: "Selecione...",
            allowClear: true
        });
        popularComboOSVisita();
    }
});


$('#btnLimpar').click(function () {
    $('#ddlGrupo').val(null).trigger('change');
    $('#ddlCliente').val(null).trigger('change');
    $('#ddlTecnico').val(null).trigger('change');

    $("#ddlVisita").val(null).trigger('change');
    $("#ddlOS").val(null).trigger('change');
    //$("#selectModeloRelatorio").val(null).trigger('change');

    //$('#DT_INICIAL').val('');
    //$('#DT_FINAL').val('');
    //$('#DT_INICIAL').focus();

    $('#ddlVisita').prop("disabled", "disabled");
    $('#ddlOS').prop("disabled", "disabled");
    $('#ddlVisita').select2({
        placeholder: "Selecione Cliente ou Técnico..."
    });
    $('#ddlOS').select2({
        placeholder: "Selecione Cliente ou Técnico..."
    });
});

$('#btnImprimir').click(function () {

    

    var ModeloRelatorio = '';
    if (null != $("#selectModeloRelatorio").val() && $("#selectModeloRelatorio").val() != '')
        ModeloRelatorio = $("#selectModeloRelatorio").val();

    var StatusRelatorio = '';
    if (null != $("#selectStatusRelatorio").val() && $("#selectStatusRelatorio").val() != '')
        StatusRelatorio = $("#selectStatusRelatorio").val();
    

    var parametros = ModeloRelatorio + "|" + StatusRelatorio;

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
                window.open(URLSite + '/RelatorioPecas.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

    
});

function popularComboModelo() {
    var URL = URLAPI + "ModeloAPI/ObterLista";
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
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.MODELO != null) {
                preencherComboModelo(data.MODELO);
            }
            $('#ddlModelo').val('').trigger('change');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}
function preencherComboModelo(modeloJO) {
    LimparCombo($("#ddlModelo"));
    var modelos = JSON.parse(modeloJO);
    for (i = 0; i < modelos.length; i++) {
        MontarCombo($("#ddlModelo"), modelos[i].CD_MODELO, modelos[i].CD_MODELO);
    }
}

function popularComboGrupo() {
    var URL = URLAPI + "GrupoAPI/ObterLista";
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
            if (res.grupos != null) {
                preencherComboGrupos(res.grupos);
            }
            $('#ddlGrupo').val('').trigger('change');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}
function preencherComboGrupos(grupos) {
    LimparCombo($("#ddlGrupo"));
    for (i = 0; i < grupos.length; i++) {
        MontarCombo($("#ddlGrupo"), grupos[i].CD_GRUPO, grupos[i].DS_GRUPO);
    }
}

function popularComboClientes() {
    var URL = URLAPI + "ClienteAPI/";

    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaComboPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    }
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
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
    //var token = sessionStorage.getItem("token");
    if (null == CD_CLIENTE || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

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
        MontarCombo($("#ddlTecnico"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
}
function preencherComboTecnicosCliente(tecnicos) {
    LimparCombo($("#ddlTecnico"));
    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#ddlTecnico"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}

function popularComboAtivosCliente(codigoCliente) {
    LimparCombo($("#ddlEquipamento"));

    if (null != codigoCliente || codigoCliente == "" || codigoCliente == "0" || codigoCliente == 0) {
        return;
    }

    var URL = URLAPI + "AtivoAPI/ObterListaAtivoCliente";
    URL = URL + "?CD_Cliente=" + codigoCliente + "&SomenteATIVOSsemDTDEVOLUCAO=true";
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
            if (res.listaAtivosClientes != null) {
                preencherAtivosCliente(res.listaAtivosClientes);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}
function preencherAtivosCliente(listaAtivosClientesJO) {
    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

    for (i = 0; i < listaAtivosClientes.length; i++) {
        MontarCombo($("#ddlEquipamento"), listaAtivosClientes[i].CD_ATIVO_FIXO, listaAtivosClientes[i].DS_ATIVO_FIXO);
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

function popularComboVisitas() {

    var CD_CLIENTE = $("#ddlCliente option:selected").val();
    var CD_TECNICO = $("#ddlTecnico option:selected").val();

    var visitaEntity = new Object();
    //var check = false;
    if (CD_CLIENTE != null && CD_CLIENTE != "" && CD_CLIENTE != "0" && CD_CLIENTE != 0 && CD_CLIENTE != undefined) {
        visitaEntity.cliente = new Object();
        visitaEntity.cliente.CD_CLIENTE = CD_CLIENTE;
        //check = true;
    }

    if (CD_TECNICO != null && CD_TECNICO != "" && CD_TECNICO != "0" && CD_TECNICO != 0 && CD_TECNICO != undefined) {
        visitaEntity.tecnico = new Object();
        visitaEntity.tecnico.CD_TECNICO = CD_TECNICO;
        //check = true;
    }
    //SL00035513
    var DT_INICIAL = $('#DT_INICIAL').val();
    var parts = DT_INICIAL.split('/');
    visitaEntity.DT_DATA_VISITA = parts[2] + "-" + parts[1] + "-" + parts[0]; //Data no formato Universal

    visitaEntity.tpStatusVisitaOS = new Object();
    visitaEntity.tpStatusVisitaOS.ST_TP_STATUS_VISITA_OS = 3; //Finalizada

    //if (!check) {
    //    $('#ddlVisita').prop("disable", "disable");
    //} else {
    //    //$('#ddlVisita').removerProp("disable");
    //    //$('#ddlVisita').removerAttr("disable");
    //}
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "VisitaApi/ObterLista",
        data: JSON.stringify(visitaEntity),
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
        success: function (data) {
            $("#loader").css("display", "none");
            preencherComboVisitas(data.result);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }
    });
}

function preencherComboVisitas(visitas) {
    LimparCombo($("#ddlVisita"));

    for (i = 0; i < visitas.length; i++) {
        var dataFormatada = FormatarDataHora(visitas[i].DT_DATA_VISITA); //Está no util.js
        var descricao = dataFormatada + ' - ' +
            visitas[i].cliente.NM_CLIENTE + ' - ' +
            visitas[i].ID_VISITA;

        MontarCombo($("#ddlVisita"), visitas[i].ID_VISITA, descricao);
    }
}


function popularComboOSVisita() {
    var osEntityFilter = new Object();

    //var CD_CLIENTE = $("#ddlCliente").val();
    var CD_CLIENTE = $("#ddlCliente option:selected").val();
    var CD_TECNICO = $("#ddlTecnico option:selected").val();
    var ID_VISITA = $("#ddlVisita option:selected").val();

    var URL = URLAPI + "OSAPI/ObterListaComboOS";

    if (CD_CLIENTE == null || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0 || CD_CLIENTE == undefined)
        CD_CLIENTE = 0;

    if (CD_TECNICO == null && CD_TECNICO == "" && CD_TECNICO == "0" && CD_TECNICO == 0 && CD_TECNICO == undefined) {
        CD_TECNICO = "0";
    }
    //SL00035513
    var DT_INICIAL = $('#DT_INICIAL').val();
    var parts = DT_INICIAL.split('/');
    var DT_DATA_VISITA = parts[2] + "-" + parts[1] + "-" + parts[0]; //Data no formato Universal

    if (ID_VISITA == null || ID_VISITA == "" || ID_VISITA == "0" || ID_VISITA == 0 || ID_VISITA == undefined)
        ID_VISITA = 0;


    //ddlVisita
    //var VISITA = $("#ddlVisita option:selected").val();
    //if (VISITA == null || VISITA == "" || VISITA == "0" || VISITA == 0 || VISITA == undefined) {
    //    $('#ddlOS').prop("disable", "disable");
    //} else {
    //    $('#ddlOS').removeAttr("disable");
    //}

    //var token = sessionStorage.getItem("token");
    URL = URL + "?CD_Cliente=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&DT_DATA_VISITA=" + DT_DATA_VISITA + "&ID_VISITA=" + ID_VISITA;

    $.ajax({
        type: 'POST',
        url: URL,
        data: JSON.stringify(osEntityFilter),
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
            if (res.preenchimentoOS != null) {
                preencherOSVisita(res.preenchimentoOS);
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });

}
function preencherOSVisita(preenchimentoOSJO) {
    LimparCombo($("#ddlOS"));

    var preenchimentoOS = JSON.parse(preenchimentoOSJO);

    for (i = 0; i < preenchimentoOS.length; i++) {
        MontarCombo($("#ddlOS"), preenchimentoOS[i].ID_OS_Formatado, preenchimentoOS[i].ID_OS_Formatado);
    }
}