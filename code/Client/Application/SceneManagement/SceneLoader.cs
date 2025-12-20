using Godot;

public static class SceneLoader
{
    public static void Load(string scenePath)
    {
        var tree = Engine.GetMainLoop() as SceneTree;

        if(tree == null)
        {
            GD.PrintErr("SceneLoader: SceneTree not available.");
            return;
        }

        tree.ChangeSceneToFile(scenePath);
    }
}