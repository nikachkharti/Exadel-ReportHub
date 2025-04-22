using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Repository;
using Serilog;
using Item = ReportHub.Domain.Entities.Item;

namespace ReportHub.Infrastructure.Middleware
{
    public class DataSeedingMiddleware
    {
        private readonly RequestDelegate _next;
        private ICurrencyRepository _currencyRepository;
        private ICountryRepository _countryRepository;

        public DataSeedingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                _currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
                _countryRepository = scope.ServiceProvider.GetRequiredService<ICountryRepository>();

                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                var itemRepository = scope.ServiceProvider.GetRequiredService<IItemRepository>();
                var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();
                var planRepository = scope.ServiceProvider.GetRequiredService<IPlanRepository>();
                var saleRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();

                var countryId = await SeedCountryAndCurrencyAsync();

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
                            CountryId = countryId,
                            ClientId = "67fa2d8114e2389cd8064454",
                            Email = "jonhode1@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd8064458",
                            Name = "Bill Butcher",
                            CountryId = countryId,
                            ClientId = "67fa2d8114e2389cd8064453",
                            Email = "bb@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd8064459",
                            Name = "Freddy Krueger",
                            CountryId = countryId,
                            ClientId = "67fa2d8114e2389cd8064452",
                            Email = "freddy@gmail.com"
                        },
                        new Customer()
                        {
                            Id = "67fa2d8114e2389cd806445a",
                            Name = "John Cenna",
                            CountryId = countryId,
                            ClientId = "67fa2d8114e2389cd8064452",
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

                #region SALES SEED
                var existingSales = await saleRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingSales.Any())
                {
                    Log.Information("Seeding initial sales data...");

                    var sales = new List<Sale>()
                    {
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064480",
                            ClientId = "67fa2d8114e2389cd8064452",
                            ItemId = "67fa2d8114e2389cd8064460", // CRM system building
                            Amount = 3000.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-15)
                        },
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064481",
                            ClientId = "67fa2d8114e2389cd8064452",
                            ItemId = "67fa2d8114e2389cd8064461", // Landing page
                            Amount = 1000.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-14)
                        },
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064482",
                            ClientId = "67fa2d8114e2389cd8064453",
                            ItemId = "67fa2d8114e2389cd8064463", // Villa Building
                            Amount = 180000.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-12)
                        },
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064483",
                            ClientId = "67fa2d8114e2389cd8064453",
                            ItemId = "67fa2d8114e2389cd8064464", // Destroy service
                            Amount = 9000.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-11)
                        },
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064484",
                            ClientId = "67fa2d8114e2389cd8064454",
                            ItemId = "67fa2d8114e2389cd8064465", // Frontend course
                            Amount = 350.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-9)
                        },
                        new Sale()
                        {
                            Id = "67fa2d8114e2389cd8064485",
                            ClientId = "67fa2d8114e2389cd8064454",
                            ItemId = "67fa2d8114e2389cd8064466", // Fullstack course
                            Amount = 700.00m,
                            SaleDate = DateTime.UtcNow.AddDays(-8)
                        }
                    };

                    foreach (var sale in sales)
                    {
                        Log.Information($"Seeding sale for item {sale.ItemId} with amount {sale.Amount}");
                        await saleRepository.Insert(sale);
                    }

                    Log.Information("Sales seeding completed");
                }
                else
                {
                    Log.Information("Database already contains sales data. Skipping seeding...");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error($"Data seeding failed: {ex.Message}");
            }

            await _next(context);
        }

        private async Task<string> SeedCountryAndCurrencyAsync()
        {
            #region COUNTRY & CURRENCY SEED
            var existingCountries = await _countryRepository.GetAll(pageNumber: 1, pageSize: 1);
            var countryId = "";
            if (!existingCountries.Any())
            {
                Log.Information("Fetching and seeding countries and currencies from REST Countries API...");

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://restcountries.com/v3.1/all");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var countriesData = System.Text.Json.JsonDocument.Parse(jsonString).RootElement;

                    var countryEntities = new List<Country>();
                    var currencyEntities = new List<Currency>();

                    foreach (var country in countriesData.EnumerateArray())
                    {
                        var name = country.GetProperty("name").GetProperty("common").GetString();
                        var currencies = country.TryGetProperty("currencies", out var currencyProp)
                            ? currencyProp.EnumerateObject()
                            : Enumerable.Empty<System.Text.Json.JsonProperty>();

                        var countryEntity = new Country()
                        {
                            Id = ObjectId.GenerateNewId().ToString(), 
                            Name = name!
                        };
                        countryId = countryEntity.Id;

                        countryEntities.Add(countryEntity);

                        foreach (var currency in currencies)
                        {
                            currencyEntities.Add(new Currency()
                            {
                                Id = ObjectId.GenerateNewId().ToString(),
                                Code = currency.Name,
                                CountryId = countryEntity.Id
                            });
                        }
                    }

                    foreach (var c in countryEntities)
                    {
                        Log.Information($"Seeding country: {c.Name}");
                        await _countryRepository.Insert(c);
                    }

                    foreach (var c in currencyEntities)
                    {
                        Log.Information($"Seeding currency: {c.Code}");
                        await _currencyRepository.Insert(c);
                    }

                    Log.Information("Country and currency seeding completed");
                }
                else
                {
                    Log.Warning("Failed to fetch data from REST Countries API");
                }
            }
            else
            {
                Log.Information("Database already contains country data. Skipping seeding...");
            }

            return countryId;
            #endregion

        }
    }
}
