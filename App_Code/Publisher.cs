using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace TopicPublisher 
{ 
    public class Publisher : IDisposable 
    { 
        private bool disposed; 
        private readonly ISession session; 
        private readonly ITopic topic; 

        public Publisher(ISession session, string topicName) 
        { 
            this.session = session; 
            DestinationName = topicName; 
            topic = new ActiveMQTopic(DestinationName); 
            Producer = session.CreateProducer(topic); 
        } 

        public IMessageProducer Producer 
        { 
            get; 
            private set; 
        } 

        public string DestinationName 
        { 
            get; 
            private set; 
        } 

        public void SendMessage(string message) 
        { 
            if (disposed) throw new ObjectDisposedException(GetType().Name); 
            var textMessage = Producer.CreateTextMessage(message); 
            Producer.Send(textMessage); 
        } 

        public void Dispose() 
        { 
            if (disposed) return; 
            Producer.Close(); 
            Producer.Dispose(); 
            disposed = true; 
        } 
    } 
}