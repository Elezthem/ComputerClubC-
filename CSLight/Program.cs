using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace CSLight
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(10);
            computerClub.Work();
        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        public Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computersCount)
        {
            Random random = new Random();

            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }

            CreateNewClients(25, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 251), random));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба {_money} грн. Ждем нового клиента!");
                Console.WriteLine($"У нас новый клиент, и он хочет купить {newClient.DesiredMinutes} минут.");
                ShowAllComputersState();

                Console.Write("\nВы предлагаете ему компьютер под номером: ");
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int computerNumper))
                {
                    computerNumper -= 1;

                    if (computerNumper >= 0 && computerNumper < _computers.Count)
                    {
                        if (_computers[computerNumper].IsTaken)
                        {
                            Console.WriteLine("Вы посадили клиента, за компьютер, который уже занят. Клиент разозлился и ушел(");
                        }
                        else
                        {
                            if (newClient.CheckSolvercy(_computers[computerNumper]))
                            {
                                Console.WriteLine("Клиент успешно оплатил и сел за компьютер " + computerNumper);
                                _money += newClient.Pay();
                                _computers[computerNumper].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватает денег на балансе и он ушел(");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы сами не знаете за какой копьютер посадить клиента. Он обиделся и ушел(");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова!");
                }
                Console.WriteLine("Чтобы перейти к следующему клиенту, нажмите любую клавишу");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllComputersState()
        {
            Console.WriteLine("\nСписок всех компьютеров:");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.WriteLine(i + 1 + " - ");
                _computers[i].ShowState();
            }
        }

        private void SpendOneMinute()
        {
            foreach (var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    class Computer
    {
        private Client _client;
        private int _minutesRemaining;
        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }

        public int PricePerMinutes { get; private set; }

        public Computer(int pricesPerMinutes)
        {
            PricePerMinutes = pricesPerMinutes;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void BecomenEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowState()
        {
            if (IsTaken)
                Console.WriteLine($"Компьтер занят, осталось минут: {_minutesRemaining}");
            else
                Console.WriteLine($"Компьютер свободен! Цена за минуту: {PricePerMinutes}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;

        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvercy(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinutes;
            if (_money >= _moneyToPay)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }


        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}