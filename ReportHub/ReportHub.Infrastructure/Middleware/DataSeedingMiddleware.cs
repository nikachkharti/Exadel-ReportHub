﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;
using Item = ReportHub.Domain.Entities.Item;

namespace ReportHub.Infrastructure.Middleware
{
    public class DataSeedingMiddleware
    {
        private readonly RequestDelegate _next;

        public DataSeedingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
                var planRepository = scope.ServiceProvider.GetRequiredService<IPlanRepository>();

                #region CLIENTS SEED
                var existingClients = await clientRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingClients.Any())
                {
                    var clients = new List<Client>()
                    {
                        new Client()
                        {
                            Id = "67fa2d8114e2389cd8064452",
                            Name = "Alpha Soft",
                            Specialization = "Software Development"
                        },
                        new Client()
                        {
                            Id = "67fa2d8114e2389cd8064453",
                            Name = "Brick CO",
                            Specialization = "Builiding and Development"
                        },
                        new Client()
                        {
                            Id = "67fa2d8114e2389cd8064454",
                            Name = "Eduworld",
                            Specialization = "Education and schoolarship"
                        }
                    };

                    foreach (var client in clients)
                    {
                        Log.Information($"Seeding client: {client.Name}");
                        await clientRepository.Insert(client);
                    }

                    Log.Information("Client seeding completed");
                }
                else
                {
                    Log.Information("Database already contains clients data. Skipping seeding...");
                }

                #endregion

