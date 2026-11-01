using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<byte[]> ExtractEmbeddingAsync(byte[] image);
    }
}
