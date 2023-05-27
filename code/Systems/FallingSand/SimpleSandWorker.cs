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
		cc.LogicUpdate( this, out sleep );

	}



	public void CommitChanges()
	{
		if ( !wchunk.TryGetTarget( out var chunk ) ) return;

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

				var idk = sourcchunk.GetCell( src );
				var old = GetCell( dst );
				SetCell( dst, idk, true );
				sourcchunk.SetCell( src, old, true );

				old.OnHit( this, idk );
				iprev = i + 1;
			}
		}

		chunk.Changes.Clear();
	}
}

