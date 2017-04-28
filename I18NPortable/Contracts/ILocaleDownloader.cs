using System.IO;
using System.Threading.Tasks;

namespace I18NPortable.Contracts
{
    public interface ILocaleDownloader
    {
        Task<Stream> Load(string fileName);
    }
}