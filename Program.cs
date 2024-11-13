using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BankAccount
{
    // Приватні поля
    private string lastName;
    private string firstName;
    private DateTime lastTransactionDate;
    private decimal balance;

    // Властивості для доступу до полів
    public string LastName
    {
        get { return lastName; }
        set { lastName = value; }
    }

    public string FirstName
    {
        get { return firstName; }
        set { firstName = value; }
    }

    public DateTime LastTransactionDate
    {
        get { return lastTransactionDate; }
        set { lastTransactionDate = value; }
    }

    public decimal Balance
    {
        get { return balance; }
        set { balance = value; }
    }

    // Конструктор без параметрів
    public BankAccount() { }

    // Конструктор з параметрами
    public BankAccount(string lastName, string firstName, DateTime lastTransactionDate, decimal balance)
    {
        LastName = lastName;
        FirstName = firstName;
        LastTransactionDate = lastTransactionDate;
        Balance = balance;
    }

    // Метод для збереження запису у рядок
    public override string ToString()
    {
        return $"{LastName},{FirstName},{LastTransactionDate:yyyy-MM-dd},{Balance}";
    }

    // Метод для створення запису з рядка
    public static BankAccount FromString(string data)
    {
        var parts = data.Split(',');
        if (parts.Length != 4) throw new FormatException("Некоректний формат рядка.");

        return new BankAccount(
            parts[0],
            parts[1],
            DateTime.Parse(parts[2]),
            decimal.Parse(parts[3])
        );
    }
}

public class BankDatabase
{
    private const string FilePath = "database.txt";
    private List<BankAccount> accounts;

    public BankDatabase()
    {
        accounts = LoadFromFile();
    }

    // Метод для завантаження даних із файлу
    private List<BankAccount> LoadFromFile()
    {
        var records = new List<BankAccount>();

        if (File.Exists(FilePath))
        {
            foreach (var line in File.ReadAllLines(FilePath))
            {
                try
                {
                    records.Add(BankAccount.FromString(line));
                }
                catch (FormatException)
                {
                    Console.WriteLine("Помилка форматування запису: " + line);
                }
            }
        }

        return records;
    }

    // Метод для збереження даних у файл
    private void SaveToFile()
    {
        File.WriteAllLines(FilePath, accounts.Select(a => a.ToString()));
    }

    // Додавання запису
    public void AddAccount(BankAccount account)
    {
        accounts.Add(account);
        SaveToFile();
    }

    // Редагування запису
    public void EditAccount(int index, BankAccount updatedAccount)
    {
        if (index >= 0 && index < accounts.Count)
        {
            accounts[index] = updatedAccount;
            SaveToFile();
        }
        else
        {
            Console.WriteLine("Запис із вказаним індексом не знайдено.");
        }
    }

    // Видалення запису
    public void DeleteAccount(int index)
    {
        if (index >= 0 && index < accounts.Count)
        {
            accounts.RemoveAt(index);
            SaveToFile();
        }
        else
        {
            Console.WriteLine("Запис із вказаним індексом не знайдено.");
        }
    }

    // Виведення всіх записів
    public void DisplayAccounts()
    {
        Console.WriteLine("Прізвище\tІм'я\tДата останньої операції\tСума вкладу");
        Console.WriteLine("--------------------------------------------------------");
        for (int i = 0; i < accounts.Count; i++)
        {
            var acc = accounts[i];
            Console.WriteLine($"{i + 1}. {acc.LastName}\t{acc.FirstName}\t{acc.LastTransactionDate:yyyy-MM-dd}\t{acc.Balance}");
        }
    }

    // Пошук за датою операції
    public void SearchByDate(DateTime date)
    {
        var results = accounts.Where(a => a.LastTransactionDate == date).ToList();
        if (results.Count == 0)
        {
            Console.WriteLine("Записів з вказаною датою не знайдено.");
        }
        else
        {
            Console.WriteLine("Результати пошуку:");
            foreach (var acc in results)
            {
                Console.WriteLine($"{acc.LastName}, {acc.FirstName}, {acc.LastTransactionDate:yyyy-MM-dd}, {acc.Balance}");
            }
        }
    }

