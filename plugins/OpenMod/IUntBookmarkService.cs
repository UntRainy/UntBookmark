using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace UntBookmark.OpenMod
{
    [Service]
    public interface IUntBookmarkService
    {
        /// <summary>
        /// Updates the bookmark IP address asynchronously.
        /// This method retrieves the fake IP address from the Steam Game Server Networking Sockets API
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<string> UpdateBookmarkIPAsync();
    }
}
