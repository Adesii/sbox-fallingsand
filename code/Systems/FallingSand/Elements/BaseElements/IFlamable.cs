namespace Sand.Systems.FallingSand.Elements;

public interface IFlamable
{
	float Flammability { get; }
	void Ignite( Sandworker worker, Cell Target, Cell Origin )
	{
		if ( Game.Random.Float( 0f, 1f ) >= Flammability )
			return;
		worker.SetCell( Target.Position, new FireElement(), true );
	}
}

