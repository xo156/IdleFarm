using System.Globalization;

namespace IdleFarm.UI {
    public static class NumberFormatter {
        private static readonly string[] Suffixes = { "", "K", "M", "B", "T", "Qa", "Qi" };

        // 큰 숫자를 1.2K, 3.4M처럼 짧게 보여주기 위한 표시용 함수
        public static string Format(double value) {
            var suffixIndex = 0;

            while (value >= 1000.0d && suffixIndex < Suffixes.Length - 1) {
                value /= 1000.0d;
                suffixIndex++;
            }

            if (suffixIndex == 0) {
                return value.ToString("0", CultureInfo.InvariantCulture);
            }

            return value.ToString("0.##", CultureInfo.InvariantCulture) + Suffixes[suffixIndex];
        }
    }
}
