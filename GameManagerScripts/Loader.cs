using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{
    public enum Scene
    {
        MainMenu,
        LoadingScene,
        GameScene
    }

    private static Scene targetSceneIndex;

    public static void Load(Scene targetSceneName)
    {
        targetSceneIndex = targetSceneName;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());


        
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetSceneIndex.ToString());
    }
}
