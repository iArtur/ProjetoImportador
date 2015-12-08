using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MundiPagg.Importador.Core.DTO
{
    /// <summary>
    /// Classe que representa a estrutura de cartão necessária para
    /// </summary>
    public class CartaoCredito
    {
        public string Numero { get; set; }
        public string NomeProprietario { get; set; }
        public string Bandeira { get; set; }
        public string MesExpiracao { get; set; }
        public string AnoExpiracao { get; set; }
        public string CodigoSeguranca { get; set; }

    }
}
