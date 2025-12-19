using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileEncryptor.Core.Interfaces;
using FileEncryptor.Core.Models;

namespace FileEncryptor.Core
{
    public class FileCryptoService : ICryptoService
    {
        // The 16 byte salt for the password
        private const int saltSize = 16;

        // at a time we only read 64 kB into the memory
        private const int bufferSize = 64 * 1024;

        public async Task ProcessFileAsync(CryptoOptions options, IProgress<double> progress, CancellationToken cancellationToken)
        {
            using(SymmetricAlgorithm algorithm = GetAlgorithm(options.Algorithm))
            {
                algorithm.Mode = options.Mode;
                algorithm.Padding = options.Padding;

                using (FileStream sourceStream = new FileStream(options.InputFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream destStream = new FileStream(options.OutputFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(options.Password);
                    ICryptoTransform? transform = null;

                    // Encryption:
                    if(options.Action == CryptoAction.Encrypt)
                    {
                        // generate the Salt:
                        byte[] salt = GenerateRandomBytes(saltSize);

                        // generate key and IV:
                        using(var keyDerivation = new Rfc2898DeriveBytes(passwordBytes, salt, 10000, HashAlgorithmName.SHA256))
                        {
                            algorithm.Key = keyDerivation.GetBytes(algorithm.KeySize / 8);
                            algorithm.IV = keyDerivation.GetBytes(algorithm.BlockSize / 8);
                        }

                        // Write the salt and the IV on the first lines of the encrypted file!
                        await destStream.WriteAsync(salt, 0, salt.Length);
                        await destStream.WriteAsync(algorithm.IV, 0, algorithm.IV.Length);

                        transform = algorithm.CreateEncryptor();
                    }
                    // Decryption:
                    else
                    {
                        // Read the salt from the file:
                        byte[] salt = new byte[saltSize];
                        int saltRead = await sourceStream.ReadAsync(salt, 0, saltSize);
                        if (saltRead < saltSize) throw new Exception("Error: The file is too short (Damaged header)");

                        // Read the IV:
                        byte[] iv = new byte[algorithm.BlockSize / 8];
                        int ivRead = await sourceStream.ReadAsync(iv, 0, iv.Length);
                        if (ivRead < iv.Length) throw new Exception("Error: The file is too short (Missing IV)");

                        // Generating the key from the salt and the password:
                        using (var keyDerivation = new Rfc2898DeriveBytes(passwordBytes, salt, 10000, HashAlgorithmName.SHA256))
                        {
                            algorithm.Key = keyDerivation.GetBytes(algorithm.KeySize / 8);
                            algorithm.IV = iv;
                        }

                        transform = algorithm.CreateDecryptor();
                    }
                    await RunCryptoLoopAsync(sourceStream, destStream, transform, progress, cancellationToken);
                }
            }
        }

        private async Task RunCryptoLoopAsync(
        Stream source,
        Stream destination,
        ICryptoTransform transform,
        IProgress<double> progress,
        CancellationToken token)
        {
            using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
            {
                byte[] buffer = new byte[bufferSize]; // 64 kB buffer
                long totalBytesRead = 0;
                long fileLength = source.Length;
                int bytesRead;

                while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                {
                    // 1. if the Cancel was pushed
                    token.ThrowIfCancellationRequested();

                    // 2. Encryption
                    await cryptoStream.WriteAsync(buffer, 0, bytesRead, token);

                    // 3. Progress counting
                    totalBytesRead += bytesRead;
                    if (fileLength > 0 && progress != null)
                    {
                        // get the percentage of the progress
                        double percentage = (double)totalBytesRead / fileLength * 100;
                        progress.Report(percentage);
                    }
                }

                // Clearing the buffers
                if (!cryptoStream.HasFlushedFinalBlock)
                    cryptoStream.FlushFinalBlock();
            }
        }

        #region Private Methods
        // Algorithm selector:
        private SymmetricAlgorithm GetAlgorithm(SupportedAlgorithm algoType)
        {
            switch (algoType)
            {
                case SupportedAlgorithm.Aes: return Aes.Create();
                case SupportedAlgorithm.Des: return DES.Create();
                case SupportedAlgorithm.TripleDes: return TripleDES.Create();
                case SupportedAlgorithm.Rijndael: return Rijndael.Create();
                case SupportedAlgorithm.Rc2: return RC2.Create();
                default: throw new NotSupportedException("The selected algorithm is not supported.");
            }
        }

        // safe random number generator for the "salt"
        private byte[] GenerateRandomBytes(int length)
        {
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        #endregion
    }
}
