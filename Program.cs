using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Receive
{
    public static void Main(string [] args)
    {
        var host = "localhost";
        var port = 6672;

        if(args.Length > 0)
        {
            host = args[0];
        }

        if(args.Length > 1)
        {
            if (int.TryParse(args[1], out var p))
            {
                port = p;
            }
        }

        var factory = new ConnectionFactory() { HostName = host, UserName = "sub", Password = "sub", Port = port };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello",

                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}