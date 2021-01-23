using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using OrderApi.Models;
using OrderApi.Validators;
using OrderApi.Service.v1.Query;
using OrderApi.Service.v1.Command;
using OrderApi.Domain;
using OrderApi.Data.Database;
using OrderApi.Data.Repository;
using OrderApi.Messaging.Receive.Options.v1;
using OrderApi.Service.v1.Services;
using OrderApi.Messaging.Receive.Receiver.v1;

namespace OrderApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddOptions();

            // Rabbit Mq configurations
            var MqConfigSettings = Configuration.GetSection("RabbitMq");
            var MqConfig = MqConfigSettings.Get<RabbitMqConfiguration>();
            services.Configure<RabbitMqConfiguration>(MqConfigSettings);

            services.AddDbContext<OrderContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddAutoMapper(typeof(Startup));

            services.AddMvc().AddFluentValidation();

            // swagger configurations
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Order API",
                    Description = "A simple API to create or pay orders",
                    Contact = new OpenApiContact
                    {
                        Name = "Yaroslav Prokopenko",
                        Email = "misterrprokop@gmail.com",
                        Url = new Uri("https://github.com/YarikProkop777/OrderApi")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext = actionContext as ActionExecutingContext;
                    
                    if(actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());

            // register services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddTransient<IValidator<OrderModel>, OrderModelValidator>();

            // register query services
            services.AddTransient<IRequestHandler<GetPaidOrdersQuery, List<Order>>, GetPaidOrdersQueryHandler>();
            services.AddTransient<IRequestHandler<GetOrderByIdQuery, Order>, GetOrderByIdQueryHandler>();
            services.AddTransient <IRequestHandler<GetOrdersByCustomerIdQuery, List<Order>>, GetOrdersByCustomerIdQueryHandler>();

            // register command services
            services.AddTransient<IRequestHandler<CreateOrderCommand, Order>, CreateOrderCommandHandler>();
            services.AddTransient<IRequestHandler<PayOrderCommand, Order>, PayOrderCommandHandler>();
            services.AddTransient<IRequestHandler<UpdateOrderCommand>, UpdateOrderCommandHandler>();

            // register additional services
            services.AddTransient<ICustomerNameUpdateService, CustomerNameUpdateService>();

            if (MqConfig.Enabled)
            {
                services.AddHostedService<CustomerFullNameUpdateReceiver>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // send 'Strict-Transport-Security' header from server to client(browser)
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // using swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
                c.RoutePrefix = string.Empty;
            });

            // use routing with endpoints
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
