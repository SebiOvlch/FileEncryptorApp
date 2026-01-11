using FileEncryptor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptor.Core.Interfaces
{
    public interface IHashService
    {
        Task<string> ComputeFileHashAsync(string filePath, HashType hashType, IProgress<double>? progress = null);
    }
}
