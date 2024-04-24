var modalChart = null;
var chartTotalAtivo = null;
var AutoRefresh = true;
setInterval('carregarPagina()', 20000);

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarPagina();
    carregarComboGrupo();
    carregarComboModelo();
    carregarComboLinhaProdutoGp();
    AutoRefresh = false;
});

$('#chartTotalAtivo').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoTotalAtivo(true);
});

function carregarPagina() {

    if (AutoRefresh == true) {
        carregarBoxClienteAtivo();
        carregarBoxEquipamentoComodato();
        carregarBoxEquipamentoLocado();
        carregarBoxPeriodo();
        carregarBoxVenda();
        carregarBoxPecaTrocada();
        carregarBoxLinhaProduto();

        carregarGraficoTotalAtivo(false);

        carregarGridMVCCliente();
        carregarGridMVCEquipamento();
        carregarGridMVCHistorico();
        carregarGridMVCHistoricoValores();
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

function carregarBoxPeriodo() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxPeriodo' + filtros;
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
            $("#PeriodosTitulo").text(JSON.parse(data.PeriodosTitulo));
            $("#Periodos").text(JSON.parse(data.TOTAL));

            $('#boxPeriodo').removeClass();
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
            if (parseInt(PERCENTUAL_GRAFICO_REALIZADO) >= 20)
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
            if (parseInt(PERCENTUAL_GRAFICO_RESTANTE) >= 20)
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

function carregarBoxVenda() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxVenda' + filtros;
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
            $("#VendasTitulo").text(JSON.parse(data.VendasTitulo));
            $("#VendasAnoAtual").text(JSON.parse(data.TOTAL));
            $("#VendasTituloAnoAnterior").text(JSON.parse(data.VendasTituloAnoAnterior));
            $("#PercentualAnoAnterior").text(JSON.parse(data.PercentualAnoAnterior));
            $('#badgePercentualAnoAnterior').removeClass();
            $('#badgePercentualAnoAnterior').addClass(JSON.parse(data.CSS));

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

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

function carregarBoxLinhaProduto() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxLinhaProduto' + filtros;
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
            //$("#Depreciacao12Meses").text(JSON.parse(data.Depreciacao12Meses));
            //$("#CustoManutencao").text(JSON.parse(data.CustoManutencao));
            $("#ClientesAtendidos").text(JSON.parse(data.ClientesAtendidos));
            $("#Investimento").text(JSON.parse(data.Investimento));

        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarGraficoTotalAtivo(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoTotalAtivo' + filtros;
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

            if (data.ATIVO != null) {
                var lista = JSON.parse(data.ATIVO);
                var jsonLabel = [], jsonData = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].ANO);
                    jsonData.push(parseFloat(lista[i].TOTAL));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoTotalAtivoDados(modalWindow, jsonLabel, jsonData);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoTotalAtivoDados(modalWindow, jsonLabel, jsonData) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartTotalAtivo").getContext('2d');
        if (chartTotalAtivo != null)
            chartTotalAtivo.destroy();
    }

    /*var myChart*/ chartTotalAtivo = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel, //["2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018"],
            datasets: [{
                //label: 'Bar Dataset',
                data: jsonData, //[15, 23, 34, 52, 68, 71, 92, 106, 131, 142, 163, 178],
                backgroundColor: 'rgba(58, 183, 40, 0.2)', //[
                //'rgba(255, 99, 132, 0.2)',
                //'rgba(54, 162, 235, 0.2)',
                //'rgba(255, 206, 86, 0.2)',
                //'rgba(75, 192, 192, 0.2)',
                //'rgba(153, 102, 255, 0.2)',
                //'rgba(255, 159, 64, 0.2)'
                //],
                borderColor: 'rgba(58, 183, 40, 1)', //[
                //'rgba(255,99,132,1)',
                //'rgba(54, 162, 235, 1)',
                //'rgba(255, 206, 86, 1)',
                //'rgba(75, 192, 192, 1)',
                //'rgba(153, 102, 255, 1)',
                //'rgba(255, 159, 64, 1)'
                //],
                borderWidth: 1,
                //steppedLine: 'before'
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
        modalChart = chartTotalAtivo;
}

function carregarGridMVCCliente() {
    var URL = URLObterListaCliente + filtros;

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
                $("#tableMVCCliente").freezeHeaderDash({ 'height': '370px' });
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
    var URL = URLObterListaEquipamento + filtros;

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
                $("#tableMVCEquipamento").freezeHeaderDash({ 'height': '290px' });

                //$('#DEPRECIACAO').text(res.DEPRECIACAO);
                $('#ManutPc').text("R$ " + res.ManutPc);
                $('#ManutHs').text("R$ " + res.ManutHs);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarGridMVCHistorico() {
    var URL = URLObterListaHistorico + filtros;

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
                $('#gridMVCHistorico').html(res.Html);
                $("#tableMVCHistorico").freezeHeaderDash({ 'height': '210px' });
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}

function carregarGridMVCHistoricoValores() {
    var URL = URLObterListaHistoricoValores + filtros;

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
                $('#gridMVCHistoricoValores').html(res.Html);
                $("#tableMVCHistoricoValores").freezeHeaderDash({ 'height': '210px' });
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }

    });

}
