jQuery(document).ready(function () {
    $("#EscondeDivLogin").hide();

});


$('#btnFiltrar').click(function () {
    carregarGridMVC();
});

$("#cm_star-empty").click(function () {
    $("#nota").val(0);
});

$("#cm_star-1").click(function () {
    $("#nota").val(1);
});

$("#cm_star-2").click(function () {
    $("#nota").val(2);
});

$("#cm_star-3").click(function () {
    $("#nota").val(3);
});

$("#cm_star-4").click(function () {
    $("#nota").val(4);
});

$("#cm_star-5").click(function () {
    $("#nota").val(5);
});


function SalvarAvaliacaoVisita() {
    var URL = URLSITE + "VisitaPadrao/AvaliarVisita";

    var ID_VISITA = $("#ID_VISITA").val();
    
    var visitaResposta = new Object();
    visitaResposta.ID_VISITA = ID_VISITA;

    if ($("#nota").val() == "" || $("#nota").val() == null || $("#nota").val() == undefined) {
        visitaResposta.NotaPesquisa = 0;
    }
    else {
        visitaResposta.NotaPesquisa = $("#nota").val();
    }
    
    if (visitaResposta.NotaPesquisa == 0 || visitaResposta.NotaPesquisa == '0') {
        Alerta("ERRO", "A nota de avaliação deverá conter no minimo 1 estrela.");
        return;
    }
    else {
        visitaResposta.ID_OS = 0;
        visitaResposta.Justificativa = $("#justificativa").val();
        visitaResposta.DS_RESPOSTA1 = '';
        visitaResposta.DS_RESPOSTA2 = '';
        visitaResposta.DS_RESPOSTA3 = '';
        visitaResposta.DS_RESPOSTA4 = '';
        visitaResposta.DS_RESPOSTA5 = '';
        //visitaResposta.nidUsuarioAtualizacao = nidUsuario;

        ////var token = sessionStorage.getItem("token");

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            data: JSON.stringify(visitaResposta),
            beforeSend: function (xhr) {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                var page = "";
                $("#loader").css("display", "none");
                if (res.Error) {
                    page = "PesquisaRedirectErro";
                }
                else {
                    page = "PesquisaRedirect";
                }

                setTimeout(function () {// wait for 5 secs(2)
                    var urlAtual = window.location.href;
                    var arrUrl = urlAtual.split("/");
                    var novaURL = "";
                    var total = arrUrl.length - 1;
                    for (var i = 0; i < total; i++) {
                        novaURL += arrUrl[i] + "/";
                    }
                    novaURL += page;
                    window.location.href = novaURL;

                }, 2000);
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }
        });
    }

    
}

function SalvarAvaliacaoOS() {
    var URL = URLSITE + "OsPadrao/AvaliarVisita";

    var ID_OS = $("#ID_VISITA").val();

    var visitaResposta = new Object();
    visitaResposta.ID_OS = ID_OS;

    if ($("#nota").val() == "" || $("#nota").val() == null || $("#nota").val() == undefined) {
        visitaResposta.NotaPesquisa = 0;
    }
    else {
        visitaResposta.NotaPesquisa = $("#nota").val();
    }

    if (visitaResposta.NotaPesquisa == 0 || visitaResposta.NotaPesquisa == '0') {
        Alerta("ERRO", "A nota de avaliação deverá conter no minimo 1 estrela.");
        return;
    }
    else {

        visitaResposta.ID_VISITA = 0;
        visitaResposta.Justificativa = $("#justificativa").val();
        visitaResposta.DS_RESPOSTA1 = '';
        visitaResposta.DS_RESPOSTA2 = '';
        visitaResposta.DS_RESPOSTA3 = '';
        visitaResposta.DS_RESPOSTA4 = '';
        visitaResposta.DS_RESPOSTA5 = '';
        //visitaResposta.nidUsuarioAtualizacao = nidUsuario;

        ////var token = sessionStorage.getItem("token");

        $.ajax({
            type: 'POST',
            url: URL,
            dataType: "json",
            cache: false,
            async: false,
            contentType: "application/json",
            //headers: { "Authorization": "Basic " + localStorage.token },
            data: JSON.stringify(visitaResposta),
            beforeSend: function (xhr) {
                $("#loader").css("display", "block");
            },
            complete: function () {
                $("#loader").css("display", "none");
            },
            success: function (res) {
                var page = "";
                $("#loader").css("display", "none");
                if (res.Error) {
                    page = "PesquisaRedirectErro";
                }
                else {
                    page = "PesquisaRedirect";
                }

                setTimeout(function () {// wait for 5 secs(2)
                    var urlAtual = window.location.href;
                    var arrUrl = urlAtual.split("/");
                    var novaURL = "";
                    var total = arrUrl.length - 1;
                    for (var i = 0; i < total; i++) {
                        novaURL += arrUrl[i] + "/";
                    }
                    novaURL += page;
                    window.location.href = novaURL;

                }, 2000);
            },
            error: function (res) {
                $("#loader").css("display", "none");
                //atualizarPagina();
                Alerta("ERRO", res.responseText);
            }
        });
    }
}




