using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static WebApi.Models.Entidad;

namespace WebApi.Data
{
    public class FacturaData
    {
        public static bool Registrar(Invoices invoices)
        {
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                oConexion.Open();
                using (SqlTransaction transaction = oConexion.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmdInsertInvoice = new SqlCommand("INSERT INTO Invoices (NombreCliente, NombreEmpresa, DireccionEmpresa, TelEmpresa, NombreClienteEnvio, NombreEmpresaEnvio, DireccionEmpresaEnvio, TelEmpresaEnvio, NombreVendedor, OrdenCompra, EnviarPor, TerminosCondiciones, Vencimiento) " +
                            "VALUES (@NombreCliente, @NombreEmpresa, @DireccionEmpresa, @TelEmpresa, @NombreClienteEnvio, @NombreEmpresaEnvio, @DireccionEmpresaEnvio, @TelEmpresaEnvio, @NombreVendedor, @OrdenCompra, @EnviarPor, @TerminosCondiciones, @Vencimiento); SELECT SCOPE_IDENTITY();", oConexion, transaction);

                        cmdInsertInvoice.Parameters.AddWithValue("@NombreCliente", invoices.NombreCliente);
                        cmdInsertInvoice.Parameters.AddWithValue("@NombreEmpresa", invoices.NombreEmpresa);
                        cmdInsertInvoice.Parameters.AddWithValue("@DireccionEmpresa", invoices.DireccionEmpresa);
                        cmdInsertInvoice.Parameters.AddWithValue("@TelEmpresa", invoices.TelEmpresa);
                        cmdInsertInvoice.Parameters.AddWithValue("@NombreClienteEnvio", invoices.NombreClienteEnvio);
                        cmdInsertInvoice.Parameters.AddWithValue("@NombreEmpresaEnvio", invoices.NombreEmpresaEnvio);
                        cmdInsertInvoice.Parameters.AddWithValue("@DireccionEmpresaEnvio", invoices.DireccionEmpresaEnvio);
                        cmdInsertInvoice.Parameters.AddWithValue("@TelEmpresaEnvio", invoices.TelEmpresaEnvio);
                        cmdInsertInvoice.Parameters.AddWithValue("@NombreVendedor", invoices.NombreVendedor);
                        cmdInsertInvoice.Parameters.AddWithValue("@OrdenCompra", invoices.OrdenCompra);
                        cmdInsertInvoice.Parameters.AddWithValue("@EnviarPor", invoices.EnviarPor);
                        cmdInsertInvoice.Parameters.AddWithValue("@TerminosCondiciones", invoices.TerminosCondiciones);
                        cmdInsertInvoice.Parameters.AddWithValue("@Vencimiento", invoices.Vencimiento);

                        cmdInsertInvoice.ExecuteScalar();

                        foreach (var detail in invoices.Invoice_Details)
                        {
                            SqlCommand cmdInsertDetail = new SqlCommand("INSERT INTO Invoice_Detail (CodigoProducto, DescripcionProducto, Cantidad, PrecioUnitario, Total) " +
                                "VALUES (@CodigoProducto, @DescripcionProducto, @Cantidad, @PrecioUnitario, @Total); SELECT SCOPE_IDENTITY(); ", oConexion, transaction);

                            /*cmdInsertDetail.Parameters.AddWithValue("@IdCabecera", detail.IdCabecera);*/
                            cmdInsertDetail.Parameters.AddWithValue("@CodigoProducto", detail.CodigoProducto);
                            cmdInsertDetail.Parameters.AddWithValue("@DescripcionProducto", detail.DescripcionProducto);
                            cmdInsertDetail.Parameters.AddWithValue("@Cantidad", detail.Cantidad);
                            cmdInsertDetail.Parameters.AddWithValue("@PrecioUnitario", detail.PrecioUnitario);
                            cmdInsertDetail.Parameters.AddWithValue("@Total", detail.Total);

                            cmdInsertDetail.ExecuteNonQuery();  // Ejecutar inserción 
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al ejecutar la transacción: " + ex.Message);
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public static List<Product> Listar_Products()
        {
            List<Product> ProductList = new List<Product>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("listar_productos", oConexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                try
                {
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ProductList.Add(new Product()
                            {
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                DescripcionProducto = dr["DescripcionProducto"].ToString(),
                                Stock = Convert.ToInt32(dr["Stock"]),
                                PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"].ToString()),
                            });
                        }
                    }
                    return ProductList;
                } 
                catch (Exception ex)
                {
                    Console.WriteLine("Error al ejecutar cmd.ExecuteNonQuery(): " + ex.Message);
                    throw ex;
                }
            }
        }

        public static List<Invoices> Listar_Facturas()
        {
            List<Invoices> invoices = new List<Invoices>();
            using (SqlConnection oConexion = new SqlConnection(Conexion.rutaConexion))
            {
                SqlCommand cmd = new SqlCommand("listar_facturas", oConexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                try
                {
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var invoice = new Invoices
                            {
                                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"].ToString()),
                                IdCabecera = Convert.ToInt32(dr["IdCabecera"]),
                                NombreCliente = dr["NombreCliente"].ToString(),
                                NombreEmpresa = dr["NombreEmpresa"].ToString(),
                                DireccionEmpresa = dr["DireccionEmpresa"].ToString(),
                                TelEmpresa = dr["TelEmpresa"].ToString(),
                                NombreVendedor = dr["NombreVendedor"].ToString(),
                                OrdenCompra = dr["OrdenCompra"].ToString(),
                                EnviarPor = dr["EnviarPor"].ToString(),
                                Vencimiento = Convert.ToDateTime(dr["Vencimiento"].ToString()),
                                TerminosCondiciones = dr["TerminosCondiciones"].ToString()
                            };

                            var detail = new Invoice_Detail
                            {
                                CodigoProducto = dr["CodigoProducto"].ToString(),
                                DescripcionProducto = dr["DescripcionProducto"].ToString(),
                                Cantidad = Convert.ToInt32(dr["Cantidad"]),
                                PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                                Total = Convert.ToDecimal(dr["Total"])
                            };

                            invoice.Invoice_Details = new List<Invoice_Detail> { detail };
                            invoices.Add(invoice);
                        }
                    }
                    return invoices;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al ejecutar cmd.ExecuteNonQuery(): " + ex.Message);
                    throw ex;
                }
            }
        }
    }
}