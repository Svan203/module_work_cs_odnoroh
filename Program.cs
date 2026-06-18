
using System.Text;

class Program
{
    delegate string TextOperation(string line);

    private static readonly Encoding FileEncoding = Encoding.UTF8;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        const string inputFile = "textPD23.txt";
        const string resultFile = "resultPD23.txt";
        const string logFile = "logPD23.txt";

        EnsureTextFile(inputFile);
        EnsureFileExists(resultFile);
        EnsureFileExists(logFile);

        File.WriteAllText(resultFile, string.Empty, FileEncoding);

        Console.WriteLine("Завдання 1. Обробка файлу textPD23.txt");

        ProcessFile(inputFile, resultFile, ToUpperCase, "Рядки у верхньому регістрі");
        ProcessFile(inputFile, resultFile, CountCharacters, "Кількість символів у рядках");
        ProcessFile(inputFile, resultFile, CountWords, "Кількість слів у рядках");

        Console.WriteLine("Результати записано у файл resultPD23.txt");
        Console.WriteLine();

        Console.WriteLine("Завдання 2. Введіть 4 повідомлення для запису в logPD23.txt");

        MessagePublisher publisher = new MessagePublisher();
        FileLogger logger = new FileLogger(logFile);
        logger.Subscribe(publisher);

        for (int i = 1; i <= 4; i++)
        {
            Console.Write($"Повідомлення {i}: ");
            string message = Console.ReadLine() ?? string.Empty;
            publisher.Send(message);
        }

        Console.WriteLine("Повідомлення записано у файл logPD23.txt");
    }

    static void ProcessFile(string sourcePath, string resultPath, TextOperation operation, string title)
    {
        string[] lines = File.ReadAllLines(sourcePath, FileEncoding);

        using StreamWriter writer = new StreamWriter(resultPath, append: true, FileEncoding);
        writer.WriteLine($"--- {title} ---");

        foreach (string line in lines)
        {
            writer.WriteLine(operation(line));
        }

        writer.WriteLine();
    }

    static string ToUpperCase(string line)
    {
        return line.ToUpper();
    }

    static string CountCharacters(string line)
    {
        return $"Рядок: \"{line}\" | Символів: {line.Length}";
    }

    static string CountWords(string line)
    {
        string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return $"Рядок: \"{line}\" | Слів: {words.Length}";
    }

    static void EnsureTextFile(string path)
    {
        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            string[] testLines =
            {
                "Modular work one",
                "Mazur Vitalii PD-23",
                "This file contains several text lines"
            };

            File.WriteAllLines(path, testLines, FileEncoding);
        }
    }

    static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, string.Empty, FileEncoding);
        }
    }
}

class MessagePublisher
{
    public event Action<string>? MessageSent;

    public void Send(string message)
    {
        MessageSent?.Invoke(message);
    }
}

class FileLogger
{
    private readonly string filePath;

    public FileLogger(string filePath)
    {
        this.filePath = filePath;
    }

    public void Subscribe(MessagePublisher publisher)
    {
        publisher.MessageSent += LogMessage;
    }

    private void LogMessage(string message)
    {
        string logLine = $"[{DateTime.Now:HH:mm:ss}] {message}";
        File.AppendAllText(filePath, logLine + Environment.NewLine, Encoding.UTF8);
    }
}
