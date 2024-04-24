var modalChart = null;
var chartVolumeVenda = null;
var chartVenda = null;
var AutoRefresh = true;
var VendasMy = false;
setInterval('carregarPagina()', 20000);

jQuery(document).ready(function () {
    $('.js-example-basic-single').select2();

    carregarPagina();
    carregarComboRegional();
    carregarComboVendedor();
    carregarComboGrupo();
    //carregarComboModelo();
    carregarComboLinhaProdutoGp();
    AutoRefresh = false;
});

$('#btnMinhasVendas').click(function () {

    if (nidPerfil != perfilTecnico3M) {
        return false;
    } else if (nidUsuario == 0) {
        return false;
    }

    var URL = URLAPI + "TecnicoAPI/ObterViaUsuario?ID_USUARIO=" + nidUsuario;
    //var token = sessionStorage.getItem("token");
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
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.tecnico != null) {
                $("#TECNICO").val(res.tecnico.CD_TECNICO);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            //atualizarPagina();
            Alerta("ERRO", res.responseText);
        }
    });

    VendasMy = !VendasMy;
    var Class = '';

    if (VendasMy == true)
        Class = 'btn btn-success btn-sm';
    else {
        Class = 'btn btn-primary btn-sm';
        $("#TECNICO").val('');
    }

    $("#btnMinhasVendas").removeClass();
    $("#btnMinhasVendas").addClass(Class);
    AplicarFiltros();
});

$('#chartModelo').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoModelo(true);
});

$('#chartVolumeVenda').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoVolumeVenda(true);
});

$('#chartVenda').click(function () {
    $('#modalGrafico').modal({
        show: true
    });

    carregarGraficoVenda(true);
});

function carregarPagina() {

    if (AutoRefresh == true) {
        carregarBoxClienteAtivo();
        carregarBoxAtendimentoAndamento();
        carregarBoxFechadorAtivo();
        carregarBoxIdentificadorAtivo();
        carregarBoxPeriodo();
        carregarBoxVenda();
        carregarBoxVendedor();

        //carregarGraficoModelo(false);
        carregarGraficoVolumeVenda(false);
        carregarGraficoVenda(false);

        carregarGridMVCCliente();
        carregarGridMVCEquipamento();
        carregarGridMVCHistorico();
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

function carregarBoxAtendimentoAndamento() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxUnitizacao' + filtros;
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
            $("#Atendimentos").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxFechadorAtivo() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxFechadorAtivo' + filtros;
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
            $("#Fechadores").text(JSON.parse(data.TOTAL));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarBoxIdentificadorAtivo() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxIdentificadorAtivo' + filtros;
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
            $("#Identificadores").text(JSON.parse(data.TOTAL));
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

function carregarBoxVendedor() {
    var URL = URLAPI + 'DashboardAPI/ObterBoxVendedor' + filtros;
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

            $("#ClientesAtendidos").text(JSON.parse(data.ClientesAtendidos));
            $("#Equipamentos").text(JSON.parse(data.Equipamentos));
            $("#Depreciacao12Meses").text(JSON.parse(data.Depreciacao12Meses));
            $("#CustoManutencao").text(JSON.parse(data.CustoManutencao));
            $("#Investimento").text(JSON.parse(data.Investimento));
            $("#QtdeEquipamentosSemConsumo").text(JSON.parse(data.QtdeEquipamentosSemConsumo));
            //$("#QtdeClientesEmprestimo").text(JSON.parse(data.QtdeClientesEmprestimo));
            $("#SolicitacaoEnvio").text(JSON.parse(data.SolicitacaoEnvio));
            $("#AnaliseMarketing").text(JSON.parse(data.AnaliseMarketing));
            $("#AnaliseAreaTecnica").text(JSON.parse(data.AnaliseAreaTecnica));
            $("#AnalisePlanejamento").text(JSON.parse(data.AnalisePlanejamento));
            $("#EnviadoCliente").text(JSON.parse(data.EnviadoCliente));
            $("#Instalado").text(JSON.parse(data.Instalado));
            $("#SolicitacaoRetirada").text(JSON.parse(data.SolicitacaoRetirada));
            $("#AnaliseLogistica").text(JSON.parse(data.AnaliseLogistica));
            $("#ComTransportadora").text(JSON.parse(data.ComTransportadora));
            $("#EmAgendamentoTMS").text(JSON.parse(data.EmAgendamentoTMS));
            $("#Devolvido3M").text(JSON.parse(data.Devolvido3M));
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });
}

