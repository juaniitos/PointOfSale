using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Data;
using static WebApi.Models.Entidad;

namespace WebApi.Controllers
{
    public class FacturaController : ApiController
    {

        // Obtener productos
        [Route("api/factura/products")]
        public List<Product> GetProducts()
        {
            return FacturaData.Listar_Products();
        }

        // Crear factura
        [Route("api/factura/invoices")]
        public bool Post([FromBody] Invoices invoice)
        {
            return FacturaData.Registrar(invoice);
        }

        // GET api/factura/invoices
        [Route("api/factura/invoices")]
        public List<Invoices> Get()
        {
            return FacturaData.Listar_Facturas();
        }
    }
}