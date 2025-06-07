Console Sudoku: C# Generator & Solver
A C# console application for generating and playing Sudoku puzzles. This project demonstrates strong algorithmic skills and the ability to create a polished user experience in a terminal environment.

Key Features
Smart Generator: Uses a backtracking algorithm to generate puzzles, each mathematically guaranteed to have exactly one unique solution.
Interactive Console UI: Clean and responsive interface with arrow-key navigation and real-time, color-coded validation for correct (green) and incorrect (red) moves.
Variable Difficulty: Four difficulty levels (from Easy to Hard) that alter the number of starting clues.
File Saving: Option to save any puzzle to a .txt file. The app automatically handles unique naming to prevent overwriting files.

Tech Stack
Language: C# (.NET)
Core Algorithm: Recursive Backtracking
Environment: Console Application
Usage

Run the project:
# .NET SDK is required
dotnet run

Controls:
Arrows: Move the cursor.
1-9: Input a number.
Esc: Exit and generate a new puzzle.
