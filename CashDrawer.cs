using System;
using System.IO.Ports;

class CashDrawer
{
    public void Open()
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {

            SerialPort serialPort = new SerialPort();

            if (serialPort is SerialPort)
            {
                serialPort.PortName = port;
                serialPort.BaudRate = 9600;
                try
                {
                    serialPort.Open();
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write("OPEN");
                    }
                    serialPort.Close();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }

        }
    }
    
}