    // Сортування за сумою вкладу
    public void SortByBalance()
    {
        accounts = accounts.OrderByDescending(a => a.Balance).ToList();
        SaveToFile();
    }
}

class Program
{
    static void Main()
    {
        BankDatabase bankDb = new BankDatabase();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("a - Додати запис");
            Console.WriteLine("e - Редагувати запис");
            Console.WriteLine("d - Видалити запис");
            Console.WriteLine("s - Показати всі записи");
            Console.WriteLine("f - Пошук за датою операції");
            Console.WriteLine("o - Сортування за сумою вкладу");
            Console.WriteLine("Enter - Вихід");

            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.A:
                    AddAccount(bankDb);
                    break;
                case ConsoleKey.E:
                    EditAccount(bankDb);
                    break;
                case ConsoleKey.D:
                    DeleteAccount(bankDb);
                    break;
                case ConsoleKey.S:
                    bankDb.DisplayAccounts();
                    break;
                case ConsoleKey.F:
                    SearchByDate(bankDb);
                    break;
                case ConsoleKey.O:
                    bankDb.SortByBalance();
                    Console.WriteLine("База даних відсортована за сумою вкладу.");
                    break;
                case ConsoleKey.Enter:
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Невідома команда.");
                    break;
            }
        }
    }

    static void AddAccount(BankDatabase db)
    {
        Console.Write("Прізвище: ");
        string lastName = Console.ReadLine();

        Console.Write("Ім'я: ");
        string firstName = Console.ReadLine();

        DateTime lastTransactionDate = GetValidDate("Дата останньої операції (yyyy-MM-dd): ");
        decimal balance = GetValidDecimal("Сума вкладу: ");

        var newAccount = new BankAccount(lastName, firstName, lastTransactionDate, balance);
        db.AddAccount(newAccount);
        Console.WriteLine("Запис додано.");
    }

    static void EditAccount(BankDatabase db)
    {
        Console.Write("Введіть номер запису для редагування: ");
        int index = GetValidInteger() - 1;

        Console.Write("Прізвище: ");
        string lastName = Console.ReadLine();

        Console.Write("Ім'я: ");
        string firstName = Console.ReadLine();

        DateTime lastTransactionDate = GetValidDate("Дата останньої операції (yyyy-MM-dd): ");
        decimal balance = GetValidDecimal("Сума вкладу: ");

        var updatedAccount = new BankAccount(lastName, firstName, lastTransactionDate, balance);
        db.EditAccount(index, updatedAccount);
        Console.WriteLine("Запис відредаговано.");
    }

    static void DeleteAccount(BankDatabase db)
    {
        Console.Write("Введіть номер запису для видалення: ");
        int index = GetValidInteger() - 1;
        db.DeleteAccount(index);
        Console.WriteLine("Запис видалено.");
    }

    static void SearchByDate(BankDatabase db)
    {
        DateTime date = GetValidDate("Введіть дату для пошуку (yyyy-MM-dd): ");
        db.SearchByDate(date);
    }

    // Додаткові методи для перевірки введення
    static int GetValidInteger()
    {
        int result;
        while (!int.TryParse(Console.ReadLine(), out result) || result <= 0)
        {
            Console.WriteLine("Некоректне значення! Будь ласка, введіть додатне ціле число.");
        }
        return result;
    }

    static DateTime GetValidDate(string prompt)
    {
        DateTime date;
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out date)) return date;
            Console.WriteLine("Некоректний формат дати! Будь ласка, введіть дату у форматі yyyy-MM-dd.");
        }
    }

    static decimal GetValidDecimal(string prompt)
    {
        decimal result;
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out result)) return result;
            Console.WriteLine("Некоректне значення! Будь ласка, введіть дійсне число.");
        }
    }
}
