using System;
using System.IO;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace SisMaper.Tools
{
    public static class Encrypt
    {
        private static RSACryptoServiceProvider _RSA = new(1024);

        static Encrypt()
        {
            try
            {
                var cspParams = new CspParameters()
                {
                    KeyContainerName = "SisMaper"
                };
                _RSA = new RSACryptoServiceProvider(cspParams);
            }
            catch
            {
                // ignored
            }
        }

        public static string ToSha512(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public static string RSAEncryption(string input)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(input);
                byte[] encryptedData = _RSA.Encrypt(data, false);
                return Convert.ToBase64String(encryptedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static string RSADecryption(string input)
        {
            try
            {
                var data = Convert.FromBase64String(input);
                byte[] decryptedData = _RSA.Decrypt(data, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }


        public static void ImportKey()
        {
            var fileChoser = new OpenFileDialog
            {
                DefaultExt = ".pem",
                Filter = "PEM Files (*.pem)|*.pem",
                Title = "Selecione o arquivo PEM disponibilizado",
                Multiselect = false
            };

            if (!fileChoser.ShowDialog().IsTrue()) return;

            var file = File.OpenText(fileChoser.FileName);
            var pem = file.ReadToEnd();
            try
            {
                SystemSounds.Exclamation.Play();
                _RSA = RSAKeys.ImportPrivateKey(pem);
                MessageBox.Show("A chave PEM foi importada com sucesso! A aplicação será encerrada!", "Importar PEM");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location.Replace(".dll", ".exe"));
                Application.Current.Shutdown();
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Não foi possível importar a chave PEM!", "Erro ao Importar PEM");
            }
            catch
            {
                Application.Current.Shutdown();
            }
        }
    }
}