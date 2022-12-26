using MonoDI.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.UI
{
    public class RestartView : InjectedMono
    {
        [SerializeField] private GameObject _root;

        public void OnRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        [Sub]
        private void OnLose(LoseSignal signal)
        {
            _root.SetActive(true);
        }
        
    }

    public struct LoseSignal : ISignal
    {
        
    }
}
