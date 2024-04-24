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
    //$('.js-example-basic-single').select2({ minimumInputLength: 1 });
    $('.js-example-basic-single').select2();
    $('.js-example-basic-multiple').select2();

    $('#DT_DEV_INICIAL').mask('00/00/0000');
    $('#DT_DEV_FINAL').mask('00/00/0000');
    //$('#DT_DEV_FINAL').val(Date.now.toString("dd/MM/yyyy"));

    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    carregarComboMotivo();
});

$('#btnLimpar').click(function () {
    $('#cliente_CD_CLIENTE').val(null);
    $('#vendedor_CD_VENDEDOR').val(null);
    $('#grupo_CD_GRUPO').val(null);
    $('#motivo_CD_MOTIVO_DEVOLUCAO').val(null);
    carregarComboCliente();
    carregarComboVendedor();
    carregarComboGrupo();
    carregarComboMotivo();
    $('#DT_INICIAL').val('');
    $('#DT_FINAL').val('');
});


//$('#btnTeste').click(function () {

//    $("#cliente_CD_CLIENTE").on("select2:select select2:unselect", function (e) {

//        //this returns all the selected item
//        var items = $(this).val();

//        //Gets the last selected item
//        var lastSelectedItem = e.params.data.id;

//    })

//    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
//    var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR option:selected").val();
//    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
//    var CD_MOTIVO_DEVOLUCAO = $("#motivo_CD_MOTIVO_DEVOLUCAO option:selected").val();
//    var DT_DEV_INICIAL = $('#DT_DEV_INICIAL').val();
//    var DT_DEV_FINAL = $('#DT_DEV_FINAL').val();

//    var check = checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL);

//    if (CD_CLIENTE == undefined || CD_CLIENTE == "")
//        CD_CLIENTE = '';
//    if (CD_VENDEDOR == undefined || CD_VENDEDOR == "")
//        CD_VENDEDOR = '';
//    if (CD_GRUPO == undefined || CD_GRUPO == "")
//        CD_GRUPO = '';
//    if (CD_MOTIVO_DEVOLUCAO == undefined || CD_MOTIVO_DEVOLUCAO == "")
//        CD_MOTIVO_DEVOLUCAO = '';

//    //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021
//    var ModeloTabela = 'N';
//    if ($("#chkModeloTabela").prop('checked')) {
//        ModeloTabela = 'S';
//    }
//    //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

//    if (check) {
//        //window.location = '../RelatorioRecolhidos.aspx?idKey=' + CD_CLIENTE + "|" + CD_ATIVO_FIXO + "|" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL;
//        //window.open(URLSite + '/RelatorioDevolvidos.aspx?idKey=' + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO + "|", '_blank');


//        //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021
//        //var URL = URLCriptografarChave + "?Conteudo=" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO; -- Antes da melhoria

//        var URL = URLCriptografarChave + "?Conteudo=" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO + "|" + ModeloTabela;
//        //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

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
//                $("#loader").css("display", "none");
//                if (res.idKey != null && res.idKey != '') {
//                    window.open(URLSite + '/RelatorioDevolvidos.aspx?idKey=' + res.idKey, '_blank');
//                }
//            },
//            error: function (res) {
//                $("#loader").css("display", "none");
//                //atualizarPagina();
//                Alerta("ERRO", res.responseText);
//            }

//        });

//    }

//    //if (check) {

//    //    //var URL = URLObterLista + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_ATIVO_FIXO=" + CD_ATIVO_FIXO + "&DT_DEV_INICIAL=" + DT_DEV_INICIAL + "&DT_DEV_FINAL=" + DT_DEV_FINAL;
//    //    var URL = URLImprimir + "?idKey=" + CD_CLIENTE + "|" + CD_ATIVO_FIXO + "|" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL;

//    //    $.ajax({
//    //        url: URL,
//    //        processData: true,
//    //        dataType: "json",
//    //        contentType: "application/json",
//    //        success: function (res) {

//    //        },
//    //        error: function (res) {
//    //            //atualizarPagina();
//    //            Alerta("ERRO", res.responseText);
//    //        }
//    //    });
//    //}
//});

