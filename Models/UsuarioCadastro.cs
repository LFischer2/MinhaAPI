namespace MinhaAPI.Models
{
    public class UsuarioCadastro
    {
        public string Nome_Completo { get; set; }
        public DateTime Data_Nascimento { get; set; }
        public string CPF { get; set; }
        public string Cidade { get; set; }
        public string Sexo { get; set; }
        public string Tipo_Sanguineo { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}