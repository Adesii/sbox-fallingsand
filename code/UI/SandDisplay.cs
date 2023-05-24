using Sand.Systems.FallingSand;

namespace Sand.UI;

public class SandDisplay : Panel
{
	public override bool HasContent => true;

	TimeSince LastDraw = 0;
	public override void DrawContent( ref RenderState state )
	{
		//var attribsss = new RenderAttributes();
		//attribsss.Set( "Texture", Texture.White );
		//Graphics.DrawQuad( new Rect( MousePosition, 100 ), Material.UI.Basic, Color.Red, attribsss );
		int line = 5;
		foreach ( var chunk in SandWorld.Instance.chunks )
		{
			if ( chunk.Value.Texture == null || !chunk.Value.Texture.IsLoaded ) continue;
			var attribs = new RenderAttributes();
			attribs.Set( "Texture", chunk.Value.Texture );
			Rect rect = new( new Vector2( chunk.Key.x * chunk.Value.Size.x, chunk.Value.Size.y - (chunk.Key.y * chunk.Value.Size.y) ), new Vector2( SandWorld.ChunkWidth, SandWorld.ChunkHeight ) ); ;
			rect.Position += SandWorld.WorldPosition;
			rect *= ScaleToScreen / ((float)SandWorld.ZoomLevel / 10f);

			if ( !chunk.Value.IsCurrentlySleeping )
			{
				//DebugOverlay.Texture( Texture.Transparent, rect, 0.2f );
				var keepaliverect = new Rect( chunk.Value.rect_minX, chunk.Value.rect_minY, chunk.Value.rect_maxX, chunk.Value.rect_maxY );
				var top = keepaliverect.Top;
				var bottom = keepaliverect.Bottom;
				keepaliverect.Top = -bottom;
				keepaliverect.Bottom = -top;

				keepaliverect += new Vector2( 0, chunk.Value.Size.y );


				keepaliverect *= ScaleToScreen / ((float)SandWorld.ZoomLevel / 10f);
				keepaliverect.Position += rect.Position;


				DebugOverlay.Texture( Texture.Transparent, keepaliverect, 0.2f );
			}

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

