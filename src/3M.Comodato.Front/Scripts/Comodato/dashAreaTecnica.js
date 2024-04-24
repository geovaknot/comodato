var modalChart = null;
//var chartAtendimento = null;
var chartPeriodoRealizadoMes = null;
var chartAtendimentoTecnicoRegional = null;
var chartTrocaPecaMes = null;
var chartTipoManutencao = null;
var AutoRefresh = true;
setInterval('carregarPagina()', 20000);

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarPagina();
    carregarComboGrupo();
    AutoRefresh = false;
});

//$('#chartAtendimento').click(function () {
//    $('#modalGrafico').modal({
//        show: true
//    });

//    carregarGraficoAtendimento(true);
//});

$('#chartPeriodoRealizadoMes').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoPeriodoRealizadoMes(true);
});

$('#chartAtendimentoTecnicoRegional').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoAtendimentoTecnicoRegional(true);
});

$('#chartTipoManutencao').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoTipoManutencao(true);
});

$('#chartTrocaPecaMes').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoTrocaPecaMes(true);
});

function carregarPagina() {

    if (AutoRefresh == true) {
        carregarBoxClienteAtivo();
        carregarBoxEquipamentoComodato();
        carregarBoxEquipamentoLocado();
        //carregarBoxSolicitacaoAtendimentoPendente();
        //carregarBoxAtendimentoAndamento();
        carregarBoxAtendimentoVisitasAndamento();
        carregarBoxDistribuicaoKAT();
        //carregarBoxAtivoEnviadoNaoInstalado();
        //carregarBoxExcecaoAtendimento();
        carregarBoxAtendimentoAreaTecnica();
        carregarBoxEquipamentoEnviado();
        carregarBoxPecaTrocada();
        carregarBoxPesquisaSatisfacao();

        //carregarGraficoAtendimento(false);
        carregarGraficoPeriodoRealizadoMes(false);
        carregarGraficoAtendimentoTecnicoRegional(false);
        carregarGraficoTipoManutencao(false);
        carregarGraficoTrocaPecaMes(false);

        //carregarGridMVCClienteDistribuicaoGM();
        carregarGridMVCCliente();
        carregarGridMVCTecnico();
    }
}

function carregarBoxClienteAtivo() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxClienteAtivo' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#ClienteAtivo").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxEquipamentoComodato() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxEquipamentoComodatoLocado' + filtros + '&CD_TIPO=1';
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#EquipamentoComodato").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxEquipamentoLocado() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxEquipamentoComodatoLocado' + filtros + '&CD_TIPO=4';
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#EquipamentoLocado").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

    URL = URLAPI + 'DashboardAPI/ObterBoxEquipamentoComodatoLocado' + filtros + '&CD_TIPO=1006';
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#EquipamentoLocadoDI").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

//function carregarBoxSolicitacaoAtendimentoPendente() {
//    var URL = URLAPI + 'DashboardAPI/ObterBoxSolicitacaoAtendimentoPendente' + filtros;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: 'json',
//        contentType: 'application/json',
//        cache: false,
//        async: true,
//        success: function (data) {
//            $("#loader").css("display", "none");
//            $("#Solicitacoes").text(JSON.parse(data.TOTAL));
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });
//}

//function carregarBoxAtendimentoAndamento() {
//    var URL = URLAPI + 'DashboardAPI/ObterBoxAtendimentoAndamento' + filtros;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: 'json',
//        contentType: 'application/json',
//        cache: false,
//        async: true,
//        success: function (data) {
//            $("#loader").css("display", "none");
//            $("#Atendimentos").text(JSON.parse(data.TOTAL));
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });
//}

function carregarBoxAtendimentoVisitasAndamento() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxAtendimentoVisitasAndamento' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#VisitasAndamento").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxDistribuicaoKAT() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxDistribuicaoKAT' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#DistribuicaoKAT_A").text(JSON.parse(data.TOTALA));
            $("#DistribuicaoKAT_B").text(JSON.parse(data.TOTALB));
            $("#DistribuicaoKAT_C").text(JSON.parse(data.TOTALC));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

//function carregarBoxAtivoEnviadoNaoInstalado() {
//    var URL = URLAPI + 'DashboardAPI/ObterBoxAtivoEnviadoNaoInstalado' + filtros;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: 'json',
//        contentType: 'application/json',
//        cache: false,
//        async: true,
//        success: function (data) {
//            $("#loader").css("display", "none");
//            $("#AtivosEnviados").text(JSON.parse(data.TOTALEnviados));
//            $("#AtivosNaoInstalados").text(JSON.parse(data.TOTALNaoInstalados) + ' não instalados');
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });
//}

