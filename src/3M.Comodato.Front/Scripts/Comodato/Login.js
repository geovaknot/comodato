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

$('#cdsSenha').keydown(function (event) {
    var keyCode = (event.keyCode ? event.keyCode : event.which);
    if (keyCode == 13) {
        $('#loginWeb').trigger('click');
    }
});

function ValidarLogin() {
    var login = $("#cdsLogin").val();
    var senha = $("#cdsSenha").val();

    var URL = URLAPI + "UsuarioAPI/GetToken";
    var body = {
        grant_type: 'password',
        username: login,
        password: senha
    };

    console.log("Login", login)
    console.log("Senha", senha)

    var token = null;

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        //contentType: "on/x-www-form-urlencoded; charset=UTF-8",
        data: body,
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            //$("#loader").css("display", "none");
        },
        success: function (res) {
            //$("#loader").css("display", "none");
            token = res.access_token;
            loginToken(token);
        },
        error: function (res) {
            $('#lblERRO').val("Usuário ou senha Incorreta!");
            ExibirCampo($('#lblERRO'));
            $("#loader").css("display", "none");
            Alerta("ERRO", "Usuário ou senha Incorreta!");
        }

    });
    
}

function loginToken(token) {
    
    var login = $("#cdsLogin").val();
    var senha = $("#cdsSenha").val();

    console.log("Login", login)
    console.log("Senha", senha)

    var userLogin = {
        cdsLogin: login,
        cdsSenha: senha,
        token: token
    };
    var URL = URLSite + "Usuario/FazerLogin";

    $.ajax({
        type: 'POST',
        url: URL,
        dataType: "json",
        contentType: "application/json",
        data: JSON.stringify(userLogin),
        beforeSend: function () {
            $("#loader").css("display", "block");
        },
        complete: function () {
            //$("#loader").css("display", "none");
        },
        success: function (res) {
            $("#loader").css("display", "none");
            if (res.Status == "Sucesso") {
                sessionStorage.setItem("token", token);
                window.location.href = URLSite + "Home/Index";
            } else {
                if (res.Mensagem == "TrocarSenha!") {
                    window.location.href = URLSite + "Usuario/TrocarSenha";
                } else {
                    $('#lblERRO').val("Usuário ou senha Incorreta!");
                    ExibirCampo($('#lblERRO'));
                }
                
            }
            //token = res.access_token;
        },
        error: function (res) {
            $("#loader").css("display", "none");
            Alerta("ERRO", JSON.parse(res.responseText).Message);
        }

    });
}
