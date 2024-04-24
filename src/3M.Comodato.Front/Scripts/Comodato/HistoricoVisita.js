jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $('#DT_INICIO').mask('00/00/0000');
    $('#DT_FIM').mask('00/00/0000');

    carregarComboClientes();
    if (localStorage['HistoricoVisita_CD_CLIENTE'] != undefined && localStorage['HistoricoVisita_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['HistoricoVisita_CD_CLIENTE']).trigger('change');
    }
    else
        carregarComboTecnico();

    if (localStorage['HistoricoVisita_CD_TECNICO'] != undefined && localStorage['HistoricoVisita_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['HistoricoVisita_CD_TECNICO']).trigger('change');
    }

    if (localStorage['HistoricoVisita_DT_INICIO'] != undefined && localStorage['HistoricoVisita_DT_INICIO'] != "") {
        $('#DT_INICIO').val(localStorage['HistoricoVisita_DT_INICIO']).trigger('change');
    }

    if (localStorage['HistoricoVisita_DT_FIM'] != undefined && localStorage['HistoricoVisita_DT_FIM'] != "") {
        $('#DT_FIM').val(localStorage['HistoricoVisita_DT_FIM']).trigger('change');
    }

    carregarGridMVC();
});

$('#DT_VISITA-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null).trigger('change');

    localStorage.removeItem('HistoricoVisita_CD_CLIENTE');
    localStorage.removeItem('HistoricoVisita_CD_TECNICO');
    //localStorage.removeItem('HistoricoVisita_DT_INICIO');
    //localStorage.removeItem('HistoricoVisita_DT_FIM');

    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    carregarGridMVC();

    localStorage['HistoricoVisita_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
    localStorage['HistoricoVisita_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['HistoricoVisita_DT_INICIO'] = $("#DT_INICIO").val();
    localStorage['HistoricoVisita_DT_FIM'] = $("#DT_FIM").val();
});

$("#cliente_CD_CLIENTE").change(function () {
    carregarComboTecnico();
    novaVisitaRetroativa();
});

$("#tecnico_CD_TECNICO").change(function () {
    novaVisitaRetroativa();
});

$('#btnNovaVisita').click(function () {
    var URLCriarVisita = URLSITE +"Agenda/MontarIdKey?";

    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    //var DT_INICIO = $('#DT_INICIO').val();
    //var DT_FIM = $('#DT_FIM').val();

    OcultarCampo($('#validaDT_VISITA'));

    //if (DT_INICIO == '' || DT_FIM == '') {
    //    ExibirCampo($('#validaDT_VISITA'));
    //    return;
    //}


    var URL = URLCriarVisita + "CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO;

    //var URL = URLCriarVisita + "?idkey=" + idkey;
    //var URL = URLCriarVisita + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&DT_INICIO=" + DT_INICIO + "&DT_FIM=" + DT_FIM;
    window.location.href = URL;

    //$.ajax({
    //    type: 'GET',
    //    url: URL,
    //    dataType: "json",
    //    cache: false,
    //    contentType: "application/json",
    //    beforeSend: function () {
    //        $("#loader").css("display", "block");
    //    },
    //    complete: function () {
    //        $("#loader").css("display", "none");
    //    },
    //    success: function (res) {
    //        $("#loader").css("display", "none");
            
    //    },
    //    error: function (res) {
    //        $("#loader").css("display", "none");
    //        Alerta("ERRO", res.responseText);
    //    }
    //});
})

function novaVisitaRetroativa() {

    var Cliente = $('#cliente_CD_CLIENTE').val();
    var Tecnico = $('#tecnico_CD_TECNICO').val();

    if (Tecnico != '' && Cliente != '') {
        $('#btnNovaVisita').prop('disabled', false);
    } else {

        $('#btnNovaVisita').prop('disabled', true);
    }

}

function carregarComboClientes() {
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
                var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
                if (CD_CLIENTE != "" && CD_CLIENTE != "0" && CD_CLIENTE != 0) {
                    carregarGridMVC();
                }
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
        $("#cliente_CD_CLIENTE").empty();
    else
        LimparCombo($("#cliente_CD_CLIENTE"));

    for (i = 0; i < clientes.length; i++) {
        var NM_CLIENTE = clientes[i].NM_CLIENTE + " (" + clientes[i].CD_CLIENTE + ") " + clientes[i].EN_CIDADE + " - " + clientes[i].EN_ESTADO;
        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
    }
}

function carregarComboTecnico() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var URL = URLAPI;
    //var token = sessionStorage.getItem("token");
    if (null == CD_CLIENTE || CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {

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
            //async: false,
            contentType: "application/json",
            data: JSON.stringify(tecnicoEntity),
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                if (res.tecnicos != null) {
                    preencherComboTecnicos(res.tecnicos);
                }
                $("#loader").css("display", "none");
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
        //var token = sessionStorage.getItem("token");
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
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO + " (" + tecnicos[i].CD_TECNICO + ")");
    }
}

function preencherComboTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    for (i = 0; i < tecnicos.length; i++) {
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO + " (" + tecnicos[i].tecnico.CD_TECNICO + ")");
    }
}

function carregarGridMVC() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var DT_INICIO = $('#DT_INICIO').val();
    var DT_FIM = $('#DT_FIM').val();

    OcultarCampo($('#validaDT_VISITA'));

    if (DT_INICIO == '' || DT_FIM == '') {
        ExibirCampo($('#validaDT_VISITA'));
        return;
    }

    if (CD_CLIENTE == "" || CD_CLIENTE == undefined)
        CD_CLIENTE = 0;

    if (CD_TECNICO == undefined)
        CD_TECNICO = "";

    var URL = URLObterListaVisita + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&DT_INICIO=" + DT_INICIO + "&DT_FIM=" + DT_FIM;
    atribuirParametrosPaginacao("gridmvc", URLObterListaVisita, '{"CD_CLIENTE" : "' + CD_CLIENTE + '", "CD_TECNICO":"' + CD_TECNICO + '", "DT_INICIO":"' + DT_INICIO + '", "DT_FIM":"' + DT_FIM +'" }');

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
