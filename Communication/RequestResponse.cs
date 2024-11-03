using System.Reactive.Linq;

namespace Communication.Generic
{

    public class RequestResponse<TRequest, TReply>
        : IRequester<TRequest, TReply> where TRequest : class where TReply : class
        , IReplier<TRequest, TReply>
    {
        // for Request
        [Ninject.Inject]
        public IPublisher<TRequest>? Publisher { get; set; }
        [Ninject.Inject]
        public ISubscriber<TReply>? Subscriber { get; set; }

        // for SubscribeRequests
        [Ninject.Inject]
        public IPublisher<TReply>? Replies { get; set; }
        [Ninject.Inject]
        public ISubscriber<TRequest>? Requests { get; set; }

        public async Task<TReply> Request(TRequest message, CancellationToken cancellationToken = default)
        {
            if (Publisher == null)
            {
                throw new System.Exception("Publisher is not set, should be set by DI");
            }
            if (Subscriber == null)
            {
                throw new System.Exception("Subscriber is not set, should be set by DI");
            }
            if (Requests == null)
            {
                throw new System.Exception("Replies is not set, should be set by DI");
            }
            await Publisher.Publish(message, cancellationToken);
            await Requests.MessageReceived.Publish(message);
            return await Subscriber.MessageReceived.FirstAsync();
        }

        public IDisposable SubscribeRequests(Func<TRequest, TReply> handler)
        {
            if (Requests == null)
            {
                throw new System.Exception("Requests is not set, should be set by DI");
            }
            if (Replies == null)
            {
                throw new System.Exception("Replies is not set, should be set by DI");
            }

            return Requests.MessageReceived.Subscribe(async request =>
            {
                var reply = handler(request);
                await Replies.Publish(reply);
            });
        }
    }

}