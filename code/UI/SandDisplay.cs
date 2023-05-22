using Sand.Systems.FallingSand;

namespace Sand.UI;

public class SandDisplay : Panel
{
	public override bool HasContent => true;

	bool drawing = false;
	int typeToUse = 1;

	protected override void OnMouseDown( MousePanelEvent e )
	{
		drawing = true;
		if ( e.MouseButton == MouseButtons.Right )
		{
			typeToUse = 2;
		}
		else
		{
			typeToUse = 1;
		}

	}
	protected override void OnMouseUp( MousePanelEvent e )
	{
		drawing = false;
	}
	TimeSince LastDraw = 0;
	public override void DrawContent( ref RenderState state )
	{
		if ( drawing && LastDraw > 0.1f )
		{
			var newpos = MousePosition / ScaleFromScreen;
			newpos.y = state.Height - newpos.y;
			newpos.y -= state.Height;
			newpos.x -= state.Width / 2f;
			newpos = SandWorld.Instance.ToLocal( newpos );
			SandWorld.SetCellBrush( (int)(newpos.x / 2), (int)(newpos.y / 2), 10, typeToUse );
		}
		//var attribsss = new RenderAttributes();
		//attribsss.Set( "Texture", Texture.White );
		//Graphics.DrawQuad( new Rect( MousePosition, 100 ), Material.UI.Basic, Color.Red, attribsss );

		foreach ( var chunk in SandWorld.Instance.chunks )
		{
			if ( chunk.Value.Texture == null || !chunk.Value.Texture.IsLoaded ) continue;
			var attribs = new RenderAttributes();
			attribs.Set( "Texture", chunk.Value.Texture );
			Rect rect = new( new Vector2( chunk.Key.x * chunk.Value.Size.x * ScaleFromScreen / Screen.Aspect, (chunk.Value.Size.y - ((chunk.Value.Size.y * chunk.Key.y))) * ScaleFromScreen / Screen.Aspect ), new Vector2( SandWorld.ChunkWidth, SandWorld.ChunkHeight ) / 2 );
			rect *= 2.5f;
			rect.Position += new Vector2( 1000, 0 );
			//DebugOverlay.Texture( Texture.Transparent, rect );

			Graphics.DrawQuad( rect, Material.FromShader( "shaders/sanddrawer.shader" ), Color.White, attribs );
		}

		return;
		/* 		float scale = (state.Height / SandWorld.WorldHeight);
				scale *= (ComputedStyle.Height.Value.Value / 100f);

				//draw the cells in SandWorld
				var world = SandWorld.Instance;
				for ( int x = 0; x < SandWorld.WorldWidth; x++ )
				{
					for ( int y = 0; y < SandWorld.WorldHeight; y++ )
					{
						var cell = world.GetCell( new Vector2Int( x, y ) );
						if ( cell.type == 0 )
							continue;

						var pos = new Vector2( x, y );
						pos *= scale;

						pos.y = state.Height - pos.y;
						//pos.x = state.X - pos.x;

						//pos.x += state.Width / 2;
						//pos.y += state.Height / 2;

						DrawQuad( pos, scale, cell.color );
					}

				} */
	}

	private void DrawQuad( Vector2 Position, float Size, Color col )
	{
		Rect rect = new( Position, new Vector2( Size, Size ) );
		var attribs = new RenderAttributes();
		attribs.Set( "Texture", Texture.White );
		Graphics.DrawQuad( rect, Material.UI.Basic, col, attribs );
	}
}

