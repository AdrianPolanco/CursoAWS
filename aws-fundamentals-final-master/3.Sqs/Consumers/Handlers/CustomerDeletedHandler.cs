﻿using Consumers.Messages;
using MediatR;

namespace Consumers.Handlers
{
    public class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
    {
        private readonly ILogger<CustomerDeletedHandler> _logger;

        public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
        {
            _logger = logger;
        }
        public Task<Unit> Handle(CustomerDeleted request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(request.Id.ToString());
            return Unit.Task;
        }
    }
}
