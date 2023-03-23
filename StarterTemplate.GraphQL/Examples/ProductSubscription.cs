using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace StarterTemplate.GraphQL.Examples
{
    /// <summary>
    /// Subscriptions using websockets protocol
    /// </summary>
    [ExtendObjectType("Subscription")]
    public class ProductSubscription
    {
        /// <summary>
        /// Listens to a topic to receive subscription messages.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="eventReceiver"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SubscribeAndResolve]
        public async ValueTask<ISourceStream<ProductEditPayload>> OnProductUpdated(string topic, [Service] ITopicEventReceiver eventReceiver, CancellationToken cancellationToken)
        {
            // Gets the messages that are passed from the mutation sendMessage
            return await eventReceiver.SubscribeAsync<string, ProductEditPayload>(
                    topic, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}