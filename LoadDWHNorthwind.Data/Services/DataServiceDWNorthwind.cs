

using LoadDWHNorthwind.Data.Context;
using LoadDWHNorthwind.Data.Entities.DWNorthwind;
using LoadDWHNorthwind.Data.Interfaces;
using LoadDWHNorthwind.Data.Result;
using Microsoft.EntityFrameworkCore;


namespace LoadDWHNorthwind.Data.Services
{
    public class DataServiceDWHNorthwind : IDataServiceDWHNorthwind
    {
        private readonly NorthwindContext _northwindContext;
        private readonly DWNorthwindContext _dwnorthwindContext;

        public DataServiceDWHNorthwind(NorthwindContext norwindContext,
                                   DWNorthwindContext dwnorthwindContext)
        {
            _northwindContext = norwindContext;
            _dwnorthwindContext = dwnorthwindContext;
        }

        public async Task<OperationResult> LoadDHW()
        {
            OperationResult result = new OperationResult();
            try
            {

                await LoadDimEmployee();
                await LoadDimProduct();
                await LoadDimCustomers();
                await LoadDimShippers();
                //await LoadFactSales();
                //await LoadFactCustomerServed();


            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el DWH Ventas. {ex.Message}";
            }

            return result;
        }

        private async Task<OperationResult> LoadDimEmployee()
        {
            OperationResult result = new OperationResult();

            try
            {
                var employees = await _northwindContext.Employees.AsNoTracking().Select(emp => new DimEmployee()
                {
                    EmployeeID = emp.EmployeeID,
                    EmployeeName = string.Concat(emp.FirstName, " ", emp.LastName)
                }).ToListAsync();

                int[] employeeIds = employees.Select(emp => emp.EmployeeKey).ToArray();

                await _dwnorthwindContext.DimEmployees.Where(cd => employeeIds.Contains(cd.EmployeeID))
                                                .AsNoTracking()
                                                .ExecuteDeleteAsync();

                await _dwnorthwindContext.DimEmployees.AddRangeAsync(employees);

                await _dwnorthwindContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando la dimension de empleado {ex.Message}";
            }


            return result;
        }

        private async Task<OperationResult> LoadDimProduct()
        {
            OperationResult result = new OperationResult();
            try
            {

                var productCategories = await (from product in _northwindContext.Products
                                               join category in _northwindContext.Categories on product.CategoryID equals category.CategoryID
                                               select new DimProduct()
                                               {
                                                   CategoryID = category.CategoryID,
                                                   ProductName = product.ProductName,
                                                   CategoryName = category.CategoryName,
                                                   ProductKey = product.ProductID
                                               }).AsNoTracking().ToListAsync();



                int[] productsIds = productCategories.Select(c => c.ProductKey).ToArray();


                await _dwnorthwindContext.DimProducts.Where(c => productsIds.Contains(c.ProductKey))
                                                        .AsNoTracking()
                                                        .ExecuteDeleteAsync();

                await _dwnorthwindContext.DimProducts.AddRangeAsync(productCategories);

                await _dwnorthwindContext.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando la dimension de producto y categoria. {ex.Message}";
            }
            return result;
        }

        private async Task<OperationResult> LoadDimCustomers()
        {
            OperationResult operation = new OperationResult() { Success = false };


            try
            {

                var customers = await _northwindContext.Customers.Select(cust => new DimCustomer()
                {
                    CustomerID = cust.CustomerID,
                    CustomerName = cust.CompanyName

                }).AsNoTracking()
                  .ToListAsync();


                string[] customersIds = customers.Select(cust => cust.CustomerID).ToArray();

                await _dwnorthwindContext.DimCustomers.Where(cust => customersIds.Contains(cust.CustomerKey))
                                          .AsNoTracking()
                                          .ExecuteDeleteAsync();

                await _dwnorthwindContext.DimCustomers.AddRangeAsync(customers);
                await _dwnorthwindContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                operation.Success = false;
                operation.Message = $"Error: {ex.Message} cargando la dimension de clientes.";
            }
            return operation;
        }

