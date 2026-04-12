using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKH.Application.Interfaces.Messaging
{
    public interface IMessagePublisher
    {
        void Publish<T>(string queueName, T message);
    }
}