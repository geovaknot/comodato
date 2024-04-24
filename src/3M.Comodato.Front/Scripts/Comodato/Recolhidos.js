$('#DT_DATAS-container .input-daterange').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEV_INICIAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

$('#DT_DEV_FINAL-container .input-group.date').datepicker({
    language: "pt-BR",
    autoclose: true,
    todayHighlight: true
});

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    $("#filtroAtual").val("Cliente");
    //carregarComboCliente();
    //carregarComboAtivo();
});


$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null).trigger('change');
    $('#ativo_CD_ATIVO_FIXO').val(null).trigger('change');
    $('#DT_DEV_INICIAL').val('');
    $('#DT_DEV_FINAL').val('');
});

//$('#btnImprimir').click(function () {
//    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
//    var CD_ATIVO_FIXO = $("#ativo_CD_ATIVO_FIXO option:selected").val();
//    var DT_DEV_INICIAL = $('#DT_DEV_INICIAL').val();
//    var DT_DEV_FINAL = $('#DT_DEV_FINAL').val();

//    var check = checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL);

//    if (CD_CLIENTE == undefined)
//        CD_CLIENTE = '';
//    if (CD_ATIVO_FIXO == undefined)
//        CD_ATIVO_FIXO = '';

//    if (check) {
//        var URL = URLCriptografarChave + "?Conteudo=" + CD_CLIENTE + "|" + CD_ATIVO_FIXO + "|" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL;

//        $.ajax({
//            url: URL,
//            processData: true,
//            dataType: "json",
//            cache: false,
//            contentType: "application/json",
//            beforeSend: function () {
//                $("#loader").css("display", "block");
//            },
//            complete: function () {
//                $("#loader").css("display", "none");
//            },
//            success: function (res) {
//                if (res.idKey != null && res.idKey != '') {
//                    window.open(URLSite + '/RelatorioRecolhidos.aspx?idKey=' + res.idKey, '_blank');
//                }
//            },
//            error: function (res) {
//                //atualizarPagina();
//                Alerta("ERRO", res.responseText);
//            }

//        });

//    }
//});
$('#btnImprimir').click(function () {
    var filtroAtual = $("#filtroAtual").val();
    var DT_INICIO = $("#DT_DEV_INICIAL").val();
    var DT_FIM = $("#DT_DEV_FINAL").val();
    var listaSelecionados = '';
    OcultarCampo($("#validaSelecionados"));

    switch (filtroAtual) {
        case "Cliente":
            $("INPUT[id*='ClientesSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = '0';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
        case "Modelo":
            $("INPUT[id*='ModelosSelecionados']").each(function (index) {
                if (this.disabled == true) {
                    listaSelecionados = '0';
                }
                else {
                    if (this.checked == true)
                        listaSelecionados = listaSelecionados + this.value + ",";
                }
            });
            break;
    }

    if (listaSelecionados == '') {
        ExibirCampo($("#validaSelecionados"));
        return false;
    }

    var URL = URLCriptografarChave + "?Conteudo=" + listaSelecionados + "|" + DT_INICIO + "|" + DT_FIM + "|" + filtroAtual;

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
                window.open(URLSite + '/RelatorioRecolhidos.aspx?idKey=' + res.idKey, '_blank');
            }
        },
        error: function (res) {
            //atualizarPagina();
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseText);
        }

    });

    //window.open(URLSite + '/RelatorioGeralManutencao.aspx?idKey=' + listaSelecionados + "|" + DT_INICIO + "|" + DT_FIM + "|" + filtroAtual, '_blank');

});


//function carregarComboCliente() {
//    var URL = URLAPI + "ClienteAPI/ObterListaPorUsuarioPerfil?nidUsuario=" + nidUsuario;

//    $.ajax({
//        type: 'GET',
//        url: URL,
//        dataType: "json",
//        cache: false,
//        contentType: "application/json",
//        //headers: { "Authorization": "Basic " + localStorage.token },
//        //data: null,
//        beforeSend: function () {
//            $("#loader").css("display", "block");
//        },
//        complete: function () {
//            $("#loader").css("display", "none");
//        },
//        success: function (res) {
//            if (res.clientes != null) {
//                LoadClientes(res.clientes);
//            }
//        },
//        error: function (res) {
//            //atualizarPagina();
//            Alerta("ERRO", res.responseText);
//        }

