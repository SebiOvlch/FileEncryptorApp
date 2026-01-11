using FileEncryptor.Core.Enums;
using FileEncryptor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Core.Services
{
    public class HashService : IHashService
    {
        public async Task<string> ComputeFileHashAsync(string filePath, HashType hashType, IProgress<double>? progress = null)
        {
            // Choosing the algoritmh
            using HashAlgorithm algorithm = hashType switch
            {
                HashType.MD5 => MD5.Create(),
                HashType.SHA1 => SHA1.Create(),
                HashType.SHA256 => SHA256.Create(),
                HashType.SHA384 => SHA384.Create(),
                HashType.SHA512 => SHA512.Create(),
                HashType.SHA3_256 => SHA3_256.Create(),
                HashType.SHA3_384 => SHA3_384.Create(),
                HashType.SHA3_512 => SHA3_512.Create(),
                _ => throw new ArgumentException("Doesn't supported Hash Type.")
            };

            int bufferSize = 1024 * 1024;
            byte[] buffer = new byte[bufferSize];

            using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true);

            long totalBytes = stream.Length;
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                algorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);

                totalRead += bytesRead;

                if (progress != null)
                {
                    double percent = (double)totalRead / totalBytes * 100;
                    progress.Report(percent);
                }
            }

            // Lezárjuk a hash számítást
            algorithm.TransformFinalBlock(buffer, 0, 0);

            if(algorithm.Hash != null)
            {
                return Convert.ToHexString(algorithm.Hash);
            }
            else
            {
                throw new Exception(message: $"Something went wrong. Please try again!");
            }
        }
    }
}
