jQuery(document).ready(function () {
    $("#cdsSenha").prop("readonly", "readonly");
    DropDownPerfilChange();

    var perfil = $("#perfil_nidPerfil option:selected").val();
    if (perfil == perfilCliente) {
        $("#cd_empresa").val(null).trigger('change');
        $("#cd_empresa").hide();
        $("#Empresa").hide();
    } else {
        $("#cd_empresa").show();
        $("#Empresa").show();
    }
});


$("#perfil_nidPerfil").change(function () {
    DropDownPerfilChange();

    var perfil = $("#perfil_nidPerfil option:selected").val();
    if (perfil == perfilCliente) {
        $("#cd_empresa").val(null).trigger('change');
        $("#cd_empresa").hide();
        $("#Empresa").hide();
    } else {
        $("#cd_empresa").show();
        $("#Empresa").show();
    }
});

function DropDownPerfilChange() {
    //var nidPerfilExternoPadrao = $("#nidPerfilExternoPadrao").val();
    var nidPerfil = $("#perfil_nidPerfil option:selected").val();

    //if (nidPerfilExternoPadrao == nidPerfil) {
    if (nidPerfil == perfilCliente || nidPerfil == perfilTecnicoExterno || nidPerfil == perfilCoordenador) {
        $("#divSenhaExterno").show();
        var cdsSenhaHidden = $("#cdsSenhaHidden").val();
        $("#cdsSenha").val(cdsSenhaHidden);
    }
    else {
        $("#divSenhaExterno").hide();
        $("#cdsSenha").val('');
    }
}