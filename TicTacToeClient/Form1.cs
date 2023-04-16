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
    private bool choice, flag = true;
    private int roomId;
    private string? response;
    private Task? t;
    public Form1()
    {
        InitializeComponent();
    }

    public void ChangeButtonTextEmpty()
    {
        if (buttons != null)
            foreach (Button b in buttons)
                b.Text = string.Empty;
    }

    public bool? PlayChecker()
    {
        if (button1.Text == X_O && button1.Text == button2.Text && button2.Text == button3.Text)
            return true;
        else if (button1.Text == X_O && button1.Text == button4.Text && button4.Text == button7.Text)
            return true;
        else if (button1.Text == X_O && button1.Text == button5.Text && button5.Text == button9.Text)
            return true;
        else if (button2.Text == X_O && button2.Text == button5.Text && button5.Text == button8.Text)
            return true;
        else if (button3.Text == X_O && button3.Text == button5.Text && button5.Text == button7.Text)
            return true;
        else if (button3.Text == X_O && button3.Text == button6.Text && button6.Text == button9.Text)
            return true;
        else if (button4.Text == X_O && button4.Text == button5.Text && button5.Text == button6.Text)
            return true;
        else if (button7.Text == X_O && button7.Text == button8.Text && button8.Text == button9.Text)
            return true;
        else if (button1.Text != string.Empty && button2.Text != string.Empty && button3.Text != string.Empty &&
            button4.Text != string.Empty && button5.Text != string.Empty &&
            button6.Text != string.Empty && button7.Text != string.Empty &&
            button8.Text != string.Empty && button9.Text != string.Empty)
            return null;
        else
            return false;
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
    public void ClientRequest(Button btn)
    {
        if (sw != null && sr != null)
        {
            if (PlayChecker() == true)
            {
                label5.Text = "You win game";
                sw.WriteLine(btn.Name + '/');
                buttons?.Remove(btn);

                btn.Enabled = false;
                ButtonDeactivate();

                sr?.Close();
                sw?.Close();

                sr = null;
                sw = null;
                buttons?.Clear();

                Invoke(() =>
                {
                    btn.Enabled = true;
                    comboBox1.Enabled = true;
                    textBox1.Enabled = true;
                });
            }
            else if (PlayChecker() == false)
            {
                sw.WriteLine(btn.Name);
                buttons?.Remove(btn);

                Invoke(() =>
                {
                    btn.Enabled = false;
                    ButtonDeactivate();
                });
            }
            else
            {
                label5.Text = "Game is draw...";
                sw.WriteLine(btn.Name + '=');
                buttons?.Remove(btn);

                btn.Enabled = false;
                ButtonDeactivate();

                sr?.Close();
                sw?.Close();

                sr = null;
                sw = null;
                buttons?.Clear();

                btn.Enabled = true;
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
            }
        }
    }

    public void ClientResponse()
    {
        if (sw != null && sr != null)
        {
            response = sr.ReadLine();

            if (response != null)
            {
                if (response.Last() == '/')
                {
                    response = response.Remove(response.Length - 1);

                    Invoke(() =>
                    {
                        buttons?.Remove(ButtonChecker(response) ?? new Button());
                        label5.Text = "Rival win...";
                    });

                    sw?.Close();
                    sr?.Close();
                    ns?.Close();
                    client?.Close();

                    sw = null;
                    sr = null;
                    ns = null;
                    client = null;
                    flag = false;
                    buttons?.Clear();

                    Invoke(() =>
                    {
                        btn.Enabled = true;
                        comboBox1.Enabled = true;
                        textBox1.Enabled = true;
                    });
                }
                else if (response.Last() == '=')
                {
                    response = response.Remove(response.Length - 1);

                    Invoke(() =>
                    {
                        buttons?.Remove(ButtonChecker(response) ?? new Button());
                        label5.Text = "Game is draw...";
                    });

                    sw?.Close();
                    sr?.Close();
                    ns?.Close();
                    client?.Close();

                    sw = null;
                    sr = null;
                    ns = null;
                    client = null;
                    flag = false;
                    buttons?.Clear();

                    Invoke(() =>
                    {
                        btn.Enabled = true;
                        comboBox1.Enabled = true;
                        textBox1.Enabled = true;
                    });
                }
                else
                    Invoke(() =>
                    {
                        buttons?.Remove(ButtonChecker(response) ?? new Button());
                        ButtonActivate();
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
                            ChangeButtonTextEmpty();

                            label2.Text = message.Remove(message.Length - 1);

                            comboBox1.Enabled = false;
                            button.Enabled = false;
                            textBox1.Enabled = false;

                            string? mes = await sr.ReadLineAsync();
                            string[]? arr = mes?.Split('+');

                            label2.Text = arr![0];
                            label4.Text = arr[1];

                            ButtonActivate();
                            flag = true;

                            t = new Task(() =>
                            {
                                while (flag)
                                {
                                    ClientResponse();
                                }
                            });

                            t.Start();
                        }
                        else if (c == '+')
                        {
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
                            ChangeButtonTextEmpty();

                            label2.Text = message.Remove(message.Length - 1);

                            comboBox1.Enabled = false;
                            button.Enabled = false;
                            textBox1.Enabled = false;
                            flag = true;

                            t = new Task(() =>
                            {
                                while (flag)
                                {
                                    ClientResponse();

                                }
                            });

                            t.Start();
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
            if (sw != null)
                ClientRequest(button);
        }
    }
}