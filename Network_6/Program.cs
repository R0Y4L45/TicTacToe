using Server;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

TcpListener? server = new TcpListener(IPAddress.Parse("127.1.1.1"), 1);
server.Start();

StreamReader? reader = null;
StreamWriter? writer = null;
TcpClient? client = null;
Dictionary<int, HashSet<ClientInfo>>? dic = new Dictionary<int, HashSet<ClientInfo>>();

void Method(int roomId)
{
    string? response_First, response_Last;

    ClientInfo clientFirst = dic[roomId].First();
    ClientInfo clientLast = dic[roomId].Last();

    if (clientFirst != null && clientLast != null && clientFirst.Client != null && clientLast.Client != null)
    {
        NetworkStream? nStream_First = null;
        StreamReader? rStream_First = null;
        StreamWriter? wStream_First = null;

        NetworkStream? nStream_Last = null;
        StreamReader? rStream_Last = null;
        StreamWriter? wStream_Last = null;

        nStream_First = clientFirst.Client?.GetStream();
        nStream_Last = clientLast.Client?.GetStream();

        if (nStream_First != null && nStream_Last != null)
        {
            rStream_First = new StreamReader(nStream_First!);
            wStream_First = new StreamWriter(nStream_First!);
            rStream_Last = new StreamReader(nStream_Last);
            wStream_Last = new StreamWriter(nStream_Last);

            wStream_First.AutoFlush = true;
            wStream_Last.AutoFlush = true;
        }
        else
            throw new Exception();

        //wStream_First.WriteLine("X");
        //wStream_Last.WriteLine("O");

        while (true)
        {

            response_First = rStream_First.ReadLine();
            wStream_Last.WriteLine(response_First);

            response_Last = rStream_Last.ReadLine();
            wStream_First.WriteLine(response_Last);

            response_First = default;
            response_Last = default;
        }
    }
};

while (true)
{
    client = server.AcceptTcpClient();
    reader = new StreamReader(client.GetStream());

    string? response = reader.ReadLine() ?? string.Empty;
    string[]? splitedResponse = response.Split(' ');

    if (splitedResponse != null && int.TryParse(splitedResponse[0], out int roomId))
    {
        bool x_o = bool.Parse(splitedResponse[1]);

        if (dic.ContainsKey(roomId))
        {
            if (dic[roomId].Count == 1)
            {
                if (dic[roomId].First().X_O == x_o)
                {
                    using (writer = new StreamWriter(client.GetStream()))
                    {
                        string choice = x_o ? "X" : "O";
                        writer.WriteLine($"{choice} was selected. Please choice another...-");
                        writer.Flush();

                        Console.WriteLine($"Client disconnected : {client.Client.RemoteEndPoint}");
                        reader.Close();
                        client.Close();
                    }
                }
                else
                {
                    Console.WriteLine($"Client connected : {client.Client.RemoteEndPoint}");

                    dic[roomId].Add(new ClientInfo()
                    {
                        Client = client,
                        X_O = x_o
                    });

                    try
                    {
                        string choice = x_o ? "X" : "O";
                        writer = new StreamWriter(client.GetStream());
                        writer.WriteLine($"You joined. Game start...+");
                        writer.Flush();

                        writer = new StreamWriter(dic[roomId].First().Client!.GetStream());
                        writer.WriteLine($"{choice} is connected. Game start...+Your turn");
                        writer.Flush();

                        Task.Run(() =>
                        {
                            Method(roomId);
                        });
                    }
                    catch (Exception ex)
                    {
                        using (writer = new StreamWriter(client.GetStream()))
                        {
                            Console.WriteLine(ex.Message);

                            writer.WriteLine($"Client couldn't connect...-");
                            writer.Flush();

                            Console.WriteLine($"Client disconnected : {client.Client.RemoteEndPoint}");
                            reader.Close();
                            client.Close();
                        }
                    }
                }
            }
            else
            {
                using (writer = new StreamWriter(client.GetStream()))
                {
                    writer.WriteLine("Room is Full...-");
                    writer.Flush();

                    Console.WriteLine("Room is Full");
                    Console.WriteLine($"Client couldn't connect : {client.Client.RemoteEndPoint}");

                    reader.Close();
                    client.Close();
                }
            }
        }
        else
        {
            HashSet<ClientInfo>? clientInfos = new HashSet<ClientInfo>();
            clientInfos.Add(new ClientInfo()
            {
                Client = client,
                X_O = x_o
            });

            dic.Add(roomId, clientInfos);

            string choice = x_o ? "X" : "O";

            writer = new StreamWriter(client.GetStream());
            writer.WriteLine($"{choice} connected. Waiting for another player.../");
            writer.Flush();

            Console.WriteLine($"Client connected : {client.Client.RemoteEndPoint}");
        }
    }
    else
    {
        Console.WriteLine($"Client disconnected : {client.Client.RemoteEndPoint}");

        reader.Close();
        client.Close();
    }

}


