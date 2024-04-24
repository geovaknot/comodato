var modalChart = null;
var chartValorPecaEnviadaMes = null;
var chartMaquinaEnviadaDevolvidaMes = null;
var chartEnvioEquipamentoLinhaProd = null;
var chartTipoEnvioEquipamento = null;
var chartFamiliaModelo = null;
var AutoRefresh = true;
setInterval('carregarPagina()', 20000);

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarPagina();
    carregarComboGrupo();
    carregarComboModelo();
    //carregarComboAtivos();
    AutoRefresh = false;
});

$('#chartValorPecaEnviadaMes').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoValorPecaEnviadaMes(true);
});

$('#chartMaquinaEnviadaDevolvidaMes').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoMaquinaEnviadaDevolvidaMes(true);
});

$('#chartEnvioEquipamentoLinhaProd').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoEnvioEquipamentoLinhaProd(true);
});

$('#chartTipoEnvioEquipamento').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoTipoEnvioEquipamento(true);
});

$('#chartFamiliaModelo').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoFamiliaModelo(true);
});

function carregarPagina() {

    if (AutoRefresh == true) {
        carregarBoxClienteAtivo();
        carregarBoxEquipamentoComodato();
        carregarBoxEquipamentoLocado();
        carregarBoxEquipamentoEnviado();
        carregarBoxProjEnvioEquip();
        carregarBoxPecaEnviada();
        carregarBoxAtendimento();
        carregarBoxEnvioEquipamento();

        carregarGraficoValorPecaEnviadaMes(false);
        carregarGraficoMaquinaEnviadaDevolvidaMes(false);
        carregarGraficoEnvioEquipamentoLinhaProd(false);
        carregarGraficoTipoEnvioEquipamento(false);
        carregarGraficoFamiliaModelo(false);

        //carregarGraficoTeste(false);

        carregarGridMVCCliente();
        carregarGridMVCEquipamento();
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

    //   URL = URLAPI + 'DashboardAPI/ObterBoxEquipamentoComodatoLocado' + filtros + '&CD_TIPO=1006';
    //
    //   $.ajax({
    //       type: 'POST',
    //       url: URL,
    //       dataType: 'json',
    //       contentType: 'application/json',
    //       cache: false,
    //       async: true,
    //       success: function (data) {
    //           $("#loader").css("display", "none");
    //           $("#EquipamentoLocadoDI").text(JSON.parse(data.TOTAL));
    //       },
    //       error: function (res) {
    //           $("#loader").css("display", "none");
    //           Alerta("ERRO", JSON.parse(res.responseText).Message);
    //       }
    //   });
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

function carregarBoxProjEnvioEquip() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxProjEnvioEquip' + filtros;
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
            $("#ProjEnvioEquip").text(JSON.parse(data.TOTAL));

            //$("#PeriodosTitulo").text(JSON.parse(data.PeriodosTitulo));
            //$("#Periodos").text(JSON.parse(data.TOTAL));

            $('#boxPeriodo_WF').removeClass();
            $('#PERCENTUAL_GRAFICO_GAP_WF').removeClass();
            if (JSON.parse(data.TIPO_GAP) == '+') {
                $('#boxPeriodo_WF').addClass("card text-black bg-success rounded-0");
                $('#PERCENTUAL_GRAFICO_GAP_WF').addClass("bg-warning");
            }
            else {
                $('#boxPeriodo_WF').addClass("card text-black bg-warning rounded-0");
                $('#PERCENTUAL_GRAFICO_GAP_WF').addClass("bg-danger");
            }
            var PERCENTUAL_GRAFICO_REALIZADO = JSON.parse(data.PERCENTUAL_GRAFICO_REALIZADO);
            var PERCENTUAL_GRAFICO_GAP = JSON.parse(data.PERCENTUAL_GRAFICO_GAP);
            var PERCENTUAL_GRAFICO_RESTANTE = JSON.parse(data.PERCENTUAL_GRAFICO_RESTANTE);

            $("#PERCENTUAL_GRAFICO_REALIZADO_WF").css("width", PERCENTUAL_GRAFICO_REALIZADO + '%');
            if (parseInt(PERCENTUAL_GRAFICO_REALIZADO) >= 20)
                $("#PERCENTUAL_GRAFICO_REALIZADO_WF_label").text(PERCENTUAL_GRAFICO_REALIZADO + '%');
            else
                $("#PERCENTUAL_GRAFICO_REALIZADO_WF_label").text('');
            $('#PERCENTUAL_GRAFICO_REALIZADO_WF').prop('title', PERCENTUAL_GRAFICO_REALIZADO + '%');
            $('#PERCENTUAL_GRAFICO_REALIZADO_WF_label').prop('title', PERCENTUAL_GRAFICO_REALIZADO + '%');

            $("#PERCENTUAL_GRAFICO_GAP_WF").css("width", PERCENTUAL_GRAFICO_GAP + '%');
            if (parseInt(PERCENTUAL_GRAFICO_GAP) >= 20)
                $("#PERCENTUAL_GRAFICO_GAP_WF_label").text(PERCENTUAL_GRAFICO_GAP + '%');
            else
                $("#PERCENTUAL_GRAFICO_GAP_WF_label").text('');
            $('#PERCENTUAL_GRAFICO_GAP_WF').prop('title', PERCENTUAL_GRAFICO_GAP + '%');
            $('#PERCENTUAL_GRAFICO_GAP_WF_label').prop('title', PERCENTUAL_GRAFICO_GAP + '%');

            $("#PERCENTUAL_GRAFICO_RESTANTE_WF").css("width", PERCENTUAL_GRAFICO_RESTANTE + '%');
            if (parseInt(PERCENTUAL_GRAFICO_RESTANTE) >= 20)
                $("#PERCENTUAL_GRAFICO_RESTANTE_WF_label").text(PERCENTUAL_GRAFICO_RESTANTE + '%');
            else
                $("#PERCENTUAL_GRAFICO_RESTANTE_WF_label").text('');
            $('#PERCENTUAL_GRAFICO_RESTANTE_WF').prop('title', PERCENTUAL_GRAFICO_RESTANTE + '%');
            $('#PERCENTUAL_GRAFICO_RESTANTE_WF_label').prop('title', PERCENTUAL_GRAFICO_RESTANTE + '%');

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxPecaEnviada() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxPecaEnviada' + filtros;
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
            $("#PecaEnviada").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxAtendimento() {
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
            $("#loader").css("display", "none");
            $("#Atendimento").text(JSON.parse(data.Atendimento));
            $("#TotalPeriodos").text(JSON.parse(data.TotalPeriodos));
            $("#PeriodosRealizados").text(JSON.parse(data.PeriodosRealizados));
            $("#PeriodosPlanejados").text(JSON.parse(data.PeriodosPlanejados));
            $("#VisitasRealizadas").text(JSON.parse(data.VisitasRealizadas));
            $("#OSRealizadas").text(JSON.parse(data.OSRealizadas));
            //$("#ClientesAtendidos").text(JSON.parse(data.ClientesAtendidos));
            $("#Tecnicos").text(JSON.parse(data.Tecnicos));
            $("#Vigencia").text(JSON.parse(data.Vigencia));
            $("#ClientesPerdidos").text(JSON.parse(data.ClientesPerdidos));
            $("#ValorPecaEnviada").text(JSON.parse(data.ValorPecaEnviada));
            $("#ValorMetaPecaRecupEnviado").text(JSON.parse(data.ValorMetaPecaRecupEnviado));
            $("#ValorPecaEnviada3M1").text(JSON.parse(data.ValorPecaEnviada3M1));
            $("#ValorPecaEnviadaRec").text(JSON.parse(data.ValorPecaEnviadaRec));
            $("#ValorPecaRecuperadaMes").text(JSON.parse(data.ValorPecaRecuperadaMes));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarBoxEnvioEquipamento() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxEnvioEquipamento' + filtros;
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
            $("#EnvioEquipamentoITEM_1").text(JSON.parse(data.EnvioEquipamentoITEM_1));
            $("#EnvioEquipamentoTOTAL_1").text(JSON.parse(data.EnvioEquipamentoTOTAL_1));

            $("#EnvioEquipamentoITEM_2").text(JSON.parse(data.EnvioEquipamentoITEM_2));
            $("#EnvioEquipamentoTOTAL_2").text(JSON.parse(data.EnvioEquipamentoTOTAL_2));

            $("#EnvioEquipamentoITEM_3").text(JSON.parse(data.EnvioEquipamentoITEM_3));
            $("#EnvioEquipamentoTOTAL_3").text(JSON.parse(data.EnvioEquipamentoTOTAL_3));

            $("#EnvioEquipamentoITEM_4").text(JSON.parse(data.EnvioEquipamentoITEM_4));
            $("#EnvioEquipamentoTOTAL_4").text(JSON.parse(data.EnvioEquipamentoTOTAL_4));

            $("#EnvioEquipamentoITEM_5").text(JSON.parse(data.EnvioEquipamentoITEM_5));
            $("#EnvioEquipamentoTOTAL_5").text(JSON.parse(data.EnvioEquipamentoTOTAL_5));

            $("#EnvioEquipamentoITEM_6").text(JSON.parse(data.EnvioEquipamentoITEM_6));
            $("#EnvioEquipamentoTOTAL_6").text(JSON.parse(data.EnvioEquipamentoTOTAL_6));

            $("#EnvioEquipamentoITEM_7").text(JSON.parse(data.EnvioEquipamentoITEM_7));
            $("#EnvioEquipamentoTOTAL_7").text(JSON.parse(data.EnvioEquipamentoTOTAL_7));

            $("#EnvioEquipamentoITEM_8").text(JSON.parse(data.EnvioEquipamentoITEM_8));
            $("#EnvioEquipamentoTOTAL_8").text(JSON.parse(data.EnvioEquipamentoTOTAL_8));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoValorPecaEnviadaMes(modalWindow) {

    var URL = URLAPI + 'DashboardAPI/ObterGraficoValorPecaEnviadaMes' + filtros;
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

            if (data.VALORPECAS != null) {
                var lista = JSON.parse(data.VALORPECAS);
                var jsonLabel = [], jsonData3M1 = [], jsonData3M2 = [], jsonData3M3 = [], jsonData3M4 = [], jsonDataMeta = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].cdsMes);
                    jsonData3M1.push(parseFloat(lista[i].TOTAL_3M1));
                    jsonData3M2.push(parseFloat(lista[i].TOTAL_3M2));
                    jsonData3M3.push(parseFloat(lista[i].TOTAL_3M3));
                    jsonData3M4.push(parseFloat(lista[i].TOTAL_3M4));
                    //     jsonDataMeta.push(parseFloat(lista[i].TOTAL_METAS));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoValorPecaEnviadaMesDados(modalWindow, jsonLabel, jsonData3M1, jsonData3M2, jsonData3M3, jsonData3M4, jsonDataMeta);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoValorPecaEnviadaMesDados(modalWindow, jsonLabel, jsonData3M1, jsonData3M2, jsonData3M3, jsonData3M4,jsonDataMeta) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartValorPecaEnviadaMes").getContext('2d');
        if (chartValorPecaEnviadaMes != null)
            chartValorPecaEnviadaMes.destroy();
    }

    /*var myChart*/ chartValorPecaEnviadaMes = new Chart(ctx, {
        type: 'line',
        data: {
            labels: jsonLabel, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
                label: '3M1',
                data: jsonData3M1, //[120, 135, 145, 129, 155, 160, 0, 0, 0, 0, 0, 0],
                //backgroundColor: 'rgba(0, 0, 255, 0.2)',
                //'rgba(54, 162, 235, 0.2)', //[
                //'rgba(255, 99, 132, 0.2)',
                //'rgba(54, 162, 235, 0.2)',
                //'rgba(255, 206, 86, 0.2)',
                //'rgba(75, 192, 192, 0.2)',
                //'rgba(153, 102, 255, 0.2)',
                //'rgba(255, 159, 64, 0.2)'
                //],
                borderColor: 'rgba(0, 0, 255, 1)',
                //'rgba(54, 162, 235, 1)', //[
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
                label: 'REC',
                data: jsonData3M2, //[120, 135, 145, 129, 155, 160, 0, 0, 0, 0, 0, 0],
                //backgroundColor: 'rgba(255, 0, 0, 1)',
                //'rgba(153, 102, 255, 0.2)', //[
                //'rgba(255, 99, 132, 0.2)',
                //'rgba(54, 162, 235, 0.2)',
                //'rgba(255, 206, 86, 0.2)',
                //'rgba(75, 192, 192, 0.2)',
                //'rgba(153, 102, 255, 0.2)',
                //'rgba(255, 159, 64, 0.2)'
                //],
                borderColor: 'rgba(255, 0, 0, 1)', 
                //'rgba(153, 102, 255, 1)', //[
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
            label: 'PL1',
            data: jsonData3M3, //[120, 135, 145, 129, 155, 160, 0, 0, 0, 0, 0, 0],
            borderDash: [5, 3], // borderDash: [Tamanho das Dashes, Distancia entre as Dashes]
            backgroundColor: 'rgba(0, 0, 255, 0.1)', //[
            //'rgba(255, 99, 132, 0.2)',
            //'rgba(54, 162, 235, 0.2)',
            //'rgba(255, 206, 86, 0.2)',
            //'rgba(75, 192, 192, 0.2)',
            //'rgba(153, 102, 255, 0.2)',
            //'rgba(255, 159, 64, 0.2)'
            //],
            borderColor: 'rgba(0, 0, 255, 0.8)', //[
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
            label: 'PL-REC',
            data: jsonData3M4, //[120, 135, 145, 129, 155, 160, 0, 0, 0, 0, 0, 0],
            borderDash: [5,3], // borderDash: [Tamanho das Dashes, Distancia entre as Dashes]
            backgroundColor: 'rgba(255, 0, 0, 0.1)', //[
            //'rgba(255, 99, 132, 0.2)',
            //'rgba(54, 162, 235, 0.2)',
            //'rgba(255, 206, 86, 0.2)',
            //'rgba(75, 192, 192, 0.2)',
            //'rgba(153, 102, 255, 0.2)',
            //'rgba(255, 159, 64, 0.2)'
            //],
            borderColor: 'rgba(255, 0, 0, 0.8)', //[
            //'rgba(255,99,132,1)',
            //'rgba(54, 162, 235, 1)',
            //'rgba(255, 206, 86, 1)',
            //'rgba(75, 192, 192, 1)',
            //'rgba(153, 102, 255, 1)',
            //'rgba(255, 159, 64, 1)'
            //],
            borderWidth: 1
        }],

            //{   Comentado em 09/08/2019
            //
            //    label: 'Meta',
            //    data: jsonDataMeta, //[128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128],
            //    backgroundColor: 'transparent',
            //    borderColor: 'rgba(236, 70, 47, 1)',
            //    borderWidth: 1,
            //    type: 'line'
            //}]
        },
        options: {
            responsive: true,
            legend: {
                display: true,
                position: 'bottom'
            }//,
            //scales: {
            //    xAxes: [{
            //        //stacked: true,
            //    }],
            //    yAxes: [{
            //        //stacked: true,
            //        ticks: {
            //            beginAtZero: true
            //        }
            //    }]
            //}
        }
    });

    if (modalWindow == true)
        modalChart = chartValorPecaEnviadaMes;
}

