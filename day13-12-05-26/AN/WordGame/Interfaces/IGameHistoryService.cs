namespace WordGame.Interfaces
{
    public interface IGameHistoryService
    {
        void ListAllGames(int userId);
        void ViewGameHistoryById();
    }
}
