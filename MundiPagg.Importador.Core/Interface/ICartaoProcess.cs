using MundiPagg.Importador.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MundiPagg.Importador.Core.Interface
{
  public  interface ICartaoProcess
    {
      /// <summary>
      /// Process encripty csv
      /// </summary>
      /// <param name="key"></param>
      /// <param name="fileName"></param>
      /// <returns></returns>
      List<CartaoCredito> ProcessarPlanilha(string privateEncryptKey, string contentFile);


      /// <summary>
      /// Enviar os cartões para a Api da MundiPagg
      /// </summary>
      /// <param name="itens"></param>
      void EnviarCartoes(List<CartaoCredito> itens, Guid merchantKey, string uri);

      /// <summary>
      /// Método para criação de csv encriptiradp
      /// </summary>
      /// <param name="itens"></param>
      /// <returns></returns>
      string CriarPlanilhaEncriptada(List<CartaoCredito> itens, string key);
      

    }
}
