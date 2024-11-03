
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Ninject;

namespace Communication
{
    public class SimpleHandler
    {
        public static SimpleReply Handle(SimpleRequest request)
        {
            request.Message += ", Handle";
            Console.WriteLine($"Handle, request: {request.Message}");

            var tcs = new TaskCompletionSource<SimpleReply>();

            var requester = Service.GetInstance().Kernel.Get<IRequester<SimpleRequest, SimpleReply>>();


            using (requester.Request(request).ContinueWith(task =>
            {
                Console.WriteLine($"Handle, continued with: {task.Result.Message}");
                if (task.IsCompletedSuccessfully)
                {
                    tcs.SetResult(task.Result);
                }
                else if (task.IsFaulted)
                {
                    tcs.SetException(task.Exception);
                }
                else if (task.IsCanceled)
                {
                    tcs.SetCanceled();
                }
            }))
            {
                Console.WriteLine($"Handle, return result {tcs.Task.Result} for request: {request.Message}");
                return tcs.Task.Result;
            }
        }
    }


    public class SimpleRequest
    {
        public string? Message { get; set; }
    }

    public class SimpleReply
    {
        public string? Message { get; set; }
    }

    public class SimpleMessageOut
    {
        public string? Message { get; set; }
    }

    public class SimpleMessageIn
    {
        public string? Message { get; set; }
    }

    public class SimpleReplier : IHandlers<SimpleRequest, SimpleReply>
    {
        private List<Func<SimpleRequest, SimpleReply>> handlers = new List<Func<SimpleRequest, SimpleReply>>();

        public IEnumerable<Func<SimpleRequest, SimpleReply>> Handlers => handlers.AsEnumerable();

        public IDisposable SubscribeRequests(Func<SimpleRequest, SimpleReply> handler)
        {

            handlers.Add(handler);

            return new Unsubscriber(handlers, handler);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<Func<SimpleRequest, SimpleReply>> _handlers;
            private readonly Func<SimpleRequest, SimpleReply> _handler;

            public Unsubscriber(List<Func<SimpleRequest, SimpleReply>> handlers, Func<SimpleRequest, SimpleReply> handler)
            {
                _handlers = handlers;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_handlers.Contains(_handler))
                {
                    _handlers.Remove(_handler);
                }
            }
        }

    }

    public class SimpleRequester : IRequester<SimpleRequest, SimpleReply>
    {
        public Task<SimpleReply> Request(SimpleRequest message, CancellationToken cancellationToken = default)
        {
            message.Message += ", SimpleRequester.Request";
            Console.WriteLine($"SimpleRequester, Request: {message.Message}");
            var tcs = new TaskCompletionSource<SimpleReply>();

            var transformer = Service.GetInstance().Kernel.Get<ITransformer<SimpleMessageIn, SimpleMessageOut, SimpleRequest, SimpleReply>>();
            var publisher = Service.GetInstance().Kernel.Get<IPublisher<SimpleMessageIn>>();
            var subscriber = Service.GetInstance().Kernel.Get<IMySubject<SimpleMessageOut>>();

            var task = publisher.Publish(transformer.Transform(message));

            Console.WriteLine($"SimpleRequester, received task: {task == null}");

            subscriber.MessageReceived.Subscribe(message =>
                        {
                            Console.WriteLine($"SimpleRequester, message received: {message.Message}");
                            SimpleReply reply = transformer.Transform(message);
                            Console.WriteLine($"SimpleRequester, message transformed: {reply.Message}");
                            tcs.SetResult(reply);
                        });
            return tcs.Task;
        }
    }

    public class SimpleTransformer : ITransformer<SimpleMessageIn, SimpleMessageOut, SimpleRequest, SimpleReply>
    {
        public SimpleMessageIn Transform(SimpleRequest message)
        {
            return new SimpleMessageIn() { Message = message.Message + ", transformed to SimpleMessageIn" };
        }

        public SimpleReply Transform(SimpleMessageOut request)
        {
            return new SimpleReply() { Message = request.Message + ", transformed to SimpleRequest" };
        }
    }

    public class SimplePublisher : IPublisher<SimpleMessageIn>
    {
        public Task Publish(SimpleMessageIn message, CancellationToken cancellationToken = default)
        {
            message.Message += ", SimplePublisher.Publish";
            Console.WriteLine($"SimplePublisher Publish: {message.Message}");
            // Simulate publishing the message
            return Task.Run(() =>
            {
                Console.WriteLine($"Message publishing ... {message.Message}");
                var subscriber = Service.GetInstance().Kernel.Get<IMySubject<SimpleMessageOut>>();
                Thread.Sleep(1000);
                subscriber.OnNext(new SimpleMessageOut { Message = message.Message + ", Publisher" });
                Console.WriteLine($"Message published: {message.Message}");
            }, cancellationToken);
        }
    }

    public class SimpleSubscriber : IMySubject<SimpleMessageOut>
    {
        private readonly Subject<SimpleMessageOut> _subject = new Subject<SimpleMessageOut>();

        public IObservable<SimpleMessageOut> MessageReceived => _subject;

        // not used but required by ISubject<T>
        public IDisposable Subscribe(IObserver<SimpleMessageOut> observer)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception exception)
        {
            _subject.OnError(exception);
        }

        public void OnNext(SimpleMessageOut value)
        {
            value.Message += ", SimpleSubscriber.OnNext";
            Console.WriteLine($"SimpleSubscriber, OnNext: {value.Message}");
            _subject.OnNext(value);
        }
    }
}

