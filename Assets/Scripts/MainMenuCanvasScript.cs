using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRpg
{
    public class MainMenuCanvasScript : MonoBehaviour
    {
        public void ComeInAsServer()
        {
            GameAdmin.GAME_MODE = GameAdmin.GameMode.Server;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        public void ComeInAsClient()
        {
            GameAdmin.GAME_MODE = GameAdmin.GameMode.Client;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}