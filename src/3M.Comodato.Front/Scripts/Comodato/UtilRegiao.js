function carregarComboRegiao() {
    var URL = URLAPI + "RegiaoAPI/ObterLista";
    ////var token = sessionStorage.getItem("token");
    $.ajax({
        type: 'GET',
        url: URL,
        dataType: "json",
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
            if (res != null) {
                LoadRegioes(res);
            }
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}

function LoadRegioes(regioes) {
    LimparCombo($("#regiao_CD_REGIAO"));

    for (i = 0; i < regioes.regioes.length; i++) {
        MontarCombo($("#regiao_CD_REGIAO"), regioes.regioes[i].CD_REGIAO, regioes.regioes[i].DS_REGIAO);
    }
}