function carregarGraficoMaquinaEnviadaDevolvidaMes(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoMaquinaEnviadaDevolvidaMes' + filtros;
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

            if (data.LISTAMAQUINA != null) {
                var lista = JSON.parse(data.LISTAMAQUINA);
                var jsonLabel = [], jsonDataENVIO = [], jsonDataDEVOLUCAO = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].DS_MES);
                    jsonDataENVIO.push(parseFloat(lista[i].TOTAL_ENVIO));
                    jsonDataDEVOLUCAO.push(parseFloat(lista[i].TOTAL_DEVOLUCAO));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoMaquinaEnviadaDevolvidaMesDados(modalWindow, jsonLabel, jsonDataENVIO, jsonDataDEVOLUCAO);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoMaquinaEnviadaDevolvidaMesDados(modalWindow, jsonLabel, jsonDataENVIO, jsonDataDEVOLUCAO) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartMaquinaEnviadaDevolvidaMes").getContext('2d');
        if (chartMaquinaEnviadaDevolvidaMes != null)
            chartMaquinaEnviadaDevolvidaMes.destroy();
    }

    /*var myChart*/ chartMaquinaEnviadaDevolvidaMes = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
                label: 'Envio',
                data: jsonDataENVIO, //[44, 68, 93, 71, 82, 109, 91, 120, 89, 84, 80, 102],
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            },
            {
                label: 'Devolução',
                data: jsonDataDEVOLUCAO, //[64, 70, 96, 78, 82, 35, 44, 174, 63, 49, 23, 29],
                backgroundColor: 'rgba(255, 159, 64, 0.5)',
                borderColor: 'rgba(255, 159, 64, 1)',
                borderWidth: 1,
                type: 'bar'
            }]
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
        modalChart = chartMaquinaEnviadaDevolvidaMes;
}

