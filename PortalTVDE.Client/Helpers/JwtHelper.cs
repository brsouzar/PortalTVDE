    namespace PortalTVDE.Client.Helpers
{
    public class JwtHelper
    {
        // Método auxiliar para converter Base64URL (usado em JWT) para bytes.
        // Ele adiciona o padding '=' necessário para a decodificação padrão do .NET.
        public static byte[] ParseBase64WithoutPadding(string base64)
        {
            // 1. Substitui caracteres URL-safe (Base64URL) para Base64 Padrão
            base64 = base64.Replace('-', '+').Replace('_', '/');

            // 2. Adiciona o padding '=' para que o comprimento seja divisível por 4
            switch (base64.Length % 4)
            {
                case 0:
                    // Já é múltiplo de 4, não precisa de padding
                    break;
                case 2:
                    // Adiciona '=='
                    base64 += "==";
                    break;
                case 3:
                    // Adiciona '='
                    base64 += "=";
                    break;
                default:
                    // Caso o comprimento seja 1 (o que indica string malformada)
                    throw new ArgumentException("A string Base64URL está malformada.", nameof(base64));
            }

            // 3. Converte para bytes usando a função padrão do .NET
            return Convert.FromBase64String(base64);
        }

        // Exemplo de como você usaria isso dentro do ParseClaimsFromJwt:
        /*
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            // ...
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload); // <-- É aqui que ele é chamado
            // ...
        }
        */
    }
}
