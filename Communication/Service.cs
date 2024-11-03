using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Ninject;

namespace Communication
{


    public class CommunicationModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IHandlers<SimpleRequest, SimpleReply>)).To(typeof(SimpleReplier)).InSingletonScope();
            Bind(typeof(IRequester<SimpleRequest, SimpleReply>)).To(typeof(SimpleRequester));
            Bind(typeof(ITransformer<SimpleMessageIn, SimpleMessageOut, SimpleRequest, SimpleReply>)).To(typeof(SimpleTransformer));
            Bind(typeof(IPublisher<SimpleMessageIn>)).To(typeof(SimplePublisher));
            Bind(typeof(IMySubject<SimpleMessageOut>)).To(typeof(SimpleSubscriber)).InSingletonScope();
            // Bind other dependencies here
        }
    }

    public class Service //: IReplier<TRequest, TReply> where TRequest : class where TReply : class
    {
        // private List<Subscriber<TRequest, TReply>> requests = new List<Subscriber<TRequest, TReply>>();
        // // private List<subscribers> requests = new List<subscribers>();

        private static Service? _instance = null;
        private static object serviceLocker = new object();
        public IKernel Kernel { get; private set; }


        public static Service GetInstance()
        {
            if (_instance == null)
            {
                lock (serviceLocker)
                {
                    if (_instance == null)
                    {
                        _instance = new Service();
                        // _instance.Init();
                    }
                }
            }
            return _instance;
        }

        private Service()
        {
            Kernel = new StandardKernel(new CommunicationModule());
        }


        public IEnumerable<TReply> Run<TRequest, TReply>(TRequest request) where TRequest : class where TReply : class
        {
            var repliers = Kernel.GetAll<IHandlers<TRequest, TReply>>();
            Console.WriteLine($"Run, found {repliers.Count()} repliers");
            var results = new List<TReply>();
            foreach (var replier in repliers)
            {
                // Use the replier as needed
                foreach (var handler in replier.Handlers)
                {
                    results.Add(handler(request));
                }
            }
            return results;
        }

        public IDisposable SubscribeRequests<TRequest, TReply>(Func<TRequest, TReply> handler) where TRequest : class where TReply : class
        {
            var replier = Kernel.Get<IHandlers<TRequest, TReply>>();
            var disposable = replier.SubscribeRequests(handler);
            Console.WriteLine($"SubscribeRequests, Handlers: {replier.Handlers.Count()}");
            return disposable;
        }



    }

}

