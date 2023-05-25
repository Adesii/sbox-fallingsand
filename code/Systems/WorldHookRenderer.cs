using Sand.Systems.FallingSand;

namespace Sand.Systems;

[SceneCamera.AutomaticRenderHook]
public class WorldHookRenderer : RenderHook
{
	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		if ( renderStage == Stage.BeforePostProcess )
		{
			Graphics.Clear( Color.Gray, true, true, true );
			return;
		}

	}
}