function carregarGraficoEnvioEquipamentoLinhaProd(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoEnvioEquipamentoLinhaProd' + filtros;
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
            if (data.LISTAENVIO != null) {
                var lista = JSON.parse(data.LISTAENVIO);

                var jsonLabel = [], jsonDataIDENTIFICACAO = [], jsonDataFECHADOR = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].DS_MES);
                    //jsonDataUNITIZACAO.push(parseFloat(lista[i].TOTAL_UNITIZACAO));
                    jsonDataIDENTIFICACAO.push(parseFloat(lista[i].TOTAL_IDENTIFICACAO));
                    jsonDataFECHADOR.push(parseFloat(lista[i].TOTAL_FECHADOR));
                }

                //var jsonLabel = [], jsonDataROLO = [], jsonDataIDENTIFICACAO = [];
                //for (i = 0; i < lista.length; i++) {
                //    jsonLabel.push(lista[i].DS_MES);
                //    jsonDataROLO.push(parseFloat(lista[i].TOTAL_ROLO));
                //    jsonDataIDENTIFICACAO.push(parseFloat(lista[i].TOTAL_IDENTIFICACAO));
                //}
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoEnvioEquipamentoLinhaProdDados(modalWindow, jsonLabel, jsonDataIDENTIFICACAO, jsonDataFECHADOR);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoEnvioEquipamentoLinhaProdDados(modalWindow, jsonLabel, jsonDataIDENTIFICACAO, jsonDataFECHADOR) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartEnvioEquipamentoLinhaProd").getContext('2d');
        if (chartEnvioEquipamentoLinhaProd != null)
            chartEnvioEquipamentoLinhaProd.destroy();
    }

    /*var myChart*/ chartEnvioEquipamentoLinhaProd = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
            //    label: 'Unitização',
            //    data: jsonDataUNITIZACAO, //[24, 55, 75, 42, 60, 77, 54, 66, 58, 50, 59, 80],
            //    backgroundColor: 'rgba(58, 183, 40, 0.5)',
            //    borderColor: 'rgba(58, 183, 40, 1)',
            //    borderWidth: 1
            //},
            
                label: 'Identificação',
                data: jsonDataIDENTIFICACAO, //[20, 13, 18, 29, 22, 32, 37, 54, 31, 34, 21, 17],
                backgroundColor: 'rgba(214, 90, 31, 0.5)',
                borderColor: 'rgba(214, 90, 31, 1)',
                borderWidth: 1,
                type: 'bar'
            },
            {
                label: 'Fechador',
                data: jsonDataFECHADOR, //[20, 13, 18, 29, 22, 32, 37, 54, 31, 34, 21, 17],
                backgroundColor: 'rgba(58, 90, 214, 0.5)',
                borderColor: 'rgba(58, 90, 214, 1)',
                borderWidth: 1,
                type: 'bar'
            }]
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
        modalChart = chartEnvioEquipamentoLinhaProd;
}