//function carregarBoxExcecaoAtendimento() {
//    var URL = URLAPI + 'DashboardAPI/ObterBoxExcecaoAtendimento' + filtros;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: 'json',
//        contentType: 'application/json',
//        cache: false,
//        async: true,
//        success: function (data) {
//            $("#loader").css("display", "none");
//            $("#Excecoes").text(JSON.parse(data.TOTAL));
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });
//}

function carregarBoxAtendimentoAreaTecnica() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxAtendimento' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#Atendimento").text(JSON.parse(data.Atendimento));
            $("#TotalPeriodos").text(JSON.parse(data.TotalPeriodos));
            $("#Tecnicos").text(JSON.parse(data.Tecnicos));
            $("#PeriodosRealizados").text(JSON.parse(data.PeriodosRealizados));
            $("#PeriodosPlanejados").text(JSON.parse(data.PeriodosPlanejados));

            $('#PERCENTUAL_GRAFICO_GAP').removeClass();
            if (JSON.parse(data.TIPO_GAP) == '+') {
                $('#boxPeriodo').addClass("card text-black bg-success rounded-0");
                $('#PERCENTUAL_GRAFICO_GAP').addClass("bg-warning");
            }
            else {
                $('#boxPeriodo').addClass("card text-black bg-warning rounded-0");
                $('#PERCENTUAL_GRAFICO_GAP').addClass("bg-danger");
            }
            var PERCENTUAL_GRAFICO_REALIZADO = JSON.parse(data.PERCENTUAL_GRAFICO_REALIZADO);
            var PERCENTUAL_GRAFICO_GAP = JSON.parse(data.PERCENTUAL_GRAFICO_GAP);
            var PERCENTUAL_GRAFICO_RESTANTE = JSON.parse(data.PERCENTUAL_GRAFICO_RESTANTE);

            $("#PERCENTUAL_GRAFICO_REALIZADO").css("width", PERCENTUAL_GRAFICO_REALIZADO + '%');
            if (parseInt(PERCENTUAL_GRAFICO_REALIZADO) >= 10)
                $("#PERCENTUAL_GRAFICO_REALIZADO_label").text(PERCENTUAL_GRAFICO_REALIZADO + '%');
            else
                $("#PERCENTUAL_GRAFICO_REALIZADO_label").text('');
            $('#PERCENTUAL_GRAFICO_REALIZADO').prop('title', PERCENTUAL_GRAFICO_REALIZADO + '%');
            $('#PERCENTUAL_GRAFICO_REALIZADO_label').prop('title', PERCENTUAL_GRAFICO_REALIZADO + '%');

            $("#PERCENTUAL_GRAFICO_GAP").css("width", PERCENTUAL_GRAFICO_GAP + '%');
            if (parseInt(PERCENTUAL_GRAFICO_GAP) >= 20)
                $("#PERCENTUAL_GRAFICO_GAP_label").text(PERCENTUAL_GRAFICO_GAP + '%');
            else
                $("#PERCENTUAL_GRAFICO_GAP_label").text('');
            $('#PERCENTUAL_GRAFICO_GAP').prop('title', PERCENTUAL_GRAFICO_GAP + '%');
            $('#PERCENTUAL_GRAFICO_GAP_label').prop('title', PERCENTUAL_GRAFICO_GAP + '%');

            $("#PERCENTUAL_GRAFICO_RESTANTE").css("width", PERCENTUAL_GRAFICO_RESTANTE + '%');
            if (parseInt(PERCENTUAL_GRAFICO_RESTANTE) >= 10)
                $("#PERCENTUAL_GRAFICO_RESTANTE_label").text(PERCENTUAL_GRAFICO_RESTANTE + '%');
            else
                $("#PERCENTUAL_GRAFICO_RESTANTE_label").text('');
            $('#PERCENTUAL_GRAFICO_RESTANTE').prop('title', PERCENTUAL_GRAFICO_RESTANTE + '%');
            $('#PERCENTUAL_GRAFICO_RESTANTE_label').prop('title', PERCENTUAL_GRAFICO_RESTANTE + '%');
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarBoxEquipamentoEnviado() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxEquipamentoEnviado' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#EquipamentoEnviado").text(JSON.parse(data.TOTALEnviado));
            $("#EquipamentoDevolucao").text(JSON.parse(data.TOTALDevolucao));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

