using MediatR;

namespace Consumers.Messages
{
    public class CustomerDeleted: ISqsMessage
    {
        public Guid Id { get; init; } = default!;

    }
}
