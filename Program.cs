using Communication;
// See https://aka.ms/new-console-template for more information

Console.WriteLine("--start--");

var replier = new SimpleReplier();

var disposable = Service.GetInstance().SubscribeRequests<SimpleRequest, SimpleReply>(SimpleHandler.Handle);

Console.WriteLine("subscribed, about to run");
var results = Service.GetInstance().Run<SimpleRequest, SimpleReply>(new SimpleRequest() { Message = "starting" });
var counter = 1;
Console.WriteLine($"got results, results.length: {results.Count()}");
foreach (var result in results)
{
    Console.WriteLine($"result {counter++}: {result.Message}");
}




Console.WriteLine("--end--");