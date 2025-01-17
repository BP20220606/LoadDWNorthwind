﻿

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

                await LimpiarDatos();
                await LoadDimDates();
                await LoadDimProduct();
                await LoadDimCustomers();
                await LoadDimEmployee();
                await LoadDimShippers();
                await LoadFactOrders();
                await LoadFactCustomerServed();


            }
            catch (Exception ex)
            {

                result.Success = false;
                result.Message = $"Error cargando el DWH Northwind. {ex.Message}";
            }

            return result;
        }

        private async Task LimpiarDatos()
        {
            try
            {
                // Eliminar todas las filas de las tablas de dimensiones y hechos utilizando ExecuteDeleteAsync
                await _dwnorthwindContext.FactOrders.ExecuteDeleteAsync();
                await _dwnorthwindContext.FactCustomersServed.ExecuteDeleteAsync();
                await _dwnorthwindContext.DimEmployees.ExecuteDeleteAsync();
                await _dwnorthwindContext.DimProducts.ExecuteDeleteAsync();
                await _dwnorthwindContext.DimShippers.ExecuteDeleteAsync();
                await _dwnorthwindContext.DimCustomers.ExecuteDeleteAsync();
                await _dwnorthwindContext.DimDates.ExecuteDeleteAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar datos del DWH Northwind: {ex.Message}");
            }
        }

        private async Task<OperationResult> LoadDimEmployee()
        {
            OperationResult result = new OperationResult();

            try
            {
                var employees = await _northwindContext.Employees.AsNoTracking().Select(emp => new DimEmployee()
                {
                    EmployeeID = emp.EmployeeID,
                    EmployeeName = string.Concat(emp.FirstName, " ", emp.LastName),
                    EmployeeTitle = emp.Title
                }).ToListAsync();

                int[] employeeIds = employees.Select(emp => emp.EmployeeID).ToArray();

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
                                               join category in _northwindContext.Categories on product.CategoryId equals category.CategoryID
                                               select new DimProduct()
                                               {
                                                   ProductID = product.ProductId,
                                                   ProductName = product.ProductName,
                                                   CategoryID = category.CategoryID,
                                                   CategoryName = category.CategoryName

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
                result.Message = $"Error cargando la dimension de producto. {ex.Message}";
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
                    CustomerName = cust.CompanyName,
                    Country = cust.Country

                }).AsNoTracking()
                  .ToListAsync();


                string[] customersIds = customers.Select(cust => cust.CustomerID).ToArray();


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
                var shippers = await _northwindContext.Shippers.AsNoTracking().Select(ship => new DimShipper()
                {
                    ShipperID = ship.ShipperID,
                    ShipperName = ship.CompanyName
                }).ToListAsync();

            
                int[] shipperIds = shippers.Select(ship => ship.ShipperID).ToArray();

               
                await _dwnorthwindContext.DimShippers.AddRangeAsync(shippers);

             
                await _dwnorthwindContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando la dimensión de shippers: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> LoadDimDates()
        {
            var resultado = new OperationResult { Success = true };

            try
            {
                // Obtener todas las fechas únicas de los pedidos sin necesidad de un ciclo innecesario
                var orderDates = await _northwindContext.Orders
                    .AsNoTracking()
                    .Where(o => o.OrderDate.HasValue)
                    .Select(o => o.OrderDate.Value.Date) // Solo las fechas, sin horas
                    .Distinct()  // Solo fechas únicas
                    .ToListAsync();

                // Crear las entradas para DimDate en una sola acción sin foreach
                var fechas = orderDates.Select(orderDate => new DimDate
                {
                    DateKey = orderDate.Year * 10000 + orderDate.Month * 100 + orderDate.Day, // Calcular DateKey manualmente
                    Date = orderDate,
                    Year = orderDate.Year,
                    Month = orderDate.Month,
                    Day = orderDate.Day,
                    Quarter = (orderDate.Month - 1) / 3 + 1, // Calcula el trimestre
                    MonthName = orderDate.ToString("MMMM"),
                    DayName = orderDate.DayOfWeek.ToString(),
                }).ToList();

                // Insertar las fechas generadas en la tabla DimDates de una sola vez
                await _dwnorthwindContext.DimDates.AddRangeAsync(fechas);
                await _dwnorthwindContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                resultado.Success = false;
                resultado.Message = $"Error cargando la dimensión de fechas: {ex.Message}";
            }

            return resultado;
        }

        private async Task<OperationResult> LoadFactOrders()
        {
            var result = new OperationResult();

            try
            {
                // Obtener las órdenes desde la vista
                var orders = await _northwindContext.VwOrders.AsNoTracking().ToListAsync();

                // Obtener los IDs de las órdenes ya existentes en la tabla FactOrders para evitar duplicados
                int[] orderIds = await _dwnorthwindContext.FactOrders.Select(o => o.OrderID).ToArrayAsync();


                // Diccionarios de cache para evitar consultas repetidas
                var customerCache = new Dictionary<string, DimCustomer>();
                var employeeCache = new Dictionary<int, DimEmployee>();
                var shipperCache = new Dictionary<int, DimShipper>();
                var productCache = new Dictionary<int, DimProduct>();
                var dateCache = new Dictionary<int, DimDate>();

                // Lista para almacenar los registros de FactOrder a insertar
                var factOrders = new List<FactOrder>();

                // Recorrer todas las órdenes y agregar a la lista de factOrders
                foreach (var order in orders)
                {
                    // Obtener o cargar las dimensiones necesarias, usando el cache
                    if (!customerCache.TryGetValue(order.CustomerID, out var customer))
                    {
                        customer = await _dwnorthwindContext.DimCustomers
                            .FirstOrDefaultAsync(c => c.CustomerID == order.CustomerID);
                        customerCache[order.CustomerID] = customer;
                    }

                    if (!employeeCache.TryGetValue(order.EmployeeID, out var employee))
                    {
                        employee = await _dwnorthwindContext.DimEmployees
                            .FirstOrDefaultAsync(e => e.EmployeeID == order.EmployeeID);
                        employeeCache[order.EmployeeID] = employee;
                    }

                    if (!shipperCache.TryGetValue(order.ShipperID, out var shipper))
                    {
                        shipper = await _dwnorthwindContext.DimShippers
                            .FirstOrDefaultAsync(s => s.ShipperID == order.ShipperID);
                        shipperCache[order.ShipperID] = shipper;
                    }

                    if (!productCache.TryGetValue(order.ProductID, out var product))
                    {
                        product = await _dwnorthwindContext.DimProducts
                            .FirstOrDefaultAsync(p => p.ProductID == order.ProductID);
                        productCache[order.ProductID] = product;
                    }

                    if (!dateCache.TryGetValue(order.DateKey ?? 0, out var date) && order.DateKey.HasValue)
                    {
                        date = await _dwnorthwindContext.DimDates
                            .FirstOrDefaultAsync(d => d.DateKey == order.DateKey);
                        dateCache[order.DateKey.Value] = date;
                    }

                    // Verificar que todas las dimensiones existan antes de agregar el FactOrder
                    if (customer != null && employee != null && shipper != null && product != null && date != null)
                    {
                        var factOrder = new FactOrder
                        {
                            OrderID = order.OrderID,
                            CustomerKey = customer.CustomerKey,
                            EmployeeKey = employee.EmployeeKey,
                            ShipperKey = shipper.ShipperKey,
                            ProductKey = product.ProductKey,
                            DateKey = date.DateKey,
                            Country = order.Country,
                            TotalVentas = Convert.ToDecimal(order.TotalVentas ?? 0),
                            CantidadVentas = order.Cantidad.HasValue ? order.Cantidad.Value : 0
                        };

                        factOrders.Add(factOrder);
                    }
                }

                // Insertar todos los registros a la vez usando AddRangeAsync para eficiencia
                if (factOrders.Any())
                {
                    await _dwnorthwindContext.FactOrders.AddRangeAsync(factOrders);
                    await _dwnorthwindContext.SaveChangesAsync();
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error loading fact orders: {ex.Message}";
                if (ex.InnerException != null)
                {
                    result.Message += $" Inner exception: {ex.InnerException.Message}";
                }
            }

            return result;
        }


        private async Task<OperationResult> LoadFactCustomerServed()
        {
            OperationResult result = new OperationResult() { Success = true };

            try
            {
                // Materializa los datos de las consultas antes de comenzar el bucle
                var customerServeds = await _northwindContext.VwCustomersServed.AsNoTracking().ToListAsync();

                int[] customerIds = await _dwnorthwindContext.FactCustomersServed.Select(cli => cli.IDclientesatendidos).ToArrayAsync();



                // Lista para almacenar las nuevas entidades a agregar
                List<FactCustomerServed> factCustomerServeds = new List<FactCustomerServed>();

                foreach (var customer in customerServeds)
                {
                    var employee = await _dwnorthwindContext.DimEmployees
                                                            .AsNoTracking()
                                                            .SingleOrDefaultAsync(emp => emp.EmployeeID == customer.EmployeeID);

                    if (employee != null)
                    {
                        FactCustomerServed factClienteAtendido = new FactCustomerServed()
                        {
                            EmployeeKey = employee.EmployeeKey,
                            NombreEmpleado = employee.EmployeeName,
                            TotalClientesAtendidos = customer.NumeroClientes,
                        };

                        factCustomerServeds.Add(factClienteAtendido);
                    }
                }

                // Agrega todas las nuevas entidades a la vez
                await _dwnorthwindContext.FactCustomersServed.AddRangeAsync(factCustomerServeds);

                // Guarda los cambios una sola vez
                await _dwnorthwindContext.SaveChangesAsync();

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error cargando el fact de clientes atendidos {ex.Message} ";
            }

            return result;
        }
    }
}

