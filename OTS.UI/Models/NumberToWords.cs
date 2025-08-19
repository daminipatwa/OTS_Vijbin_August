using System;

namespace OTS.UI.Models
{
    public class NumberToWords
    {
        private static string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static string[] teens = { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static string[] tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        private static string[] thousands = { "", "Thousand", "Million", "Billion" };

        public static string ConvertToWords(decimal number)
        {
            if (number == 0)
                return units[0];

            string words = "";
            int intPart = (int)number;
            int unit = 0;

            while (intPart > 0)
            {
                int chunk = intPart % 1000;
                if (chunk > 0)
                {
                    string chunkWords = ChunkToWords(chunk);
                    words = chunkWords + " " + thousands[unit] + " " + words;
                }

                intPart /= 1000;
                unit++;
            }

            if (number % 1 > 0)
            {
                int decimalPart = (int)((number % 1) * 100); // handle up to 2 decimal places
                words = words + "and " + ChunkToWords(decimalPart) + " Paise";
            }

            return words.Trim();
        }

        private static string ChunkToWords(int chunk)
        {
            string words = "";

            if (chunk >= 100)
            {
                int hundreds = chunk / 100;
                words += units[hundreds] + " Hundred ";
                chunk %= 100;
            }

            if (chunk >= 20)
            {
                int ten = chunk / 10;
                words += tens[ten] + " ";
                chunk %= 10;
            }

            if (chunk >= 10)
            {
                words += teens[chunk - 10] + " ";
            }
            else if (chunk > 0)
            {
                words += units[chunk] + " ";
            }

            return words.Trim();
        }

    }


}
