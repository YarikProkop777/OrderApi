using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using OrderApi.Messaging.Receive.Options.v1;
using OrderApi.Service.v1.Models;
using OrderApi.Service.v1.Services;

namespace OrderApi.Messaging.Receive.Receiver.v1
{
    public class CustomerFullNameUpdateReceiver : BackgroundService
    {
        private readonly string _hostName;
        private readonly string _queueName;
        private readonly string _userName;
        private readonly string _password;

        private readonly ICustomerNameUpdateService _customerNameUpdateService;

        private IConnection _connection;
        private IModel _channel;


        public CustomerFullNameUpdateReceiver(ICustomerNameUpdateService service, IOptions<RabbitMqConfiguration> options)
        {
            _hostName = options.Value.Hostname;
            _queueName = options.Value.QueueName;
            _userName = options.Value.UserName;
            _password = options.Value.Password;

            _customerNameUpdateService = service;

            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };

            _connection = factory.CreateConnection();
            // add event handler
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            // register event handlers
            consumer.Received += OnConsumerReceived;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.Shutdown += OnConsumerShutDown;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

        #region Event Handlers

        private void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var content = Encoding.UTF8.GetString(e.Body.ToArray());
            var updateCustomerModel = JsonConvert.DeserializeObject<UpdateCustomerFullNameModel>(content);

            _customerNameUpdateService.UpdateCustomerNameInOrders(updateCustomerModel);

            _channel.BasicAck(e.DeliveryTag, false);
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutDown(object sender, ShutdownEventArgs e)
        {
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        #endregion
    }
}