//    });
//}

//function LoadClientes(clientes) {
//    LimparCombo($("#cliente_CD_CLIENTE"));

//    for (i = 0; i < clientes.length; i++) {
//        var NM_CLIENTE = clientes[i].NM_CLIENTE + ' (' + clientes[i].CD_CLIENTE + ') ' + clientes[i].EN_CIDADE + ' - ' + clientes[i].EN_ESTADO;
//        //$("#cliente_CD_CLIENTE").append("<option value='" + clientes[i].CD_CLIENTE + "'>" + clientes[i].NM_CLIENTE + "</option>");
//        MontarCombo($("#cliente_CD_CLIENTE"), clientes[i].CD_CLIENTE, NM_CLIENTE);
//    }
//}

function checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL) {
    var data_1 = new Date(formatarData(DT_DEV_INICIAL));
    var data_2 = new Date(formatarData(DT_DEV_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        Alerta("Aviso", "A data inicial não pode ser maior que a data final!");
        return false;
    } else if (data_1.getFullYear() < data.getFullYear() - 5) {
        Alerta("Aviso", "A data inicial excedeu o limite de 5 anos!");
        return false;
    } else if (data_2.getFullYear() < data.getFullYear() - 5) {
        Alerta("Aviso", "A data final excedeu o limite de 5 anos!");
        return false;
    } else {
        return true;
    }
}

function formatarData(data) {
    var dt = data.split("/");
    return [dt[1], dt[0], dt[2]].join('/');
}

//function carregarComboAtivo() {
//    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();

//    if (CD_CLIENTE == '' || CD_CLIENTE == "0" || CD_CLIENTE == null) {
//        CD_CLIENTE = 0;
//    }

//    var URL = URLAPI + "AtivoAPI/ObterListaComboAtivosRecolhidos?CD_Cliente=" + CD_CLIENTE;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        cache: false,
//        dataType: "json",
//        contentType: "application/json",
//        //headers: { "Authorization": "Basic " + localStorage.token },
//        //data: null,
//        beforeSend: function () {
//            $("#loader").css("display", "block");
//        },
//        complete: function () {
//            $("#loader").css("display", "none");
//        },
//        success: function (res) {
//            if (res.listaAtivosClientes != null) {
//                LoadAtivos(res.listaAtivosClientes);
//            }
//        },
//        error: function (res) {
//            //atualizarPagina();
//            Alerta("ERRO", res.responseText);
//        }

//    });

//}

//function LoadAtivos(listaAtivosClientesJO) {
//    LimparCombo($("#ativo_CD_ATIVO_FIXO"));

//    var listaAtivosClientes = JSON.parse(listaAtivosClientesJO);

//    for (i = 0; i < listaAtivosClientes.length; i++) {
//        MontarCombo($("#ativo_CD_ATIVO_FIXO"), listaAtivosClientes[i].CD_ATIVO_FIXO, listaAtivosClientes[i].DS_ATIVO_FIXO);
//    }
//}

//$("#cliente_CD_CLIENTE").change(function () {
//    $('#ativo_CD_ATIVO_FIXO').val(null).trigger('change');
//    carregarComboAtivo();
//});

$('#btnMarcarCliente').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ClientesSelecionados']").prop('checked', true);
    $("INPUT[id*='ClientesSelecionados']").prop('disabled', true);
});
$('#btnDesmarcarCliente').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ClientesSelecionados']").prop('checked', false);
    $("INPUT[id*='ClientesSelecionados']").prop('disabled', false);
});


$('#btnMarcarModelo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ModelosSelecionados']").prop('checked', true);
    $("INPUT[id*='ModelosSelecionados']").prop('disabled', true);
});

$('#btnDesmarcarModelo').click(function (e) {
    e.preventDefault();
    $("INPUT[id*='ModelosSelecionados']").prop('checked', false);
    $("INPUT[id*='ModelosSelecionados']").prop('disabled', false);
});

$('a[data-toggle="list"]').on('shown.bs.tab', function (e) {
    e.target // newly activated tab
    e.relatedTarget // previous active tab

    $('#filtroAtual').val(e.target.text);
})