function carregarGraficoVolumeVenda(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoVolumeVenda' + filtros;
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

            if (data.VOLUMEVENDA != null) {
                var lista = JSON.parse(data.VOLUMEVENDA);
                var jsonLabel = [], jsonData = [];
                for (i = 0; i < lista.length; i++) {
                    jsonLabel.push(lista[i].linhaProduto.DS_LINHA_PRODUTO);
                    jsonData.push(parseFloat(lista[i].TOT_VENDAS_PERC));
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoVolumeVendaDados(modalWindow, jsonLabel, jsonData);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoVolumeVendaDados(modalWindow, jsonLabel, jsonData) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartVolumeVenda").getContext('2d');
        if (chartVolumeVenda != null)
            chartVolumeVenda.destroy();
    }

    /*var myChart*/ chartVolumeVenda = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: jsonLabel, //["Outros", "Filme", "PVA", "Tinta", "Etiqueta", "Fita"],
            datasets: [{
                label: 'Linha de Produto',
                data: jsonData, //[62, 150, 53, 34, 98, 110],
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
                    'rgba(128, 0, 255, 0.8)',
                    'rgba(64, 0, 128, 0.8)',
                    'rgba(128, 0, 0, 0.8)'
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
                    'rgba(128, 0, 255, 1)',
                    'rgba(64, 0, 128, 1)',
                    'rgba(128, 0, 0, 1)'
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
        modalChart = chartVolumeVenda;
}

function carregarGraficoVenda(modalWindow) {
    var URL = URLAPI + 'DashboardAPI/ObterGraficoVenda' + filtros;
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

            if (data.VENDA != null) {
                var lista = JSON.parse(data.VENDA);
                var jsonLabel1 = [], jsonData1 = [];
                var jsonLabel2 = [], jsonData2 = [];
                for (i = 0; i < lista.length; i++) {
                    if (i <= 11) {
                        jsonLabel1.push(lista[i].DS_MES);
                        jsonData1.push(parseFloat(lista[i].TOT_VENDAS));
                    }
                    else {
                        jsonLabel2.push(lista[i].DS_MES);
                        jsonData2.push(parseFloat(lista[i].TOT_VENDAS));
                    }
                }
            }
            //comboPecasEstoque.val('').trigger('change');
            carregarGraficoVendaDados(modalWindow, jsonLabel1, jsonLabel2, jsonData1, jsonData2);
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }
    });

}

function carregarGraficoVendaDados(modalWindow, jsonLabel1, jsonLabel2, jsonData1, jsonData2) {

    var ctx;

    if (modalWindow == true) {
        ctx = document.getElementById("chartGrafico").getContext('2d');
        if (modalChart != null)
            modalChart.destroy();
    }
    else {
        ctx = document.getElementById("chartVenda").getContext('2d');
        if (chartVenda != null)
            chartVenda.destroy();
    }

    /*var myChart*/ chartVenda = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: jsonLabel1, //["Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez"],
            datasets: [{
                label: 'Ano Anterior',
                data: jsonData1, //[44, 68, 93, 71, 82, 109, 91, 120, 89, 84, 80, 102],
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            },
            {
                label: 'Ano Atual',
                data: jsonData2, //[64, 70, 96, 78, 82, 35, 44, 174, 63, 49, 23, 29],
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
        modalChart = chartVenda;
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

                $('#ManutPc').text(res.ManutPc);
                $('#ManutHs').text(res.ManutHs);
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

