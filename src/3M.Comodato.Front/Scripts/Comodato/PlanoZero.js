jQuery(document).ready(function () {
    OcultarCampo($('#validaComboGrupoModelo'));
    OcultarCampo($("#validaPeca"));
    OcultarCampo($("#validaQuantidadeMaq"));
    OcultarCampo($("#validaComboCriticidade"));
    OcultarCampo($("#validaQuantidadeMin"));
    OcultarCampo($("#validaPonderacao"));

    $('.js-select-basic-single').select2();
    $('#txtQtMaquina').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 0, allowZero: true }); //.ForceNumericOnly();
    //$('#txtQtEstoqueMin').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 2, allowZero: true }); //.ForceNumericOnly();
    $('#txtPonderacao').maskMoney({ allowNegative: false, thousands: '.', decimal: ',', precision: 2, allowZero: true }); //.ForceNumericOnly();

    popularComboGruposModelo();
    //popularComboModelo();
    //popularComboStatus();
    //popularComboPecas();
});

//function popularComboStatus() {
//    $("#ddlStatus").empty();
//    MontarCombo($("#ddlStatus"), '', 'Selecione...');
//    MontarCombo($("#ddlStatus"), '1', 'Ativo');
//    MontarCombo($("#ddlStatus"), '0', 'Inativo');
//    $('#ddlStatus').val('').trigger('change');
//}

function popularComboGruposModelo() {
    var URL = URLAPI + "GrupoModeloAPI/ObterLista";

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
            if (res.grupoModelos != null) {
                LoadGruposModelo(res.grupoModelos);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });
}

function LoadGruposModelo(gruposModelo) {
    LimparCombo($("#ddlGrupoModelo"));

    for (i = 0; i < gruposModelo.length; i++) {
        MontarCombo($("#ddlGrupoModelo"), gruposModelo[i].ID_GRUPO_MODELO, gruposModelo[i].CD_GRUPO_MODELO);
    }
}

//function popularComboModelo() {
//    var URL = URLAPI + "ModeloAPI/ObterLista";

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: "json",
//        cache: false,
//        contentType: "application/json",
//        beforeSend: function () {
//            $("#loader").css("display", "block");
//        },
//        complete: function () {
//            $("#loader").css("display", "none");
//        },
//        success: function (data) {
//            if (data.MODELO != null) {
//                preencherComboModelo(data.MODELO);popularComboPecas
//                $('#ddlModelo').val('').trigger('change');
//            }
//        },
//        error: function (res) {
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });
//}

//function preencherComboModelo(modeloJO) {
//    LimparCombo($("#ddlModelo"));
//    var modelos = JSON.parse(modeloJO);
//    for (i = 0; i < modelos.length; i++) {
//        MontarCombo($("#ddlModelo"), modelos[i].CD_MODELO, modelos[i].CD_MODELO);
//    }
//}

