using Godot;
using System;

public partial class MainMenuController : Node
{
    public override void _Ready()
    {
        var playButton = GetNode<Button>("Button_Play");
        playButton.Pressed += OnPlayPressed;
    }

    public void OnPlayPressed()
    {
        AppManager.Instance.ChangeState(AppState.Loading);
    }
}
