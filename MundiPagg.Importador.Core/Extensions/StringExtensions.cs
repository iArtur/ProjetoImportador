using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MundiPagg.Importador.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Método para encriptação de string base 64 usando chave RSA
        /// </summary>
        /// <param name="textToEncript"></param>
        /// <returns></returns>
        public static string Encript64StringRSA(this string textToEncripty, string key)
        {

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            byte[] data = Encoding.UTF8.GetBytes(textToEncripty);
            byte[] enc = rsa.Encrypt(data, false);
            return Convert.ToBase64String(enc).ToString();

        }

        /// <summary>
        /// Método para decriptação de string base 64 usando chave RSA
        /// </summary>
        /// <param name="textToDecripty"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decripty64StringRSA( this string textToDecripty, string key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            byte[] textEncript = Convert.FromBase64String(textToDecripty);
            byte[] texto = rsa.Decrypt(textEncript, false);
            return Encoding.UTF8.GetString(texto);
        }
    }
}
