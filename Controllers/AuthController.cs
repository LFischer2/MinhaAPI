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
        using var conexao = _db.ObterConexao();

        var usuario = conexao.QueryFirstOrDefault(
            "SELECT * FROM usuarios WHERE cpf = @CPF",
            new { login.CPF }
        );

        if (usuario == null)
            return Unauthorized("Usuário não encontrado");

        bool senhaValida = BCrypt.Net.BCrypt.Verify(login.Senha, usuario.senha_hash);

        if (!senhaValida)
            return Unauthorized("Senha inválida");

        return Ok(new { mensagem = "Login OK", user_id = usuario.id });
    }

    [HttpPost("cadastro")]
public IActionResult Cadastro([FromBody] UsuarioCadastro user)
{
    using var conexao = _db.ObterConexao();

    var senhaHash = BCrypt.Net.BCrypt.HashPassword(user.Senha);

    var sql = @"INSERT INTO usuarios 
    (nome_completo, data_nascimento, cpf, cidade, sexo, tipo_sanguineo, celular, email, senha_hash)
    VALUES (@Nome, @Nascimento, @CPF, @Cidade, @Sexo, @Tipo, @Celular, @Email, @SenhaHash)";

    conexao.Execute(sql, new
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
    using var conexao = _db.ObterConexao();

    var sql = "SELECT * FROM doacoes WHERE usuario_id = @Id";

    var doacoes = conexao.Query(sql, new { Id = usuarioId });

    return Ok(doacoes);
}

}