        private async Task<OperationResult> LoadDimShippers()
        {
            OperationResult result = new OperationResult();

            try
            {
                // Obtener shippers de la base de datos de origen y mapear a DimShipper
                var shippers = await _northwindContext.Shippers.AsNoTracking().Select(ship => new DimShipper()
                {
                    ShipperID = ship.ShipperID,
                    ShipperName = ship.CompanyName
                }).ToListAsync();

                // Obtener los IDs de los shippers como array de enteros
                int[] shipperIds = shippers.Select(ship => ship.ShipperID).ToArray();

                // Borrar registros existentes en DWNorthwind con esos IDs
                await _dwnorthwindContext.DimShippers
                    .Where(cd => shipperIds.Contains(cd.ShipperID))
                    .AsNoTracking()
                    .ExecuteDeleteAsync();

                // Agregar nuevos registros a DWNorthwind
                await _dwnorthwindContext.DimShippers.AddRangeAsync(shippers);

                // Guardar cambios en DWNorthwind
                await _dwnorthwindContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando la dimensión de shippers: {ex.Message}";
            }

            return result;
        }
        //private async Task<OperationResult> LoadFactSales()
        //{
        //    OperationResult result = new OperationResult();

        //    try
        //    {
        //        var ventas = await _northwindContext.Vwventas.AsNoTracking().ToListAsync();


        //        int[] ordersId = await _dwnorthwindContext.FactOrders.Select(cd => cd.OrderNumber).ToArrayAsync();

        //        if (ordersId.Any())
        //        {
        //            await _dwnorthwindContext.FactOrders.Where(cd => ordersId.Contains(cd.OrderNumber))
        //                                          .AsNoTracking()
        //                                          .ExecuteDeleteAsync();
        //        }

        //        foreach (var venta in ventas)
        //        {
        //            var customer = await _dwnorthwindContext.DimCustomers.SingleOrDefaultAsync(cust => cust.CustomerId == venta.CustomerId);
        //            var employee = await _dwnorthwindContext.DimEmployees.SingleOrDefaultAsync(emp => emp.EmployeeId == venta.EmployeeId);
        //            var shipper = await _dwnorthwindContext.DimShippers.SingleOrDefaultAsync(ship => ship.ShipperId == venta.ShipperId);
        //            var product = await _dwnorthwindContext.DimProductCategories.SingleOrDefaultAsync(pro => pro.ProductId == venta.ProductId);

        //            FactOrder factOrder = new FactOrder()
        //            {
        //                CantidadVentas = venta.Cantidad.Value,
        //                Country = venta.Country,
        //                CustomerKey = customer.CustomerKey,
        //                EmployeeKey = employee.EmployeeKey,
        //                DateKey = venta.DateKey.Value,
        //                ProductKey = product.ProductKey,
        //                Shipper = shipper.ShipperKey,
        //                TotalVentas = Convert.ToDecimal(venta.TotalVentas)
        //            };

        //            await _dwnorthwindContext.FactOrders.AddAsync(factOrder);

        //            await _dwnorthwindContext.SaveChangesAsync();
        //        }



        //        result.Success = true;
        //    }
        //    catch (Exception ex)
        //    {

        //        result.Success = false;
        //        result.Message = $"Error cargando el fact de ventas {ex.Message} ";
        //    }

        //    return result;
        //}

        //private async Task<OperationResult> LoadFactCustomerServed()
        //{
        //    OperationResult result = new OperationResult() { Success = true };

        //    try
        //    {
        //        var customerServeds = await _northwindContext.VwServedCustomers.AsNoTracking().ToListAsync();

        //        int[] customerIds = _dwnorthwindContext.FactClienteAtendidos.Select(cli => cli.ClienteAtendidoId).ToArray();

        //        //Limpiamos la tabla de facts //

        //        if (customerIds.Any())
        //        {
        //            await _dwnorthwindContext.FactClienteAtendidos.Where(fact => customerIds.Contains(fact.ClienteAtendidoId))
        //                                                    .AsNoTracking()
        //                                                    .ExecuteDeleteAsync();
        //        }

        //        //Carga el fact de clientes atendidos. //
        //        foreach (var customer in customerServeds)
        //        {
        //            var employee = await _dwnorthwindContext.DimEmployees
        //                                              .SingleOrDefaultAsync(emp => emp.EmployeeId ==
        //                                                                       customer.EmployeeId);


        //            FactClienteAtendido factClienteAtendido = new FactClienteAtendido()
        //            {
        //                EmployeeKey = employee.EmployeeKey,
        //                TotalClientes = customer.TotalCustomersServed
        //            };


        //            await _dwnorthwindContext.FactClienteAtendidos.AddAsync(factClienteAtendido);

        //            await _dwnorthwindContext.SaveChangesAsync();
        //        }

        //        result.Success = true;

        //    }
        //    catch (Exception ex)
        //    {

        //        result.Success = false;
        //        result.Message = $"Error cargando el fact de clientes atendidos {ex.Message} ";
        //    }
        //return result;
    }
    }