//function carregarGraficoAtendimento(modalWindow) {
//    var URL = URLAPI + 'DashboardAPI/ObterGraficoAtendimento' + filtros;

//    $.ajax({
//        type: 'POST',
//        url: URL,
//        dataType: 'json',
//        contentType: 'application/json',
//        cache: false,
//        async: true,
//        success: function (data) {
//            $("#loader").css("display", "none");
//            if (data.ATENDIMENTO != null) {
//                var lista = JSON.parse(data.ATENDIMENTO);
//                var jsonLabel = [], jsonData = [];
//                for (i = 0; i < lista.length; i++) {
//                    jsonLabel.push(lista[i].TITULO);
//                    jsonData.push(parseFloat(lista[i].TOTAL));
//                }
//            }
//            //comboPecasEstoque.val('').trigger('change');
//            carregarGraficoAtendimentoData(modalWindow, jsonLabel, jsonData);
//        },
//        error: function (res) {
//            $("#loader").css("display", "none");
//            Alerta("ERRO", JSON.parse(res.responseText).Message);
//        }
//    });

//}

//function carregarGraficoAtendimentoData(modalWindow, jsonLabel, jsonData) {

//    var ctx;

//    if (modalWindow == true) {
//        ctx = document.getElementById("chartGrafico").getContext('2d');
//        if (modalChart != null)
//            modalChart.destroy();
//    }
//    else {
//        ctx = document.getElementById("chartAtendimento").getContext('2d');
//        if (chartAtendimento != null)
//            chartAtendimento.destroy();
//    }

//    /*var myChart*/ chartAtendimento = new Chart(ctx, {
//        type: 'horizontalBar',
//        data: {
//            labels: jsonLabel, //["Treinamento", "Instalação", "Atend.Corret.", "Atend.Prev.", "Em Andamento", "Solic.Corret.", "Solic.Prev.", "Solicitadas"],
//            datasets: [{
//                //label: 'Bar Dataset',
//                data: jsonData, //[0, 2, 2, 3, 4, 4, 5, 7],
//                backgroundColor: 'rgba(58, 183, 40, 0.2)', //[
//                //'rgba(255, 99, 132, 0.2)',
//                //'rgba(54, 162, 235, 0.2)',
//                //'rgba(255, 206, 86, 0.2)',
//                //'rgba(75, 192, 192, 0.2)',
//                //'rgba(153, 102, 255, 0.2)',
//                //'rgba(255, 159, 64, 0.2)'
//                //],
//                borderColor: 'rgba(58, 183, 40, 1)', //[
//                //'rgba(255,99,132,1)',
//                //'rgba(54, 162, 235, 1)',
//                //'rgba(255, 206, 86, 1)',
//                //'rgba(75, 192, 192, 1)',
//                //'rgba(153, 102, 255, 1)',
//                //'rgba(255, 159, 64, 1)'
//                //],
//                borderWidth: 1,
//                steppedLine: 'before'
//            }]
//        },
//        options: {
//            responsive: true,
//            legend: {
//                display: false
//            },
//            scales: {
//                yAxes: [{
//                    ticks: {
//                        beginAtZero: true
//                    }
//                }]
//            }
//        }
//    });

//    if (modalWindow == true)
//        modalChart = chartAtendimento;
//}

