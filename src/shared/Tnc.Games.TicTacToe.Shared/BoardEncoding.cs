using System;
using System.Linq;

namespace Tnc.Games.TicTacToe.Shared;

public static class BoardEncoding
{
    // Simple mapping: keypad 1..9 -> index 0..8 (1->0, 2->1, ... 9->8)
    public static int KeypadToIndex(int keypad)
    {
        if (keypad < 1 || keypad > 9) throw new ArgumentOutOfRangeException(nameof(keypad));
        return keypad - 1;
    }

    public static int IndexToKeypad(int index)
    {
        if (index < 0 || index > 8) throw new ArgumentOutOfRangeException(nameof(index));
        return index + 1;
    }

    // Encode a board represented as string[] ("X","O","E") to 9-char string
    public static string ToStateKey(string[] board)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));
        return string.Concat(board.Select(s => (s ?? "E")[0]));
    }

    // Decode 9-char key into string[] of "X","O","E"
    public static string[] FromStateKey(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != 9) throw new ArgumentException("Key must be 9 characters long", nameof(key));
        return key.Select(c => c == 'X' ? "X" : c == 'O' ? "O" : "E").ToArray();
    }

    // Convert string[] board to CellValue[]
    public static CellValue[] ToCellValues(string[] board)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));
        return board.Select(s => s == "X" ? CellValue.X : s == "O" ? CellValue.O : CellValue.E).ToArray();
    }

    // Convert CellValue[] to string[]
    public static string[] ToBoardStrings(CellValue[] board)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));
        return board.Select(c => c == CellValue.X ? "X" : c == CellValue.O ? "O" : "E").ToArray();
    }

    // Get indices of empty cells
    public static int[] GetLegalMoves(string[] board)
    {
        if (board == null) throw new ArgumentNullException(nameof(board));
        if (board.Length != 9) throw new ArgumentException("Board must be length 9", nameof(board));
        return board.Select((v, idx) => new { v, idx }).Where(x => x.v == "E").Select(x => x.idx).ToArray();
    }
}
