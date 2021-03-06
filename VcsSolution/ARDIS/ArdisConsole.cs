﻿using System;
using System.Collections.Generic;
using System.IO.Ports;

/// <summary>
/// ARDIS - Arduino Information System
/// Используется для обработки информации с порта приёма
/// информации о посещаемости
/// </summary>
namespace ARDIS
{
    public static class ArdisConsole
    {
        private static SerialPort port;
        private static ArdisWorker worker;
        private static ArdisSerialReader ardisReader;
        private static string portName = "COM7";
        private static int serialRate = 9600;
        private static Parity parity = Parity.None;
        private static int dataBits = 8;
        private static StopBits stopBits = StopBits.One;

        private static bool waitCommand = false;
        private static List<string> messageList = new List<string>();
        private static int deviceId = 1;

        public static void Main()
        {
            bool isConfigure = false;
            while (!isConfigure)
            {
                try
                {
                    ConfigureServer();
                    isConfigure = true;
                }
                catch (Exception err)
                {
                    Console.WriteLine($"\nОшибка! {err.Message}\n\n");
                }
            }
            ardisReader = new ArdisSerialReader(port);
            var ardisTask = ardisReader.StartReadingAsync(new ArdisSerialReader.WriteLog(WriteLog));
            while(true)
            {
                Console.WriteLine("Press any key, for send messages...");
                Console.ReadKey();
                waitCommand = true;
                Console.Write("Device Id: ");
                string deviceId = Console.ReadLine();
                Console.Write("Command: ");
                string command = Console.ReadLine();
                Console.Write("Value: ");
                string value = Console.ReadLine();
                waitCommand = false;

                SendMessage($"|{deviceId}|{command}|{value}|");
            }
            
        }

        public static void SendMessage(string msg)
        {
            try
            {        
                ardisReader.SendMessage(msg);
                Console.WriteLine($"{msg} | #SENDED#");
            } catch(Exception err)
            {
                Console.WriteLine($"{msg} ###ERROR WHILE SENDING MESSAGE### ---{err.Message}---");
            }
        }

        public static void WriteLog(string log)
        {
            if(log == "?\r")
            {
                Console.WriteLine("?");
                ardisReader.SendMessage(deviceId.ToString());
                Console.WriteLine(deviceId.ToString());
                deviceId++;
            } else if(log[0] == '|' || log[0] == '!')
            {
                Console.WriteLine(log);
            } else if (!waitCommand)
            {
                string s = "NOT ASSIGNED";
                try
                {
                    s = log.Remove(log.Length - 1);
                    string classroom = "";
                    string cardNumber = "";
                    bool isCardNum = false;
                    for(int i = 0; i < s.Length; i++)
                    {
                        if(s[i] != '|' && !isCardNum)
                        {
                            classroom += s[i];
                        }
                        else if(!isCardNum)
                        {
                            isCardNum = true;
                        } else
                        {
                            cardNumber += s[i];
                        }
                    }
                    worker.Send(cardNumber, classroom);
                    Console.WriteLine($"{s}| #SENDED#");
                }
                catch (Exception err)
                {
                    Console.WriteLine($"{s}| ###ERROR WHILE SENDING TO DATABASE### --{err.Message}--");
                }
                foreach(string msg in messageList)
                {
                    Console.WriteLine(msg);
                    messageList.Remove(msg);
                }
            } else
            {
                string s = "NOT ASSIGNED";
                try
                {
                    s = log.Remove(log.Length - 1);
                    string classroom = "";
                    string cardNumber = "";
                    bool isCardNum = false;
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] != '|' && !isCardNum)
                        {
                            classroom += s[i];
                        }
                        else if (!isCardNum)
                        {
                            isCardNum = true;
                        }
                        else
                        {
                            cardNumber += s[i];
                        }
                    }
                    worker.Send(cardNumber, classroom);
                    messageList.Add($"{s} | #SENDED#");                  
                }
                catch (Exception err)
                {
                    messageList.Add($"{s} | ###ERROR WHILE SENDING TO DATABASE### --{err.Message}--");
                }
            }
        }

        private static void WaitKey()
        {
            Console.WriteLine("Нажмите Enter, чтобы завершить работу...");
            Console.ReadLine();
        }

        private static void AutoConfigure()
        {
            port = new SerialPort(portName, serialRate, parity, dataBits, stopBits);
            worker = new ArdisWorker();
            Console.WriteLine("***Конфигурация выполнена успешно***\n" +
                "Параметры:\n" +
                $"-portName = {portName}\n" +
                $"-serialRate = {serialRate}\n" +
                $"-parity = {parity}\n" +
                $"-dataBits = {dataBits}\n" +
                $"-stopBits = {stopBits}\n\n");
        }

        private static void ConfigureServer()
        {
            Console.WriteLine("***Вы хотите выполнить конфигурацию сервера вручную?***\n" +
                "-Введите любое сообщение, чтобы согласиться\n" +
                "-Для автоматического режима просто нажмите Enter");
            if (Console.ReadLine().Length < 1)
            {
                AutoConfigure();
                return;
            }
            Conf_GetPortName();
            Conf_GetSerialRate();
            Conf_GetParity();
            Conf_GetDataBits();
            Conf_GetStopBits();
            AutoConfigure();
        }

        private static void Conf_GetPortName()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("***Введите название порта***");
                    portName = Console.ReadLine();
                    return;
                }
                catch (Exception error)
                {
                    Console.WriteLine($"***Ошибка! {error.Message}***");
                }
            }
        }

        private static void Conf_GetSerialRate()
        {
            while(true)
            {
                Console.WriteLine("***Введите частоту обращения SerialRate***");
                if (int.TryParse(Console.ReadLine(), out serialRate))
                {
                    return;
                } else
                {
                    Console.WriteLine("***Ошибка парсинга!***");
                }
            }
        }

        private static void Conf_GetParity()
        {
            while (true)
            {
                Console.WriteLine("***Выберите паритет***\n" +
                    "0 - None\n" +
                    "1 - Odd\n" +
                    "2 - Even\n" +
                    "3 - Mark\n" +
                    "4 - Space");
                if (int.TryParse(Console.ReadLine(), out int res))
                {
                    if(res < 0 || res > 4)
                    {
                        parity = Parity.None;
                        return;
                    }
                    parity = (Parity)res;
                    return;
                } else
                {
                    Console.WriteLine("***Ошибка! Выберите значение из диапазона 0-4!***");
                }
            }
        }

        private static void Conf_GetDataBits()
        {
            while (true)
            {
                Console.WriteLine("***Введите количество информационных битов***");
                if (int.TryParse(Console.ReadLine(), out dataBits))
                {
                    return;
                }
                else
                {
                    Console.WriteLine("***Ошибка парсинга!***");
                }
            }
        }

        private static void Conf_GetStopBits()
        {
            while (true)
            {
                Console.WriteLine("***Выберите биты остановки***\n" +                   
                    "1 - One\n" +
                    "2 - Two\n" +
                    "3 - OnePointFive\n");
                if (int.TryParse(Console.ReadLine(), out int res))
                {
                    if (res < 1 || res > 3)
                    {
                        Console.WriteLine("***Ошибка! StopBits не может быть равно 0!");
                    }
                    else
                    {
                        stopBits = (StopBits)res;
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("***Ошибка! Выберите значение из диапазона 1-3!***");
                }
            }
        }
    }   
}
