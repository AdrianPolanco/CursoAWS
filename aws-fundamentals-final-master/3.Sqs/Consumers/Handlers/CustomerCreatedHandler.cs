using Consumers.Messages;
using MediatR;

namespace Consumers.Handlers
{
    public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
    {
        private readonly ILogger<CustomerCreatedHandler> _logger;

        public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
        {
            _logger = logger;
        }
        public Task<Unit> Handle(CustomerCreated request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(request.FullName);
            //throw new Exception("Intentional dead letter queue exception"); 
            //If any exceptions shows up and we have configured a Dead Letter Queue (a queue where our messages that have failed will go)
            //AWS SQS automatically will send the message to our Dead Letter Queue depending on the number of tryings we have configured
            return Unit.Task;
        }
    }
}