function carregarBoxPecaTrocada() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxPecaTrocada' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#Pecas").text(JSON.parse(data.TOTAL));

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxPesquisaSatisfacao() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxPesquisaSatisfacao' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            $("#PesquisaSatisfacao").text(JSON.parse(data.TOTAL));

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarGraficoPeriodoRealizadoMes(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoPeriodoRealizadoMes' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.PERIODO != null) {
                var lista = JSON.parse(data.PERIODO);
                var jsonLabel = [], jsonData = [], jsonDataMeta = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].DS_MES);
                    jsonData.push(parseFloat(lista[i].TOTAL));
                    jsonDataMeta.push(parseFloat(lista[i].META));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoPeriodoRealizadoMesData(modalWindow, jsonLabel, jsonData, jsonDataMeta);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoPeriodoRealizadoMesData(modalWindow, jsonLabel, jsonData, jsonDataMeta) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartPeriodoRealizadoMes").getContext('2d');
        if (chartPeriodoRealizadoMes != null)
            chartPeriodoRealizadoMes.destroy();
    }

    /*var myChart*/ chartPeriodoRealizadoMes = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
                label: 'Período',
                data: jsonData, //[120, 135, 145, 129, 155, 160, 0, 0, 0, 0, 0, 0],
                backgroundColor: 'rgba(153, 102, 255, 0.2)', //[
                //'rgba(255, 99, 132, 0.2)',
                //'rgba(54, 162, 235, 0.2)',
                //'rgba(255, 206, 86, 0.2)',
                //'rgba(75, 192, 192, 0.2)',
                //'rgba(153, 102, 255, 0.2)',
                //'rgba(255, 159, 64, 0.2)'
                //],
                borderColor: 'rgba(153, 102, 255, 1)', //[
                //'rgba(255,99,132,1)',
                //'rgba(54, 162, 235, 1)',
                //'rgba(255, 206, 86, 1)',
                //'rgba(75, 192, 192, 1)',
                //'rgba(153, 102, 255, 1)',
                //'rgba(255, 159, 64, 1)'
                //],
                borderWidth: 1
            },
            {
                label: 'Meta',
                data: jsonDataMeta, //[28, 38, 48, 58, 68, 78, 88, 98, 108, 118, 128, 138],
                backgroundColor: 'transparent',
                borderColor: 'rgba(236, 70, 47, 1)',
                borderWidth: 1,
                type: 'line'
            }]
        },
        options: {
            responsive: true,
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    if (modalWindow == true)
        modalChart = chartPeriodoRealizadoMes;
}

function carregarGraficoAtendimentoTecnicoRegional(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoAtendimentoTecnicoRegional' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.ATENDIMENTO != null) {
                var lista = JSON.parse(data.ATENDIMENTO);
                var jsonLabel = [], jsonDataREALIZADO = [], jsonDataPLANEJADO = [];
                for (i = 0; i < lista.length; i++) {
                    // jsonLabel.push(lista[i].tecnico.NM_TECNICO);
                    jsonLabel.push(lista[i].tecnico.NM_REDUZIDO);
                    jsonDataREALIZADO.push(parseFloat(lista[i].TOTAL_VISITA_REALIZADO));
                    jsonDataPLANEJADO.push(parseFloat(lista[i].TOTAL_VISITA_PLANEJADO));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoAtendimentoTecnicoRegionalData(modalWindow, jsonLabel, jsonDataREALIZADO, jsonDataPLANEJADO);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoAtendimentoTecnicoRegionalData(modalWindow, jsonLabel, jsonDataREALIZADO, jsonDataPLANEJADO) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartAtendimentoTecnicoRegional").getContext('2d');
        if (chartAtendimentoTecnicoRegional != null)
            chartAtendimentoTecnicoRegional.destroy();
    }

    /*var myChart*/ chartAtendimentoTecnicoRegional = new Chart(ctx, {
        type: 'horizontalBar',
        //type: 'bar',
        data: {
            labels: jsonLabel, //["Flavia", "Lima", "Rafael", "George", "Paulo", "Walter"],
            datasets: [{
                label: 'Kat Realizado',
                data: jsonDataREALIZADO, 
                backgroundColor: 'rgba(54, 162, 235, 0.2)', 
                borderColor: 'rgba(54, 162, 235, 1)', 
                borderWidth: 1
            },
            {
                label: 'Kat Planejado',
                data: jsonDataPLANEJADO, 
                backgroundColor: 'rgba(153, 102, 255, 0.2)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 1
            }
            ]
        },
        options: {
            responsive: true,
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    },
                    //stacked: true
                }]//,
                //xAxes: [{
                //    stacked: true
                //}]
            }
        }
    });

    if (modalWindow == true)
        modalChart = chartAtendimentoTecnicoRegional;
}