$('#btnImprimir').click(function () {

    $("#cliente_CD_CLIENTE").on("select2:select select2:unselect", function (e) {

        //this returns all the selected item
        var items = $(this).val();

        //Gets the last selected item
        var lastSelectedItem = e.params.data.id;

    })

    var CD_CLIENTE = $("#cliente_CD_CLIENTE option:selected").val();
    var CD_VENDEDOR = $("#vendedor_CD_VENDEDOR option:selected").val();
    var CD_GRUPO = $("#grupo_CD_GRUPO option:selected").val();
    var CD_MOTIVO_DEVOLUCAO = $("#motivo_CD_MOTIVO_DEVOLUCAO option:selected").val();
    var DT_DEV_INICIAL = $('#DT_DEV_INICIAL').val();
    var DT_DEV_FINAL = $('#DT_DEV_FINAL').val();

    var check = checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL);

	if (CD_CLIENTE == undefined || CD_CLIENTE == "")
		CD_CLIENTE = '';
    if (CD_VENDEDOR == undefined || CD_VENDEDOR == "")
		CD_VENDEDOR = '';
    if (CD_GRUPO == undefined || CD_GRUPO == "")
        CD_GRUPO = '';
    if (CD_MOTIVO_DEVOLUCAO == undefined || CD_MOTIVO_DEVOLUCAO == "")
        CD_MOTIVO_DEVOLUCAO = '';

    //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021
    var ModeloTabela = 'N';
    if ($("#chkModeloTabela").prop('checked')) {
        ModeloTabela = 'S';
    }
    //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

    if (check) {
        //window.location = '../RelatorioRecolhidos.aspx?idKey=' + CD_CLIENTE + "|" + CD_ATIVO_FIXO + "|" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL;
        //window.open(URLSite + '/RelatorioDevolvidos.aspx?idKey=' + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO + "|", '_blank');


        //BEGIN Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021
        //var URL = URLCriptografarChave + "?Conteudo=" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO; -- Antes da melhoria
       
        var URL = URLCriptografarChave + "?Conteudo=" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL + '|' + CD_CLIENTE + "|" + CD_VENDEDOR + "|" + CD_GRUPO + "|" + CD_MOTIVO_DEVOLUCAO + "|" + ModeloTabela;
        //END Melhoria - IM8003789 - Ubirajara Lisboa - 04/02/2021

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
                    window.open(URLSite + '/RelatorioDevolvidos.aspx?idKey=' + res.idKey, '_blank');
                }
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }

        });

    }

    //if (check) {

    //    //var URL = URLObterLista + "?CD_CLIENTE=" + CD_CLIENTE + "&CD_ATIVO_FIXO=" + CD_ATIVO_FIXO + "&DT_DEV_INICIAL=" + DT_DEV_INICIAL + "&DT_DEV_FINAL=" + DT_DEV_FINAL;
    //    var URL = URLImprimir + "?idKey=" + CD_CLIENTE + "|" + CD_ATIVO_FIXO + "|" + DT_DEV_INICIAL + "|" + DT_DEV_FINAL;

    //    $.ajax({
    //        url: URL,
    //        processData: true,
    //        dataType: "json",
    //        contentType: "application/json",
    //        success: function (res) {

    //        },
    //        error: function (res) {
    //            //atualizarPagina();
    //            Alerta("ERRO", res.responseText);
    //        }
    //    });
    //}
});

function carregarComboCliente() {
    
    //BEGIN - IM8003789 - Melhoria - Ubirajara Lisboa - 12/02/2021
    var URL = URLAPI + "ClienteAPI/ObterListaComboPorUsuarioPerfil?nidUsuario=" + nidUsuario;
    //END - IM8003789 - Melhoria - Ubirajara Lisboa - 12/02/2021
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

function checarDatas(DT_DEV_INICIAL, DT_DEV_FINAL) {
    var data_1 = new Date(formatarData(DT_DEV_INICIAL));
    var data_2 = new Date(formatarData(DT_DEV_FINAL));
    var data = new Date();

    if (data_1 > data_2) {
        alert("A data inicial não pode ser maior que a data final!");
        return false;
    //} else if (data_1.getFullYear() < data.getFullYear() - 5) {
    //    alert("A data inicial excedeu o limite de 5 anos!");
    //    return false;
    //} else if (data_2.getFullYear() < data.getFullYear() - 5) {
    //    alert("A data final excedeu o limite de 5 anos!");
    //    return false;
    //} else if (data_1 > data) {
    //    alert("A data inicial não pode ser posterior a hoje!");
    //    return false;
    //} else if (data_2 > data) {
    //    alert("A data final não pode ser posterior a hoje!");
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

function carregarComboMotivo() {
    var URL = URLAPI + "MotivoDevolucaoAPI/ObterLista";
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
            if (res.motivos != null) {
                LoadMotivos(res.motivos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadMotivos(motivos) {
    LimparCombo($("#motivo_CD_MOTIVO_DEVOLUCAO"));

    for (i = 0; i < motivos.length; i++) {
        MontarCombo($("#motivo_CD_MOTIVO_DEVOLUCAO"), motivos[i].CD_MOTIVO_DEVOLUCAO, motivos[i].DS_MOTIVO_DEVOLUCAO);
    }
}
