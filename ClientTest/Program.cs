using System.Net;
using System.Net.Sockets;

Console.ReadLine();
TcpClient client = new TcpClient();

client.Connect(IPAddress.Parse("127.1.1.1"), 1);

NetworkStream ns = client.GetStream();

StreamReader sr = new StreamReader(ns);
StreamWriter sw = new StreamWriter(ns);
sw.AutoFlush = true;

Console.WriteLine("Enter room id : ");
string? roomID = Console.ReadLine();

sw.WriteLine(roomID);

string? line = sr.ReadLine();  

if (line != null && line == "X")
{
    while (true) 
    {
        Console.WriteLine("Enter word : ");
        
        sw.WriteLine(Console.ReadLine());

        Console.WriteLine("Sending : " + sr.ReadLine());
    }
}
else if(line != null && line == "O")
{
    while (true)
    {
        Console.WriteLine("Sending : " + sr.ReadLine());

        Console.WriteLine("Enter word : ");
        sw.WriteLine(Console.ReadLine());

    }
}
else
    Console.WriteLine("You don't choice correct room");
