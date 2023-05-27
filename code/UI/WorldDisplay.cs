using Sand.Systems.FallingSand;

namespace Sand.UI;

public class WorldDisplay : Panel
{
	public override bool HasContent => true;


	public override void DrawContent( ref RenderState state )
	{
		SandWorld.WorldLimit limit = SandWorld.Limit;
		Rect worldRect = limit.GetRect();
		worldRect *= new Vector2( SandWorld.ChunkWidth, SandWorld.ChunkHeight );
		//worldRect += new Vector2( 0, SandWorld.ChunkHeight * 2 );
		worldRect.Top += 1;

		/* var top = worldRect.Top;
		var bottom = worldRect.Bottom;
		worldRect.Top = -bottom;
		worldRect.Bottom = -top; */
		worldRect.Bottom += 1;

		worldRect.Position += SandWorld.WorldPosition;
		worldRect *= ScaleToScreen / ((float)SandWorld.ZoomLevel / 10f);

		if ( SandWorld.Limit.RightLimit == 0 )
		{
			worldRect.Right = Screen.Width;
		}
		if ( SandWorld.Limit.LeftLimit == 0 )
		{
			worldRect.Left = 0;
		}
		if ( SandWorld.Limit.UpLimit == 0 )
		{
			worldRect.Top = Screen.Height;
		}
		if ( SandWorld.Limit.DownLimit == 0 )
		{
			worldRect.Bottom = 0;
		}

		//worldRect.Left -= SandWorld.Limit.LeftLimit * SandWorld.ChunkWidth * SandWorld.ZoomLevel;
		//worldRect.Right += SandWorld.Limit.RightLimit * SandWorld.ChunkWidth * SandWorld.ZoomLevel;
		//worldRect.Top += SandWorld.Limit.UpLimit * SandWorld.ChunkHeight * SandWorld.ZoomLevel;
		//worldRect.Bottom -= SandWorld.Limit.DownLimit * SandWorld.ChunkHeight * SandWorld.ZoomLevel;

		var attribs = new RenderAttributes();
		attribs.Set( "Texture", Texture.White );
		Graphics.DrawQuad( worldRect, Material.UI.Basic, Color.Black, attribs );
	}
}

