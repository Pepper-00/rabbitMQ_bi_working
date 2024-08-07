using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare("serverToClientQueue", false, false, false, null);
        channel.QueueDeclare("clientToServerQueue", false, false, false, null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"server received from client: {message}");
        };

        channel.BasicConsume(queue: "serverToClientQueue",
                             autoAck: true,
                             consumer: consumer);

        //Console.WriteLine("this is client. press enter to send message.");
        while (true)
        {
            var test_enter = Console.ReadLine();
            var body = Encoding.UTF8.GetBytes(test_enter);

            channel.BasicPublish("", "clientToServerQueue", null, body: body);
            Console.WriteLine($"client sends to Server: {test_enter}");
        }
    }
}
