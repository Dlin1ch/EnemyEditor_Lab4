using System;
using System.Collections.Generic;
using System.Text;

namespace Clicker
{
    public class BigNumber
    {
        // Основание системы счисления — 1000
        private const int Base = 1000;

        // Массив блоков (разрядов). Младший разряд — в конце массива
        private int[] number;

        // Убирает ведущие нули из массива
        private int[] TrimLeadingZeros(int[] arr)
        {
            // Находим индекс последнего ненулевого блока
            int lastNonZero = arr.Length - 1;
            while (lastNonZero > 0 && arr[lastNonZero] == 0)
            {
                lastNonZero--;
            }

            // Если все нули — возвращаем [0]
            if (lastNonZero == 0 && arr[0] == 0)
            {
                return new int[] { 0 };
            }

            // Создаём новый массив без лишних нулей
            int[] result = new int[lastNonZero + 1];
            Array.Copy(arr, result, lastNonZero + 1);

            return result;
        }
        // Сравнение двух чисел: -1 (меньше), 0 (равно), 1 (больше)
        public int CompareTo(BigNumber other)
        {
            // Сравниваем по длине (без ведущих нулей)
            if (this.number.Length > other.number.Length)
                return 1;
            if (this.number.Length < other.number.Length)
                return -1;

            // Если длины равны — сравниваем блоки слева направо
            for (int i = this.number.Length - 1; i >= 0; i--)
            {
                if (this.number[i] > other.number[i])
                    return 1;
                if (this.number[i] < other.number[i])
                    return -1;
            }

            return 0;
        }

        // Конструктор — принимает строку с числом
        public BigNumber(string numberStr)
        {
            // Убираем возможные пробелы
            numberStr = numberStr.Trim();

            // Вычисляем количество блоков
            int blockCount = (numberStr.Length + 2) / 3;

            // Создаём массив блоков
            number = new int[blockCount];

            // Заполняем массив справа налево
            int strIndex = numberStr.Length;
            int blockIndex = 0;

            while (strIndex > 0)
            {
                // Берём до 3 символов справа
                int take = Math.Min(3, strIndex);
                string blockStr = numberStr.Substring(strIndex - take, take);

                // Преобразуем в число и кладём в массив
                number[blockIndex] = int.Parse(blockStr);

                strIndex -= take;
                blockIndex++;
            }
        }

        // Конструктор — принимает массив блоков
        public BigNumber(int[] blocks)
        {
            number = blocks;
        }
        // Приватный метод сложения (немутирующий — возвращает новый объект)
        private BigNumber Add(BigNumber other)
        {
            // Определяем, какой массив длиннее
            int maxLength = Math.Max(this.number.Length, other.number.Length);

            // Результат будет на 1 блок длиннее (на случай переноса)
            int[] result = new int[maxLength + 1];

            int carry = 0;

            for (int i = 0; i < maxLength; i++)
            {
                // Берём блоки из обоих чисел (если индекс выходит за границы — 0)
                int a = (i < this.number.Length) ? this.number[i] : 0;
                int b = (i < other.number.Length) ? other.number[i] : 0;

                // Складываем блоки и перенос
                int sum = a + b + carry;

                // Записываем остаток от деления на Base
                result[i] = sum % Base;

                // Вычисляем перенос
                carry = sum / Base;
            }

            // Если после последнего шага остался перенос — записываем его
            if (carry > 0)
            {
                result[maxLength] = carry;
            }

            // Убираем лишние ведущие нули
            result = TrimLeadingZeros(result);

            return new BigNumber(result);
        }
        // Приватный метод вычитания (немутирующий)
        private BigNumber Subtract(BigNumber other)
        {
            // Предполагаем, что this >= other
            int maxLength = this.number.Length;

            int[] result = new int[maxLength];

            int borrow = 0;

            for (int i = 0; i < maxLength; i++)
            {
                // Берём блоки (если индекс выходит за границы — 0)
                int a = this.number[i];
                int b = (i < other.number.Length) ? other.number[i] : 0;

                // Вычитаем с учётом заёма
                int diff = a - b - borrow;

                if (diff < 0)
                {
                    // Занимаем у старшего блока
                    diff += Base;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result[i] = diff;
            }

            // Убираем лишние ведущие нули
            result = TrimLeadingZeros(result);

            return new BigNumber(result);
        }
        // Приватный метод умножения на double (немутирующий)
        private BigNumber Multiply(double multiplier)
        {
            // Результат будет такой же длины + 1 блок на случай переноса
            int[] result = new int[this.number.Length + 1];

            double carry = 0;

            for (int i = 0; i < this.number.Length; i++)
            {
                // Умножаем блок на множитель и добавляем перенос
                double product = this.number[i] * multiplier + carry;

                // Записываем остаток от деления на Base
                result[i] = (int)(product % Base);

                // Вычисляем перенос
                carry = Math.Floor(product / Base);
            }

            // Если остался перенос — записываем его
            int extraIndex = this.number.Length;
            while (carry > 0)
            {
                if (extraIndex >= result.Length)
                {
                    // Расширяем массив, если нужно
                    Array.Resize(ref result, result.Length + 1);
                }
                result[extraIndex] = (int)(carry % Base);
                carry = Math.Floor(carry / Base);
                extraIndex++;
            }

            // Убираем лишние ведущие нули
            result = TrimLeadingZeros(result);

            return new BigNumber(result);
        }
        // Приватный метод деления на double (немутирующий)
        private BigNumber Divide(double divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException("Деление на ноль");

            int[] result = new int[this.number.Length];

            double remainder = 0;

            // Идём слева направо (от старшего блока к младшему)
            for (int i = this.number.Length - 1; i >= 0; i--)
            {
                // Сносим остаток, умноженный на базу, и добавляем текущий блок
                double current = remainder * Base + this.number[i];

                // Делим на divisor
                result[i] = (int)(current / divisor);

                // Новый остаток
                remainder = current % divisor;
            }

            // Убираем лишние ведущие нули
            result = TrimLeadingZeros(result);

            return new BigNumber(result);
        }

        // Переопределяем ToString()
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = number.Length - 1; i >= 0; i--)
            {
                if (i == number.Length - 1)
                    sb.Append(number[i].ToString());
                else
                    sb.Append(number[i].ToString("D3"));
            }
            return sb.ToString();
        }
        // Переопределение оператора +
        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            return a.Add(b);
        }
        // Переопределение оператора -
        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            // Если a < b — результат будет отрицательным, но мы это не поддерживаем
            if (a.CompareTo(b) < 0)
            {
                throw new InvalidOperationException("Результат вычитания отрицательный");
            }

            return a.Subtract(b);
        }

        // Переопределение оператора >
        public static bool operator >(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) > 0;
        }

        // Переопределение оператора <
        public static bool operator <(BigNumber a, BigNumber b)
        {
            return a.CompareTo(b) < 0;
        }
        // Переопределение оператора * (BigNumber * double)
        public static BigNumber operator *(BigNumber a, double b)
        {
            return a.Multiply(b);
        }

        // Переопределение оператора * (double * BigNumber)
        public static BigNumber operator *(double a, BigNumber b)
        {
            return b.Multiply(a);
        }
        // Переопределение оператора / (BigNumber / double)
        public static BigNumber operator /(BigNumber a, double b)
        {
            return a.Divide(b);
        }
        // Создаёт копию числа
        public BigNumber Clone()
        {
            int[] copy = new int[this.number.Length];
            Array.Copy(this.number, copy, this.number.Length);
            return new BigNumber(copy);
        }
    }
}
