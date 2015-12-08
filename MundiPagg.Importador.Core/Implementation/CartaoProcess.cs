
using GatewayApiClient;
using GatewayApiClient.DataContracts;
using GatewayApiClient.DataContracts.EnumTypes;
using GatewayApiClient.TransactionReportFile.Report;
using Microsoft.VisualBasic.FileIO;
using MundiPagg.Importador.Core.DTO;
using MundiPagg.Importador.Core.Interface;
using MundiPagg.Importador.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MundiPagg.Importador.Core.Implementation
{
    public class CartaoProcess : ICartaoProcess
    {
        public List<DTO.CartaoCredito> ProcessarPlanilha(string privateEncryptKey, string contentFile)
        {
            List<CartaoCredito> cartoesDecript = new List<CartaoCredito>();
            //string t = File.ReadAllText(fileName);
            string t = contentFile.Replace('\n', '\r');
            string[] lines = t.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
            //removo o header
            lines = lines.Reverse().Take(lines.Length - 1).ToArray();
            int num_rows = lines.Length;
            foreach (var line in lines)
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter wr = new StreamWriter(stream);
                wr.Write(line.Decripty64StringRSA(privateEncryptKey));
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

            return cartoesDecript;
        }

        public void EnviarCartoes(List<CartaoCredito> itens, Guid merchantKey, string uri)
        {
            foreach (CartaoCredito c in itens)
            {
                var transaction = new GatewayApiClient.DataContracts.CreditCardTransaction()
                {
                    AmountInCents = 100,
                    CreditCard = new CreditCard()
                    {
                        CreditCardNumber = c.Numero,
                        CreditCardBrand = RetornarBandeira(c.Bandeira),
                        ExpMonth = Convert.ToInt32(c.MesExpiracao),
                        ExpYear = Convert.ToInt32(c.AnoExpiracao),
                        //SecurityCode = "123",
                        HolderName = c.NomeProprietario
                    }
                };

                try
                {

                    // Creates the client that will send the transaction.
                    var serviceClient = new GatewayServiceClient(merchantKey,new Uri(uri));
                    // Authorizes the credit card transaction and returns the gateway response.
                    var httpResponse = serviceClient.Sale.Create(transaction);

                    // API response code
                    Console.WriteLine("Status: {0}", httpResponse.HttpStatusCode);

                    var createSaleResponse = httpResponse.Response;
                    if (httpResponse.HttpStatusCode == HttpStatusCode.Created)
                    {
                        foreach (var creditCardTransaction in createSaleResponse.CreditCardTransactionResultCollection)
                        {
                            Console.WriteLine(creditCardTransaction.AcquirerMessage);
                        }
                    }
                    else
                    {
                        if (createSaleResponse.ErrorReport != null)
                        {
                            foreach (ErrorItem errorItem in createSaleResponse.ErrorReport.ErrorItemCollection)
                            {
                                Console.WriteLine("Error {0}: {1}", errorItem.ErrorCode, errorItem.Description);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public string CriarPlanilhaEncriptada(List<CartaoCredito> itens, string key)
        {
            string filePath = "C://Teste//";
            string fileName = "PlanilhaCartoes.csv";

            StringBuilder sb = new StringBuilder();
            string header = "NumeroCartao,Bandeira,DataValidade,NomePortador";
            sb.AppendLine(header);
            foreach (var item in itens)
            {
                string linha = string.Empty;
                linha = linha + item.Numero + ",";
                linha = linha + item.Bandeira + ",";
                linha = linha + item.MesExpiracao + "/" + item.AnoExpiracao.ToString() + ",";
                linha = linha + item.NomeProprietario;
                sb.AppendLine(linha.Encript64StringRSA(key));
            }

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath + fileName;
        }


        #region MetodosPrivados

        protected CreditCardBrandEnum RetornarBandeira(string bandeira)
        {
            switch (bandeira)
            {
                case "VISA":
                    return CreditCardBrandEnum.Visa;

                case "MASTERCARD":
                    return CreditCardBrandEnum.Mastercard;

                default:
                    return 0;

            }
        }

        #endregion
    }
}
