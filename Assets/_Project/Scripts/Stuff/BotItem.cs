using UnityEngine;

public class BotItem : Stuff
{
    protected override void FellOffPlatform()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}