function carregarGraficoTipoManutencao(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoAtendimento' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.ATENDIMENTO != null) {
                var lista = JSON.parse(data.ATENDIMENTO);
                var jsonLabel = [], jsonData = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].TITULO);
                    jsonData.push(parseFloat(lista[i].TOTAL));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoTipoManutencaoData(modalWindow, jsonLabel, jsonData);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoTipoManutencaoData(modalWindow, jsonLabel, jsonData) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartTipoManutencao").getContext('2d');
        if (chartTipoManutencao != null)
            chartTipoManutencao.destroy();
    }

    /*var myChart*/ chartTipoManutencao = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: jsonLabel, //["Preventiva", "Corretiva", "Instalação", "Treinamento"],
            datasets: [{
                //label: 'Bar Dataset',
                data: jsonData, //[238, 115, 29, 22],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)'//,
                    //'rgba(153, 102, 255, 0.2)',
                    //'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)'//,
                    //'rgba(153, 102, 255, 1)',
                    //'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            legend: {
                display: true,
                position: 'left'
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    },
                    display: false
                }]
            }
        }
    });

    if (modalWindow == true)
        modalChart = chartTipoManutencao;
}

function carregarGraficoTrocaPecaMes(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoTrocaPecaMes' + filtros;
    //var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'POST',
        url: URL,
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        },
        success: function (data) {
            $("#loader").css("display", "none");
            if (data.PECA != null) {
                var lista = JSON.parse(data.PECA);
                var jsonLabel = [], jsonDataTOTAL = [], jsonDataHORAS = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].DS_MES);
                    jsonDataTOTAL.push(parseFloat(lista[i].TOTAL));
                    jsonDataHORAS.push(parseFloat(lista[i].QT_HORAS));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoTrocaPecaMesData(modalWindow, jsonLabel, jsonDataTOTAL, jsonDataHORAS);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoTrocaPecaMesData(modalWindow, jsonLabel, jsonDataTOTAL, jsonDataHORAS) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartTrocaPecaMes").getContext('2d');
        if (chartTrocaPecaMes != null)
            chartTrocaPecaMes.destroy();
    }

    /*var myChart*/ chartTrocaPecaMes = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
                label: 'Peças',
                data: jsonDataTOTAL, //[44, 68, 93, 71, 82, 109, 91, 120, 89, 84, 80, 102],
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            },
            {
                label: 'Horas',
                data: jsonDataHORAS, //[64, 70, 96, 78, 82, 35, 44, 174, 63, 49, 23, 29],
                backgroundColor: 'rgba(255, 159, 64, 0.5)',
                borderColor: 'rgba(255, 159, 64, 1)',
                borderWidth: 1//,
                //type: 'bar'
            }
            ]
        },
        options: {
            responsive: true,
            legend: {
                display: true,
                position: 'bottom'
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    if (modalWindow == true)
        modalChart = chartTrocaPecaMes;
}

function carregarGridMVCCliente() {
    var URL = URLObterListaClienteVisaoResumidaAreaTecnica + filtros;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                $('#gridMVCCliente').html(res.Html);
                $(function () {
                    var m = $('table tr th:contains("VENDAS ANTERIOR")');
                    //m.text('VENDAS ' + ((new Date).getFullYear() - 1).toString().substr(2, 2));
                    m.text('VENDAS ' + ((new Date).getFullYear() - 1));
                });
                $(function () {
                    var m = $('table tr th:contains("VENDAS ATUAL")');
                    //m.text('VENDAS ' + (new Date).getFullYear().toString().substr(2, 2));
                    m.text('VENDAS ' + (new Date).getFullYear());
                });
                $(function () {
                    var m = $('table tr th:contains("GM ANTERIOR")');
                    //m.text('GM ' + ((new Date).getFullYear() - 1).toString().substr(2, 2));
                    m.text('GM ' + ((new Date).getFullYear() - 1));
                });
                $(function () {
                    var m = $('table tr th:contains("GM ATUAL")');
                    //m.text('GM ' + (new Date).getFullYear().toString().substr(2, 2));
                    m.text('GM ' + (new Date).getFullYear());
                });
                $("#tableMVCCliente").freezeHeaderDash({ 'height': '315px' });
                //$("#hdScrolltableMVCCliente").addClass("mx-0 my-0 px-0 py-0");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarGridMVCTecnico() {
    var URL = URLObterListaTecnico + filtros;

    $.ajax({
        url: URL,
        processData: true,
        dataType: "json",
        cache: false,
        async: true,
        contentType: "application/json",
        //beforeSend: function () {
        //    $("#loader").css("display", "block");
        //},
        //complete: function () {
        //    $("#loader").css("display", "none");
        //},
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Success") {
                $('#gridMVCTecnico').html(res.Html);
                $("#tableMVCTecnico").freezeHeaderDash({ 'height': '315px' });
                //$("#hdScrolltableMVCCliente").addClass("mx-0 my-0 px-0 py-0");
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}
