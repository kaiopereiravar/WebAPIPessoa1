using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Eventos
{
    public class RabbitMQProducer : IRabbitMQProducer //Quem erdar dessa interface( criada nos eventos), precisa ter metodo que esta dentro dela 
    {
        private readonly string _server;//Assim que chamarem o Rabbit, as informações que estão no appsetings vai preencher esses três classes
        private readonly string _username;
        private readonly string _password;

        public RabbitMQProducer(IConfiguration configuration)// configurations é uma representação do nosso appsettings la no startup(sem ele, o appsettings não funcionaria)
        {
            _server = configuration.GetValue<string>("Rabbit:Server"); //esta pegando a configuração que fica localizado no startup, que referencia o nosso appsetings, no app settings ele apresenta como string, então estamos pegando a string que esta dentro desse parênteses, que esta la no nosso appsettings
            _username = configuration.GetValue<string>("Rabbit:UserName");
            _password = configuration.GetValue<string>("Rabbit:Password");
        }

        public void EnviarMensagem<T>(T message, string queue, string exchange,string routingKey)//Void significa que ele não retorna nada
        {
            var factory = new ConnectionFactory
            {
                HostName = _server,
                UserName = _username,
                Password = _password,
            };

            var con = factory.CreateConnection(); //usar a factory e criar uma conexão
            using var channel = con.CreateModel(); //utilizar essa conexão

            channel.QueueDeclare(queue, true, false, true); //Para criar filas automaticamente quando subir o codigo
            channel.ExchangeDeclare(exchange, "topic", true, true); //aqui é a Exchange(broker) q organiza as ações
            channel.QueueBind(queue, exchange, routingKey); //a ligação da fila com o broker, e o passamento da RoutingKey

            var json = JsonConvert.SerializeObject(message); //Transformando esse arquivo json(message...) em string, é oq vamos mandar de mensagem para a fila
            var body = Encoding.UTF8.GetBytes(json);//não conseguimos mandar um texto na fila, entt transformamos em Bytes

            channel.BasicPublish(exchange: exchange, routingKey: routingKey, body: body); //jogando a msg no rabbit Obs: body é o Byte de uma string
        }
    }
}
