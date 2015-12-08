using Microsoft.VisualStudio.TestTools.UnitTesting;
using MundiPagg.Importador.Core.DTO;
using MundiPagg.Importador.Core.Helpers;
using MundiPagg.Importador.Core.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MundiPagg.Importador.Tests
{
    [TestClass]
    public class CartaoProcessTest
    {
        [TestMethod]
        public void criar_planilha_encripitada()
        {
            string publ = "<RSAKeyValue><Modulus>ry5CqAHxMZU1NfbMMHu5bguQrK5u8wKfuoK4zcMCiVTjdsHyt9u/3ysmg3cKfpCcnGDHiSqcrFOoEnSYeKeEz2vo/KyF330diMl4MdjsvmQX+nq4Lr2Tgy4OAJBTp4dZMlg7wgJs+gVqIbAzYwbOPTZI2TgjsPVcPx5XCym8OpU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
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

            CartaoProcess p = new CartaoProcess();
            string fileName = p.CriarPlanilhaEncriptada(listaCartoes, publ);
            Assert.IsNotNull(fileName);
        }

        [TestMethod]
        public void testar_processar_planilha()
        {
            const string privKey = "<RSAKeyValue><Modulus>ry5CqAHxMZU1NfbMMHu5bguQrK5u8wKfuoK4zcMCiVTjdsHyt9u/3ysmg3cKfpCcnGDHiSqcrFOoEnSYeKeEz2vo/KyF330diMl4MdjsvmQX+nq4Lr2Tgy4OAJBTp4dZMlg7wgJs+gVqIbAzYwbOPTZI2TgjsPVcPx5XCym8OpU=</Modulus><Exponent>AQAB</Exponent><P>7foWyYG+KAwpP1ojFUg0/7j+0WQLkwzCX1F7/BwC1dU6x+10t/u8uzfnBXDFCFHU/FcN+0hv8xKfqwU6su+xVw==</P><Q>vHKstraurzentQLYYnE6kMGZ5uxWx44iBecmpeNmfTWQTHW/zg2YOZjsf5HpHhJJ28paoO8GZeNBxBwrmcgj8w==</Q><DP>Gthoc6jYK6pbiNMBPCheGi7jR/myOI9q6dfGVcVPKJIaRG1sCkkynCgNPpbfEzYwAZtMb3lXb6M9McywN3lElw==</DP><DQ>p2NhAMcSMTdJc2J8nCyhbdVaBCGoD4eztvSUgsOT1OIQM6pf3gJ7VaX9ZG9R11E9Y8rBZ7QJRdVwJGc1X1rEew==</DQ><InverseQ>o6c047+veG3SXG5Ze5JAhYRgDDF08jZK2RgpGoIh30tJBks3DkhSM3aecDsqBLEC4SscJI+787TJxulJ9rTkew==</InverseQ><D>BO//BUuSDULhLjqyXvbLGgIYcKLf/K3JFmbYfPAvUkiYEAD/wtUzmhTWGo7QjGhYtFhOohvshjeQvVMXoxBHACJLyoloS2rHm/9VK4S01Kwb5k/7lflU0ZY5MVMQ1nSp9bjQpzg8z2phhSBW2JpEqMN1mskXm5VMlf0PZtdV6Dc=</D></RSAKeyValue>";
            string filePath = "C://Teste//";
            string fileName = "PlanilhaCartoes.csv";
            string t = File.ReadAllText(filePath + fileName);
            CartaoProcess c = new CartaoProcess();
            List<CartaoCredito> list = c.ProcessarPlanilha(privKey, t);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count != 0, "lista vazia");
        }
    }
}
