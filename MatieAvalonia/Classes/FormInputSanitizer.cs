using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Controls;

namespace MatieAvalonia.Classes;

/// <summary>Ограничение ввода по типу поля (длина, допустимые символы).</summary>
public static class FormInputSanitizer
{
    public static void Wire(TextBox textBox, Func<string?, string> sanitize)
    {
        var gate = false;
        textBox.TextChanged += (_, _) =>
        {
            if (gate)
                return;
            var cur = textBox.Text ?? "";
            var next = sanitize(cur);
            if (next == cur)
                return;
            gate = true;
            var caret = textBox.CaretIndex;
            textBox.Text = next;
            textBox.CaretIndex = Math.Clamp(caret, 0, next.Length);
            gate = false;
        };
    }

    /// <summary>ФИО и подобное: буквы (в т.ч. кириллица), пробел, дефис, апостроф, точка (инициалы).</summary>
    public static string PersonName(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (char.IsLetter(ch) || ch is ' ' or '-' or '\'' or '.')
                sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>Логин: буквы, цифры, «_» и «.».</summary>
    public static string Login(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (char.IsLetterOrDigit(ch) || ch is '_' or '.')
                sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>Пароль: без управляющих символов (кроме обычного пробела не используем).</summary>
    public static string Password(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (!char.IsControl(ch))
                sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>Цена <c>numeric(7,2)</c>: цифры и один разделитель; запятая или точка.</summary>
    public static string PriceMoney72(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(12);
        var hasSep = false;
        foreach (var ch in raw)
        {
            if (char.IsDigit(ch))
            {
                if (sb.Length >= 12)
                    break;
                sb.Append(ch);
                continue;
            }

            if ((ch == ',' || ch == '.') && !hasSep && sb.Length > 0)
            {
                sb.Append(',');
                hasSep = true;
            }
        }

        return sb.ToString();
    }

    /// <summary>Проверка числа цены после ввода (запятая как в UI).</summary>
    public static bool TryParsePriceMoney72(string? text, out decimal value)
    {
        value = default;
        var t = (text ?? "").Trim().Replace(',', '.');
        if (string.IsNullOrEmpty(t))
            return false;
        if (!decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            return false;
        return value is >= 0 and <= DbFieldLimits.ServicePriceMax;
    }

    public static string CardDigits(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (char.IsDigit(ch))
                sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>Номер карты в UI: до 19 цифр, пробел после каждых 4 (как на карте). В БД сохраняют только цифры.</summary>
    public static string CardNumberSpaced(string? raw)
    {
        var digits = string.Concat((raw ?? "").Where(char.IsDigit));
        if (digits.Length > 19)
            digits = digits[..19];
        if (digits.Length == 0)
            return "";
        var sb = new StringBuilder(digits.Length + (digits.Length - 1) / 4);
        for (var i = 0; i < digits.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
                sb.Append(' ');
            sb.Append(digits[i]);
        }

        return sb.ToString();
    }

    /// <summary>Срок карты ММ/ГГ: только цифры, косая черта после месяца.</summary>
    public static string CardExpiry(string? raw)
    {
        var digits = string.Concat((raw ?? "").Where(char.IsDigit));
        if (digits.Length > 4)
            digits = digits[..4];
        if (digits.Length <= 2)
            return digits;
        return digits[..2] + "/" + digits[2..];
    }

    /// <summary>Сумма пополнения баланса под <c>numeric(20,2)</c>: до 18 цифр до разделителя, 2 после.</summary>
    public static string BalanceMoney202(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(22);
        var hasSep = false;
        var intDigits = 0;
        var fracDigits = 0;
        foreach (var ch in raw)
        {
            if (char.IsDigit(ch))
            {
                if (!hasSep)
                {
                    if (intDigits >= 18)
                        continue;
                    intDigits++;
                    sb.Append(ch);
                }
                else
                {
                    if (fracDigits >= 2)
                        continue;
                    fracDigits++;
                    sb.Append(ch);
                }

                continue;
            }

            if ((ch == ',' || ch == '.') && !hasSep && intDigits > 0)
            {
                hasSep = true;
                sb.Append(',');
            }
        }

        return sb.ToString();
    }

    /// <summary>Пустая строка — 0; иначе сумма в пределах <see cref="DbFieldLimits.BalanceMonetaryMax"/>, не более двух знаков после запятой.</summary>
    public static bool TryParseBalanceTopUp(string? text, out decimal value)
    {
        value = 0;
        var t = (text ?? "").Trim();
        if (string.IsNullOrEmpty(t))
            return true;
        t = t.Replace(',', '.');
        if (!decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            return false;
        if (value < 0)
            return false;
        var scale = (decimal.GetBits(value)[3] >> 16) & 0xFF;
        if (scale > 2)
            return false;
        return value <= DbFieldLimits.BalanceMonetaryMax;
    }

    /// <summary>Время ЧЧ:ММ — цифры и «:» не раньше позиции 2.</summary>
    public static string TimeHm(string? raw)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(5);
        foreach (var ch in raw)
        {
            if (sb.Length >= 5)
                break;
            if (char.IsDigit(ch))
            {
                sb.Append(ch);
                continue;
            }

            if (ch == ':' && sb.Length is >= 1 and < 3)
                sb.Append(':');
        }

        return sb.ToString();
    }

    /// <summary>Короткий текст (название услуги и т.п.): без управляющих символов.</summary>
    public static string PlainLine(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (!char.IsControl(ch))
                sb.Append(ch);
        }

        return sb.ToString();
    }

    /// <summary>Комментарий отзыва: переносы строк, без прочих управляющих символов.</summary>
    public static string ReviewComment(string? raw, int maxLen)
    {
        if (string.IsNullOrEmpty(raw))
            return "";
        var sb = new StringBuilder(Math.Min(raw.Length, maxLen));
        foreach (var ch in raw)
        {
            if (sb.Length >= maxLen)
                break;
            if (ch == '\r')
                continue;
            if (ch == '\n')
            {
                sb.Append('\n');
                continue;
            }

            if (!char.IsControl(ch))
                sb.Append(ch);
        }

        return sb.ToString();
    }
}
