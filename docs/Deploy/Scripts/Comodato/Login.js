$(document).ready(function () {
    ExibirCampo($('#lblERRO'));
    localStorage.clear();
});

$("#cdsLogin").blur(function () {
    OcultarCampo($('#lblERRO'));
});

$("#cdsLogin").keypress(function () {
    OcultarCampo($('#lblERRO'));
});

$("#cdsSenha").blur(function () {
    OcultarCampo($('#lblERRO'));
});

$("#cdsSenha").keypress(function () {
    OcultarCampo($('#lblERRO'));
});
