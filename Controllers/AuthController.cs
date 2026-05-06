using Microsoft.AspNetCore.Mvc;
using Dapper;
using MinhaAPI.Data;
using BCrypt.Net;
using MinhaAPI.Models;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly Database _db;

    public AuthController(Database db)
    {
        _db = db;
    }

    // 🔹 LOGIN
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        using var conn = _db.GetConnection();

        var user = conn.QueryFirstOrDefault(
            "SELECT * FROM usuarios WHERE cpf = @CPF",
            new { login.CPF }
        );

        if (user == null)
            return Unauthorized("Usuário não encontrado");

        bool senhaValida = BCrypt.Net.BCrypt.Verify(login.Senha, user.senha_hash);

        if (!senhaValida)
            return Unauthorized("Senha inválida");

        return Ok(new { mensagem = "Login OK", user_id = user.id });
    }

    [HttpPost("cadastro")]
public IActionResult Cadastro([FromBody] UsuarioCadastro user)
{
    using var conn = _db.GetConnection();

    var senhaHash = BCrypt.Net.BCrypt.HashPassword(user.Senha);

    var sql = @"INSERT INTO usuarios 
    (nome_completo, data_nascimento, cpf, cidade, sexo, tipo_sanguineo, celular, email, senha_hash)
    VALUES (@Nome, @Nascimento, @CPF, @Cidade, @Sexo, @Tipo, @Celular, @Email, @SenhaHash)";

    conn.Execute(sql, new
    {
        Nome = user.Nome_Completo,
        Nascimento = user.Data_Nascimento,
        CPF = user.CPF,
        Cidade = user.Cidade,
        Sexo = user.Sexo,
        Tipo = user.Tipo_Sanguineo,
        Celular = user.Celular,
        Email = user.Email,
        SenhaHash = senhaHash
    });

    return Ok("Usuário cadastrado");
}

[HttpGet("doacoes/{usuarioId}")]
public IActionResult ListarDoacoes(int usuarioId)
{
    using var conn = _db.GetConnection();

    var sql = "SELECT * FROM doacoes WHERE usuario_id = @Id";

    var doacoes = conn.Query(sql, new { Id = usuarioId });

    return Ok(doacoes);
}

}

