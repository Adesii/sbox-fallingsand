using System.Diagnostics;
using Sand.util;
using Sandbox.Debug;

namespace Sand.Systems.FallingSand;

public class SimpleSandWorker : Sandworker
{

	public SimpleSandWorker( SandWorld world, SandChunk chunk ) : base( world, chunk )
	{
	}

	public override void UpdateCell( Vector2Int Position, out bool sleep )
	{
		if ( !wchunk.TryGetTarget( out var chunk ) )
		{
			sleep = true;
			Log.Info( "No chunk" );
			return;
		}
		Cell cc = chunk.GetCell( Position );
		cc.PreStep( this );
		cc.Step( this );
		cc.PostStep( this, out sleep );

	}



	public void CommitChanges()
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;
		//Remove moves that have their destinations filled
		/* for ( int i = 0; i < chunk.Changes.Count; i++ )
		{
			var moveinfo = chunk.Changes[i];
			if ( (!IsEmpty( moveinfo.To ) || moveinfo.Source.IsEmpty( moveinfo.From )) && !moveinfo.Swap )
			{
				chunk.Changes.RemoveAt( i );
				i--;
			}
		} */

		//sort by destination
		chunk.Changes.Sort( ( a, b ) => (int)a.To.Distance( b.To ) );

		//pick random source for each destination

		int iprev = 0;

		//Log.Info( $"Changes: {chunk.Changes.Count}" );


		for ( int i = 0; i < chunk.Changes.Count; i++ )
		{
			if ( i == chunk.Changes.Count - 1 || chunk.Changes[i + 1].To != chunk.Changes[i].To )
			{
				int rand = iprev + Game.Random.Int( i - iprev );

				Vector2Int dst = chunk.Changes[rand].To;
				Vector2Int src = chunk.Changes[rand].From;
				SandChunk sourcchunk = chunk.Changes[rand].Source;

				//if ( (sourcchunk.IsEmpty( src ) || !IsEmpty( dst )) ) continue;
				//cells[dst] = cells[src];
				//cells[src] = default;
				var idk = sourcchunk.GetCell( src );
				var old = GetCell( dst );
				//CommitedMoveCell( chunk.GetIndex( dst ), sourcchunk.GetIndex( src ), ref idk, true );
				SetCell( dst, idk, true );
				sourcchunk.SetCell( src, old, true );
				//Log.Info( $"Moved {src} to {dst}" );

				/* if ( dst != src )
				{
					chunk.KeepAlive( dst );
					sourcchunk.KeepAlive( src );
				} */

				//DrawPixel( dst, srcell.color );
				iprev = i + 1;
			}
		}

		chunk.Changes.Clear();



		chunk.Draw();
	}
}

