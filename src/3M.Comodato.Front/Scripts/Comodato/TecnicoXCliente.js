jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();
    //$('.js-example-basic-single').select2({ minimumInputLength: 3 });
    //$('#tecnico_CD_TECNICO').select2();

    carregarComboCliente();
    if (localStorage['TecnicoXCliente_CD_CLIENTE'] != undefined && localStorage['TecnicoXCliente_CD_CLIENTE'] != "") {
        $('#cliente_CD_CLIENTE').val(localStorage['TecnicoXCliente_CD_CLIENTE']).trigger('change');
    }
    else
        carregarComboTecnico();

    if (localStorage['TecnicoXCliente_CD_TECNICO'] != undefined && localStorage['TecnicoXCliente_CD_TECNICO'] != "") {
        $('#tecnico_CD_TECNICO').val(localStorage['TecnicoXCliente_CD_TECNICO']).trigger('change');
    }

    if (localStorage['TecnicoXCliente_nvlQtdeTecnicos'] != undefined && localStorage['TecnicoXCliente_nvlQtdeTecnicos'] != "") {
        $('#nvlQtdeTecnicos').val(localStorage['TecnicoXCliente_nvlQtdeTecnicos']);
    }

    carregarGridMVC();
});

$("#cliente_CD_CLIENTE").change(function () {
    carregarComboTecnico();
});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#nvlQtdeTecnicos').val('');

    localStorage.removeItem('TecnicoXCliente_CD_CLIENTE');
    localStorage.removeItem('TecnicoXCliente_CD_TECNICO');
    localStorage.removeItem('TecnicoXCliente_nvlQtdeTecnicos');

    carregarGridMVC();
});

$('#btnFiltrar').click(function () {
    carregarGridMVC();

    localStorage['TecnicoXCliente_CD_CLIENTE'] = $("#cliente_CD_CLIENTE option:selected").val();
    localStorage['TecnicoXCliente_CD_TECNICO'] = $("#tecnico_CD_TECNICO option:selected").val();
    localStorage['TecnicoXCliente_nvlQtdeTecnicos'] = $("#nvlQtdeTecnicos").val();
});

function carregarGridMVC() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_TECNICO = $("#tecnico_CD_TECNICO option:selected").val();
    var nvlQtdeTecnicos = $('#nvlQtdeTecnicos').val();

    if (CD_CLIENTE == "")
        CD_CLIENTE = 0;

    var URL = URLObterLista + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_TECNICO=" + CD_TECNICO + "&nvlQtdeTecnicos=" + nvlQtdeTecnicos;
    atribuirParametrosPaginacao("gridmvc", URLObterLista, '{"CD_CLIENTE" : "' + CD_CLIENTE + '", "CD_TECNICO":"' + CD_TECNICO + '", "nvlQtdeTecnicos":"' + nvlQtdeTecnicos + '" }');

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

//function carregarComboCliente() {
//    $('#cliente_CD_CLIENTE').select2({ placehoder: 'Carregando', disabled: true })

//    var camposNecessarios = ["CD_CLIENTE", "NM_CLIENTE", "EN_CIDADE", "EN_ESTADO"].join(",");
//    var listaClientes = undefined;
    

//    if (nidPerfil == perfilCliente)
//        listaClientes = ClienteAPI.ObterListaPerfilClienteAsync(nidUsuario, camposNecessarios);
//    else
//        listaClientes = ClienteAPI.ObterListaPorUsuarioPerfilAsync(nidUsuario, camposNecessarios);

//    listaClientes.done(function (retorno) {
//        LoadClientes(JSON.parse(retorno.clientes));
//        $('#cliente_CD_CLIENTE').select2({ placehoder: 'Selecione', disabled: false });
//    })
//        .catch(function () {
//            console.error("Não foi possível carregar a lista de clientes ", listaClientes.mensagem);
//        });

//}


//SL00035236
function carregarComboCliente() {
    //var URL = URLAPI + "ClienteAPI/ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    var URL = URLAPI + "ClienteAPI/";
    //var token = sessionStorage.getItem("token");
    if (nidPerfil == perfilCliente) {
        URL = URL + "ObterListaPerfilCliente?nidUsuario=" + nidUsuario;
    }
    else {
        //BEGIN - IM8016434 - Melhoria - Ubirajara Lisboa - 12/02/2021
        URL = URL + "ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
        //END - IM8016434 - Melhoria - Ubirajara Lisboa - 12/02/2021
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


//function LoadClientes(clientes) {
//    if (nidPerfil == perfilCliente) {
//        if (clientes.length == 0)
//            LimparCombo($("#cliente_CD_CLIENTE"));
//        else
//            $("#cliente_CD_CLIENTE").empty();
//    }
//    else
//        LimparCombo($("#cliente_CD_CLIENTE"));


//    let dados = clientes.map(function (cliente, i) {
//        return {
//            id: cliente.CD_CLIENTE,
//            text: cliente.NM_CLIENTE + " (" + cliente.CD_CLIENTE + ") " + cliente.EN_CIDADE + " - " + cliente.EN_ESTADO
//        }
//    });

//    $('#cliente_CD_CLIENTE').select2({
//        data: dados
//    })
//}

function carregarComboTecnico() {
    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var URL = URLAPI;
    //var token = sessionStorage.getItem("token");
    if (CD_CLIENTE == "" || CD_CLIENTE == "0" || CD_CLIENTE == 0) {
        URL = URLAPI + "TecnicoAPI/ObterListaAtivos";

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
    }
    else {
        URL = URLAPI + "TecnicoXClienteAPI/ObterLista"; //?CD_CLIENTE=" + CD_CLIENTE;

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
                if (res.tecnicos != null) {
                    LoadTecnicosCliente(res.tecnicos);
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

function LoadTecnicos(tecnicosJO) {
    LimparCombo($("#tecnico_CD_TECNICO"));
    var tecnicos = JSON.parse(tecnicosJO);

    for (i = 0; i < tecnicos.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].CD_TECNICO, tecnicos[i].NM_TECNICO);
    }
}

function LoadTecnicosCliente(tecnicos) {
    LimparCombo($("#tecnico_CD_TECNICO"));

    for (i = 0; i < tecnicos.length; i++) {
        //$("#tecnico_CD_TECNICO").append("<option value='" + tecnicos[i].tecnico.CD_TECNICO + "'>" + tecnicos[i].tecnico.NM_TECNICO + "</option>");
        MontarCombo($("#tecnico_CD_TECNICO"), tecnicos[i].tecnico.CD_TECNICO, tecnicos[i].tecnico.NM_TECNICO);
    }
}