function carregarGraficoTipoEnvioEquipamento(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoTipoEnvioEquipamento' + filtros;
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

            if (data.TIPOSOLICITACAO != null) {
                var lista = JSON.parse(data.TIPOSOLICITACAO);
                var jsonLabel = [], jsonData = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].wfTipoSolicitacao.DS_TIPO_SOLICITACAO);
                    jsonData.push(parseFloat(lista[i].TOTAL));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoTipoEnvioEquipamentoData(modalWindow, jsonLabel, jsonData);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoTipoEnvioEquipamentoData(modalWindow, jsonLabel, jsonData) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartTipoEnvioEquipamento").getContext('2d');
        if (chartTipoEnvioEquipamento != null)
            chartTipoEnvioEquipamento.destroy();
    }

    /*var myChart*/ chartTipoEnvioEquipamento = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: jsonLabel, //["NC", "NL", "TR", "CP"],
            datasets: [{
                label: 'Tipo de solicitação',
                data: jsonData, //[62, 150, 53, 34],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.2)',
                    'rgba(255, 159, 64, 0.2)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
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
                    },
                    display: false
                }]
            }
        }
    });

    if (modalWindow == true)
        modalChart = chartTipoEnvioEquipamento;
}

function carregarGraficoFamiliaModelo(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoFamiliaModelo' + filtros;
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

            if (data.FAMILAMODELO != null) {
                var lista = JSON.parse(data.FAMILAMODELO);
                var jsonLabel = [], jsonData = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].grupoModelo.DS_GRUPO_MODELO);
                    jsonData.push(parseFloat(lista[i].TOTAL));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoFamiliaModeloDados(modalWindow, jsonLabel, jsonData);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoFamiliaModeloDados(modalWindow, jsonLabel, jsonData) {

    var ctx;
    var fontSizeX;
    var fontSizeY;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
        fontSizeX = 12;
        fontSizeY = 12;
    }
    else {
        ctx = document.getElementById("chartFamiliaModelo").getContext('2d');
        if (chartFamiliaModelo != null)
            chartFamiliaModelo.destroy();
        fontSizeX = 8;
        fontSizeY = 7;
    }

    /*var myChart*/ chartFamiliaModelo = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["700a", "700r", "1400 RFS", "800 af", "800 af3", "800 r", "800 rf", "800 rks", "100CF", "800a"],
            datasets: [{
                //label: 'Bar Dataset',
                data: jsonData, //[62, 150, 53, 34, 67, 46, 83, 115, 100, 37],
                backgroundColor: [
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)',
                    'rgba(128, 255, 0, 0.8)',
                    'rgba(0, 128, 128, 0.8)',
                    'rgba(0, 128, 192, 0.8)',
                    'rgba(128, 0, 128, 0.8)',
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)',
                    'rgba(128, 255, 0, 0.8)',
                    'rgba(0, 128, 128, 0.8)',
                    'rgba(0, 128, 192, 0.8)',
                    'rgba(128, 0, 128, 0.8)',
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)',
                    'rgba(128, 255, 0, 0.8)',
                    'rgba(0, 128, 128, 0.8)',
                    'rgba(0, 128, 192, 0.8)',
                    'rgba(128, 0, 128, 0.8)',
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)',
                    'rgba(128, 255, 0, 1)',
                    'rgba(0, 128, 128, 1)',
                    'rgba(0, 128, 192, 1)',
                    'rgba(128, 0, 128, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)',
                    'rgba(128, 255, 0, 1)',
                    'rgba(0, 128, 128, 1)',
                    'rgba(0, 128, 192, 1)',
                    'rgba(128, 0, 128, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)',
                    'rgba(128, 255, 0, 1)',
                    'rgba(0, 128, 128, 1)',
                    'rgba(0, 128, 192, 1)',
                    'rgba(128, 0, 128, 1)',
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)'
                ],
                borderWidth: 1
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
                        beginAtZero: true,
                        fontSize: fontSizeY
                    }
                }],
                xAxes: [{
                    ticks: {
                        fontSize: fontSizeX
                    }
                }]

            }
        }
    });

    if (modalWindow == true)
        modalChart = chartFamiliaModelo;
}

function carregarGridMVCCliente() {
    var URL = URLObterListaClienteVisaoResumida + filtros;

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

function carregarGridMVCEquipamento() {
    var URL = URLObterListaEquipamentoWorkFlow + filtros;

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
                $('#gridMVCEquipamento').html(res.Html);
                $("#tableMVCEquipamento").freezeHeaderDash({ 'height': '347px' });

                $('#DEPRECIACAO').text(res.DEPRECIACAO);
                $('#TOT_PECAS').text(res.TOT_PECAS);
                $('#TOT_MAO_OBRA').text(res.TOT_MAO_OBRA);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}