                #region CUSTOMERS SEED
                var existingCustomers = await customerRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingCustomers.Any())
                {
                    Log.Information("Seeding initial customers data...");

                    var customers = new List<Customer>()
                    {
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd8064457",
                            Name = "John Doe",
                            Address = "Doe street 12",
                            Email = "jonhode1@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd8064458",
                            Name = "Bill Butcher",
                            Address = "Butch street 1",
                            Email = "bb@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd8064459",
                            Name = "Freddy Krueger",
                            Address = "Krug street 31",
                            Email = "freddy@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd806445a",
                            Name = "John Cenna",
                            Address = "Cen street 20",
                            Email = "joncena@gmail.com"
                        }
                    };

                    foreach (var customer in customers)
                    {
                        Log.Information($"Seeding customer: {customer.Name}");
                        await customerRepository.Insert(customer);
                    }

                    Log.Information("Customer seeding completed");
                }
                else
                {
                    Log.Information("Database already contains customers data. Skipping seeding...");
                }

                #endregion

                #region ITEMS SEED
                var existingItems = await itemRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingItems.Any())
                {
                    Log.Information("Seeding initial items data...");

                    var items = new List<Item>()
                    {
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064460",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "CRM system building",
                            Currency = "USD",
                            Description = "Building of CRM system for small and medium companies",
                            Price = 3000.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064461",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "Landing page building",
                            Currency = "USD",
                            Description = "Building of landing page",
                            Price = 1000.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064462",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "AML monitoring",
                            Currency = "USD",
                            Description = "Providing an AML monitoring service for small and medium companies",
                            Price = 18000.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064463",
                            ClientId = "67fa2d8114e2389cd8064453",
                            Name = "Villa Building",
                            Currency = "EUR",
                            Description = "Villa building",
                            Price = 180000.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064464",
                            ClientId = "67fa2d8114e2389cd8064453",
                            Name = "Destroy service",
                            Currency = "EUR",
                            Description = "Destroy service for old buildings",
                            Price = 9000.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064465",
                            ClientId = "67fa2d8114e2389cd8064454",
                            Name = "Course of frontend software development",
                            Currency = "USD",
                            Description = "Learn HTML CSS and Javascript",
                            Price = 350.00m
                        },
                        new Item()
                        {
                            Id = "67fa2d8114e2389cd8064466",
                            ClientId = "67fa2d8114e2389cd8064454",
                            Name = "Course of fullstrack software development",
                            Currency = "USD",
                            Description = "Learn full stack programming",
                            Price = 700.00m
                        }
                    };

                    foreach (var item in items)
                    {
                        Log.Information($"Seeding items: {item.Name}");
                        await itemRepository.Insert(item);
                    }

                    Log.Information("Item seeding completed");
                }
                else
                {
                    Log.Information("Database already contains items data. Skipping seeding...");
                }

                #endregion

                #region INVOICES SEED
                var existingInvoices = await invoiceRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingInvoices.Any())
                {
                    Log.Information("Seeding initial invoices data...");

                    var invoices = new List<Invoice>()
                    {
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446a",
                            ClientId = "67fa2d8114e2389cd8064452",
                            CustomerId = "67fa2d8114e2389cd8064457",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "USD",
                            Amount = 4000.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064460",
                                "67fa2d8114e2389cd8064461"
                            }
                        },
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446b",
                            ClientId = "67fa2d8114e2389cd8064452",
                            CustomerId = "67fa2d8114e2389cd8064458",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "USD",
                            Amount = 1000.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064461"
                            }
                        },
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446c",
                            ClientId = "67fa2d8114e2389cd8064453",
                            CustomerId = "67fa2d8114e2389cd8064459",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "EUR",
                            Amount = 189000.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064464",
                                "67fa2d8114e2389cd8064463"
                            }
                        },
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446d",
                            ClientId = "67fa2d8114e2389cd8064453",
                            CustomerId = "67fa2d8114e2389cd8064459",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "EUR",
                            Amount = 180000.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064463"
                            }
                        },
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446e",
                            ClientId = "67fa2d8114e2389cd8064454",
                            CustomerId = "67fa2d8114e2389cd806445a",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "USD",
                            Amount = 350.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064465"
                            }
                        },
                        new Invoice()
                        {
                            Id = "67fa2d8114e2389cd806446f",
                            ClientId = "67fa2d8114e2389cd8064454",
                            CustomerId = "67fa2d8114e2389cd806445a",
                            IssueDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(20),
                            Currency = "USD",
                            Amount = 700.00m,
                            PaymentStatus = "Paid",
                            ItemIds = new List<string>()
                            {
                                "67fa2d8114e2389cd8064466"
                            }
                        }
                    };

                    foreach (var invoice in invoices)
                    {
                        Log.Information($"Seeding invoices: {invoice.Id}");
                        await invoiceRepository.Insert(invoice);
                    }

                    Log.Information("Item seeding completed");
                }
                else
                {
                    Log.Information("Database already contains invoice data. Skipping seeding...");
                }

                #endregion


                #region PLANS SEED
                var existingPlans = await planRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingPlans.Any())
                {
                    Log.Information("Seeding initial plans data...");

                    var plans = new List<Plan>()
                    {
                        new Plan()
                        {
                            Id = "680234508ed022f95e0789d9",
                            ClientId = "67fa2d8114e2389cd8064452",
                            ItemId = "67fa2d8114e2389cd8064460",
                            Amount = 3000.00m,
                            StartDate = DateTime.UtcNow.AddDays(1),
                            EndDate = DateTime.UtcNow.AddMonths(3),
                            Status = PlanStatus.Planned
                        },
                        new Plan()
                        {
                            Id = "680234508ed022f95e0789da",
                            ClientId = "67fa2d8114e2389cd8064453",
                            ItemId = "67fa2d8114e2389cd8064464",
                            Amount = 9000.00m,
                            StartDate = DateTime.UtcNow.AddDays(10),
                            EndDate = DateTime.UtcNow.AddMonths(6),
                            Status = PlanStatus.Planned
                        },
                        new Plan()
                        {
                            Id = "680234508ed022f95e0789db",
                            ClientId = "67fa2d8114e2389cd8064454",
                            ItemId = "67fa2d8114e2389cd8064465",
                            Amount = 350.00m,
                            StartDate = DateTime.UtcNow.AddDays(-5),
                            EndDate = DateTime.UtcNow.AddMonths(1),
                            Status = PlanStatus.InProgress
                        }
                    };

                    foreach (var plan in plans)
                    {
                        Log.Information($"Seeding plan for item: {plan.ItemId}");
                        await planRepository.Insert(plan);
                    }

                    Log.Information("Plan seeding completed");
                }
                else
                {
                    Log.Information("Database already contains plan data. Skipping seeding...");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error($"Data seeding failed: {ex.Message}");
            }

            await _next(context);
        }
    }
}
