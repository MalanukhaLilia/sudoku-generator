using System;
using System.IO;
using System.Text;

class SudokuGenerator
{
    private static int[,] originalSudoku;
    private static int[,] solvedSudoku;
    private static Random rand = new Random();

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.SetWindowSize(120, 40);

        while (true)
        {
            int[,] sudoku = GenerateSudoku();
            if (sudoku != null)
            {
                originalSudoku = (int[,])sudoku.Clone();
                solvedSudoku = GetSolvedSudoku((int[,])sudoku.Clone());
                DisplaySudoku(sudoku);
                AskToSaveSudoku(originalSudoku);
                PlaySudoku(sudoku);
            }
            else
            {
                Console.WriteLine("Не вдалося згенерувати судоку. \nСпробуємо ще раз...");
                System.Threading.Thread.Sleep(4000);
                Console.Clear();
            }
        }
    }

    // Метод для генерації судоку.
    static int[,] GenerateSudoku()
    {
        int[,] sudoku = new int[9, 9];
        if (FillRemaining(sudoku, 0, 0))
        {
            SetDifficultyLevel(sudoku);
            return sudoku;
        }
        else
        {
            return null;
        }
    }

    // Метод для рекурсивного заповнення пустих клітинок судоку.
    static bool FillRemaining(int[,] sudoku, int row, int col)
    {
        if (row == 9)
        {
            return true;
        }
        if (col == 9)
        {
            return FillRemaining(sudoku, row + 1, 0);
        }
        if (sudoku[row, col] != 0)
        {
            return FillRemaining(sudoku, row, col + 1);
        }
        int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Shuffle(nums, rand);
        foreach (int num in nums)
        {
            if (IsSafe(sudoku, row, col, num))
            {
                sudoku[row, col] = num;
                if (FillRemaining(sudoku, row, col + 1))
                {
                    return true;
                }
                sudoku[row, col] = 0;
            }
        }
        return false;
    }

    // Метод для перемішування елементів масиву.
    static void Shuffle(int[] array, Random rand)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Метод для розв'язання судоку. Повертає вирішене судоку або null, якщо розв'язок відсутній.
    static int[,] GetSolvedSudoku(int[,] sudoku)
    {
        bool isSolved = Solve(sudoku);
        if (isSolved)
        {
            return sudoku;
        }
        else
        {
            return null;
        }
    }

    // Рекурсивний метод для розв'язання судоку. Повертає true, якщо розв'язок знайдено, або false, якщо ні.
    static bool Solve(int[,] sudoku)
    {
        int row, col;
        if (!FindEmptyLocation(sudoku, out row, out col))
        {
            return true;
        }

        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(sudoku, row, col, num))
            {
                sudoku[row, col] = num;
                if (Solve(sudoku))
                {
                    return true;
                }
                sudoku[row, col] = 0;
            }
        }
        return false;
    }

    // Метод визначає, куди можна вставити нове число під час розв'язання судоку, шукаючи порожні клітинки 
    static bool FindEmptyLocation(int[,] sudoku, out int row, out int col)
    {
        row = -1;
        col = -1;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[i, j] == 0)
                {
                    row = i;
                    col = j;
                    return true;
                }
            }
        }
        return false;
    }

    // Метод для перевірки, чи має судоку єдиний розв'язок. 
    // Клонує оригинальний судоку, щоб проводити операції, не змінюючи оригинал.
    static bool HasUniqueSolution(int[,] sudoku)
    {
        int[,] tempSudoku = (int[,])sudoku.Clone();
        return SolveSudoku(tempSudoku, 0, 0);
    }

    // Метод для встановлення рівня складності судоку.
    static void SetDifficultyLevel(int[,] sudoku)
    {
        int difficultyLevel = GetDifficultyLevel();
        int emptyCells = GetEmptyCellsCount(difficultyLevel);
        int totalCells = 9 * 9;
        int cellsToRemove = totalCells - emptyCells;

        RemoveRandomNumbers(sudoku, cellsToRemove);
    }

    // Метод для отримання кількості порожніх клітинок в залежності від складності.
    static int GetEmptyCellsCount(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1:
                return 76;
            case 2:
                return 35;
            case 3:
                return 30;
            case 4:
                return 25;
            default:
                throw new ArgumentException("Неможливо обчислити кількість порожніх клітинок.");
        }
    }

    // Метод для видалення випадкових чисел з судоку для зазначеної кількості порожніх клітинок.
    static void RemoveRandomNumbers(int[,] sudoku, int cellsToRemove)
    {
        Random rand = new Random();
        while (cellsToRemove > 0)
        {
            int row = rand.Next(0, 9);
            int col = rand.Next(0, 9);
            if (sudoku[row, col] != 0)
            {
                int backup = sudoku[row, col];
                sudoku[row, col] = 0;

                // Перевірка на єдиний розв'язок після видалення клітинки.
                if (HasUniqueSolution(sudoku))
                {
                    cellsToRemove--;
                }
                else
                {
                    // Відновлення видаленного значення, якщо розв'язок не 1.
                    sudoku[row, col] = backup;
                }
            }
        }
    }

    // Метод для рекурсивного розв'язку судоку.
    // Метод перевіряє, чи число може бути поміщене в певну клітинку, використовуючи метод IsSafe.
    // Якщо так, то це число поміщається в клітинку, і метод викликає сам себе для наступної клітинки.
    static bool SolveSudoku(int[,] sudoku, int row, int col)
    {
        if (row == 9)
        {
            return true;
        }
        if (col == 9)
        {
            return SolveSudoku(sudoku, row + 1, 0);
        }
        if (sudoku[row, col] != 0)
        {
            return SolveSudoku(sudoku, row, col + 1);
        }
        for (int num = 1; num <= 9; num++)
        {
            if (IsSafe(sudoku, row, col, num))
            {
                sudoku[row, col] = num;
                if (SolveSudoku(sudoku, row, col + 1))
                {
                    return true;
                }
                sudoku[row, col] = 0;
            }
        }
        return false;
    }

    // Метод перевіряє, чи може певне число бути поміщене в певну клітинку, дотримуючись правил гри.
    static bool IsSafe(int[,] sudoku, int row, int col, int num)
    {
        for (int x = 0; x < 9; x++)
        {
            if (sudoku[row, x] == num || sudoku[x, col] == num)
            {
                return false;
            }
        }
        int startRow = row - row % 3;
        int startCol = col - col % 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (sudoku[i + startRow, j + startCol] == num)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Метод для прорішування судоку у консолі.
    static void PlaySudoku(int[,] sudoku)
    {
        int cursorRow = 0;
        int cursorCol = 0;

        while (true)
        {
            DisplaySudoku(sudoku, cursorRow, cursorCol);

            if (IsSudokuSolved(sudoku))
            {
                Console.SetCursorPosition(0, 15);
                Console.WriteLine("Вітаємо! Ви вирішили судоку!");
                System.Threading.Thread.Sleep(4000);
                Console.Clear();
                break;
            }

            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
            else if (char.IsDigit(key.KeyChar))
            {
                int num = int.Parse(key.KeyChar.ToString());
                if (num >= 1 && num <= 9)
                {
                    int row = cursorRow / 3;
                    int col = cursorCol / 3;
                    if (originalSudoku[row, col] == 0)
                    {
                        sudoku[row, col] = num;
                    }
                }
            }
            else
            {
                MoveCursor(key.Key, ref cursorRow, ref cursorCol);
            }
        }
    }

    // Метод для переміщення курсора у консолі за допомогою стрілок.
    static void MoveCursor(ConsoleKey key, ref int cursorTop, ref int cursorLeft)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                cursorLeft = Math.Max(0, cursorLeft - 3);
                break;
            case ConsoleKey.RightArrow:
                cursorLeft = Math.Min(24, cursorLeft + 3);
                break;
            case ConsoleKey.UpArrow:
                cursorTop = Math.Max(0, cursorTop - 3);
                break;
            case ConsoleKey.DownArrow:
                cursorTop = Math.Min(24, cursorTop + 3);
                break;
        }
    }

    // Метод для виведення у консоль рівнів складності.
    static int GetDifficultyLevel()
    {
        Console.WriteLine("Виберіть рівень складності:");
        Console.Write("1. ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("Для демонстрації");
        Console.ResetColor();
        Console.WriteLine(" (76 чисел)");

        Console.Write("2. ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Легкий");
        Console.ResetColor();
        Console.WriteLine(" (35 чисел)");

        Console.Write("3. ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Середній");
        Console.ResetColor();
        Console.WriteLine(" (30 чисел)");

        Console.Write("4. ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Важкий");
        Console.ResetColor();
        Console.WriteLine(" (25 чисел)");

        return GetChoice();
    }

    // Метод для отримання вибору користувача, зчитавши ведений ключ.
    static int GetChoice()
    {
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    return 1; // Для демонстрації
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    return 2; // Легкий
                case ConsoleKey.D3:
                    return 3; // Середній
                case ConsoleKey.D4:
                    return 4; // Важкий
                default:
                    break;
            }
        }
    }

    // Метод для перевірки правильності вводу користувачем чисел у судоку.
    static bool IsSudokuSolved(int[,] sudoku)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!IsRowValid(sudoku, i) || !IsColumnValid(sudoku, i) || !IsBoxValid(sudoku, i))
            {
                return false;
            }
        }
        return true;
    }

    // Метод, який перевіряє вірність кожного рядка судоку. 
    static bool IsRowValid(int[,] sudoku, int row)
    {
        bool[] found = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            int num = sudoku[row, i];
            if (num < 1 || num > 9 || found[num - 1])
            {
                return false;
            }
            found[num - 1] = true;
        }
        return true;
    }

    // Метод, який перевіряє вірність кожного стовпця судоку.
    static bool IsColumnValid(int[,] sudoku, int col)
    {
        bool[] found = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            int num = sudoku[i, col];
            if (num < 1 || num > 9 || found[num - 1])
            {
                return false;
            }
            found[num - 1] = true;
        }
        return true;
    }

    // Метод, який перевіряє вірність кожного квадрата 3x3 у судоку.
    static bool IsBoxValid(int[,] sudoku, int box)
    {
        bool[] found = new bool[9];
        int startRow = (box / 3) * 3;
        int startCol = (box % 3) * 3;
        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                int num = sudoku[i, j];
                if (num < 1 || num > 9 || found[num - 1])
                {
                    return false;
                }
                found[num - 1] = true;
            }
        }
        return true;
    }

    // Метод для відображення судоку у консолі.
    // За допомогою інших методів створює таблицю, додає та змінює колір значенням у судоку, встановлює курсор.
    static void DisplaySudoku(int[,] sudoku, int cursorRow = 0, int cursorCol = 0)
    {
        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine("┌───────┬───────┬───────┐");
        for (int row = 0; row < 9; row++)
        {
            if (row > 0 && row % 3 == 0)
            {
                Console.WriteLine("├───────┼───────┼───────┤");
            }
            PrintRow(sudoku, row, cursorRow, cursorCol);
        }
        Console.WriteLine("└───────┴───────┴───────┘");
        SetCursorPosition(cursorRow, cursorCol);
    }

    // Метод виводить один рядок судоку на консоль.
    static void PrintRow(int[,] sudoku, int row, int cursorRow, int cursorCol)
    {
        for (int col = 0; col < 9; col++)
        {
            if (col == 0)
            {
                Console.Write("│ ");
            }

            bool isCursorCell = row == cursorRow / 3 && col == cursorCol / 3;
            bool isOriginal = sudoku[row, col] == originalSudoku[row, col];
            bool isValidInput = sudoku[row, col] == solvedSudoku[row, col];

            SetCellColor(isCursorCell, isOriginal, isValidInput, sudoku[row, col]);

            if (sudoku[row, col] == 0)
            {
                Console.Write("  ");
            }
            else
            {
                Console.Write($"{sudoku[row, col]} ");
            }

            Console.ResetColor();

            if ((col + 1) % 3 == 0)
            {
                Console.Write("│ ");
            }
        }
        Console.WriteLine();
    }

    // Метод для встановлення кольору числа у клітинці.
    static void SetCellColor(bool isCursorCell, bool isOriginal, bool isValidInput, int number)
    {
        ConsoleColor foregroundColor;

        if (isCursorCell)
        {
            ConsoleColor backgroundColor = ConsoleColor.Gray;

            if (!isValidInput)
            {
                foregroundColor = ConsoleColor.Red;
            }
            else
            {
                foregroundColor = ConsoleColor.Green;
            }

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
        }
        else
        {
            if (!isOriginal && !isValidInput) 
            {
                foregroundColor = ConsoleColor.Red;
            }
            else if (!isOriginal) 
            {
                foregroundColor = ConsoleColor.Green;
            }
            else 
            {
                foregroundColor = ConsoleColor.DarkGray;
            }

            Console.ForegroundColor = foregroundColor;
        }
    }

    // Метод, що встановлює позицію курсора у консолі.
    static void SetCursorPosition(int cursorRow, int cursorCol)
    {
        int row = (cursorRow / 3) * 4 + 1;
        int col = (cursorCol / 3) * 6 + 2 + cursorCol % 3 * 2;

        Console.SetCursorPosition(col, row);
    }

    // Метод для запиту у користувача, чи хоче він зберегти судоку
    static void AskToSaveSudoku(int[,] sudoku)
    {
        Console.SetCursorPosition(0, 15);
        Console.WriteLine("Чи бажаєте зберегти це судоку?");
        Console.WriteLine("1. Так");
        Console.WriteLine("2. Ні");

        int choice = GetChoice();
        Console.Clear();
        if (choice == 1)
        {
            SaveSudoku(originalSudoku, "судоку", "txt");
        }
    }

    // Метод для збереження судоку у форматі txt
    static void SaveSudoku(int[,] sudoku, string fileName, string format)
    {
        fileName = GetUniqueFileName(fileName, format);
        string downloadsFolderPath = GetDownloadsFolderPath();
        string filePath = Path.Combine(downloadsFolderPath, fileName + "." + format);
        string content = FormatSudoku(sudoku);

        bool isValidContent = !string.IsNullOrWhiteSpace(content);
        bool isValidFilePath = !string.IsNullOrWhiteSpace(filePath);

        if (isValidContent && isValidFilePath)
        {
            if (format.ToLower() == "txt")
            {
                File.WriteAllText(filePath, content);
                Console.WriteLine($"Судоку збережено у завантаженнях у форматі .txt. \nГарної гри!");
                System.Threading.Thread.Sleep(5000);
            }
            else
            {
                Console.WriteLine("Невірний формат. Судоку не буде збережено.");
            }
        }
        else
        {
            Console.WriteLine("Не вдалося зберегти судоку. Перевірте введені дані.");
        }
    }

    // Метод для отримання шляху до папки завантажень
    static string GetDownloadsFolderPath()
    {
        string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string downloadsFolderName = "Downloads";
        return Path.Combine(userProfilePath, downloadsFolderName);

    }

    // Метод для перевірки назви файлу, робить кожну назву для файлу унікальною.
    static string GetUniqueFileName(string fileName, string format)
    {
        string downloadsFolderPath = GetDownloadsFolderPath();
        string filePath = Path.Combine(downloadsFolderPath, fileName + "." + format);

        int count = 1;
        while (File.Exists(filePath))
        {
            fileName = $"судоку ({count})";
            filePath = Path.Combine(downloadsFolderPath, fileName + "." + format);
            count++;
        }

        return fileName;
    }

    // Метод для форматування судоку в файлі формату txt.
    static string FormatSudoku(int[,] sudoku)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐");
        sb.Append("│");

        for (int row = 0; row < 9; row++)
        {
            if (row > 0)
            {
                if (row != 3 && row != 6)
                {
                    sb.AppendLine("├───┼───┼───┤├───┼───┼───┤├───┼───┼───┤");
                }

                sb.Append("│");
            }

            for (int col = 0; col < 9; col++)
            {
                if (col == 3 || col == 6)
                {
                    sb.Append("│");
                }
                if (col > 0)
                {
                    sb.Append("│");
                }
                int num = sudoku[row, col];
                if (num == 0)
                {
                    sb.Append("   ");
                }
                else
                {
                    if (num == 0)
                    {
                        sb.Append("   ");
                    }
                    else
                    {
                        string numAsString = num.ToString();
                        sb.Append($" {numAsString} ");
                    }
                }
            }
            sb.AppendLine("│");
            if (row == 2 || row == 5)
            {
                sb.AppendLine("└───┴───┴───┘└───┴───┴───┘└───┴───┴───┘");
                sb.AppendLine("┌───┬───┬───┐┌───┬───┬───┐┌───┬───┬───┐");
            }
        }
        sb.AppendLine("└───┴───┴───┘└───┴───┴───┘└───┴───┴───┘");
        return sb.ToString();
    }
}
