using Consumers.Messages;
using MediatR;

namespace Consumers.Handlers
{
    public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
    {
        private readonly ILogger<CustomerUpdatedHandler> _logger;

        public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
        {
            _logger = logger;
        }
        public Task<Unit> Handle(CustomerUpdated request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(request.GitHubUsername);

            return Unit.Task;
        }
    }
}
