using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace RSAtext {
    class Program {
        // расширенный алгоритм Евклида ax + by = GCD(a, b)
        static BigInteger euklid(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y) {
            if (a == 0) {
                x = 0;
                y = 1;

                return b;
            }

            BigInteger x1, y1;
            BigInteger d = euklid(b % a, a, out x1, out y1);
            x = y1 - (b / a) * x1;
            y = x1;

            return d;
        }

        // проверка числа на простоту
        static bool isPrime(BigInteger n) {
            BigInteger div = 2;

            while (div < n) {
                if (n % div == 0)
                    return false;

                div++;
            }

            return true;
        }

        // возведение числа a в степень n по модулю mod
        static BigInteger powMod(BigInteger a, BigInteger n, BigInteger mod) {
            if (n == 0)
                return 1;

            if (n % 2 == 1)
                return (a * powMod(a, n - 1, mod)) % mod;

            BigInteger half = powMod(a, n / 2, mod);

            return (half * half) % mod;
        }

        // вычисление секретной экспоненты (d) c помощью p, q и открытой экспоненты
        static BigInteger secretExponent(BigInteger p, BigInteger q, BigInteger e) {
            BigInteger phi = (p - 1) * (q - 1); // euler function(n) = (p - 1)(q - 1)

            BigInteger d, y;
            euklid(e, phi, out d, out y);

            if (d < 0)
                d += phi;

            return d;
        }

        static BigInteger encrypt(BigInteger m, BigInteger e, BigInteger n) {
            return powMod(m, e, n);
        }

        static BigInteger decrypt(BigInteger c, BigInteger d, BigInteger n) {
            return powMod(c, d, n);
        }

        static BigInteger getPrime(string message) {
            Console.Write(message);
            BigInteger n;

            while(!BigInteger.TryParse(Console.ReadLine(), out n) || !isPrime(n))
                Console.Write("Число {0} не является простым. Повторите попытку: ");

            return n;
        }

        static void Main(string[] args) {
            Console.Write("Что вы хотите сделать? (encrypt - зашифровать, decrypt - дешифровать): ");
            string option = Console.ReadLine();

            while (option != "decrypt" && option != "encrypt") {
                Console.Write("неверно введена опция. Повторите попытку: ");
                option = Console.ReadLine();
            }

            if (option == "encrypt") {
                BigInteger p = getPrime("Введите p: ");
                BigInteger q = getPrime("Введите q: ");
                BigInteger e = getPrime("Введите e: ");

                BigInteger n = p * q;
                BigInteger d = secretExponent(p, q, e);

                Console.Write("Введите текст для шифрования: ");
                string text = Console.ReadLine();

                byte[] bytes = Encoding.ASCII.GetBytes(text);
                BigInteger[] encrypted = new BigInteger[bytes.Length];

                Console.WriteLine("n = {0}, закрытая экспонента: {1}", n, d);
                Console.Write("Результат шифрования: ");

                for (int i = 0; i < bytes.Length; i++) {
                    encrypted[i] = encrypt(bytes[i], e, n);
                    //Console.Write("{0} -> {1}\n", bytes[i], encrypted[i]);
                    Console.Write("{0} ", encrypted[i]);
                }

                Console.WriteLine();
            }
            else {
                Console.Write("Введите n: ");
                BigInteger n = BigInteger.Parse(Console.ReadLine());
                Console.Write("Введите d: ");
                BigInteger d = BigInteger.Parse(Console.ReadLine());

                Console.Write("Введите числа для дешифрования: ");
                string text = Console.ReadLine();
                string[] bytes = text.Split(' ');
                byte[] decrypted = new byte[bytes.Length];

                Console.Write("Результат дешифрования: ");
                for (int i = 0; i < bytes.Length; i++) {
                    decrypted[i] = (byte)decrypt(BigInteger.Parse(bytes[i]), d, n);
                    //Console.Write("{0} -> {1}\n", bytes[i], decrypted[i]);
                }
                
                Console.WriteLine(Encoding.ASCII.GetString(decrypted));
            }

            Console.ReadKey();
        }
    }
}
