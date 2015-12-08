using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using MundiPagg.Importador.Core.DTO;
using MundiPagg.Importador.Core.Helpers;
using MundiPagg.Importador.Core.Extensions;
namespace testEncript
{
    class Program
    {
        static void Main(string[] args)
        {
            string publ = "<RSAKeyValue><Modulus>ry5CqAHxMZU1NfbMMHu5bguQrK5u8wKfuoK4zcMCiVTjdsHyt9u/3ysmg3cKfpCcnGDHiSqcrFOoEnSYeKeEz2vo/KyF330diMl4MdjsvmQX+nq4Lr2Tgy4OAJBTp4dZMlg7wgJs+gVqIbAzYwbOPTZI2TgjsPVcPx5XCym8OpU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            string priv = "<RSAKeyValue><Modulus>ry5CqAHxMZU1NfbMMHu5bguQrK5u8wKfuoK4zcMCiVTjdsHyt9u/3ysmg3cKfpCcnGDHiSqcrFOoEnSYeKeEz2vo/KyF330diMl4MdjsvmQX+nq4Lr2Tgy4OAJBTp4dZMlg7wgJs+gVqIbAzYwbOPTZI2TgjsPVcPx5XCym8OpU=</Modulus><Exponent>AQAB</Exponent><P>7foWyYG+KAwpP1ojFUg0/7j+0WQLkwzCX1F7/BwC1dU6x+10t/u8uzfnBXDFCFHU/FcN+0hv8xKfqwU6su+xVw==</P><Q>vHKstraurzentQLYYnE6kMGZ5uxWx44iBecmpeNmfTWQTHW/zg2YOZjsf5HpHhJJ28paoO8GZeNBxBwrmcgj8w==</Q><DP>Gthoc6jYK6pbiNMBPCheGi7jR/myOI9q6dfGVcVPKJIaRG1sCkkynCgNPpbfEzYwAZtMb3lXb6M9McywN3lElw==</DP><DQ>p2NhAMcSMTdJc2J8nCyhbdVaBCGoD4eztvSUgsOT1OIQM6pf3gJ7VaX9ZG9R11E9Y8rBZ7QJRdVwJGc1X1rEew==</DQ><InverseQ>o6c047+veG3SXG5Ze5JAhYRgDDF08jZK2RgpGoIh30tJBks3DkhSM3aecDsqBLEC4SscJI+787TJxulJ9rTkew==</InverseQ><D>BO//BUuSDULhLjqyXvbLGgIYcKLf/K3JFmbYfPAvUkiYEAD/wtUzmhTWGo7QjGhYtFhOohvshjeQvVMXoxBHACJLyoloS2rHm/9VK4S01Kwb5k/7lflU0ZY5MVMQ1nSp9bjQpzg8z2phhSBW2JpEqMN1mskXm5VMlf0PZtdV6Dc=</D></RSAKeyValue>";
            string filePath = "C://Teste//";
            string fileName = "PlanilhaCartoes.csv";
            List<CartaoCredito> listaCartoes = new List<CartaoCredito>();
            CartaoCredito c = new CartaoCredito()
            {
                Numero = "4012001037141112",
                Bandeira = CartaoBandeira.Visa,
                MesExpiracao = "05",
                AnoExpiracao = "2018",
                NomeProprietario = "João da Silva de Oliveira Carmo Gonçalves Lima"
            };

            CartaoCredito ca = new CartaoCredito()
            {
                Numero = "5453010000066167",
                Bandeira = CartaoBandeira.MasterCard,
                MesExpiracao = "05",
                AnoExpiracao = "2018",
                NomeProprietario = "Gilmar da silva"
            };

            listaCartoes.Add(c);
            listaCartoes.Add(ca);
            StringBuilder sb = new StringBuilder();
            string header = "NumeroCartao,Bandeira,DataValidade,NomePortador";
            sb.AppendLine(header);
            foreach (var item in listaCartoes)
            {
                string linha = string.Empty;
                linha = linha + item.Numero + ",";
                linha = linha + item.Bandeira + ",";
                linha = linha + item.MesExpiracao + "/" + item.AnoExpiracao.ToString() + ",";
                linha = linha + item.NomeProprietario;
                sb.AppendLine(linha.Encript64StringRSA(publ));
            }

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);


            File.WriteAllText(filePath + fileName, sb.ToString());
            List<CartaoCredito> cartoesDecript = new List<CartaoCredito>();
           
            string t = File.ReadAllText(filePath + fileName);

            t = t.Replace('\n', '\r');
            string[] lines = t.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
            //removo o header
            lines = lines.Reverse().Take(lines.Length - 1).ToArray();
            int num_rows = lines.Length;
            foreach (var line in lines)
            {

                MemoryStream stream = new MemoryStream();
                StreamWriter wr = new StreamWriter(stream);
                wr.Write(line.Decripty64StringRSA(priv));
                wr.Flush();
                stream.Position = 0;

                using (TextFieldParser parser = new TextFieldParser(stream))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        CartaoCredito c1 = new CartaoCredito();
                        c1.Numero = fields[0].ToString();
                        c1.Bandeira = fields[1].ToString();
                        c1.MesExpiracao = fields[2].Split('/')[0];
                        c1.AnoExpiracao = fields[2].Split('/')[1];
                        c1.NomeProprietario = fields[3].ToString();
                        cartoesDecript.Add(c1);
                    }
                }
            }
        }
    }
}
