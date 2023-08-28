using ChatRoomServer.Models;

namespace ChatRoomServer.Repository
{
    public interface IJWTManagerRepository
    {
        Tokens GenerateToken(User user);
    }
}
