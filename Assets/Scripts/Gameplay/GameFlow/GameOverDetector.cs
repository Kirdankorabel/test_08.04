using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes;

namespace Game2048.Gameplay.GameFlow
{
    public class GameOverDetector
    {
        private readonly ICubeController _cubeController;
        private readonly BoardService _boardService;

        public GameOverDetector(ICubeController cubeController, BoardService boardService)
        {
            _cubeController = cubeController;
            _boardService = boardService;
        }

        public bool CheckGameOver()
        {
            var cubes = _cubeController.ActiveCubes;
            for (int i = 0; i < cubes.Count; i++)
            {
                var cube = cubes[i];
                if (cube == null || !cube.Data.IsLaunched)
                    continue;

                if (_boardService.IsOverflowing(cube.transform.position))
                    return true;
            }
            return false;
        }
    }
}
