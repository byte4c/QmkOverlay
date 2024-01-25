using System.Text.RegularExpressions;

namespace QmkOverlay.TestConsole.Utils;

public static class KeyboardPrinter
{
    private static readonly ConsoleColor _defaultColor = Console.BackgroundColor;
    private static readonly ConsoleColor _highlightColor = ConsoleColor.Green;

    private static string? _previousState;

    internal static void Print(QmkKeyboard keyboard, ulong? keyboardId, bool isUnlocked, byte[] matrixState)
    {
        var state = Convert.ToBase64String(matrixState);
        if (_previousState == state) return;

        _previousState = state;

        Console.Clear();
        Console.WriteLine($"{keyboard} | KeyboardId: {keyboardId:X}");
        if (matrixState == null)
        {
            Console.WriteLine("No keyboard state retrieved!");
            return;
        }
        if (!isUnlocked)
        {
            Console.WriteLine("Keyboard is locked. Please unlock.");
            return;
        }

        Console.WriteLine($"{string.Join(" ", matrixState.Select(b => b.ToString("X2")))}");

        var layer = 0;
        if ((matrixState[4] & (1 << 5)) != 0) layer += 1;
        if ((matrixState[9] & (1 << 5)) != 0) layer += 2;

        switch (layer)
        {
            case 0:
                PrintLayer("Qwerty", matrixState, new[]
                {
                    "ESC", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "`",
                    "Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "-",
                    "LCtrl", "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'", "[", "]",
                    "LSft", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "RSft",
                    "LGui", "LAlt", "Enter", "Sym", "Nav", "Space", "LAlt", "Del"
                });
                break;
            case 1:
                PrintLayer("Symbols", matrixState, new[]
                {
                    "", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11",
                    "", "", "", "(", ")", "+", "", "7", "8", "9", "", "F12",
                    "LCtrl", "", "", "{", "}", "=", "", "4", "5", "6", "", "", "&", "",
                    "LSft", "", "", "<", ">", "|", "0", "1", "2", "3", "\\", "",
                    "LGui", "LAlt", "Enter", "Sym", "Nav", "Space", "LAlt", "Del"
                });
                break;
            case 2:
                PrintLayer("Navigation", matrixState, new[]
                {
                    "", "", "", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "Vol+", "Home", "Up", "End", "PgU", "",
                    "LCtrl", "", "", "", "", "", "Vol-", "Left", "Down", "Right", "PgD", "", "", "Stop",
                    "LSft", "", "", "", "", "", "Play", "Ins", "Del", "", "", "",
                    "LGui", "LAlt", "Enter", "Sym", "Nav", "Space", "LAlt", "Del"
                });
                break;
            default:
                PrintLayer("Unknown", matrixState, new[]
                {
                    "ESC", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "`",
                    "Tab", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "-",
                    "LCtrl", "A", "S", "D", "F", "G", "H", "J", "K", "L", ";", "'", "[", "]",
                    "LSft", "Z", "X", "C", "V", "B", "N", "M", ",", ".", "/", "RSft",
                    "LGui", "LAlt", "Enter", "Sym", "Nav", "Space", "LAlt", "Del"
                });
                break;
        }
    }
    private static void PrintLayer(string layerName, byte[] matrixState, string[] keyLabels)
    {
        var template = "/* " + layerName + "\r\n" +
            " * ,-----------------------------------------.                    ,-----------------------------------------.\r\n" +
            " * |{_0_0}|{_0_1}|{_0_2}|{_0_3}|{_0_4}|{_0_5}|                    |{_5_5}|{_5_4}|{_5_3}|{_5_2}|{_5_1}|{_5_0}|\r\n" +
            " * |------+------+------+------+------+------|                    |------+------+------+------+------+------|\r\n" +
            " * |{_1_0}|{_1_1}|{_1_2}|{_1_3}|{_1_4}|{_1_5}|                    |{_6_5}|{_6_4}|{_6_3}|{_6_2}|{_6_1}|{_6_0}|\r\n" +
            " * |------+------+------+------+------+------|                    |------+------+------+------+------+------|\r\n" +
            " * |{_2_0}|{_2_1}|{_2_2}|{_2_3}|{_2_4}|{_2_5}|-------.    ,-------|{_7_5}|{_7_4}|{_7_3}|{_7_2}|{_7_1}|{_7_0}|\r\n" +
            " * |------+------+------+------+------+------| {_4_1}|    | {_9_1}|------+------+------+------+------+------|\r\n" +
            " * |{_3_0}|{_3_1}|{_3_2}|{_3_3}|{_3_4}|{_3_5}|-------|    |-------|{_8_5}|{_8_4}|{_8_3}|{_8_2}|{_8_1}|{_8_0}|\r\n" +
            " * `-----------------------------------------/       /     \\      \\-----------------------------------------'\r\n" +
            " *                   |{_4_2}|{_4_3}|{_4_4}| /{_4_5} /       \\{_9_5}\\  |{_9_4}|{_9_3}|{_9_2}|\r\n" +
            " *                   |      |      |      |/       /         \\      \\ |      |      |      |\r\n" +
            " *                   `----------------------------'           '------''--------------------'\r\n" +
            " */";

        var keyIndex = 0;
        var lastMatchIndex = 0;
        foreach (Match tagMatch in Regex.Matches(template, "\\{_(?<byte>\\d)_(?<bit>\\d)\\}"))
        {
            var matchIndex = template.IndexOf(tagMatch.Value);

            var theByte = byte.Parse(tagMatch.Groups["byte"].Value);
            var theBit = byte.Parse(tagMatch.Groups["bit"].Value);
            var highlight = (matrixState[theByte] & (1 << theBit)) != 0;

            Console.BackgroundColor = _defaultColor;
            Console.Write(template[lastMatchIndex..matchIndex]);
            Console.BackgroundColor = highlight ? _highlightColor : _defaultColor;
            Console.Write(keyLabels[keyIndex].CenterString(6));

            keyIndex++;
            lastMatchIndex = matchIndex + tagMatch.Value.Length;
        }
        Console.BackgroundColor = _defaultColor;
        Console.WriteLine(template[lastMatchIndex..]);
    }
}