function popularComboPecas() {

    var url = URLAPI + "PecaApi/ObterListaAtivos";
    var filtroPeca = { FL_ATIVO_PECA: 'S' };
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        data: JSON.stringify(filtroPeca),
        cache: false,
        async: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            var listaPecas = JSON.parse(data.PECA);

            LimparCombo($("#ddlPeca"));
            for (i = 0; i < listaPecas.length; i++) {
                var codigoPeca = listaPecas[i].CD_PECA;
                var descricaoPeca = listaPecas[i].DS_PECA;

                MontarCombo($("#ddlPeca"), codigoPeca, descricaoPeca);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function popularCamposCriticidade(ccdGrupoModelo) {
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        //url: URLAPI + 'PlanoZeroAPI/ObterCriticidadeCarteira?ccdModelo=' + ccdModelo,
        url: URLAPI + 'PlanoZeroAPI/ObterCriticidadeCarteira?ccdGrupoModelo=' + ccdGrupoModelo,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (null != data.CriticidadeCarteira) {
                var criticidadePlano = JSON.parse(data.CriticidadeCarteira);

                document.getElementById("txtA").value = criticidadePlano.nqtCriticidadeA;
                document.getElementById("txtB").value = criticidadePlano.nqtCriticidadeB;
                document.getElementById("txtC").value = criticidadePlano.nqtCriticidadeC;
                document.getElementById("txtTotal").value = criticidadePlano.nqtCriticidadeTotal;
                document.getElementById("txtMaquinasCarteira").value = criticidadePlano.nqtMaquinaCarteira;
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function popularGridPlanoZero(ccdGrupoModelo) {
    var url = actionListarPlanoZero + "?ccdGrupoModelo=" + ccdGrupoModelo;
    atribuirParametrosPaginacao("divGridPlanoZero", actionListarPlanoZero, '{"ccdGrupoModelo":"' + ccdGrupoModelo + '"}');

    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $('#divGridPlanoZero').html(data.Html);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function consultarEstoquePeca(ccdPeca) {
    var url = URLAPI + "EstoquePecaAPI/ListarQuantidadePorTipo";
    var estoquePecaEntity = {
        peca: { CD_PECA: ccdPeca },
        estoque: { TP_ESTOQUE_TEC_3M: '3M1,3M2,TEC' , FL_ATIVO: 'S' }
    };
    //var token = sessionStorage.getItem("token");

    $('#txtQtdEstoqueATec').val('0');
    $('#txtQtdEstoqueRec').val('0');
    $('#txtQtdEstoqueComTec').val('0');
    //$('#ddlPeca').val(ccdPeca).trigger('change');


    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        data: JSON.stringify(estoquePecaEntity),
        cache: false,
        async: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");
            
            if (null != data.PECAS_ESTOQUE && data.PECAS_ESTOQUE != "") {
                var listaEstoquePecas = JSON.parse(data.PECAS_ESTOQUE);

                for (i = 0; i < listaEstoquePecas.length; i++) {
                    switch (listaEstoquePecas[i].TP_ESTOQUE_TEC_3M) {
                        case "3M1":
                            $('#txtQtdEstoqueATec').val(listaEstoquePecas[i].QT_PECA);
                            break;
                        case "3M2":
                            $('#txtQtdEstoqueRec').val(listaEstoquePecas[i].QT_PECA);
                            break;
                        case "TEC":
                            $('#txtQtdEstoqueComTec').val(listaEstoquePecas[i].QT_PECA);
                            break;
                    }
                }
            }
            //if ($('#txtQtdEstoqueATec').val() === 0 &&
            //    $('#txtQtdEstoqueRec').val() === 0 &&
            //    $('#txtQtdEstoqueComTec').val() === 0) {
            //    //popularComboPecas();
            //    $('#ddlPeca').val(ccdPeca).trigger('change');
            //    $('#ddlPeca').text(ccdPeca);
            //}    
            //$('#ddlPeca').val(ccdPeca).trigger('change');
            //$('#ddlPeca').select2('val',ccdPeca);
            //$('#ddlPeca').select2("text", ccdPeca);
            //$('#ddlPeca').select2("val") = ccdPeca;
            $('#ddlPeca').val(ccdPeca).trigger('change');
            //$('#ddlPeca').text(ccdPeca);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });


}

function exibirPlanoZeroModal(idPlano) {
    var url = URLAPI + "PlanoZeroAPI/ObterLista";
    var filtro = { nidPlanoZero: idPlano };
    //var token = sessionStorage.getItem("token");

    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        cache: false,
        async: false,
        data: JSON.stringify(filtro),
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");

            if (null != data.PlanoZero && data.PlanoZero.length > 0) {

                var plano = data.PlanoZero[0];

                $('#ddlPeca').val(plano.ccdPeca).trigger('change');
                $('#txtQtMaquina').val(plano.nqtPecaModelo);
                $('#ddlCriticidade').val(plano.ccdCriticidadeAbc).trigger('change');
                //$('#txtQtEstoqueMin').val(plano.nqtEstoqueMinimo);                

                $('#hidIdPlanoZero').val(idPlano);

                if (plano.nPonderacao == null || plano.nPonderacao == '' || plano.nPonderacao == 0) {
                    $('#txtPonderacao').val('0,00');
                } else {
                    $('#txtPonderacao').val(plano.nPonderacao);
                }                

                consultarEstoquePeca(plano.ccdPeca);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function limparCamposModal() {
    $('#hidIdPlanoZero').val('');
    $("#ddlPeca").val('').trigger('change');

    OcultarCampo($("#validaPeca"));
    OcultarCampo($("#validaQuantidadeMaq"));
    OcultarCampo($("#validaComboCriticidade"));
    OcultarCampo($("#validaQuantidadeMin"));
    OcultarCampo($("#validaPonderacao"));

    $("#txtQtdEstoqueATec").val('');
    $("#txtQtdEstoqueRec").val('');
    $("#txtQtdEstoqueComTec").val('');
    $("#txtPonderacao").val('');
    $("#txtQtMaquina").val('');
    $("#ddlCriticidade").val('').trigger('change');
    $("#txtQtEstoqueMin").val('');
}

function fnSalvarPlanoZero() {
    OcultarCampo($("#validaPeca"));
    OcultarCampo($("#validaQuantidadeMaq"));
    OcultarCampo($("#validaComboCriticidade"));
    OcultarCampo($("#validaQuantidadeMin"));
    OcultarCampo($("#validaPonderacao"));

    if ($('#ddlGrupoModelo').val() == '' || $('#ddlGrupoModelo').val() == 0) {
        //ExibirCampo($('#validaPeca'));
        return false;
    }

    if ($('#ddlPeca').val() == '') {
        ExibirCampo($('#validaPeca'));
        return false;
    }

    var nqtMaquina = $('#txtQtMaquina').val();
    if (nqtMaquina == "" || nqtMaquina == "0" || nqtMaquina == 0) {
        ExibirCampo($('#validaQuantidadeMaq'));
        return false;
    }

    if ($('#ddlCriticidade').val() == '') {
        ExibirCampo($('#validaComboCriticidade'));
        return false;
    }

    if ($('#txtPonderacao').val() == '') {
        ExibirCampo($('#validaPonderacao'));
        return false;
    }

    //var nqtEstoqueMin = $('#txtQtEstoqueMin').maskMoney('unmasked')[0];
    //if (nqtEstoqueMin == "" || nqtEstoqueMin == "0" || nqtEstoqueMin == 0) {
    //    ExibirCampo($('#validaQuantidadeMin'));
    //    return false;
    //}

    var url = URLAPI;
    var nidPlanoZero = $('#hidIdPlanoZero').val();
    var adicionar = true;
    if (nidPlanoZero > 0) {
        url += "PlanoZeroApi/Alterar";
        adicionar = false;
    }
    else {
        url += "PlanoZeroApi/Adicionar";
    }
    //var token = sessionStorage.getItem("token");

    var entityPlanoZero = {
        ccdPeca: $('#ddlPeca :selected').val(),
        ccdModelo: $('#ddlModelo').val(),
        //ccdGrupoModelo: $('#ddlGrupoModelo').val(),
        ccdGrupoModelo: $('#ddlGrupoModelo :selected').text(),
        nqtPecaModelo: $('#txtQtMaquina').val(),
        nPonderacao: 0,
        //nqtEstoqueMinimo: $('#txtQtEstoqueMin').maskMoney('unmasked')[0],
        ccdCriticidadeABC: $('#ddlCriticidade').val(),
        nidPlanoZero: nidPlanoZero,
        nidUsuarioAtualizacao: nidUsuario
    };

    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        cache: false,
        data: JSON.stringify(entityPlanoZero),
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");

            //var ccdGrupoModelo = $('#ddlGrupoModelo').val();
            var ccdGrupoModelo = $('#ddlGrupoModelo :selected').text();

            Alerta("Aviso", MensagemGravacaoSucesso);
            popularCamposCriticidade(ccdGrupoModelo);
            popularGridPlanoZero(ccdGrupoModelo);
            if(adicionar) limparCamposModal();
       },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function fnExcluirPlanoZero(nidPlanoZero) {
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URLAPI + "PlanoZeroAPI/Remover?nidPlanoZero=" + nidPlanoZero + '&nidUsuario=' + nidUsuario,
        dataType: 'json',
        cache: false,
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (data) {
            $("#loader").css("display", "none");

            if (null != data.Mensagem) {
                Alerta("Aviso", JSON.parse(data.Mensagem));
            }

            //popularCamposCriticidade($('#ddlGrupoModelo').val());
            popularCamposCriticidade($('#ddlGrupoModelo :selected').text());
            //popularGridPlanoZero($('#ddlGrupoModelo').val());
            popularGridPlanoZero($('#ddlGrupoModelo :selected').text());
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

$(document).on("click", '.edit-peca', function (e) {    
    var nidPlano = $(this).data('id');    
    limparCamposModal();   
    popularComboPecas();
    exibirPlanoZeroModal(nidPlano);
    //$("#ddlPeca").prop("disabled", true);
});

$('#btnAdicionarPeca').click(function () {
    $("#ddlPeca").prop("disabled", false);
    popularComboPecas();
    
    var ccdGrupoModelo = $("#ddlGrupoModelo").val();
    if (ccdGrupoModelo == "" || ccdGrupoModelo == "0" || ccdGrupoModelo == 0) {
        ExibirCampo($('#validaComboGrupoModelo'));
        return false;
    }
    limparCamposModal();
});

$('#ddlGrupoModelo').on("select2:select", function (e) {
    OcultarCampo($('#validaComboGrupoModelo'));
    $('#txtDescricao').val('');
    //$('#ddlStatus').val('').trigger('change');

    var ccdGrupoModelo = e.params.data.id;
    if (ccdGrupoModelo == "" || ccdGrupoModelo == "0" || ccdGrupoModelo == 0) {
        $("#CodigoGrupo").val("");
        return;
    }

    $("#CodigoGrupo").val(e.params.data.text);

    var url = URLAPI + "GrupoModeloAPI/ObterDados?idGrupoModelo=" + ccdGrupoModelo;
    //var token = sessionStorage.getItem("token");

    //var grupoModeloEntity = { ID_GRUPO_MODELO: ccdGrupoModelo };

    $.ajax({

        type: 'GET',
        url: url,
        dataType: 'json',
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

            ////grupoModeloEntity.DS_GRUPO_MODELO = JSON.parse(res.grupoModelos.DS_GRUPO_MODELO.text); //grupoModeloEntity[0]

            ////if (grupoModeloEntity.length > 0) {
            ////    grupoModeloEntity = grupoModeloEntity[0];
            ////}

            //popularComboGruposModelo();

            $('#txtDescricao').val(res.grupoModelos[0].DS_GRUPO_MODELO);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

    popularCamposCriticidade(e.params.data.text);
    popularGridPlanoZero(e.params.data.text);
});

$('#ddlPeca').on("select2:select", function (e) {
    consultarEstoquePeca(e.params.data.id);
});

$('#btnSalvar').click(function () {
    fnSalvarPlanoZero();
});

$('#btnGerarPlanilha').click(function () {

    var ccdGrupoModelo = $("#CodigoGrupo").val();
    if (ccdGrupoModelo == "" || ccdGrupoModelo == "0" || ccdGrupoModelo == 0) {
        ccdGrupoModelo = "";
    }

    var url = URL + "PlanoZero/DownloadExcel?ccdGrupoModel=" + ccdGrupoModelo;

    $.ajax({
        type: "GET",
        url: url,
        xhrFields: {
            responseType: 'arraybuffer'
        }
    }).done(function (data, status, xmlHeaderRequest) {
        var reader = new FileReader();
        var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        reader.readAsDataURL(blob);

        reader.onloadend = function (e) {
            window.open(reader.result, 'Excel', 'fullscreen=yes,toolbar=0,menubar=0,scrollbars=no', '_blank');
        }
        
    });
});

$('#btnGerarPlanoZero').click(function () {

    ConfirmarSimNao('Aviso', 'Confirma a Geração do Plano Zero?', 'btnGerarPlanoZeroConfirmada()');

});

function btnGerarPlanoZeroConfirmada() {

    var URL = URLAPI + "PlanoZeroAPI/GerarPlanoZero?idUsuario=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            $("#loader").css("display", "block");
        },
        complete: function () {
            $("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.sucesso) {
                Alerta("Aviso", "Geração do Plano Zero solicitada com sucesso! Quando iniciar e finalizar, o sistema enviará e-mails informativos para os destinatários parametrizados!");
            }
            else {
                Alerta("Aviso", res.impeditivoPlanoZero);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", res.responseJSON.Message);
            console.log("teste: ", res.responseJSON.Message);
        }
    });
}


