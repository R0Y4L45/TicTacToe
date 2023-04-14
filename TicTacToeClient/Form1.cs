using System.Net.Sockets;
using System.Net;

namespace TicTacToeClient;

public partial class Form1 : Form
{
    public TcpClient? client = null;
    public NetworkStream? ns = null;
    public StreamReader? sr = null;
    public StreamWriter? sw = null;
    public string? X_O { get; set; }

    private List<Button>? buttons;
    private bool choice, first = false;
    private int roomId;
    string? response, request;
    public Form1()
    {
        InitializeComponent();
        buttons = new List<Button>()
        {
            button1,
            button2,
            button3,
            button4,
            button5,
            button6,
            button7,
            button8,
            button9
        };
    }

    public void ButtonActivate()
    {
        if (buttons != null)
            foreach (Button btn in buttons)
                btn.Enabled = true;

    }
    public void ButtonDeactivate()
    {
        if (buttons != null)
            foreach (Button btn in buttons)
                btn.Enabled = false;

    }
    public Button? ButtonChecker(string response)
    {
        if (buttons != null)
        {
            foreach (Button btn in buttons)
                if (btn.Name == response)
                {
                    Invoke(() =>
                    {
                        btn.Text = choice ? "O" : "X";
                        btn.Enabled = false;
                    });

                    return btn;
                }
        }

        return null;
    }

    public void FirstClientConnection(Button btn)
    {
        if (sw != null && sr != null)
        {
            sw.WriteLine(btn.Name);
            buttons?.Remove(btn);
            Invoke(() =>
            {
                btn.Enabled = false;

                ButtonDeactivate();
            });

            response = sr.ReadLine();
            if (response != null)
            {
                buttons?.Remove(ButtonChecker(response) ?? new Button());

                Invoke(() =>
                {
                    ButtonActivate();
                });
            }
        }
    }

    public void LastClientRequest(Button btn)
    {
        if (sw != null && sr != null)
        {
            sw.WriteLine(btn.Name);
            buttons?.Remove(btn);
            Invoke(() =>
            {
                btn.Enabled = false;
                Invoke(() =>
                {
                    ButtonDeactivate();
                });
            });
        }
    }

    public void LastClientResponse()
    {
        if (sw != null && sr != null)
        {
            response = sr.ReadLine();

            if (response != null)
            {
                Invoke(() =>
                {
                    buttons?.Remove(ButtonChecker(response) ?? new Button());
                });
            }
        }
    }

    public async void Button_Click(object? sender, EventArgs e)
    {
        Button? button = sender as Button;

        if (button == btn)
        {
            if (textBox1.Text != string.Empty && int.TryParse(textBox1.Text, out roomId))
            {
                if (comboBox1.SelectedItem != null)
                {
                    try
                    {
                        client = new TcpClient();
                        client.Connect(IPAddress.Parse("127.1.1.1"), 1);

                        ns = client.GetStream();
                        sr = new StreamReader(ns);
                        sw = new StreamWriter(ns);
                        sw.AutoFlush = true;

                        choice = comboBox1.SelectedItem.ToString() == "X" ? true : false;
                        X_O = choice ? "X" : "O";

                        sw.WriteLine(roomId.ToString() + ' ' + choice.ToString());

                        string? message = await sr.ReadLineAsync() ?? "No message...";
                        char c = message.Last();

                        if (c == '/')
                        {
                            label2.Text = message.Remove(message.Length - 1);

                            comboBox1.Enabled = false;
                            button.Enabled = false;
                            textBox1.Enabled = false;

                            string? mes = await sr.ReadLineAsync();
                            string[]? arr = mes?.Split('+');

                            label2.Text = arr![0];
                            label4.Text = arr[1];

                            first = true;

                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    LastClientResponse();

                                    Invoke(() =>
                                    {
                                        ButtonActivate();
                                    });
                                }
                            });

                            ButtonActivate();

                            
                        }
                        else if (c == '+')
                        {
                            label2.Text = message.Remove(message.Length - 1);

                            comboBox1.Enabled = false;
                            button.Enabled = false;
                            textBox1.Enabled = false;
                            first = false;

                            ButtonActivate();

                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    LastClientResponse();

                                    Invoke(() =>
                                    {
                                        ButtonActivate();
                                    });
                                }
                            });

                        }
                        else if (c == '-')
                        {
                            label2.Text = message.Remove(message.Length - 1);

                            sr.Close();
                            sw.Close();
                            ns.Close();
                            client.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Please choice X/O...");

                    if (sr != null && sw != null && ns != null && client != null)
                    {
                        sr.Close();
                        sw.Close();
                        ns.Close();
                        client.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Room ID must be numeric only...");

                if (sr != null && sw != null && ns != null && client != null)
                {
                    sr.Close();
                    sw.Close();
                    ns.Close();
                    client.Close();
                }
            }
        }
        else if (button != null && button != btn)
        {
            button.Text = X_O;

            if (first)
            {
                LastClientRequest(button);
            }
            else
            {
                LastClientRequest(button);
            }

        }


    }
}